using UnityEngine;
using TheOldRoad.Gathering;
using TheOldRoad.Farming;
using TheOldRoad.Inventory;
using TheOldRoad.Input;
using TheOldRoad.UI;
using TheOldRoad.Combat;
using TheOldRoad.Audio;
using TheOldRoad.World;

namespace TheOldRoad.Player
{
    /// <summary>
    /// Mouse cursor & contextual tool targeting controller.
    /// - Axe: Hover over trees -> highlights with glowing outline -> Left-click to chop (within reach).
    /// - Pickaxe: Hover over rocks/ore -> highlights with glowing outline -> Left-click to mine (within reach).
    /// - Hoe: Snaps to ground tile under mouse -> Green reticle in-range (<= 2.6m) to till, Red reticle if out-of-range (cannot till).
    /// - Watering Can: Hover over farm plots -> Left-click to water (within range).
    /// - Seeds: Hover over tilled plots -> Left-click to plant (within range).
    /// - Harvest: Click ripe crops to harvest immediately.
    /// </summary>
    public sealed class PlayerMouseToolTargeter : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float maxToolReachDistance = 2.8f;
        [SerializeField, Min(1f)] private float maxHoeRange = 2.6f;

        private InventorySession inventorySession;
        private PlayerGatheringInteractor gatheringInteractor;
        private PlayerFarmingInteractor farmingInteractor;
        private InventoryDebugHud hud;
        private Camera mainCamera;

        private ResourceNode currentHoveredNode;
        private FarmPlotController currentHoveredPlot;
        private Vector2 currentHoveredTile;
        private bool isTileInRange;
        private string activeToolId = string.Empty;
        private string mouseActionHint = string.Empty;

        public void Configure(
            InventorySession session,
            PlayerGatheringInteractor gathering,
            PlayerFarmingInteractor farming)
        {
            inventorySession = session;
            gatheringInteractor = gathering;
            farmingInteractor = farming;
            mainCamera = Camera.main;
        }

        private void Awake()
        {
            mainCamera = Camera.main;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnEnable()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnDisable()
        {
            Cursor.visible = true;
        }

        private void OnDestroy()
        {
            Cursor.visible = true;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void Update()
        {
            if (mainCamera == null) mainCamera = Camera.main;
            if (hud == null) hud = FindAnyObjectByType<InventoryDebugHud>();
            if (gatheringInteractor == null) gatheringInteractor = GetComponent<PlayerGatheringInteractor>();
            if (farmingInteractor == null) farmingInteractor = GetComponent<PlayerFarmingInteractor>();
            if (inventorySession == null) inventorySession = FindAnyObjectByType<InventorySession>();

            if (!Cursor.visible)
            {
                Cursor.visible = true;
            }

            if (hud != null && hud.IsAnyOverlayOpen)
            {
                ClearHoveredState();
                return;
            }

            activeToolId = hud != null ? hud.SelectedItemId : string.Empty;
            Vector3 mouseScreen = PrototypeInput.MousePosition;

            // Don't raycast into world if clicking on bottom hotbar
            if (mouseScreen.y < 80f && mouseScreen.x > (Screen.width * 0.2f) && mouseScreen.x < (Screen.width * 0.8f))
            {
                ClearHoveredState();
                return;
            }

            if (mainCamera == null) return;
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreen);
            mouseWorld.z = 0f;

            float distToPlayer = Vector2.Distance(transform.position, mouseWorld);

            // 1. Check for ResourceNode (Trees, Rocks, Iron Ore)
            ResourceNode hitNode = FindResourceNodeUnderMouse(mouseWorld);
            UpdateResourceNodeHover(hitNode, distToPlayer);

            // 2. Check for FarmPlot / Ground Tile (Hoe, Water, Seed, Harvest)
            FarmPlotController hitPlot = FindFarmPlotUnderMouse(mouseWorld);
            UpdateFarmPlotAndTileHover(hitPlot, mouseWorld, distToPlayer);

            // 3. Handle Left Click Actions
            if (PrototypeInput.GetMouseButtonDown(0))
            {
                HandleMouseLeftClick(mouseWorld, distToPlayer);
            }
        }

        private void UpdateResourceNodeHover(ResourceNode hitNode, float distToPlayer)
        {
            bool isAxe = activeToolId == "item.tool-axe";
            bool isPickaxe = activeToolId == "item.tool-pickaxe";

            if (hitNode != null && !hitNode.IsHarvested)
            {
                bool isTree = hitNode.ResourceItemId == "item.wood";
                bool isRockOrOre = hitNode.ResourceItemId == "item.stone" || hitNode.ResourceItemId == "item.iron-ore";

                // Highlight tree when holding Axe, or rock when holding Pickaxe, or when directly hovering
                if (isTree || isRockOrOre || isAxe || isPickaxe)
                {
                    if (currentHoveredNode != hitNode)
                    {
                        if (currentHoveredNode != null) currentHoveredNode.SetHighlighted(false);
                        currentHoveredNode = hitNode;
                        currentHoveredNode.SetHighlighted(true);
                    }

                    bool inRange = distToPlayer <= maxToolReachDistance;
                    if (isTree)
                    {
                        mouseActionHint = inRange
                            ? (LocalizationRuntime.IsVietnamese ? "🪓 Nhấp để chặt cây" : "🪓 Click to Chop Tree")
                            : (LocalizationRuntime.IsVietnamese ? "🚫 Cây ở quá xa" : "🚫 Tree Out of Reach");
                    }
                    else if (isRockOrOre)
                    {
                        mouseActionHint = inRange
                            ? (LocalizationRuntime.IsVietnamese ? "⛏️ Nhấp để khai thác đá/quặng" : "⛏️ Click to Mine Rock")
                            : (LocalizationRuntime.IsVietnamese ? "🚫 Mỏ đá ở quá xa" : "🚫 Rock Out of Reach");
                    }
                    return;
                }
            }

            if (currentHoveredNode != null)
            {
                currentHoveredNode.SetHighlighted(false);
                currentHoveredNode = null;
            }
        }

        private void UpdateFarmPlotAndTileHover(FarmPlotController hitPlot, Vector3 mouseWorld, float distToPlayer)
        {
            currentHoveredPlot = hitPlot;
            currentHoveredTile = new Vector2(Mathf.Round(mouseWorld.x), Mathf.Round(mouseWorld.y));

            float tileDist = Vector2.Distance(transform.position, currentHoveredTile);
            isTileInRange = tileDist <= maxHoeRange;

            bool isHoe = activeToolId == "item.tool-hoe";
            bool isWateringCan = activeToolId == "item.watering-can";
            bool isSeed = activeToolId.StartsWith("item.seed-");

            if (hitPlot != null)
            {
                if (hitPlot.IsHarvestReady)
                {
                    mouseActionHint = isTileInRange
                        ? (LocalizationRuntime.IsVietnamese ? "🎉 Nhấp để thu hoạch nông sản!" : "🎉 Click to Harvest!")
                        : (LocalizationRuntime.IsVietnamese ? "🚫 Quá xa để thu hoạch" : "🚫 Out of Reach");
                    return;
                }

                if (isWateringCan && hitPlot.IsTilled && !hitPlot.IsWatered)
                {
                    mouseActionHint = isTileInRange
                        ? (LocalizationRuntime.IsVietnamese ? "💧 Nhấp để tưới nước" : "💧 Click to Water")
                        : (LocalizationRuntime.IsVietnamese ? "🚫 Quá xa để tưới nước" : "🚫 Out of Reach");
                    return;
                }

                if (isSeed && hitPlot.IsTilled && string.IsNullOrEmpty(hitPlot.PlantedCropId))
                {
                    CropDefinition seedDef = PrototypeCropCatalog.Get(activeToolId.Replace("item.seed-", ""));
                    string sName = seedDef != null ? seedDef.DisplayName : "Hạt giống";
                    mouseActionHint = isTileInRange
                        ? (LocalizationRuntime.IsVietnamese ? $"🌾 Nhấp để gieo {sName}" : $"🌾 Click to Plant {sName}")
                        : (LocalizationRuntime.IsVietnamese ? "🚫 Quá xa để gieo hạt" : "🚫 Out of Reach");
                    return;
                }

                if (isHoe && !hitPlot.IsTilled)
                {
                    mouseActionHint = isTileInRange
                        ? (LocalizationRuntime.IsVietnamese ? "🌱 Nhấp để xới đất" : "🌱 Click to Till Soil")
                        : (LocalizationRuntime.IsVietnamese ? "🚫 Quá phạm vi xới đất" : "🚫 Out of Hoe Range");
                    return;
                }
            }
            else if (isHoe)
            {
                mouseActionHint = isTileInRange
                    ? (LocalizationRuntime.IsVietnamese ? "🌱 Nhấp để xới đất ô này" : "🌱 Click to Till Tile")
                    : (LocalizationRuntime.IsVietnamese ? "🚫 Quá phạm vi xới đất" : "🚫 Out of Hoe Range");
            }
            else if (currentHoveredNode == null)
            {
                mouseActionHint = string.Empty;
            }
        }

        private void HandleMouseLeftClick(Vector3 mouseWorld, float distToPlayer)
        {
            // 1. Gather / Chop tree / Mine rock under mouse
            if (currentHoveredNode != null && !currentHoveredNode.IsHarvested)
            {
                if (distToPlayer <= maxToolReachDistance)
                {
                    if (gatheringInteractor != null)
                    {
                        gatheringInteractor.TryGatherNode(currentHoveredNode);
                    }
                }
                else
                {
                    FloatingTextController.Spawn(
                        LocalizationRuntime.IsVietnamese ? "Quá xa, hãy bước lại gần hơn!" : "Too far, walk closer!",
                        currentHoveredNode.transform.position + Vector3.up * 1.0f,
                        new Color(1f, 0.4f, 0.4f),
                        1.8f);
                    AudioManager.PlayUiClick();
                }
                return;
            }

            // 2. Harvest ready crop
            if (currentHoveredPlot != null && currentHoveredPlot.IsHarvestReady)
            {
                if (isTileInRange && inventorySession != null)
                {
                    currentHoveredPlot.TryHarvest(inventorySession.Runtime);
                }
                else if (!isTileInRange)
                {
                    ShowOutOfRangeWarning(currentHoveredPlot.transform.position);
                }
                return;
            }

            // 3. Hoe Tilling on Grid
            if (activeToolId == "item.tool-hoe")
            {
                if (isTileInRange)
                {
                    if (currentHoveredPlot != null)
                    {
                        if (!currentHoveredPlot.IsTilled)
                        {
                            currentHoveredPlot.TryTillSoil();
                        }
                        else
                        {
                            FloatingTextController.Spawn(
                                LocalizationRuntime.IsVietnamese ? "Ô đất này đã được xới rồi!" : "This plot is already tilled!",
                                currentHoveredPlot.transform.position + Vector3.up * 0.8f,
                                new Color(0.9f, 0.9f, 0.4f),
                                1.5f);
                        }
                    }
                    else
                    {
                        // Spawn and till fresh plot at clicked grid tile
                        GameObject newPlotObj = new GameObject($"Farm Plot [{currentHoveredTile.x:F0},{currentHoveredTile.y:F0}]");
                        newPlotObj.transform.position = new Vector3(currentHoveredTile.x, currentHoveredTile.y, 0f);
                        FarmPlotController newPlot = newPlotObj.AddComponent<FarmPlotController>();
                        string plotId = $"plot.custom.{currentHoveredTile.x:F0}.{currentHoveredTile.y:F0}";
                        newPlot.Configure(plotId, false, false, string.Empty, 0f, 0);
                        newPlot.TryTillSoil();
                    }
                }
                else
                {
                    ShowOutOfRangeWarning(new Vector3(currentHoveredTile.x, currentHoveredTile.y, 0f));
                }
                return;
            }

            // 4. Watering Can
            if (activeToolId == "item.watering-can" && currentHoveredPlot != null)
            {
                if (isTileInRange)
                {
                    if (currentHoveredPlot.IsTilled && !currentHoveredPlot.IsWatered)
                    {
                        currentHoveredPlot.TryWaterSoil();
                    }
                }
                else
                {
                    ShowOutOfRangeWarning(currentHoveredPlot.transform.position);
                }
                return;
            }

            // 5. Seeds Planting
            if (activeToolId.StartsWith("item.seed-") && currentHoveredPlot != null)
            {
                if (isTileInRange)
                {
                    if (currentHoveredPlot.IsTilled && string.IsNullOrEmpty(currentHoveredPlot.PlantedCropId))
                    {
                        if (inventorySession != null && inventorySession.Runtime != null && inventorySession.Runtime.GetQuantity(activeToolId) > 0)
                        {
                            if (currentHoveredPlot.TryPlantSeed(activeToolId))
                            {
                                inventorySession.Runtime.TryRemove(activeToolId, 1);
                            }
                        }
                    }
                }
                else
                {
                    ShowOutOfRangeWarning(currentHoveredPlot.transform.position);
                }
                return;
            }
        }

        private void ShowOutOfRangeWarning(Vector3 worldPos)
        {
            FloatingTextController.Spawn(
                LocalizationRuntime.IsVietnamese ? "🚫 Quá phạm vi, hãy bước lại gần hơn!" : "🚫 Out of range, step closer!",
                worldPos + Vector3.up * 0.8f,
                new Color(1f, 0.35f, 0.35f),
                1.6f);
            AudioManager.PlayUiClick();
        }

        private ResourceNode FindResourceNodeUnderMouse(Vector3 mouseWorld)
        {
            ResourceNode[] nodes = FindObjectsByType<ResourceNode>(FindObjectsInactive.Exclude);
            ResourceNode best = null;
            float bestDist = 1.35f;

            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] == null || nodes[i].IsHarvested) continue;
                float d = Vector2.Distance(mouseWorld, nodes[i].transform.position);
                if (d <= bestDist)
                {
                    bestDist = d;
                    best = nodes[i];
                }
            }
            return best;
        }

        private FarmPlotController FindFarmPlotUnderMouse(Vector3 mouseWorld)
        {
            FarmPlotController[] plots = FindObjectsByType<FarmPlotController>(FindObjectsInactive.Exclude);
            FarmPlotController best = null;
            float bestDist = 1.05f;

            for (int i = 0; i < plots.Length; i++)
            {
                if (plots[i] == null) continue;
                float d = Vector2.Distance(mouseWorld, plots[i].transform.position);
                if (d <= bestDist)
                {
                    bestDist = d;
                    best = plots[i];
                }
            }
            return best;
        }

        private void ClearHoveredState()
        {
            if (currentHoveredNode != null)
            {
                currentHoveredNode.SetHighlighted(false);
                currentHoveredNode = null;
            }
            currentHoveredPlot = null;
            mouseActionHint = string.Empty;
        }

        private void OnGUI()
        {
            if (Event.current.type != EventType.Repaint) return;

            Vector3 mouseScreen = PrototypeInput.MousePosition;
            bool overlayOpen = (hud != null && hud.IsAnyOverlayOpen) || GameStartMenuController.IsOpen;

            // 1. Draw World Reticle & Tooltip only during normal gameplay (when no fullscreen overlay is open)
            if (!overlayOpen && mainCamera != null)
            {
                // 1. Draw Tile Reticle for Hoe / Watering / Seeds
                bool isHoe = activeToolId == "item.tool-hoe";
                bool isWater = activeToolId == "item.watering-can";
                bool isSeed = activeToolId.StartsWith("item.seed-");

                if (isHoe || isWater || isSeed || (currentHoveredPlot != null && currentHoveredPlot.IsHarvestReady))
                {
                    Vector3 tileCenterWorld = currentHoveredPlot != null
                        ? currentHoveredPlot.transform.position
                        : new Vector3(currentHoveredTile.x, currentHoveredTile.y, 0f);

                    Vector3 screenMin = mainCamera.WorldToScreenPoint(tileCenterWorld + new Vector3(-0.55f, 0.55f, 0f));
                    Vector3 screenMax = mainCamera.WorldToScreenPoint(tileCenterWorld + new Vector3(0.55f, -0.55f, 0f));

                    float x = screenMin.x;
                    float y = Screen.height - screenMin.y;
                    float w = Mathf.Abs(screenMax.x - screenMin.x);
                    float h = Mathf.Abs(screenMin.y - screenMax.y);
                    Rect tileRect = new Rect(x, y, w, h);

                    Color reticleColor = isTileInRange
                        ? (isHoe ? new Color(0.35f, 0.95f, 0.40f, 0.45f) : (isWater ? new Color(0.25f, 0.75f, 1f, 0.45f) : new Color(1f, 0.85f, 0.25f, 0.45f)))
                        : new Color(0.95f, 0.20f, 0.20f, 0.40f);

                    Color borderColor = isTileInRange ? Color.white : new Color(1f, 0.4f, 0.4f, 1f);

                    // Draw filled transparent reticle & border
                    Texture2D whiteTex = Texture2D.whiteTexture;
                    Color prevColor = GUI.color;

                    GUI.color = reticleColor;
                    GUI.DrawTexture(tileRect, whiteTex);

                    GUI.color = borderColor;
                    // Border lines
                    GUI.DrawTexture(new Rect(tileRect.x, tileRect.y, tileRect.width, 2f), whiteTex);
                    GUI.DrawTexture(new Rect(tileRect.x, tileRect.yMax - 2f, tileRect.width, 2f), whiteTex);
                    GUI.DrawTexture(new Rect(tileRect.x, tileRect.y, 2f, tileRect.height), whiteTex);
                    GUI.DrawTexture(new Rect(tileRect.xMax - 2f, tileRect.y, 2f, tileRect.height), whiteTex);

                    GUI.color = prevColor;
                }

                // 2. Draw Contextual Cursor Tooltip
                if (!string.IsNullOrEmpty(mouseActionHint))
                {
                    float cursorX = mouseScreen.x + 18f;
                    float cursorY = Screen.height - mouseScreen.y - 12f;
                    float tipWidth = mouseActionHint.Length * 7.5f + 16f;
                    Rect tipRect = new Rect(cursorX, cursorY, tipWidth, 24f);

                    Color prevColor = GUI.color;
                    GUI.color = new Color(0.04f, 0.035f, 0.03f, 0.88f);
                    GUI.DrawTexture(tipRect, Texture2D.whiteTexture);

                    GUI.color = isTileInRange || (currentHoveredNode != null && Vector2.Distance(transform.position, currentHoveredNode.transform.position) <= maxToolReachDistance)
                        ? new Color(1f, 0.85f, 0.35f, 1f)
                        : new Color(1f, 0.4f, 0.4f, 1f);

                    GUI.DrawTexture(new Rect(tipRect.x, tipRect.y, tipRect.width, 1.5f), Texture2D.whiteTexture);
                    GUI.DrawTexture(new Rect(tipRect.x, tipRect.yMax - 1.5f, tipRect.width, 1.5f), Texture2D.whiteTexture);
                    GUI.DrawTexture(new Rect(tipRect.x, tipRect.y, 1.5f, tipRect.height), Texture2D.whiteTexture);
                    GUI.DrawTexture(new Rect(tipRect.xMax - 1.5f, tipRect.y, 1.5f, tipRect.height), Texture2D.whiteTexture);

                    TheOldRoad.UI.UiFontHelper.EnsureGlobalSkinFont();
                    GUIStyle style = new GUIStyle(GUI.skin.label)
                    {
                        font = TheOldRoad.UI.UiFontHelper.CleanFont,
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 12,
                        fontStyle = FontStyle.Bold
                    };
                    style.normal.textColor = GUI.color;
                    GUI.Label(tipRect, mouseActionHint, style);

                    GUI.color = prevColor;
                }
            }
        }
    }
}
