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
        private static readonly Vector2 WorldMin = new Vector2(-30f, -18f);
        private static readonly Vector2 WorldMax = new Vector2(30f, 18f);

        [SerializeField] private BuildingDefinition cabinDefinition;
        [SerializeField] private RecipeDefinition cabinPlankRecipe;
        [SerializeField] private Vector2Int buildAreaMin = new Vector2Int(-20, -11);
        [SerializeField] private Vector2Int buildAreaMax = new Vector2Int(20, 11);

        private readonly Dictionary<string, ResourceNode> resourceNodes = new Dictionary<string, ResourceNode>();
        private readonly Dictionary<string, ConstructionJob> constructionJobs = new Dictionary<string, ConstructionJob>();
        private readonly HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();

        private InventorySession inventorySession;
        private SaveRepository saveRepository;
        private IClock clock;
        private string saveStatus = "Save not initialized.";

        public InventoryRuntime Inventory => inventorySession.Runtime;
        public string SaveStatus => saveStatus;
        public int ActiveConstructionCount => constructionJobs.Count;
        public BuildingDefinition CabinDefinition => cabinDefinition;
        public RecipeDefinition CabinPlankRecipe => cabinPlankRecipe;

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
                    .ToArray()
            };
        }

        public void SaveNow()
        {
            if (saveRepository == null) return;
            saveRepository.TrySave(CreateSaveData(), out saveStatus);
        }

        private void BuildRuntimeScene()
        {
            EnsureInputBridge();
            EnsureGameTime();
            Camera mainCamera = EnsureCamera();
            EnsureGround();
            EnsureLandmarks();
            inventorySession = EnsureInventorySession();
            EnsurePlayer(inventorySession);
            EnsureCameraFollow(mainCamera);
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
            BuildingPlacementController placement = EnsureBuildingPlacement(mainCamera, inventorySession);
            EnsureHud(inventorySession, placement);
        }

        private void EnsureGameTime()
        {
            if (FindAnyObjectByType<GameTimeController>() != null) return;

            GameObject timeObject = new GameObject("Game Time");
            timeObject.AddComponent<GameTimeController>();
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

            foreach (ResourceNodeSaveEntry resourceSave in data.resourceNodes)
            {
                if (resourceSave == null || string.IsNullOrWhiteSpace(resourceSave.nodeId)) continue;
                if (resourceNodes.TryGetValue(resourceSave.nodeId, out ResourceNode node))
                {
                    node.SetHarvested(resourceSave.harvested);
                    SetRendererAlpha(node.gameObject, resourceSave.harvested ? 0.35f : 1f);
                }
            }

            foreach (ConstructionSaveEntry constructionSave in data.constructionJobs)
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

        private void EnsureLandmarks()
        {
            EnsureDecoration("Northern Waystone", PrototypePixelArtFactory.Waystone(), new Vector3(-21f, 13f, 0f), 10);
            EnsureDecoration("Old Road Sign", PrototypePixelArtFactory.RoadSign(), new Vector3(-8f, 1.7f, 0f), 10);
            EnsureDecoration("Broken Watch Arch", PrototypePixelArtFactory.RuinedArch(), new Vector3(16f, 8.5f, 0f), 10);
            EnsureDecoration("River Footbridge", PrototypePixelArtFactory.Footbridge(), new Vector3(-14f, -4.2f, 0f), 10);
            EnsureDecoration("Abandoned Camp", PrototypePixelArtFactory.Campfire(), new Vector3(8f, -11f, 0f), 10);
        }

        private InventorySession EnsureInventorySession()
        {
            InventorySession session = FindAnyObjectByType<InventorySession>();
            if (session != null) return session;

            GameObject sessionObject = new GameObject("InventorySession");
            return sessionObject.AddComponent<InventorySession>();
        }

        private void EnsurePlayer(InventorySession session)
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

            Sprite sprite = itemId == "item.wood" ? PrototypePixelArtFactory.Tree() : PrototypePixelArtFactory.Rock();
            GameObject nodeObject = CreateSpriteObject(displayName, sprite, position, 0);

            ResourceNode node = nodeObject.AddComponent<ResourceNode>();
            node.Configure(nodeId, itemId, amount);
            resourceNodes[nodeId] = node;
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
