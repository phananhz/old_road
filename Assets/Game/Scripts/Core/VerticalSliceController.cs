using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using TheOldRoad.Building;
using TheOldRoad.Combat;
using TheOldRoad.Construction;
using TheOldRoad.Crafting;
using TheOldRoad.Economy;
using TheOldRoad.Farming;
using TheOldRoad.Gathering;
using TheOldRoad.Input;
using TheOldRoad.Inventory;
using TheOldRoad.Items;
using TheOldRoad.NPC;
using TheOldRoad.Player;
using TheOldRoad.Quest;
using TheOldRoad.Save;
using TheOldRoad.Time;
using TheOldRoad.UI;
using TheOldRoad.World;

namespace TheOldRoad.Core
{
    public sealed class VerticalSliceController : MonoBehaviour
    {
        private const string CabinId = "building.cabin";
        private const string CampfireId = "building.campfire";
        private const string CookingHearthId = "building.cooking-hearth";
        private const string AnimalPenSmallId = "building.animal-pen-small";
        private const string AnimalPenLongId = "building.animal-pen-long";
        private const string StorageShedId = "building.storage-shed";
        private const string StoneCottageId = "building.stone-cottage";
        private const string HerbalistHutId = "building.herbalist-hut";
        private const string LookoutTowerId = "building.lookout-tower";
        private const string FarmBarnId = "building.farm-barn";
        private const string FenceId = "building.fence";
        private const string GateId = "building.gate";
        private const string PerimeterFenceDragId = "building.perimeter-fence-drag";
        private const string PerimeterFenceSmallId = "building.perimeter-fence-small";
        private const string PerimeterFenceMediumId = "building.perimeter-fence-medium";
        private const string PerimeterFenceLargeId = "building.perimeter-fence-large";
        private const string PerimeterFenceGrandId = "building.perimeter-fence-grand";
        private const string PathDirtId = "building.path-dirt";
        private const string PathCobblestoneId = "building.path-cobblestone";
        private const string ScarecrowId = "building.scarecrow";
        private const int WorldSeed = 43129;
        private static readonly Vector2 WorldMin = new Vector2(-60f, -36f);
        private static readonly Vector2 WorldMax = new Vector2(60f, 36f);

        [SerializeField] private BuildingDefinition cabinDefinition;
        [SerializeField] private BuildingDefinition campfireDefinition;
        [SerializeField] private BuildingDefinition cookingHearthDefinition;
        [SerializeField] private BuildingDefinition animalPenSmallDefinition;
        [SerializeField] private BuildingDefinition animalPenLongDefinition;
        [SerializeField] private BuildingDefinition storageShedDefinition;
        [SerializeField] private BuildingDefinition stoneCottageDefinition;
        [SerializeField] private BuildingDefinition herbalistHutDefinition;
        [SerializeField] private BuildingDefinition lookoutTowerDefinition;
        [SerializeField] private BuildingDefinition farmBarnDefinition;
        [SerializeField] private BuildingDefinition fenceDefinition;
        [SerializeField] private BuildingDefinition gateDefinition;
        [SerializeField] private BuildingDefinition perimeterFenceDragDefinition;
        [SerializeField] private BuildingDefinition perimeterFenceSmallDefinition;
        [SerializeField] private BuildingDefinition perimeterFenceMediumDefinition;
        [SerializeField] private BuildingDefinition perimeterFenceLargeDefinition;
        [SerializeField] private BuildingDefinition perimeterFenceGrandDefinition;
        [SerializeField] private BuildingDefinition pathDirtDefinition;
        [SerializeField] private BuildingDefinition pathCobblestoneDefinition;
        [SerializeField] private BuildingDefinition scarecrowDefinition;
        [SerializeField] private RecipeDefinition cabinPlankRecipe;
        [SerializeField] private RecipeDefinition wornAxeRecipe;
        [SerializeField] private RecipeDefinition stonePickRecipe;
        [SerializeField] private RecipeDefinition fenceRecipe;
        [SerializeField] private RecipeDefinition gateRecipe;
        [SerializeField] private RecipeDefinition wateringCanRecipe;
        [SerializeField] private Vector2Int buildAreaMin = new Vector2Int(-100000, -100000);
        [SerializeField] private Vector2Int buildAreaMax = new Vector2Int(100000, 100000);
        [SerializeField, Min(1f)] private float autosaveIntervalSeconds = 10f;

        private readonly Dictionary<string, ResourceNode> resourceNodes = new Dictionary<string, ResourceNode>();
        private readonly Dictionary<string, DiscoverableLandmark> landmarks = new Dictionary<string, DiscoverableLandmark>();
        private readonly Dictionary<string, LootChest> lootChests = new Dictionary<string, LootChest>();
        private readonly Dictionary<string, BuildingDefinition> buildingDefinitions = new Dictionary<string, BuildingDefinition>();
        private readonly Dictionary<string, ConstructionJob> constructionJobs = new Dictionary<string, ConstructionJob>();
        private readonly HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();
        private readonly HashSet<string> harvestedResourceNodeIds = new HashSet<string>();

        private InventorySession inventorySession;
        private SaveRepository saveRepository;
        private IClock clock;
        private string saveStatus = "Save not initialized.";
        private float autosaveTimer;
        private bool talkedToVillager;
        private readonly HashSet<string> completedStoryStepIds = new HashSet<string>();

        public InventoryRuntime Inventory => inventorySession != null ? inventorySession.Runtime : null;
        public string SaveStatus => saveStatus;
        public int ActiveConstructionCount => constructionJobs.Count;
        public int DiscoveredLandmarkCount => landmarks.Values.Count(landmark => landmark != null && landmark.IsDiscovered);
        public int TotalLandmarkCount => landmarks.Count;
        public int OpenedLootChestCount => lootChests.Values.Count(chest => chest != null && chest.IsOpened);
        public int TotalLootChestCount => lootChests.Count;
        public string LastDiscoveryStatus { get; private set; } = "Follow the old road and inspect landmarks.";
        public int CompletedObjectiveCount => BuildObjectiveStates().Count(state => state.completed);
        public int TotalObjectiveCount => BuildObjectiveStates().Length;
        public string[] ObjectiveLines => BuildObjectiveStates()
            .Select(state => (state.completed ? "✓ " : "□ ") + state.text)
            .ToArray();
        public BuildingDefinition CabinDefinition => cabinDefinition;
        public RecipeDefinition CabinPlankRecipe => cabinPlankRecipe;
        public string[] ObjectiveDisplayLines => BuildObjectiveStates()
            .Select(state => (state.completed ? "[x] " : "[ ] ") + state.text)
            .ToArray();
        public string CurrentStoryTitle => BuildStoryProgress().ActiveStep != null
            ? BuildStoryProgress().ActiveStep.Title
            : "The Old Road Awaits";
        public string CurrentStoryDetail => BuildStoryProgress().ActiveStep != null
            ? BuildStoryProgress().ActiveStep.Detail
            : "The opening Roadwarden arc is complete. The Blackwood expedition is ready for the next update.";
        public string[] StoryChapterLines => BuildStoryProgress().Chapters
            .Select(chapter => chapter.Title + " — " + chapter.Summary)
            .ToArray();
        public string[] StoryJournalLines => BuildStoryProgress().CompletedSteps
            .Where(step => !string.IsNullOrWhiteSpace(step.StoryEntry))
            .Select(step => step.Title + ": " + step.StoryEntry)
            .ToArray();

        public bool IsResourceNodeHarvested(string nodeId)
        {
            return !string.IsNullOrWhiteSpace(nodeId) && harvestedResourceNodeIds.Contains(nodeId);
        }

        public void RegisterResourceNode(ResourceNode node)
        {
            if (node == null || string.IsNullOrWhiteSpace(node.NodeId)) return;

            resourceNodes[node.NodeId] = node;
            if (harvestedResourceNodeIds.Contains(node.NodeId)) node.SetHarvested(true);
        }

        public BuildingDefinition GetBuildingDefinition(string buildingId)
        {
            if (string.IsNullOrWhiteSpace(buildingId)) return null;
            return buildingDefinitions.TryGetValue(buildingId, out BuildingDefinition definition) ? definition : null;
        }

        private void Awake()
        {
            clock = new SystemClock();
            saveRepository = SaveRepository.CreateDefault();
            EnsureDefinitions();
            BuildRuntimeScene();
            LoadState();
        }

        private void Update()
        {
            autosaveTimer += UnityEngine.Time.unscaledDeltaTime;
            if (autosaveTimer < autosaveIntervalSeconds) return;

            autosaveTimer = 0f;
            SaveNow();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause) SaveNow();
        }

        private void OnApplicationQuit()
        {
            SaveNow();
        }

        public bool IsPlacementValid(Vector2Int origin, Vector2Int footprint)
        {
            return GridPlacementValidator.IsValid(
                origin,
                footprint,
                new PlacementArea(buildAreaMin, buildAreaMax),
                HasOverlap(origin, footprint));
        }

        public bool TryBeginConstruction(BuildingDefinition definition, Vector2Int origin, out string status, Vector2Int? customFootprint = null)
        {
            status = "No building selected.";
            if (definition == null) return false;

            Vector2Int footprint = customFootprint ?? definition.Footprint;

            if (!IsPlacementValid(origin, footprint))
            {
                status = "Invalid placement.";
                return false;
            }

            string constructionId = "construction." + Guid.NewGuid().ToString("N");
            if (!ConstructionRuntime.TryBegin(constructionId, definition, origin, Inventory, clock, out ConstructionJob job))
            {
                status = FormatCostError(definition);
                return false;
            }

            constructionJobs[job.constructionId] = job;
            MarkOccupied(job.Placement, footprint);
            CreateConstructionSite(job, definition, footprint);
            RefreshStoryProgressStatus("Construction started: " + GetBuildingDisplayName(definition.BuildingId) + ".");
            SaveNow();
            status = "Construction started.";
            return true;
        }

        public bool TryDemolishBuilding(Vector2 worldPosition, out string status)
        {
            status = "No building found at this location.";
            ConstructionSite[] sites = FindObjectsByType<ConstructionSite>(FindObjectsInactive.Exclude);
            ConstructionSite targetSite = null;
            float nearestDist = 3.5f;

            foreach (var site in sites)
            {
                if (site == null || site.Job == null) continue;
                Vector2Int fp = site.Definition != null ? site.Definition.Footprint : new Vector2Int(1, 1);

                float minX = site.Job.gridX - 0.5f;
                float maxX = site.Job.gridX + fp.x - 0.5f;
                float minY = site.Job.gridY - 0.5f;
                float maxY = site.Job.gridY + fp.y - 0.5f;

                if (worldPosition.x >= minX && worldPosition.x <= maxX &&
                    worldPosition.y >= minY && worldPosition.y <= maxY)
                {
                    targetSite = site;
                    break;
                }

                float dist = Vector2.Distance(worldPosition, site.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    targetSite = site;
                }
            }

            if (targetSite == null) return false;

            BuildingDefinition definition = targetSite.Definition ?? GetBuildingDefinition(targetSite.Job.buildingId);
            Vector2Int footprint = definition != null ? definition.Footprint : new Vector2Int(1, 1);

            // 100% full material refund
            if (definition != null && definition.ConstructionCosts != null)
            {
                foreach (var cost in definition.ConstructionCosts)
                {
                    if (cost.quantity > 0 && !string.IsNullOrWhiteSpace(cost.itemId))
                    {
                        Inventory.Add(cost.itemId, cost.quantity);
                        FloatingTextController.Spawn($"+{cost.quantity} {LocalizationRuntime.T(cost.itemId)}", targetSite.transform.position + Vector3.up * 1.2f, new Color(0.3f, 0.95f, 0.4f, 1f));
                    }
                }
            }

            // Unmark occupied grid cells
            UnmarkOccupied(targetSite.Job.Placement, footprint);

            // Remove from jobs map
            if (constructionJobs.ContainsKey(targetSite.Job.constructionId))
            {
                constructionJobs.Remove(targetSite.Job.constructionId);
            }

            TheOldRoad.Audio.AudioManager.PlayChopWood();
            Destroy(targetSite.gameObject);
            SaveNow();

            string bName = GetBuildingDisplayName(targetSite.Job.buildingId);
            status = LocalizationRuntime.IsVietnamese
                ? $"Đã xóa {bName} và thu hồi 100% vật phẩm!"
                : $"Demolished {bName} and refunded 100% materials!";

            PlayerSpeechBubble.Say(status);
            return true;
        }

        private void UnmarkOccupied(Vector2Int origin, Vector2Int footprint)
        {
            for (int x = 0; x < footprint.x; x++)
            {
                for (int y = 0; y < footprint.y; y++)
                {
                    occupiedCells.Remove(new Vector2Int(origin.x + x, origin.y + y));
                }
            }
        }

        public void NotifyResourceHarvested(ResourceNode node)
        {
            if (node == null) return;
            if (!string.IsNullOrWhiteSpace(node.NodeId) && node.IsHarvested) harvestedResourceNodeIds.Add(node.NodeId);
            RefreshStoryProgressStatus("Supplies gathered for the road.");
            SaveNow();
        }

        public void NotifyCrafted(RecipeDefinition recipe)
        {
            if (recipe == null) return;
            RefreshStoryProgressStatus("Crafted: " + recipe.ResultItemId + ".");
            SaveNow();
        }

        public SaveData CreateSaveData()
        {
            return new SaveData
            {
                saveVersion = SaveSerializer.CurrentVersion,
                inventory = Inventory.ToSaveEntries(),
                constructionJobs = constructionJobs.Values.Select(job => job.ToSaveEntry()).ToArray(),
                resourceNodes = BuildResourceNodeSaveEntries(),
                landmarks = landmarks.Values
                    .Select(landmark => new LandmarkSaveEntry { landmarkId = landmark.LandmarkId, discovered = landmark.IsDiscovered })
                    .ToArray(),
                lootChests = lootChests.Values
                    .Select(chest => new LootChestSaveEntry { chestId = chest.ChestId, opened = chest.IsOpened })
                    .ToArray(),
                completedStorySteps = BuildCompletedStoryStepSaveEntries(),
                player = CreatePlayerSaveEntry(),
                gameTime = CreateGameTimeSaveEntry(),
                talkedToVillager = talkedToVillager
            };
        }

        private ResourceNodeSaveEntry[] BuildResourceNodeSaveEntries()
        {
            Dictionary<string, bool> states = new Dictionary<string, bool>();
            foreach (ResourceNode node in resourceNodes.Values)
            {
                if (node == null || string.IsNullOrWhiteSpace(node.NodeId)) continue;
                states[node.NodeId] = node.IsHarvested;
                if (node.IsHarvested) harvestedResourceNodeIds.Add(node.NodeId);
            }

            foreach (string harvestedId in harvestedResourceNodeIds)
            {
                if (string.IsNullOrWhiteSpace(harvestedId)) continue;
                states[harvestedId] = true;
            }

            return states
                .Select(pair => new ResourceNodeSaveEntry { nodeId = pair.Key, harvested = pair.Value })
                .ToArray();
        }

        private StoryStepSaveEntry[] BuildCompletedStoryStepSaveEntries()
        {
            BuildStoryProgress();
            return completedStoryStepIds
                .Where(stepId => !string.IsNullOrWhiteSpace(stepId))
                .OrderBy(stepId => stepId)
                .Select(stepId => new StoryStepSaveEntry { stepId = stepId, completed = true })
                .ToArray();
        }

        public void SaveNow()
        {
            if (saveRepository == null) return;
            autosaveTimer = 0f;
            saveRepository.TrySave(CreateSaveData(), out saveStatus);
        }

        public void NotifyLandmarkDiscovered(DiscoverableLandmark landmark)
        {
            if (landmark == null) return;
            RefreshStoryProgressStatus("Journal updated: " + landmark.Title + ".");
            SaveNow();
        }

        public void NotifyLootChestOpened(LootChest chest)
        {
            if (chest == null) return;
            RefreshStoryProgressStatus("Loot found: " + chest.DisplayName + ".");
            SaveNow();
        }

        public void NotifyPrototypeStateChanged(string status)
        {
            if (!string.IsNullOrWhiteSpace(status)) RefreshStoryProgressStatus(status);
            SaveNow();
        }

        public void NotifyVillagerTalked(VillagerNpcController villager)
        {
            talkedToVillager = true;
            string fallback = "Spoke with a villager.";
            if (villager != null)
            {
                fallback = "Spoke with " + villager.VillagerName + " the " + villager.JobTitle + ".";
            }
            RefreshStoryProgressStatus(fallback);
            SaveNow();
        }

        private void RefreshStoryProgressStatus(string fallbackStatus)
        {
            HashSet<string> previous = new HashSet<string>(completedStoryStepIds);
            StoryQuestProgress progress = BuildStoryProgress();
            StoryQuestStep newStep = progress.CompletedSteps.LastOrDefault(step => !previous.Contains(step.StepId));
            LastDiscoveryStatus = newStep != null
                ? "Story updated: " + newStep.Title + "."
                : fallbackStatus;
        }

        private PlayerSaveEntry CreatePlayerSaveEntry()
        {
            PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
            if (player == null) return null;

            CabinInteriorController interior = FindAnyObjectByType<CabinInteriorController>(FindObjectsInactive.Include);
            return new PlayerSaveEntry
            {
                x = player.transform.position.x,
                y = player.transform.position.y,
                insideCabin = interior != null && interior.IsInside
            };
        }

        private static GameTimeSaveEntry CreateGameTimeSaveEntry()
        {
            GameTimeController gameTime = FindAnyObjectByType<GameTimeController>();
            if (gameTime == null) return null;

            return new GameTimeSaveEntry
            {
                absoluteMinute = gameTime.AbsoluteMinute
            };
        }

        private (string text, bool completed)[] BuildObjectiveStates()
        {
            StoryQuestProgress progress = BuildStoryProgress();
            return progress.VisibleSteps
                .Select(step => (step.Title, completedStoryStepIds.Contains(step.StepId)))
                .ToArray();
        }

        private StoryQuestProgress BuildStoryProgress()
        {
            RefreshConstructionJobs();
            InventoryRuntime inventory = Inventory;
            StoryQuestContext context = new StoryQuestContext
            {
                GetItemQuantity = itemId => inventory != null ? inventory.GetQuantity(itemId) : 0,
                HasStartedBuilding = HasStartedBuilding,
                HasCompletedBuilding = HasCompletedBuilding,
                HasDiscoveredLandmark = HasDiscoveredLandmark,
                TalkedToVillager = talkedToVillager,
                OpenedLootChestCount = OpenedLootChestCount,
                DiscoveredLandmarkCount = DiscoveredLandmarkCount
            };

            return StoryQuestRuntime.Evaluate(context, completedStoryStepIds);
        }

        private bool HasCompletedBuilding(string buildingId)
        {
            return constructionJobs.Values.Any(job => job != null && job.buildingId == buildingId && job.state == ConstructionState.Completed);
        }

        private bool HasStartedBuilding(string buildingId)
        {
            return constructionJobs.Values.Any(job => job != null && job.buildingId == buildingId);
        }

        private bool HasDiscoveredLandmark(string landmarkId)
        {
            return !string.IsNullOrWhiteSpace(landmarkId)
                && landmarks.TryGetValue(landmarkId, out DiscoverableLandmark landmark)
                && landmark != null
                && landmark.IsDiscovered;
        }

        private static bool HasAnyForagedItem(InventoryRuntime inventory)
        {
            return inventory.GetQuantity("item.wild-berries") > 0
                || inventory.GetQuantity("item.medicinal-herb") > 0
                || inventory.GetQuantity("item.mushroom") > 0;
        }

        private void RefreshConstructionJobs()
        {
            if (clock == null) return;
            long now = clock.NowUnixSeconds;
            foreach (ConstructionJob job in constructionJobs.Values)
            {
                job?.Refresh(now);
            }
        }

        private void BuildRuntimeScene()
        {
            EnsureInputBridge();
            GameTimeController gameTime = EnsureGameTime();
            Camera mainCamera = EnsureCamera();
            EnsureGround();
            EnsureRiverFlow();
            EnsureLandmarks();
            EnsureLootChests();
            inventorySession = EnsureInventorySession();
            CabinInteriorController cabinInterior = EnsureCabinInterior();
            EnsurePlayer(inventorySession, cabinInterior, gameTime);
            EnsureCameraFollow(mainCamera);
            EnsureInfiniteWorldStreamer();
            EnsureDayNightLighting(mainCamera, gameTime, inventorySession);
            EnsureProceduralResourceNodes();
            EnsurePrototypeEnemies();
            BuildingPlacementController placement = EnsureBuildingPlacement(mainCamera, inventorySession);
            EnsureHud(inventorySession, placement);
            EnsureStartMenu();
            EnsureAudioManager();
            EnsureWeather();
            EnsureHomeStorage();
            EnsureBellTowerPuzzle();
            EnsureMerchant();
            EnsurePlayerFarming();
            EnsureNightMonsters();
        }

        private void EnsureBellTowerPuzzle()
        {
            if (FindAnyObjectByType<AncientBellTowerPuzzle>() != null) return;
            GameObject ruinsObj = new GameObject("Ancient Bell Tower Ruins Puzzle");
            ruinsObj.AddComponent<AncientBellTowerPuzzle>();
        }

        private void EnsureMerchant()
        {
            if (FindAnyObjectByType<MerchantNpcController>() != null) return;
            GameObject merchantObj = new GameObject("Travelling Merchant Eldon");
            merchantObj.AddComponent<MerchantNpcController>();
        }

        private void EnsurePlayerFarming()
        {
            PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
            if (player != null && player.GetComponent<PlayerFarmingInteractor>() == null)
            {
                PlayerFarmingInteractor farming = player.gameObject.AddComponent<PlayerFarmingInteractor>();
                farming.Configure(inventorySession);
            }
        }

        private void EnsureNightMonsters()
        {
            if (FindAnyObjectByType<NightMonsterController>() != null) return;

            GameObject monsterObj1 = new GameObject("Shadow Beast");
            monsterObj1.transform.position = new Vector3(26f, -16f, 0f);
            monsterObj1.AddComponent<SpriteRenderer>().sprite = PrototypePixelArtFactory.NightMonsterSprite(0);
            monsterObj1.AddComponent<CircleCollider2D>().isTrigger = true;
            monsterObj1.AddComponent<NightMonsterController>();

            GameObject monsterObj2 = new GameObject("Shadow Beast");
            monsterObj2.transform.position = new Vector3(-35f, 18f, 0f);
            monsterObj2.AddComponent<SpriteRenderer>().sprite = PrototypePixelArtFactory.NightMonsterSprite(0);
            monsterObj2.AddComponent<CircleCollider2D>().isTrigger = true;
            monsterObj2.AddComponent<NightMonsterController>();
        }

        private void EnsureWeather()
        {
            if (FindAnyObjectByType<WeatherController>() != null) return;
            GameObject weatherObj = new GameObject("Weather Controller");
            weatherObj.AddComponent<WeatherController>();
        }

        private void EnsureHomeStorage()
        {
            if (FindAnyObjectByType<HomeStorageChest>() != null) return;
            GameObject storageObj = new GameObject("Home Storage Chest Manager");
            storageObj.AddComponent<HomeStorageChest>();
        }

        private void EnsureAudioManager()
        {
            var audio = TheOldRoad.Audio.AudioManager.Instance;
            if (audio != null)
            {
                audio.EnsureInitialized();
            }
        }

        private GameTimeController EnsureGameTime()
        {
            GameTimeController existing = FindAnyObjectByType<GameTimeController>();
            if (existing != null) return existing;

            GameObject timeObject = new GameObject("Game Time");
            return timeObject.AddComponent<GameTimeController>();
        }

        private void EnsureInputBridge()
        {
            if (FindAnyObjectByType<ImGuiPrototypeInputBridge>() != null) return;

            GameObject inputBridge = new GameObject("Prototype Input Bridge");
            inputBridge.AddComponent<ImGuiPrototypeInputBridge>();
        }

        private void LoadState()
        {
            if (!saveRepository.TryLoad(out SaveData data, out saveStatus)) return;

            Inventory.LoadFromSaveEntries(data.inventory);

            foreach (ResourceNodeSaveEntry resourceSave in data.resourceNodes ?? Array.Empty<ResourceNodeSaveEntry>())
            {
                if (resourceSave == null || string.IsNullOrWhiteSpace(resourceSave.nodeId)) continue;
                if (resourceSave.harvested) harvestedResourceNodeIds.Add(resourceSave.nodeId);
                if (resourceNodes.TryGetValue(resourceSave.nodeId, out ResourceNode node))
                {
                    node.SetHarvested(resourceSave.harvested);
                    SetRendererAlpha(node.gameObject, resourceSave.harvested ? 0.35f : 1f);
                }
            }

            foreach (LandmarkSaveEntry landmarkSave in data.landmarks ?? Array.Empty<LandmarkSaveEntry>())
            {
                if (landmarkSave == null || string.IsNullOrWhiteSpace(landmarkSave.landmarkId)) continue;
                if (landmarks.TryGetValue(landmarkSave.landmarkId, out DiscoverableLandmark landmark))
                {
                    landmark.SetDiscovered(landmarkSave.discovered);
                }
            }

            foreach (LootChestSaveEntry chestSave in data.lootChests ?? Array.Empty<LootChestSaveEntry>())
            {
                if (chestSave == null || string.IsNullOrWhiteSpace(chestSave.chestId)) continue;
                if (lootChests.TryGetValue(chestSave.chestId, out LootChest chest))
                {
                    chest.SetOpened(chestSave.opened);
                }
            }

            foreach (StoryStepSaveEntry storySave in data.completedStorySteps ?? Array.Empty<StoryStepSaveEntry>())
            {
                if (storySave == null || string.IsNullOrWhiteSpace(storySave.stepId) || !storySave.completed) continue;
                completedStoryStepIds.Add(storySave.stepId);
            }

            foreach (ConstructionSaveEntry constructionSave in data.constructionJobs ?? Array.Empty<ConstructionSaveEntry>())
            {
                if (constructionSave == null || string.IsNullOrWhiteSpace(constructionSave.constructionId)) continue;
                BuildingDefinition definition = GetBuildingDefinition(constructionSave.buildingId) ?? cabinDefinition;
                ConstructionJob job = ConstructionJob.FromSaveEntry(constructionSave);
                if (string.IsNullOrWhiteSpace(job.buildingId)) job.buildingId = definition.BuildingId;
                job.Refresh(clock.NowUnixSeconds);
                constructionJobs[job.constructionId] = job;
                MarkOccupied(job.Placement, definition.Footprint);
                CreateConstructionSite(job, definition);
            }

            if (data.gameTime != null)
            {
                GameTimeController gameTime = FindAnyObjectByType<GameTimeController>();
                if (gameTime != null) gameTime.LoadAbsoluteMinute(data.gameTime.absoluteMinute);
            }

            talkedToVillager = data.talkedToVillager;
            RestorePlayerState(data.player);
        }

        private void RestorePlayerState(PlayerSaveEntry playerSave)
        {
            if (playerSave == null) return;

            PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
            if (player == null) return;

            Vector3 savedPosition = new Vector3(playerSave.x, playerSave.y, 0f);
            if (playerSave.insideCabin)
            {
                CabinInteriorController interior = FindAnyObjectByType<CabinInteriorController>(FindObjectsInactive.Include);
                Vector3 cabinPosition = FindFirstCompletedCabinPosition();
                if (interior != null)
                {
                    interior.Enter(player, cabinPosition);
                    player.transform.position = savedPosition;
                    return;
                }
            }

            player.transform.position = savedPosition;
        }

        private Vector3 FindFirstCompletedCabinPosition()
        {
            foreach (ConstructionSite site in FindObjectsByType<ConstructionSite>(FindObjectsInactive.Exclude))
            {
                if (site != null && site.IsCompleted && site.BuildingId == CabinId) return site.transform.position;
            }

            return Vector3.zero;
        }

        private void EnsureDefinitions()
        {
            if (cabinDefinition == null)
            {
                cabinDefinition = CreatePrototypeBuilding(
                    CabinId,
                    new Vector2Int(2, 2),
                    new[]
                    {
                        new BuildCostEntry { itemId = "item.wood", quantity = 3 },
                        new BuildCostEntry { itemId = "item.stone", quantity = 2 }
                    },
                    30f,
                    new[] { "Foundation", "Frame", "Walls", "Roof", "Complete" });
            }

            campfireDefinition ??= CreatePrototypeBuilding(
                CampfireId,
                new Vector2Int(1, 1),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 2 },
                    new BuildCostEntry { itemId = "item.stone", quantity = 2 }
                },
                12f,
                new[] { "Ring", "Kindling", "Flame" });

            cookingHearthDefinition ??= CreatePrototypeBuilding(
                CookingHearthId,
                new Vector2Int(2, 1),
                new[]
                {
                    new BuildCostEntry { itemId = "item.stone", quantity = 5 },
                    new BuildCostEntry { itemId = "item.wood", quantity = 2 },
                    new BuildCostEntry { itemId = "item.iron-ore", quantity = 1 }
                },
                25f,
                new[] { "Base", "Chamber", "Ready" });

            animalPenSmallDefinition ??= CreatePrototypeBuilding(
                AnimalPenSmallId,
                new Vector2Int(3, 2),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 6 },
                    new BuildCostEntry { itemId = "item.stone", quantity = 2 }
                },
                35f,
                new[] { "Posts", "Rails", "Gate", "Ready" });

            animalPenLongDefinition ??= CreatePrototypeBuilding(
                AnimalPenLongId,
                new Vector2Int(4, 2),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 10 },
                    new BuildCostEntry { itemId = "item.stone", quantity = 4 }
                },
                45f,
                new[] { "Posts", "Rails", "Gate", "Ready" });

            storageShedDefinition ??= CreatePrototypeBuilding(
                StorageShedId,
                new Vector2Int(2, 2),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 5 },
                    new BuildCostEntry { itemId = "item.cabin-plank", quantity = 1 },
                    new BuildCostEntry { itemId = "item.stone", quantity = 2 }
                },
                30f,
                new[] { "Foundation", "Frame", "Roof", "Ready" });

            stoneCottageDefinition ??= CreatePrototypeBuilding(
                StoneCottageId,
                new Vector2Int(3, 2),
                new[]
                {
                    new BuildCostEntry { itemId = "item.stone", quantity = 8 },
                    new BuildCostEntry { itemId = "item.wood", quantity = 8 },
                    new BuildCostEntry { itemId = "item.cabin-plank", quantity = 2 }
                },
                60f,
                new[] { "Foundation", "Frame", "Walls", "Roof", "Complete" });

            herbalistHutDefinition ??= CreatePrototypeBuilding(
                HerbalistHutId,
                new Vector2Int(3, 2),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 8 },
                    new BuildCostEntry { itemId = "item.stone", quantity = 4 },
                    new BuildCostEntry { itemId = "item.medicinal-herb", quantity = 2 }
                },
                40f,
                new[] { "Foundation", "Frame", "Thatch", "Garden", "Ready" });

            lookoutTowerDefinition ??= CreatePrototypeBuilding(
                LookoutTowerId,
                new Vector2Int(2, 2),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 10 },
                    new BuildCostEntry { itemId = "item.iron-ore", quantity = 3 },
                    new BuildCostEntry { itemId = "item.torch", quantity = 1 }
                },
                35f,
                new[] { "Stilts", "Braces", "Platform", "Beacon", "Ready" });

            farmBarnDefinition ??= CreatePrototypeBuilding(
                FarmBarnId,
                new Vector2Int(4, 3),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 12 },
                    new BuildCostEntry { itemId = "item.cabin-plank", quantity = 2 },
                    new BuildCostEntry { itemId = "item.stone", quantity = 4 }
                },
                50f,
                new[] { "Foundation", "Frame", "Tin Roof", "Haystack", "Ready" });

            fenceDefinition ??= CreatePrototypeBuilding(
                FenceId,
                new Vector2Int(1, 1),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 2 }
                },
                0f,
                new[] { "Placed" });

            gateDefinition ??= CreatePrototypeBuilding(
                GateId,
                new Vector2Int(1, 1),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 4 }
                },
                0f,
                new[] { "Placed" });

            perimeterFenceDragDefinition ??= CreatePrototypeBuilding(
                PerimeterFenceDragId,
                new Vector2Int(10, 8),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 10 }
                },
                10f,
                new[] { "Marking", "Posts", "Rails", "Gate", "Ready" });

            perimeterFenceSmallDefinition ??= CreatePrototypeBuilding(
                PerimeterFenceSmallId,
                new Vector2Int(6, 4),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 8 }
                },
                10f,
                new[] { "Marking", "Posts", "Rails", "Gate", "Ready" });

            perimeterFenceMediumDefinition ??= CreatePrototypeBuilding(
                PerimeterFenceMediumId,
                new Vector2Int(10, 8),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 12 },
                    new BuildCostEntry { itemId = "item.stone", quantity = 2 }
                },
                15f,
                new[] { "Marking", "Posts", "Rails", "Gate", "Ready" });

            perimeterFenceLargeDefinition ??= CreatePrototypeBuilding(
                PerimeterFenceLargeId,
                new Vector2Int(16, 12),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 18 },
                    new BuildCostEntry { itemId = "item.stone", quantity = 4 }
                },
                20f,
                new[] { "Marking", "Posts", "Rails", "Gate", "Ready" });

            perimeterFenceGrandDefinition ??= CreatePrototypeBuilding(
                PerimeterFenceGrandId,
                new Vector2Int(24, 16),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 25 },
                    new BuildCostEntry { itemId = "item.stone", quantity = 6 }
                },
                25f,
                new[] { "Marking", "Posts", "Rails", "Gate", "Ready" });

            pathDirtDefinition ??= CreatePrototypeBuilding(
                PathDirtId,
                new Vector2Int(1, 1),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 1 }
                },
                0f,
                new[] { "Paved" });

            pathCobblestoneDefinition ??= CreatePrototypeBuilding(
                PathCobblestoneId,
                new Vector2Int(1, 1),
                new[]
                {
                    new BuildCostEntry { itemId = "item.stone", quantity = 1 }
                },
                0f,
                new[] { "Paved" });

            scarecrowDefinition ??= CreatePrototypeBuilding(
                ScarecrowId,
                new Vector2Int(1, 1),
                new[]
                {
                    new BuildCostEntry { itemId = "item.wood", quantity = 2 },
                    new BuildCostEntry { itemId = "item.wheat", quantity = 2 }
                },
                5f,
                new[] { "Frame", "Clothes", "Hat", "Ready" });

            RegisterBuildingDefinition(cabinDefinition);
            RegisterBuildingDefinition(campfireDefinition);
            RegisterBuildingDefinition(cookingHearthDefinition);
            RegisterBuildingDefinition(animalPenSmallDefinition);
            RegisterBuildingDefinition(animalPenLongDefinition);
            RegisterBuildingDefinition(storageShedDefinition);
            RegisterBuildingDefinition(stoneCottageDefinition);
            RegisterBuildingDefinition(herbalistHutDefinition);
            RegisterBuildingDefinition(lookoutTowerDefinition);
            RegisterBuildingDefinition(farmBarnDefinition);
            RegisterBuildingDefinition(fenceDefinition);
            RegisterBuildingDefinition(gateDefinition);
            RegisterBuildingDefinition(perimeterFenceDragDefinition);
            RegisterBuildingDefinition(perimeterFenceSmallDefinition);
            RegisterBuildingDefinition(perimeterFenceMediumDefinition);
            RegisterBuildingDefinition(perimeterFenceLargeDefinition);
            RegisterBuildingDefinition(perimeterFenceGrandDefinition);
            RegisterBuildingDefinition(pathDirtDefinition);
            RegisterBuildingDefinition(pathCobblestoneDefinition);
            RegisterBuildingDefinition(scarecrowDefinition);

            cabinPlankRecipe ??= CreatePrototypeRecipe(
                "recipe.cabin-planks",
                new[] { new IngredientRequirement { itemId = "item.wood", quantity = 2 } },
                "item.cabin-plank",
                1);

            wornAxeRecipe ??= CreatePrototypeRecipe(
                "recipe.worn-axe",
                new[]
                {
                    new IngredientRequirement { itemId = "item.wood", quantity = 2 },
                    new IngredientRequirement { itemId = "item.stone", quantity = 1 }
                },
                "item.tool-axe",
                1);

            stonePickRecipe ??= CreatePrototypeRecipe(
                "recipe.stone-pick",
                new[]
                {
                    new IngredientRequirement { itemId = "item.wood", quantity = 2 },
                    new IngredientRequirement { itemId = "item.stone", quantity = 3 }
                },
                "item.tool-pickaxe",
                1);

            fenceRecipe ??= CreatePrototypeRecipe(
                "recipe.wood-fence",
                new[] { new IngredientRequirement { itemId = "item.wood", quantity = 1 } },
                "item.fence-wood",
                2);

            gateRecipe ??= CreatePrototypeRecipe(
                "recipe.wood-gate",
                new[] { new IngredientRequirement { itemId = "item.wood", quantity = 2 } },
                "item.gate-wood",
                1);

            wateringCanRecipe ??= CreatePrototypeRecipe(
                "recipe.watering-can",
                new[]
                {
                    new IngredientRequirement { itemId = "item.iron-ore", quantity = 2 },
                    new IngredientRequirement { itemId = "item.wood", quantity = 1 }
                },
                "item.watering-can",
                1);
        }

        private static RecipeDefinition CreatePrototypeRecipe(
            string recipeId,
            IngredientRequirement[] ingredients,
            string resultItemId,
            int resultQuantity)
        {
            RecipeDefinition recipe = ScriptableObject.CreateInstance<RecipeDefinition>();
            recipe.ConfigureForPrototype(recipeId, ingredients, resultItemId, resultQuantity, 0f, string.Empty);
            return recipe;
        }

        private static BuildingDefinition CreatePrototypeBuilding(
            string buildingId,
            Vector2Int footprint,
            BuildCostEntry[] costs,
            float durationSeconds,
            string[] stages)
        {
            BuildingDefinition definition = ScriptableObject.CreateInstance<BuildingDefinition>();
            definition.ConfigureForPrototype(buildingId, footprint, costs, durationSeconds, stages);
            return definition;
        }

        private void RegisterBuildingDefinition(BuildingDefinition definition)
        {
            if (definition == null || string.IsNullOrWhiteSpace(definition.BuildingId)) return;
            buildingDefinitions[definition.BuildingId] = definition;
        }

        private Camera EnsureCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null) mainCamera = FindAnyObjectByType<Camera>();
            if (mainCamera != null)
            {
                mainCamera.tag = "MainCamera";
                mainCamera.transform.position = new Vector3(0f, 0f, -10f);
                mainCamera.transform.rotation = Quaternion.identity;
                mainCamera.orthographic = true;
                mainCamera.orthographicSize = 6f;
                mainCamera.clearFlags = CameraClearFlags.SolidColor;
                mainCamera.backgroundColor = new Color(0.05f, 0.07f, 0.08f, 1f);
                EnsureAudioListener(mainCamera.gameObject);
                return mainCamera;
            }

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 0f, -10f);

            mainCamera = cameraObject.AddComponent<Camera>();
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 6f;
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = new Color(0.05f, 0.07f, 0.08f, 1f);
            EnsureAudioListener(cameraObject);
            return mainCamera;
        }

        private static void EnsureAudioListener(GameObject target)
        {
            if (FindAnyObjectByType<AudioListener>() == null && target != null)
            {
                target.AddComponent<AudioListener>();
            }
        }

        private void EnsureCameraFollow(Camera mainCamera)
        {
            if (mainCamera == null) return;

            PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
            if (player == null) return;

            CameraFollow2D follow = mainCamera.GetComponent<CameraFollow2D>();
            if (follow == null) follow = mainCamera.gameObject.AddComponent<CameraFollow2D>();
            follow.Configure(player.transform, new Vector2(-100000f, -100000f), new Vector2(100000f, 100000f), 0.12f);
        }

        private void EnsureInfiniteWorldStreamer()
        {
            PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
            if (player == null) return;

            InfiniteWorldStreamer streamer = FindAnyObjectByType<InfiniteWorldStreamer>();
            if (streamer == null)
            {
                GameObject streamerObject = new GameObject("Infinite World Streamer");
                streamer = streamerObject.AddComponent<InfiniteWorldStreamer>();
            }

            streamer.Configure(player.transform, WorldSeed, 32f, 2, 3, this);
        }

        private void EnsureGround()
        {
            GameObject existingGround = GameObject.Find("Valen Outskirts Ground");
            if (existingGround != null)
            {
                SpriteRenderer existingRenderer = existingGround.GetComponent<SpriteRenderer>();
                if (existingRenderer == null) existingRenderer = existingGround.AddComponent<SpriteRenderer>();
                existingRenderer.sprite = PrototypePixelArtFactory.ValenOutskirtsGround();
                existingRenderer.sortingOrder = -10000;
                return;
            }

            GameObject ground = new GameObject("Valen Outskirts Ground");
            ground.transform.position = Vector3.zero;
            SpriteRenderer renderer = ground.AddComponent<SpriteRenderer>();
            renderer.sprite = PrototypePixelArtFactory.ValenOutskirtsGround();
            renderer.sortingOrder = -10000;
        }

        private void EnsureRiverFlow()
        {
            RiverFlowAnimator river = FindAnyObjectByType<RiverFlowAnimator>();
            if (river == null)
            {
                GameObject riverObject = new GameObject("Animated River Flow");
                river = riverObject.AddComponent<RiverFlowAnimator>();
            }

            river.Configure();
        }

        private void EnsureLandmarks()
        {
            EnsureLandmark(
                "landmark.waystone.north",
                "Northern Waystone",
                "Northern Waystone",
                "A cold marker from the old road network. Its carved bell sigil still catches the morning light.",
                PrototypePixelArtFactory.Waystone(),
                new Vector3(-21f, 13f, 0f));

            EnsureLandmark(
                "landmark.sign.old-road",
                "Old Road Sign",
                "Old Road Sign",
                "The sign points east toward a village name scratched away long ago.",
                PrototypePixelArtFactory.RoadSign(),
                new Vector3(-8f, 1.7f, 0f));

            EnsureLandmark(
                "landmark.arch.watch",
                "Broken Watch Arch",
                "Broken Watch Arch",
                "A ruined watch arch from the Roadwarden days. Someone recently cleared moss from one stone.",
                PrototypePixelArtFactory.RuinedArch(),
                new Vector3(16f, 8.5f, 0f));

            EnsureLandmark(
                "landmark.bridge.river",
                "River Footbridge",
                "River Footbridge",
                "An old timber bridge still holds over the shallow river bend. Fresh boot marks cross it.",
                PrototypePixelArtFactory.Footbridge(),
                new Vector3(-14f, -4.2f, 0f));

            EnsureLandmark(
                "landmark.camp.abandoned",
                "Abandoned Camp",
                "Abandoned Camp",
                "The ashes are old, but the stones around the fire pit were set with care.",
                PrototypePixelArtFactory.Campfire(),
                new Vector3(8f, -11f, 0f));

            EnsureLandmark(
                "landmark.bell.east",
                "Eastern Bell Marker",
                "Eastern Bell Marker",
                "A small bell marker stands far down the road. The metal is silent, but the air around it feels tense.",
                PrototypePixelArtFactory.Waystone(),
                new Vector3(46f, 9f, 0f));

            EnsureLandmark(
                "landmark.shrine.north",
                "Hunter Shrine",
                "Hunter Shrine",
                "A weathered hunter shrine watches the northern tree line. Offerings have not fully rotted away.",
                PrototypePixelArtFactory.RoadSign(),
                new Vector3(-38f, 25f, 0f));

            EnsureLandmark(
                "landmark.ridge.dragon",
                "Dragon-Scarred Ridge",
                "Dragon-Scarred Ridge",
                "Black glass and fused stone mark where dragon flame hit the northern rock. Old ash still smells bitter.",
                PrototypePixelArtFactory.RuinedArch(),
                new Vector3(34f, 27f, 0f));

            EnsureLandmark(
                "landmark.ruin.south",
                "South Ruin Gate",
                "South Ruin Gate",
                "Broken stones mark a ruined southern gate. The road once continued beyond it.",
                PrototypePixelArtFactory.RuinedArch(),
                new Vector3(34f, -25f, 0f));

            EnsureLandmark(
                "landmark.cave.blackwood",
                "Blackwood Cave Mouth",
                "Blackwood Cave Mouth",
                "Cold air moves from the cave as if the hill is breathing. Old Roadwarden marks warn that deeper tunnels connect to ruins below the forest.",
                PrototypePixelArtFactory.RuinedArch(),
                new Vector3(-48f, 18f, 0f));

            EnsureLandmark(
                "landmark.dragon.ridge",
                "Dragon-Scarred Ridge",
                "Dragon-Scarred Ridge",
                "The stone is glassy and black where something vast burned across it. Villagers call this proof that an ancient dragon still sleeps beyond the northern road.",
                PrototypePixelArtFactory.Waystone(),
                new Vector3(52f, 25f, 0f));
        }

        private void EnsurePrototypeEnemies()
        {
            if (FindAnyObjectByType<EnemyController>() != null) return;

            // Forest Wolf near eastern woods
            CreateEnemy("enemy.forest-wolf.1", "Forest Wolf", 10, 2.4f, 3, 0.95f, 5.5f, 1.3f, new Vector3(32f, 12f, 0f), new[]
            {
                new EnemyLootEntry { itemId = "item.wool", minQuantity = 1, maxQuantity = 2, dropChance = 0.9f },
                new EnemyLootEntry { itemId = "item.wild-berries", minQuantity = 1, maxQuantity = 2, dropChance = 0.5f }
            });

            // Forest Wolf near northern trees
            CreateEnemy("enemy.forest-wolf.2", "Forest Wolf", 10, 2.4f, 3, 0.95f, 5.5f, 1.3f, new Vector3(-28f, 20f, 0f), new[]
            {
                new EnemyLootEntry { itemId = "item.wool", minQuantity = 1, maxQuantity = 2, dropChance = 0.9f }
            });

            // Bandit Scout near broken arch
            CreateEnemy("enemy.bandit.1", "Bandit Scout", 14, 1.9f, 4, 1.1f, 6.0f, 1.5f, new Vector3(22f, 9.5f, 0f), new[]
            {
                new EnemyLootEntry { itemId = "item.old-coin", minQuantity = 2, maxQuantity = 5, dropChance = 1.0f },
                new EnemyLootEntry { itemId = "item.torch", minQuantity = 1, maxQuantity = 1, dropChance = 0.6f }
            });
        }

        private GameObject CreateEnemy(string id, string name, int hp, float speed, int damage, float range, float detection, float cooldown, Vector3 position, EnemyLootEntry[] loot)
        {
            GameObject enemyObj = new GameObject(name);
            enemyObj.transform.position = position;

            EnemyDefinition def = ScriptableObject.CreateInstance<EnemyDefinition>();
            def.ConfigureForPrototype(id, name, hp, speed, damage, range, detection, cooldown, loot);

            EnemyController enemy = enemyObj.AddComponent<EnemyController>();
            enemy.Configure(def);
            return enemyObj;
        }

        public void OnPlayerDied()
        {
            PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
            if (player != null)
            {
                PlayerVitals vitals = player.GetComponent<PlayerVitals>();
                if (vitals != null)
                {
                    vitals.Heal(vitals.MaxHealth / 2);
                }

                player.transform.position = Vector3.zero;
                FloatingTextController.Spawn(TheOldRoad.UI.LocalizationRuntime.T("ui.player_respawn"), Vector3.zero + Vector3.up * 0.8f, new Color(1f, 0.4f, 0.2f, 1f), 2.5f);
            }
        }

        private InventorySession EnsureInventorySession()
        {
            InventorySession session = FindAnyObjectByType<InventorySession>();
            if (session != null) return session;

            GameObject sessionObject = new GameObject("InventorySession");
            return sessionObject.AddComponent<InventorySession>();
        }

        private CabinInteriorController EnsureCabinInterior()
        {
            CabinInteriorController interior = FindAnyObjectByType<CabinInteriorController>(FindObjectsInactive.Include);
            if (interior == null)
            {
                GameObject interiorObject = new GameObject("Cabin Interior Controller");
                interior = interiorObject.AddComponent<CabinInteriorController>();
            }

            interior.EnsureBuilt();
            return interior;
        }

        private void EnsurePlayer(InventorySession session, CabinInteriorController cabinInterior, GameTimeController gameTime)
        {
            PlayerMovement movement = FindAnyObjectByType<PlayerMovement>();
            GameObject player;

            if (movement == null)
            {
                player = CreateSpriteObject("Player", PrototypePixelArtFactory.Player(), Vector3.zero, 50);
                player.name = "Player";
                player.transform.position = Vector3.zero;
                KeyboardPlayerInputSource createdInput = player.GetComponent<KeyboardPlayerInputSource>();
                if (createdInput == null) player.AddComponent<KeyboardPlayerInputSource>();
                movement = player.AddComponent<PlayerMovement>();
            }
            else
            {
                player = movement.gameObject;
            }
            EnsurePlayerVisual(player);

            KeyboardPlayerInputSource input = player.GetComponent<KeyboardPlayerInputSource>();
            if (input == null) input = player.AddComponent<KeyboardPlayerInputSource>();

            MobileJoystickInputSource joystick = player.GetComponent<MobileJoystickInputSource>();
            if (joystick == null) joystick = player.AddComponent<MobileJoystickInputSource>();

            CompositePlayerInputSource compositeInput = player.GetComponent<CompositePlayerInputSource>();
            if (compositeInput == null) compositeInput = player.AddComponent<CompositePlayerInputSource>();
            compositeInput.Configure(input, joystick);

            movement.Configure(compositeInput, 3f);

            PlayerPixelAnimator animator = player.GetComponent<PlayerPixelAnimator>();
            if (animator == null) animator = player.AddComponent<PlayerPixelAnimator>();
            animator.Configure(movement, 0.14f);

            PlayerSpeechBubble speechBubble = player.GetComponent<PlayerSpeechBubble>();
            if (speechBubble == null) speechBubble = player.AddComponent<PlayerSpeechBubble>();
            speechBubble.Configure(Camera.main);

            PlayerVitals vitals = player.GetComponent<PlayerVitals>();
            if (vitals == null) vitals = player.AddComponent<PlayerVitals>();
            vitals.Configure(20, 20);

            PlayerGatheringInteractor gathering = player.GetComponent<PlayerGatheringInteractor>();
            if (gathering == null) gathering = player.AddComponent<PlayerGatheringInteractor>();
            gathering.Configure(session, this, 1.25f);

            PlayerCraftingInteractor crafting = player.GetComponent<PlayerCraftingInteractor>();
            if (crafting == null) crafting = player.AddComponent<PlayerCraftingInteractor>();
            crafting.Configure(session, this, wornAxeRecipe, stonePickRecipe, cabinPlankRecipe);

            PlayerCookingInteractor cooking = player.GetComponent<PlayerCookingInteractor>();
            if (cooking == null) cooking = player.AddComponent<PlayerCookingInteractor>();
            cooking.Configure(session, this, 1.9f);

            PlayerLandmarkInteractor landmarkInteractor = player.GetComponent<PlayerLandmarkInteractor>();
            if (landmarkInteractor == null) landmarkInteractor = player.AddComponent<PlayerLandmarkInteractor>();
            landmarkInteractor.Configure(this, 1.35f);

            PlayerLootInteractor lootInteractor = player.GetComponent<PlayerLootInteractor>();
            if (lootInteractor == null) lootInteractor = player.AddComponent<PlayerLootInteractor>();
            lootInteractor.Configure(session, this, 1.25f);

            PlayerCabinInteractor cabinInteractor = player.GetComponent<PlayerCabinInteractor>();
            if (cabinInteractor == null) cabinInteractor = player.AddComponent<PlayerCabinInteractor>();
            cabinInteractor.Configure(cabinInterior, gameTime, 2.25f);

            PlayerNpcInteractor npcInteractor = player.GetComponent<PlayerNpcInteractor>();
            if (npcInteractor == null) npcInteractor = player.AddComponent<PlayerNpcInteractor>();
            npcInteractor.Configure(this, 1.55f);

            PlayerCombatController combat = player.GetComponent<PlayerCombatController>();
            if (combat == null) combat = player.AddComponent<PlayerCombatController>();
            combat.Configure(session);
        }

        private void EnsureDayNightLighting(Camera mainCamera, GameTimeController gameTime, InventorySession session)
        {
            if (mainCamera == null) return;

            DayNightLightingController lighting = mainCamera.GetComponent<DayNightLightingController>();
            if (lighting == null) lighting = mainCamera.gameObject.AddComponent<DayNightLightingController>();
            lighting.Configure(gameTime, session);
        }

        private void EnsureResourceNode(string nodeId, string displayName, Vector3 position, string itemId, int amount, Color color, string requiredToolItemId = "")
        {
            if (resourceNodes.ContainsKey(nodeId)) return;

            foreach (ResourceNode existingNode in FindObjectsByType<ResourceNode>(FindObjectsInactive.Exclude))
            {
                if (existingNode == null || existingNode.NodeId != nodeId) continue;
                RegisterResourceNode(existingNode);
                return;
            }

            Sprite sprite = GetResourceSprite(itemId);
            GameObject nodeObject = CreateSpriteObject(displayName, sprite, position, 0);
            SpriteRenderer renderer = nodeObject.GetComponent<SpriteRenderer>();
            if (renderer != null) renderer.color = color;

            ResourceNode node = nodeObject.AddComponent<ResourceNode>();
            node.Configure(nodeId, itemId, amount, false, requiredToolItemId);
            RegisterResourceNode(node);
        }

        private static Sprite GetResourceSprite(string itemId)
        {
            switch (itemId)
            {
                case "item.wood": return PrototypePixelArtFactory.Tree();
                case "item.stone": return PrototypePixelArtFactory.Rock();
                case "item.wild-berries": return PrototypePixelArtFactory.BerryBush();
                case "item.medicinal-herb": return PrototypePixelArtFactory.HerbPatch();
                case "item.mushroom": return PrototypePixelArtFactory.MushroomCluster();
                case "item.iron-ore": return PrototypePixelArtFactory.IronOre();
                default: return PrototypePixelArtFactory.Rock();
            }
        }

        private void EnsureProceduralResourceNodes()
        {
            EnsureLegacyResourceNodes();

            System.Random random = new System.Random(WorldSeed);
            SpawnProceduralResources(random, 44, "node.tree.proc.", "Forest Pine", "item.wood", 3);
            SpawnProceduralResources(random, 32, "node.rock.proc.", "Field Stone", "item.stone", 2);
            SpawnProceduralResources(random, 20, "node.berry.proc.", "Wild Berry Bush", "item.wild-berries", 2);
            SpawnProceduralResources(random, 18, "node.herb.proc.", "Medicinal Herb Patch", "item.medicinal-herb", 1);
            SpawnProceduralResources(random, 14, "node.mushroom.proc.", "Forest Mushroom Cluster", "item.mushroom", 2);
            SpawnProceduralResources(random, 12, "node.iron.proc.", "Iron Vein", "item.iron-ore", 1);
        }

        private void EnsureLegacyResourceNodes()
        {
            EnsureResourceNode("node.tree.01", "Roadside Pine", new Vector3(-3.5f, 1.4f, 0f), "item.wood", 3, new Color(0.1f, 0.55f, 0.18f, 1f));
            EnsureResourceNode("node.tree.02", "Old Forest Pine", new Vector3(-18f, 9f, 0f), "item.wood", 3, new Color(0.1f, 0.55f, 0.18f, 1f));
            EnsureResourceNode("node.tree.03", "Valen Birch", new Vector3(-12f, -7f, 0f), "item.wood", 3, new Color(0.1f, 0.55f, 0.18f, 1f));
            EnsureResourceNode("node.tree.04", "Hill Pine", new Vector3(7f, 10f, 0f), "item.wood", 3, new Color(0.1f, 0.55f, 0.18f, 1f));
            EnsureResourceNode("node.tree.05", "South Pine", new Vector3(18f, -5f, 0f), "item.wood", 3, new Color(0.1f, 0.55f, 0.18f, 1f));
            EnsureResourceNode("node.tree.06", "Westwood Pine", new Vector3(-24f, -1f, 0f), "item.wood", 3, new Color(0.1f, 0.55f, 0.18f, 1f));
            EnsureResourceNode("node.tree.07", "Eastwood Pine", new Vector3(22f, 8f, 0f), "item.wood", 3, new Color(0.1f, 0.55f, 0.18f, 1f));
            EnsureResourceNode("node.rock.01", "Road Stone", new Vector3(3.5f, -1.4f, 0f), "item.stone", 2, new Color(0.45f, 0.48f, 0.52f, 1f));
            EnsureResourceNode("node.rock.02", "River Stone", new Vector3(-15f, -10f, 0f), "item.stone", 2, new Color(0.45f, 0.48f, 0.52f, 1f));
            EnsureResourceNode("node.rock.03", "Broken Road Stone", new Vector3(4f, -9f, 0f), "item.stone", 2, new Color(0.45f, 0.48f, 0.52f, 1f));
            EnsureResourceNode("node.rock.04", "Old Wall Stone", new Vector3(12f, 3f, 0f), "item.stone", 2, new Color(0.45f, 0.48f, 0.52f, 1f));
            EnsureResourceNode("node.rock.05", "East Pass Stone", new Vector3(23f, -12f, 0f), "item.stone", 2, new Color(0.45f, 0.48f, 0.52f, 1f));
            EnsureResourceNode("node.rock.06", "North Ridge Stone", new Vector3(-22f, 7f, 0f), "item.stone", 2, new Color(0.45f, 0.48f, 0.52f, 1f));
            EnsureResourceNode("node.berry.01", "Roadside Berry Bush", new Vector3(-10f, -2.8f, 0f), "item.wild-berries", 2, PrototypeItemCatalog.Get("item.wild-berries").Color);
            EnsureResourceNode("node.herb.01", "Valen Mint Patch", new Vector3(11.5f, 5.6f, 0f), "item.medicinal-herb", 1, PrototypeItemCatalog.Get("item.medicinal-herb").Color);
            EnsureResourceNode("node.mushroom.01", "Old Stump Mushrooms", new Vector3(-26f, 12f, 0f), "item.mushroom", 2, PrototypeItemCatalog.Get("item.mushroom").Color);
            EnsureResourceNode("node.iron.01", "Exposed Iron Vein", new Vector3(28f, -18f, 0f), "item.iron-ore", 1, PrototypeItemCatalog.Get("item.iron-ore").Color, "item.tool-pickaxe");
        }

        private void SpawnProceduralResources(System.Random random, int count, string idPrefix, string displayPrefix, string itemId, int amount)
        {
            int created = 0;
            int attempts = 0;
            while (created < count && attempts < count * 18)
            {
                attempts++;
                float x = Mathf.Lerp(WorldMin.x + 4f, WorldMax.x - 4f, (float)random.NextDouble());
                float y = Mathf.Lerp(WorldMin.y + 4f, WorldMax.y - 4f, (float)random.NextDouble());
                Vector3 position = new Vector3(x, y, 0f);

                if (Vector2.Distance(position, Vector2.zero) < 5f) continue;
                if (Mathf.Abs(y - RoadCenterY(x)) < 1.9f && random.NextDouble() < 0.65d) continue;

                string id = idPrefix + created.ToString("00");
                string requiredToolItemId = itemId == "item.iron-ore" ? "item.tool-pickaxe" : string.Empty;
                EnsureResourceNode(id, displayPrefix + " " + (created + 1), position, itemId, amount, Color.white, requiredToolItemId);
                created++;
            }
        }

        private static float RoadCenterY(float worldX)
        {
            return 1.4f * Mathf.Sin(worldX * 0.34f) + 0.8f * Mathf.Sin(worldX * 0.11f);
        }

        private void EnsureLootChests()
        {
            EnsureLootChest("chest.journal.01", "Father's Journal Page", new Vector3(-1.8f, 2.1f, 0f), "item.roadwarden-page", 1);
            EnsureLootChest("chest.road.01", "Roadside Cache", new Vector3(-6f, -2.2f, 0f), "item.wood", 2);
            EnsureLootChest("chest.camp.01", "Abandoned Camp Chest", new Vector3(9.8f, -11.4f, 0f), "item.cabin-plank", 1);
            EnsureLootChest("chest.ruin.01", "South Ruin Chest", new Vector3(36f, -24.2f, 0f), "item.stone", 3);
            EnsureLootChest("chest.shrine.01", "Hunter Shrine Cache", new Vector3(-40f, 24f, 0f), "item.wood", 3);
            EnsureLootChest("chest.bell.01", "Bell Marker Cache", new Vector3(44.2f, 7.4f, 0f), "item.bell-fragment", 1);
            EnsureLootChest("chest.bell.fragment.01", "Silent Bell Casket", new Vector3(47.2f, 9.2f, 0f), "item.bell-fragment", 1);
            EnsureLootChest("chest.grove.01", "Forager's Hidden Pouch", new Vector3(-30f, 14.5f, 0f), "item.medicinal-herb", 2);
            EnsureLootChest("chest.bridge.01", "Bridge Toll Box", new Vector3(-11.4f, -5.6f, 0f), "item.old-coin", 4);
            EnsureLootChest("chest.mine.01", "Collapsed Mine Crate", new Vector3(31.5f, -19.5f, 0f), "item.iron-ore", 2);
            EnsureLootChest("chest.camp.torch", "Camp Torch Bundle", new Vector3(6.4f, -10.2f, 0f), "item.torch", 1);
        }

        private void EnsureLootChest(string chestId, string displayName, Vector3 position, string itemId, int quantity)
        {
            if (lootChests.ContainsKey(chestId)) return;

            GameObject chestObject = GameObject.Find(displayName);
            if (chestObject == null) chestObject = CreateSpriteObject(displayName, PrototypePixelArtFactory.ChestClosed(), position, 8);

            LootChest chest = chestObject.GetComponent<LootChest>();
            if (chest == null) chest = chestObject.AddComponent<LootChest>();
            chest.Configure(chestId, displayName, itemId, quantity);
            lootChests[chestId] = chest;
        }

        private BuildingPlacementController EnsureBuildingPlacement(Camera mainCamera, InventorySession session)
        {
            BuildingPlacementController placement = FindAnyObjectByType<BuildingPlacementController>();
            if (placement == null)
            {
                GameObject placementObject = new GameObject("BuildingPlacementController");
                placement = placementObject.AddComponent<BuildingPlacementController>();
            }

            placement.Configure(mainCamera, session, this, cabinDefinition, buildAreaMin, buildAreaMax, 1f);
            return placement;
        }

        private void EnsureHud(InventorySession session, BuildingPlacementController placement)
        {
            InventoryDebugHud hud = FindAnyObjectByType<InventoryDebugHud>();
            if (hud == null)
            {
                GameObject hudObject = new GameObject("Development HUD");
                hud = hudObject.AddComponent<InventoryDebugHud>();
            }

            hud.Configure(session, placement, this);
        }

        private void EnsureStartMenu()
        {
            if (FindAnyObjectByType<GameStartMenuController>() != null) return;

            GameObject menuObject = new GameObject("Start And Settings Menu");
            menuObject.AddComponent<GameStartMenuController>();
        }

        private void CreateConstructionSite(ConstructionJob job, BuildingDefinition definition, Vector2Int? customFootprint = null)
        {
            string buildingName = GetBuildingDisplayName(definition != null ? definition.BuildingId : job.buildingId);
            Sprite initSprite = (job.buildingId != null && job.buildingId.Contains("perimeter-fence")) ? null : PrototypePixelArtFactory.BuildingConstruction(job.buildingId, 0);
            GameObject site = CreateSpriteObject(buildingName + " Construction Site", initSprite, new Vector3(job.gridX, job.gridY, 0f), 0);
            site.name = buildingName + " Construction Site";
            ConstructionSite constructionSite = site.AddComponent<ConstructionSite>();
            constructionSite.Configure(job, definition, clock);

            if (job.buildingId == CampfireId || job.buildingId == CookingHearthId)
            {
                CampfireLightController light = site.GetComponent<CampfireLightController>();
                if (light == null) light = site.AddComponent<CampfireLightController>();
                light.Configure(constructionSite, FindAnyObjectByType<GameTimeController>());
            }

            if (job.buildingId == CabinId || job.buildingId == StoneCottageId || job.buildingId == HerbalistHutId || job.buildingId == LookoutTowerId)
            {
                HouseLightController houseLight = site.GetComponent<HouseLightController>();
                if (houseLight == null) houseLight = site.AddComponent<HouseLightController>();
                Vector3 chimneyOffset = job.buildingId == StoneCottageId
                    ? new Vector3(1.20f, 2.75f, 0f)
                    : (job.buildingId == LookoutTowerId ? new Vector3(0f, 3.2f, 0f) : new Vector3(1.00f, 2.60f, 0f));
                float glowScale = job.buildingId == StoneCottageId ? 1.15f : (job.buildingId == LookoutTowerId ? 1.4f : 0.95f);
                houseLight.Configure(constructionSite, FindAnyObjectByType<GameTimeController>(), chimneyOffset, glowScale);
            }

            if (job.buildingId == AnimalPenSmallId || job.buildingId == AnimalPenLongId)
            {
                AnimalPenController pen = site.GetComponent<AnimalPenController>();
                if (pen == null) pen = site.AddComponent<AnimalPenController>();
                string product = job.buildingId == AnimalPenLongId ? "item.wool" : "item.egg";
                float seconds = job.buildingId == AnimalPenLongId ? 60f : 45f;
                pen.Configure(constructionSite, inventorySession, this, product, seconds);
            }

            if (job.buildingId == FarmBarnId)
            {
                HappyFarmBarnController barn = site.GetComponent<HappyFarmBarnController>();
                if (barn == null) barn = site.AddComponent<HappyFarmBarnController>();
                barn.Configure(inventorySession);
            }

            if (job.buildingId.Contains("perimeter-fence"))
            {
                SpriteRenderer rootSr = site.GetComponent<SpriteRenderer>();
                if (rootSr != null) rootSr.sprite = null;

                PerimeterFenceController fence = site.GetComponent<PerimeterFenceController>();
                if (fence == null) fence = site.AddComponent<PerimeterFenceController>();
                Vector2Int fp = customFootprint ?? (definition != null ? definition.Footprint : new Vector2Int(10, 8));
                fence.Configure(fp.x, fp.y);
            }

            // Attach solid colliders to prevent walking through walls
            BoxCollider2D collider = site.GetComponent<BoxCollider2D>();
            if (collider == null && !job.buildingId.StartsWith("building.path") && !job.buildingId.Contains("perimeter-fence"))
            {
                collider = site.AddComponent<BoxCollider2D>();
                if (job.buildingId == CabinId) { collider.size = new Vector2(3.2f, 1.8f); collider.offset = new Vector2(0f, 0.4f); }
                else if (job.buildingId == StoneCottageId) { collider.size = new Vector2(3.6f, 2.0f); collider.offset = new Vector2(0f, 0.5f); }
                else if (job.buildingId == StorageShedId) { collider.size = new Vector2(2.6f, 1.6f); collider.offset = new Vector2(0f, 0.4f); }
                else if (job.buildingId == HerbalistHutId) { collider.size = new Vector2(3.2f, 1.8f); collider.offset = new Vector2(0f, 0.4f); }
                else if (job.buildingId == LookoutTowerId) { collider.size = new Vector2(2.2f, 2.0f); collider.offset = new Vector2(0f, 0.6f); }
                else if (job.buildingId == FarmBarnId) { collider.size = new Vector2(4.2f, 2.0f); collider.offset = new Vector2(0f, 0.6f); }
                else if (job.buildingId == FenceId) { collider.size = new Vector2(1.0f, 0.8f); collider.offset = new Vector2(0f, 0.1f); }
                else if (job.buildingId == ScarecrowId) { collider.size = new Vector2(0.8f, 1.2f); collider.offset = new Vector2(0f, 0.2f); }
            }
            else if (job.buildingId.StartsWith("building.path") && collider == null)
            {
                collider = site.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
                collider.size = new Vector2(1f, 1f);
                site.name = "Path_" + job.buildingId;
            }
        }

        private static string GetBuildingDisplayName(string buildingId)
        {
            switch (buildingId)
            {
                case CampfireId: return "Campfire";
                case CookingHearthId: return "Cooking Hearth";
                case AnimalPenSmallId: return "Small Animal Pen";
                case AnimalPenLongId: return "Long Animal Pen";
                case StorageShedId: return "Storage Shed";
                case StoneCottageId: return "Stone Cottage";
                case HerbalistHutId: return "Herbalist Hut";
                case LookoutTowerId: return "Lookout Tower";
                case FarmBarnId: return "Farm Barn";
                case FenceId: return "Wood Fence";
                case GateId: return "Wood Gate";
                case PerimeterFenceDragId: return "Custom Perimeter Fence";
                case PerimeterFenceSmallId: return "Small Yard Fence (6x4)";
                case PerimeterFenceMediumId: return "Medium Estate Fence (10x8)";
                case PerimeterFenceLargeId: return "Large Farmstead Fence (16x12)";
                case PerimeterFenceGrandId: return "Grand Homestead Fence (24x16)";
                case PathDirtId: return "Dirt Path";
                case PathCobblestoneId: return "Cobblestone Path";
                case ScarecrowId: return "Scarecrow";
                default: return "Cabin";
            }
        }

        private bool HasOverlap(Vector2Int origin, Vector2Int footprint)
        {
            for (int x = 0; x < footprint.x; x++)
            {
                for (int y = 0; y < footprint.y; y++)
                {
                    if (occupiedCells.Contains(new Vector2Int(origin.x + x, origin.y + y))) return true;
                }
            }

            return false;
        }

        private void MarkOccupied(Vector2Int origin, Vector2Int footprint)
        {
            for (int x = 0; x < footprint.x; x++)
            {
                for (int y = 0; y < footprint.y; y++)
                {
                    occupiedCells.Add(new Vector2Int(origin.x + x, origin.y + y));
                }
            }
        }

        private string FormatCostError(BuildingDefinition definition)
        {
            string costs = string.Join(", ", definition.ConstructionCosts.Select(cost => cost.quantity + " " + cost.itemId));
            return "Need resources: " + costs;
        }

        private static void RemovePrimitiveCollider(GameObject gameObject)
        {
            Component collider = gameObject.GetComponent("Collider");
            if (collider != null) Destroy(collider);
        }

        private static GameObject CreateSpriteObject(string name, Sprite sprite, Vector3 position, int sortingOffset)
        {
            GameObject gameObject = new GameObject(name);
            gameObject.transform.position = position;
            SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            gameObject.AddComponent<YSortSprite>().Configure(sortingOffset);
            return gameObject;
        }

        private static void EnsureDecoration(string name, Sprite sprite, Vector3 position, int sortingOffset)
        {
            if (GameObject.Find(name) != null) return;
            CreateSpriteObject(name, sprite, position, sortingOffset);
        }

        private void EnsureLandmark(string landmarkId, string objectName, string title, string journalText, Sprite sprite, Vector3 position)
        {
            if (landmarks.ContainsKey(landmarkId)) return;

            GameObject landmarkObject = GameObject.Find(objectName);
            if (landmarkObject == null) landmarkObject = CreateSpriteObject(objectName, sprite, position, 10);

            DiscoverableLandmark landmark = landmarkObject.GetComponent<DiscoverableLandmark>();
            if (landmark == null) landmark = landmarkObject.AddComponent<DiscoverableLandmark>();
            landmark.Configure(landmarkId, title, journalText);
            landmarks[landmarkId] = landmark;
        }

        private static void EnsurePlayerVisual(GameObject player)
        {
            if (player == null) return;

            SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) spriteRenderer = player.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = PrototypePixelArtFactory.Player();
            spriteRenderer.color = Color.white;
            spriteRenderer.enabled = true;
            spriteRenderer.shadowCastingMode = ShadowCastingMode.Off;
            spriteRenderer.receiveShadows = false;

            YSortSprite sorter = player.GetComponent<YSortSprite>();
            if (sorter == null) sorter = player.AddComponent<YSortSprite>();
            sorter.enabled = true;
            sorter.Configure(50);

            Renderer[] renderers = player.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null || renderer == spriteRenderer) continue;

                renderer.shadowCastingMode = ShadowCastingMode.Off;
                renderer.receiveShadows = false;
                renderer.enabled = false;
            }
        }

        private static void SetRendererAlpha(GameObject gameObject, float alpha)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer == null) return;
            Color color = renderer.material.color;
            color.a = alpha;
            renderer.material.color = color;
        }
    }
}
