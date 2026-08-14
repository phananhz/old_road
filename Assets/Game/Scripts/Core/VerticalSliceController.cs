using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TheOldRoad.Building;
using TheOldRoad.Construction;
using TheOldRoad.Crafting;
using TheOldRoad.Gathering;
using TheOldRoad.Input;
using TheOldRoad.Inventory;
using TheOldRoad.Items;
using TheOldRoad.Player;
using TheOldRoad.Save;
using TheOldRoad.Time;
using TheOldRoad.UI;
using TheOldRoad.World;

namespace TheOldRoad.Core
{
    public sealed class VerticalSliceController : MonoBehaviour
    {
        private const string CabinId = "building.cabin";
        private const int WorldSeed = 43129;
        private static readonly Vector2 WorldMin = new Vector2(-60f, -36f);
        private static readonly Vector2 WorldMax = new Vector2(60f, 36f);

        [SerializeField] private BuildingDefinition cabinDefinition;
        [SerializeField] private RecipeDefinition cabinPlankRecipe;
        [SerializeField] private Vector2Int buildAreaMin = new Vector2Int(-50, -28);
        [SerializeField] private Vector2Int buildAreaMax = new Vector2Int(50, 28);

        private readonly Dictionary<string, ResourceNode> resourceNodes = new Dictionary<string, ResourceNode>();
        private readonly Dictionary<string, DiscoverableLandmark> landmarks = new Dictionary<string, DiscoverableLandmark>();
        private readonly Dictionary<string, LootChest> lootChests = new Dictionary<string, LootChest>();
        private readonly Dictionary<string, ConstructionJob> constructionJobs = new Dictionary<string, ConstructionJob>();
        private readonly HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();

        private InventorySession inventorySession;
        private SaveRepository saveRepository;
        private IClock clock;
        private string saveStatus = "Save not initialized.";

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

        private void Awake()
        {
            clock = new SystemClock();
            saveRepository = SaveRepository.CreateDefault();
            EnsureDefinitions();
            BuildRuntimeScene();
            LoadState();
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

        public bool TryBeginConstruction(BuildingDefinition definition, Vector2Int origin, out string status)
        {
            status = "No building selected.";
            if (definition == null) return false;

            if (!IsPlacementValid(origin, definition.Footprint))
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
            MarkOccupied(job.Placement, definition.Footprint);
            CreateConstructionSite(job, definition);
            SaveNow();
            status = "Construction started.";
            return true;
        }

        public void NotifyResourceHarvested(ResourceNode node)
        {
            if (node == null) return;
            SaveNow();
        }

        public void NotifyCrafted(RecipeDefinition recipe)
        {
            if (recipe == null) return;
            SaveNow();
        }

        public SaveData CreateSaveData()
        {
            return new SaveData
            {
                saveVersion = SaveSerializer.CurrentVersion,
                inventory = Inventory.ToSaveEntries(),
                constructionJobs = constructionJobs.Values.Select(job => job.ToSaveEntry()).ToArray(),
                resourceNodes = resourceNodes.Values
                    .Select(node => new ResourceNodeSaveEntry { nodeId = node.NodeId, harvested = node.IsHarvested })
                    .ToArray(),
                landmarks = landmarks.Values
                    .Select(landmark => new LandmarkSaveEntry { landmarkId = landmark.LandmarkId, discovered = landmark.IsDiscovered })
                    .ToArray(),
                lootChests = lootChests.Values
                    .Select(chest => new LootChestSaveEntry { chestId = chest.ChestId, opened = chest.IsOpened })
                    .ToArray()
            };
        }

        public void SaveNow()
        {
            if (saveRepository == null) return;
            saveRepository.TrySave(CreateSaveData(), out saveStatus);
        }

        public void NotifyLandmarkDiscovered(DiscoverableLandmark landmark)
        {
            if (landmark == null) return;
            LastDiscoveryStatus = "Journal updated: " + landmark.Title + ".";
            SaveNow();
        }

        public void NotifyLootChestOpened(LootChest chest)
        {
            if (chest == null) return;
            LastDiscoveryStatus = "Loot found: " + chest.DisplayName + ".";
            SaveNow();
        }

        private (string text, bool completed)[] BuildObjectiveStates()
        {
            RefreshConstructionJobs();
            InventoryRuntime inventory = Inventory;

            return new[]
            {
                ("Inspect an old-road landmark", DiscoveredLandmarkCount > 0),
                ("Open an old chest", OpenedLootChestCount > 0),
                ("Gather 3 wood", inventory != null && inventory.GetQuantity("item.wood") >= 3),
                ("Gather 2 stone", inventory != null && inventory.GetQuantity("item.stone") >= 2),
                ("Forage any wild food or herb", inventory != null && HasAnyForagedItem(inventory)),
                ("Craft 1 cabin plank", inventory != null && inventory.GetQuantity("item.cabin-plank") >= 1),
                ("Start cabin construction", constructionJobs.Count > 0),
                ("Complete the first cabin", constructionJobs.Values.Any(job => job != null && job.state == ConstructionState.Completed))
            };
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
            EnsureDayNightLighting(mainCamera, gameTime, inventorySession);
            EnsureProceduralResourceNodes();
            BuildingPlacementController placement = EnsureBuildingPlacement(mainCamera, inventorySession);
            EnsureHud(inventorySession, placement);
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

            foreach (ConstructionSaveEntry constructionSave in data.constructionJobs ?? Array.Empty<ConstructionSaveEntry>())
            {
                if (constructionSave == null || string.IsNullOrWhiteSpace(constructionSave.constructionId)) continue;
                ConstructionJob job = ConstructionJob.FromSaveEntry(constructionSave);
                job.Refresh(clock.NowUnixSeconds);
                constructionJobs[job.constructionId] = job;
                MarkOccupied(job.Placement, cabinDefinition.Footprint);
                CreateConstructionSite(job, cabinDefinition);
            }
        }

        private void EnsureDefinitions()
        {
            if (cabinDefinition == null)
            {
                cabinDefinition = ScriptableObject.CreateInstance<BuildingDefinition>();
                cabinDefinition.ConfigureForPrototype(
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

            if (cabinPlankRecipe != null) return;

            cabinPlankRecipe = ScriptableObject.CreateInstance<RecipeDefinition>();
            cabinPlankRecipe.ConfigureForPrototype(
                "recipe.cabin-planks",
                new[] { new IngredientRequirement { itemId = "item.wood", quantity = 2 } },
                "item.cabin-plank",
                1,
                0f,
                string.Empty);
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
            return mainCamera;
        }

        private void EnsureCameraFollow(Camera mainCamera)
        {
            if (mainCamera == null) return;

            PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
            if (player == null) return;

            CameraFollow2D follow = mainCamera.GetComponent<CameraFollow2D>();
            if (follow == null) follow = mainCamera.gameObject.AddComponent<CameraFollow2D>();
            follow.Configure(player.transform, new Vector2(WorldMin.x + 7f, WorldMin.y + 5f), new Vector2(WorldMax.x - 7f, WorldMax.y - 5f), 0.12f);
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
                "landmark.ruin.south",
                "South Ruin Gate",
                "South Ruin Gate",
                "Broken stones mark a ruined southern gate. The road once continued beyond it.",
                PrototypePixelArtFactory.RuinedArch(),
                new Vector3(34f, -25f, 0f));
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
                EnsurePlayerVisual(player);
            }

            KeyboardPlayerInputSource input = player.GetComponent<KeyboardPlayerInputSource>();
            if (input == null) input = player.AddComponent<KeyboardPlayerInputSource>();

            MobileJoystickInputSource joystick = player.GetComponent<MobileJoystickInputSource>();
            if (joystick == null) joystick = player.AddComponent<MobileJoystickInputSource>();

            CompositePlayerInputSource compositeInput = player.GetComponent<CompositePlayerInputSource>();
            if (compositeInput == null) compositeInput = player.AddComponent<CompositePlayerInputSource>();
            compositeInput.Configure(input, joystick);

            movement.Configure(compositeInput, 3f);

            PlayerVitals vitals = player.GetComponent<PlayerVitals>();
            if (vitals == null) vitals = player.AddComponent<PlayerVitals>();
            vitals.Configure(20, 20);

            PlayerGatheringInteractor gathering = player.GetComponent<PlayerGatheringInteractor>();
            if (gathering == null) gathering = player.AddComponent<PlayerGatheringInteractor>();
            gathering.Configure(session, this, 1.25f);

            PlayerCraftingInteractor crafting = player.GetComponent<PlayerCraftingInteractor>();
            if (crafting == null) crafting = player.AddComponent<PlayerCraftingInteractor>();
            crafting.Configure(session, this, cabinPlankRecipe);

            PlayerLandmarkInteractor landmarkInteractor = player.GetComponent<PlayerLandmarkInteractor>();
            if (landmarkInteractor == null) landmarkInteractor = player.AddComponent<PlayerLandmarkInteractor>();
            landmarkInteractor.Configure(this, 1.35f);

            PlayerLootInteractor lootInteractor = player.GetComponent<PlayerLootInteractor>();
            if (lootInteractor == null) lootInteractor = player.AddComponent<PlayerLootInteractor>();
            lootInteractor.Configure(session, this, 1.25f);

            PlayerCabinInteractor cabinInteractor = player.GetComponent<PlayerCabinInteractor>();
            if (cabinInteractor == null) cabinInteractor = player.AddComponent<PlayerCabinInteractor>();
            cabinInteractor.Configure(cabinInterior, gameTime, 2.25f);
        }

        private void EnsureDayNightLighting(Camera mainCamera, GameTimeController gameTime, InventorySession session)
        {
            if (mainCamera == null) return;

            DayNightLightingController lighting = mainCamera.GetComponent<DayNightLightingController>();
            if (lighting == null) lighting = mainCamera.gameObject.AddComponent<DayNightLightingController>();
            lighting.Configure(gameTime, session);
        }

        private void EnsureResourceNode(string nodeId, string displayName, Vector3 position, string itemId, int amount, Color color)
        {
            if (resourceNodes.ContainsKey(nodeId)) return;

            foreach (ResourceNode existingNode in FindObjectsByType<ResourceNode>(FindObjectsInactive.Exclude))
            {
                if (existingNode == null || existingNode.NodeId != nodeId) continue;
                resourceNodes[nodeId] = existingNode;
                return;
            }

            Sprite sprite = GetResourceSprite(itemId);
            GameObject nodeObject = CreateSpriteObject(displayName, sprite, position, 0);
            SpriteRenderer renderer = nodeObject.GetComponent<SpriteRenderer>();
            if (renderer != null) renderer.color = color;

            ResourceNode node = nodeObject.AddComponent<ResourceNode>();
            node.Configure(nodeId, itemId, amount);
            resourceNodes[nodeId] = node;
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
            EnsureResourceNode("node.iron.01", "Exposed Iron Vein", new Vector3(28f, -18f, 0f), "item.iron-ore", 1, PrototypeItemCatalog.Get("item.iron-ore").Color);
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
                EnsureResourceNode(id, displayPrefix + " " + (created + 1), position, itemId, amount, Color.white);
                created++;
            }
        }

        private static float RoadCenterY(float worldX)
        {
            return 1.4f * Mathf.Sin(worldX * 0.34f) + 0.8f * Mathf.Sin(worldX * 0.11f);
        }

        private void EnsureLootChests()
        {
            EnsureLootChest("chest.road.01", "Roadside Cache", new Vector3(-6f, -2.2f, 0f), "item.wood", 2);
            EnsureLootChest("chest.camp.01", "Abandoned Camp Chest", new Vector3(9.8f, -11.4f, 0f), "item.cabin-plank", 1);
            EnsureLootChest("chest.ruin.01", "South Ruin Chest", new Vector3(36f, -24.2f, 0f), "item.stone", 3);
            EnsureLootChest("chest.shrine.01", "Hunter Shrine Cache", new Vector3(-40f, 24f, 0f), "item.wood", 3);
            EnsureLootChest("chest.bell.01", "Bell Marker Cache", new Vector3(44.2f, 7.4f, 0f), "item.stone", 2);
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

        private void CreateConstructionSite(ConstructionJob job, BuildingDefinition definition)
        {
            GameObject site = CreateSpriteObject("Cabin Construction Site", PrototypePixelArtFactory.CabinConstruction(0), new Vector3(job.gridX, job.gridY, 0f), 0);
            site.name = "Cabin Construction Site";
            site.AddComponent<ConstructionSite>().Configure(job, definition, clock);
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
            SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) spriteRenderer = player.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = PrototypePixelArtFactory.Player();

            YSortSprite sorter = player.GetComponent<YSortSprite>();
            if (sorter == null) sorter = player.AddComponent<YSortSprite>();
            sorter.Configure(50);

            MeshRenderer meshRenderer = player.GetComponentInChildren<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
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
