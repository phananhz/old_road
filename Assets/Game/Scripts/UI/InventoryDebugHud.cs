using UnityEngine;
using TheOldRoad.Building;
using TheOldRoad.Construction;
using TheOldRoad.Core;
using TheOldRoad.Crafting;
using TheOldRoad.Gathering;
using TheOldRoad.Input;
using TheOldRoad.Inventory;
using TheOldRoad.Items;
using TheOldRoad.NPC;
using TheOldRoad.Player;
using TheOldRoad.Time;
using TheOldRoad.World;

namespace TheOldRoad.UI
{
    /// <summary>Polished prototype HUD: medieval panels, health, minimap, hotbar, overlays, and prompts.</summary>
    public sealed class InventoryDebugHud : MonoBehaviour
    {
        private static readonly Vector2 WorldMin = new Vector2(-60f, -36f);
        private static readonly Vector2 WorldMax = new Vector2(60f, 36f);
        private static readonly Color Ink = new Color(0.05f, 0.045f, 0.04f, 0.94f);
        private static readonly Color InkSoft = new Color(0.08f, 0.07f, 0.06f, 0.88f);
        private static readonly Color Gold = new Color(0.95f, 0.74f, 0.34f, 1f);
        private static readonly Color GoldDim = new Color(0.58f, 0.43f, 0.22f, 1f);
        private static readonly Color Parchment = new Color(0.87f, 0.78f, 0.58f, 1f);
        private static readonly Color MutedText = new Color(0.75f, 0.71f, 0.62f, 1f);
        private static readonly Color PanelWarm = new Color(0.13f, 0.085f, 0.045f, 0.96f);
        private static readonly Color Shadow = new Color(0f, 0f, 0f, 0.42f);
        private static readonly Color Blood = new Color(0.72f, 0.08f, 0.06f, 1f);
        private static readonly Color BloodDark = new Color(0.20f, 0.035f, 0.03f, 1f);
        private const float PromptVisibleSeconds = 4f;

        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private BuildingPlacementController placementController;
        [SerializeField] private VerticalSliceController sliceController;

        private GUIStyle gameTitleStyle;
        private GUIStyle titleStyle;
        private GUIStyle labelStyle;
        private GUIStyle smallStyle;
        private GUIStyle centerStyle;
        private GUIStyle numberStyle;
        private GUIStyle promptStyle;
        private GUIStyle captionStyle;
        private Texture2D pixel;
        private int selectedSlot;
        private OverlayMode overlayMode;
        private int selectedBuildCategory;
        private string selectedInventoryItemId = "item.wood";
        private Vector2 buildCatalogScrollPosition;
        private string activePromptText = string.Empty;
        private float promptHideTime;
        private string buildCatalogMessage = string.Empty;
        private float buildCatalogMessageHideTime;
        private bool hasWaypoint;
        private Vector3 waypointWorldPosition;
        private bool isQuestCardExpanded = false;
        private float nextCacheRefreshTime;
        private PlayerVitals cachedVitals;
        private PlayerMovement cachedPlayer;
        private GameTimeController cachedGameTime;
        private PlayerGatheringInteractor cachedGathering;
        private PlayerCraftingInteractor cachedCrafting;
        private PlayerCookingInteractor cachedCooking;
        private PlayerLandmarkInteractor cachedLandmarkInteractor;
        private PlayerLootInteractor cachedLootInteractor;
        private PlayerCabinInteractor cachedCabinInteractor;
        private PlayerNpcInteractor cachedNpcInteractor;
        private DiscoverableLandmark[] cachedLandmarks = System.Array.Empty<DiscoverableLandmark>();
        private LootChest[] cachedLootChests = System.Array.Empty<LootChest>();
        private ResourceNode[] cachedResourceNodes = System.Array.Empty<ResourceNode>();
        private ConstructionSite[] cachedConstructionSites = System.Array.Empty<ConstructionSite>();
        private VillagerNpcController[] cachedNpcs = System.Array.Empty<VillagerNpcController>();
        private AnimalNpcController[] cachedAnimalNpcs = System.Array.Empty<AnimalNpcController>();
        private AnimalPenController[] cachedAnimalPens = System.Array.Empty<AnimalPenController>();

        public void Configure(
            InventorySession inventorySession,
            BuildingPlacementController placementController = null,
            VerticalSliceController sliceController = null)
        {
            this.inventorySession = inventorySession;
            this.placementController = placementController;
            this.sliceController = sliceController;
        }

        private void Update()
        {
            if (GameStartMenuController.IsOpen) return;
            RefreshHudCache(false);

            for (int i = 0; i < 9; i++)
            {
                if (PrototypeInput.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i))) selectedSlot = i;
            }

            if (PrototypeInput.GetKeyDown(KeyCode.Tab) || PrototypeInput.GetKeyDown(KeyCode.I)) ToggleOverlay(OverlayMode.Inventory);
            if (PrototypeInput.GetKeyDown(KeyCode.M)) ToggleOverlay(OverlayMode.Map);
            if (PrototypeInput.GetKeyDown(KeyCode.J)) ToggleOverlay(OverlayMode.Journal);
            if (PrototypeInput.GetKeyDown(KeyCode.B)) HandleBuildInput();
            if (PrototypeInput.GetKeyDown(KeyCode.Q)) TryConsumeSelectedItem();
            if (PrototypeInput.GetKeyDown(KeyCode.Escape)) overlayMode = OverlayMode.None;
        }

        private void OnGUI()
        {
            EnsureStyles();
            if (GameStartMenuController.IsOpen) return;
            RefreshHudCache(false);

            DrawScreenVignette();
            DrawStatusCard();
            DrawObjectiveCard();
            DrawMinimapCard();
            DrawCurrencyBadge();
            DrawHomeLocatorCard();
            DrawWaypointLocatorCard();
            DrawPromptRibbon();
            DrawHotbar();
            DrawOverlay();
            DrawMobileActionButtons();
        }

        private void DrawStatusCard()
        {
            Rect card = new Rect(14f, 14f, 290f, 98f);
            DrawCard(card, Ink);
            DrawCornerAccents(card, Gold);
            DrawHeaderStrip(new Rect(card.x, card.y, card.width, 24f));

            GUI.Label(new Rect(card.x + 10f, card.y + 3f, 130f, 18f), "THE OLD ROAD", gameTitleStyle);
            GUI.Label(new Rect(card.x + 145f, card.y + 4f, 50f, 16f), "Valen", smallStyle);
            if (GUI.Button(new Rect(card.x + 206f, card.y + 2f, 72f, 20f), LocalizationRuntime.T("settings")))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                GameStartMenuController.OpenSettingsFromGame();
            }

            Rect portrait = new Rect(card.x + 10f, card.y + 30f, 28f, 28f);
            DrawRect(portrait, new Color(0.18f, 0.13f, 0.09f, 1f));
            DrawBorder(portrait, GoldDim, 1.5f);
            DrawPlayerBadge(portrait);

            PlayerVitals vitals = cachedVitals;
            int currentHealth = vitals != null ? vitals.CurrentHealth : 20;
            int maxHealth = vitals != null ? vitals.MaxHealth : 20;
            GUI.Label(new Rect(card.x + 46f, card.y + 28f, 95f, 16f), LocalizationRuntime.T("roadwarden"), labelStyle);
            DrawHealthBar(new Rect(card.x + 46f, card.y + 44f, 150f, 11f), currentHealth, maxHealth);
            GUI.Label(new Rect(card.x + 204f, card.y + 36f, 76f, 18f), currentHealth + "/" + maxHealth, smallStyle);

            float chipY = card.y + 68f;
            DrawResourceChip(new Rect(card.x + 8f, chipY, 62f, 20f), LocalizationRuntime.T("wood"), GetQuantity("item.wood"), PrototypeItemCatalog.Get("item.wood").Color);
            DrawResourceChip(new Rect(card.x + 76f, chipY, 62f, 20f), LocalizationRuntime.T("stone"), GetQuantity("item.stone"), PrototypeItemCatalog.Get("item.stone").Color);
            DrawResourceChip(new Rect(card.x + 144f, chipY, 64f, 20f), LocalizationRuntime.T("food"), GetForageQuantity(), PrototypeItemCatalog.Get("item.wild-berries").Color);
            DrawResourceChip(new Rect(card.x + 214f, chipY, 66f, 20f), LocalizationRuntime.T("ore"), GetQuantity("item.iron-ore"), PrototypeItemCatalog.Get("item.iron-ore").Color);
        }

        private void DrawMinimapCard()
        {
            const float width = 180f;
            const float mapSize = 154f;
            Rect card = new Rect(Screen.width - width - 14f, 14f, width, 216f);
            DrawCard(card, Ink);
            DrawCornerAccents(card, GoldDim);

            GameTimeController gameTime = cachedGameTime;
            string timeText = gameTime != null ? gameTime.ClockText : "Day 1  06:00";
            GUI.Label(new Rect(card.x + 10f, card.y + 5f, card.width - 20f, 18f), timeText, labelStyle);

            Rect map = new Rect(card.x + 13f, card.y + 24f, mapSize, mapSize);
            DrawRect(new Rect(map.x - 3f, map.y - 3f, map.width + 6f, map.height + 6f), new Color(0.02f, 0.018f, 0.015f, 1f));
            DrawMinimap(map);

            DrawControlPill(new Rect(card.x + 10f, map.yMax + 5f, 50f, 22f), "M", LocalizationRuntime.T("map"));
            DrawControlPill(new Rect(card.x + 64f, map.yMax + 5f, 52f, 22f), "Tab", LocalizationRuntime.T("bag"));
            DrawControlPill(new Rect(card.x + 120f, map.yMax + 5f, 50f, 22f), "J", LocalizationRuntime.T("log"));
        }

        private void DrawHomeLocatorCard()
        {
            const float width = 180f;
            ConstructionSite home = FindHomeSite();
            PlayerMovement player = cachedPlayer;

            if (home == null || player == null) return;

            Vector2 delta = home.transform.position - player.transform.position;
            float distance = delta.magnitude;
            string arrow = GetDirectionArrow(delta);
            string status = home.IsCompleted ? LocalizationRuntime.T("home") : LocalizationRuntime.T("home_site");

            Rect card = new Rect(Screen.width - width - 14f, 238f, width, 28f);
            DrawCard(card, new Color(0.045f, 0.034f, 0.024f, 0.90f));
            DrawCornerAccents(card, GoldDim);

            GUI.Label(new Rect(card.x + 8f, card.y + 4f, card.width - 16f, 20f), $"{arrow} {status} {Mathf.RoundToInt(distance)}m", smallStyle);
        }

        private void DrawWaypointLocatorCard()
        {
            if (!hasWaypoint) return;

            const float width = 180f;
            PlayerMovement player = cachedPlayer;
            if (player == null) return;

            Vector2 delta = waypointWorldPosition - player.transform.position;
            float distance = delta.magnitude;
            string arrow = GetDirectionArrow(delta);

            Rect card = new Rect(Screen.width - width - 14f, 272f, width, 28f);
            DrawCard(card, new Color(0.042f, 0.032f, 0.045f, 0.90f));
            DrawCornerAccents(card, new Color(0.64f, 0.48f, 0.86f, 1f));

            GUI.Label(new Rect(card.x + 8f, card.y + 4f, card.width - 16f, 20f), $"{arrow} {LocalizationRuntime.T("waypoint")} {Mathf.RoundToInt(distance)}m", smallStyle);
        }

        private void DrawObjectiveCard()
        {
            if (sliceController == null || overlayMode != OverlayMode.None) return;

            string[] objectives = sliceController.ObjectiveDisplayLines;
            if (objectives == null || objectives.Length == 0) return;

            int activeIndex = 0;
            for (int i = 0; i < objectives.Length; i++)
            {
                if (!objectives[i].StartsWith("[x]"))
                {
                    activeIndex = i;
                    break;
                }
            }

            float cardHeight = isQuestCardExpanded ? (34f + objectives.Length * 20f) : 74f;
            Rect card = new Rect(14f, 120f, 290f, cardHeight);
            DrawCard(card, new Color(0.045f, 0.036f, 0.028f, 0.90f));
            DrawCornerAccents(card, GoldDim);

            string title = LocalizationRuntime.T("tasks") + " (" + sliceController.CompletedObjectiveCount + "/" + sliceController.TotalObjectiveCount + ")";
            GUI.Label(new Rect(card.x + 10f, card.y + 5f, 190f, 18f), title, labelStyle);

            string expandBtn = isQuestCardExpanded ? "▲ " + (LocalizationRuntime.IsVietnamese ? "Gọn" : "Less") : "▼ " + (LocalizationRuntime.IsVietnamese ? "Xem" : "More");
            if (GUI.Button(new Rect(card.xMax - 74f, card.y + 4f, 64f, 20f), expandBtn))
            {
                isQuestCardExpanded = !isQuestCardExpanded;
                TheOldRoad.Audio.AudioManager.PlayUiClick();
            }

            if (isQuestCardExpanded)
            {
                for (int i = 0; i < objectives.Length; i++)
                {
                    bool done = objectives[i].StartsWith("[x]");
                    Color previous = GUI.color;
                    GUI.color = done ? new Color(0.70f, 0.92f, 0.58f, 1f) : Parchment;
                    GUI.Label(new Rect(card.x + 12f, card.y + 28f + i * 20f, card.width - 24f, 18f), LocalizeObjectiveLine(objectives[i]), smallStyle);
                    GUI.color = previous;
                }
            }
            else
            {
                // Compact mode: show current active task only
                string activeLine = objectives[activeIndex];
                Color previous = GUI.color;
                GUI.color = new Color(1f, 0.88f, 0.55f, 1f);
                GUI.Label(new Rect(card.x + 12f, card.y + 26f, card.width - 24f, 20f), "▶ " + LocalizeObjectiveLine(activeLine), smallStyle);
                GUI.color = previous;

                GUI.Label(new Rect(card.x + 12f, card.y + 48f, card.width - 24f, 18f), LocalizationRuntime.IsVietnamese ? "(Nhấn J: Xem toàn bộ nhiệm vụ)" : "(Press J: Open Journal)", captionStyle);
            }
        }

        private void DrawPromptRibbon()
        {
            if (overlayMode != OverlayMode.None) return;

            string prompt = BuildPromptText();
            if (string.IsNullOrWhiteSpace(prompt))
            {
                activePromptText = string.Empty;
                return;
            }

            if (prompt != activePromptText)
            {
                activePromptText = prompt;
                promptHideTime = UnityEngine.Time.unscaledTime + PromptVisibleSeconds;
            }

            if (UnityEngine.Time.unscaledTime > promptHideTime) return;

            float width = Mathf.Min(620f, Screen.width - 80f);
            float y = Screen.width >= 1100f ? 20f : 140f;
            Rect ribbon = new Rect((Screen.width - width) * 0.5f, y, width, 32f);
            DrawRect(new Rect(ribbon.x + 3f, ribbon.y + 4f, ribbon.width, ribbon.height), Shadow);
            DrawRect(ribbon, new Color(0.055f, 0.038f, 0.025f, 0.90f));
            DrawBorder(ribbon, GoldDim, 1.5f);
            GUI.Label(ribbon, activePromptText, promptStyle);
        }

        private void DrawHotbar()
        {
            const int slotCount = 9;
            const float slotSize = 48f;
            const float gap = 4f;
            float totalWidth = slotCount * slotSize + (slotCount - 1) * gap;
            float startX = (Screen.width - totalWidth) * 0.5f;
            float y = Screen.height - slotSize - 16f;

            Rect backing = new Rect(startX - 14f, y - 30f, totalWidth + 28f, slotSize + 40f);
            DrawRect(new Rect(backing.x + 4f, backing.y + 5f, backing.width, backing.height), Shadow);
            DrawCard(backing, new Color(0.025f, 0.022f, 0.02f, 0.90f));
            DrawCornerAccents(backing, GoldDim);

            for (int i = 0; i < slotCount; i++)
            {
                DrawHotbarSlot(new Rect(startX + i * (slotSize + gap), y, slotSize, slotSize), i);
            }

            GUI.Label(new Rect(startX, y - 25f, totalWidth, 20f), BuildControlsText(), centerStyle);
        }

        private void DrawHotbarSlot(Rect slot, int index)
        {
            Event current = Event.current;
            if (current != null && current.type == EventType.MouseDown && current.button == 0 && slot.Contains(current.mousePosition))
            {
                selectedSlot = index;
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                current.Use();
            }

            bool selected = index == selectedSlot;
            Color background = selected ? new Color(0.40f, 0.27f, 0.10f, 0.98f) : new Color(0.095f, 0.08f, 0.065f, 0.96f);
            if (selected) DrawRect(new Rect(slot.x - 3, slot.y - 3, slot.width + 6, slot.height + 6), new Color(0.95f, 0.64f, 0.18f, 0.18f));
            DrawRect(slot, background);
            DrawBorder(slot, selected ? Gold : new Color(0.24f, 0.21f, 0.18f, 1f), selected ? 2.5f : 1.5f);

            HotbarItem item = GetHotbarItem(index);
            GUI.Label(new Rect(slot.x + 3, slot.y + 2, 14, 14), (index + 1).ToString(), numberStyle);

            if (item.IsEmpty)
            {
                DrawRect(new Rect(slot.x + slot.width * 0.32f, slot.y + slot.height * 0.50f, slot.width * 0.36f, 2), new Color(0.25f, 0.23f, 0.20f, 1f));
                return;
            }

            Rect icon = new Rect(slot.x + slot.width * 0.5f - 13f, slot.y + 11f, 26f, 22f);
            DrawItemGlyph(icon, item);
            GUI.Label(new Rect(slot.x + 1, slot.y + slot.height - 14f, slot.width - 2, 13f), LocalizeItemName(item.Name), captionStyle);
            if (item.Count > 0) GUI.Label(new Rect(slot.x + slot.width - 24f, slot.y + 28f, 22f, 16), item.Count.ToString(), numberStyle);
        }

        private void DrawMobileActionButtons()
        {
            if (overlayMode != OverlayMode.None)
            {
                DrawMobileActionButton(new Rect(Screen.width - 80f, Screen.height - 60f, 68f, 46f), "Esc", LocalizationRuntime.T("close"), KeyCode.Escape, new Color(0.50f, 0.18f, 0.14f, 0.96f));
                return;
            }

            float right = Screen.width - 14f;
            float bottom = Screen.height - 14f;

            // Attack (Space)
            DrawMobileActionButton(new Rect(right - 68f, bottom - 104f, 68f, 46f), "⚔", LocalizationRuntime.T("attack"), KeyCode.Space, new Color(0.68f, 0.16f, 0.14f, 0.96f));

            // Interact (F)
            PlayerNpcInteractor npcInteractor = cachedNpcInteractor;
            string interactLabel = npcInteractor != null && npcInteractor.CanTalkAction ? LocalizationRuntime.T("talk") : LocalizationRuntime.T("gather");
            DrawMobileActionButton(new Rect(right - 68f, bottom - 50f, 68f, 46f), "F", interactLabel, KeyCode.F, new Color(0.22f, 0.44f, 0.18f, 0.96f));

            // Craft (C)
            DrawMobileActionButton(new Rect(right - 142f, bottom - 50f, 68f, 46f), "C", LocalizationRuntime.T("craft"), KeyCode.C, new Color(0.45f, 0.31f, 0.12f, 0.96f));

            // Build (B)
            DrawMobileActionButton(new Rect(right - 216f, bottom - 50f, 68f, 46f), "B", LocalizationRuntime.T("build"), KeyCode.B, new Color(0.45f, 0.21f, 0.12f, 0.96f));

            // Contextual actions (Eat / Cabin use / Cook)
            HotbarItem currentItem = GetHotbarItem(selectedSlot);
            if (IsFoodItem(currentItem.ItemId) && currentItem.Count > 0)
            {
                DrawMobileActionButton(new Rect(right - 142f, bottom - 104f, 68f, 46f), "Q", LocalizationRuntime.T("eat"), KeyCode.Q, new Color(0.18f, 0.48f, 0.28f, 0.96f));
            }
            else
            {
                PlayerCabinInteractor cabin = cachedCabinInteractor;
                if (cabin != null && cabin.CanUseAction)
                {
                    DrawMobileActionButton(new Rect(right - 142f, bottom - 104f, 68f, 46f), "F", LocalizeActionLabel(cabin.ActionButtonLabel), KeyCode.F, new Color(0.36f, 0.20f, 0.42f, 0.96f));
                }
            }

            PlayerCookingInteractor cooking = cachedCooking;
            if (cooking != null && cooking.CanCookAction)
            {
                DrawMobileActionButton(new Rect(right - 216f, bottom - 104f, 68f, 46f), "R", LocalizationRuntime.T("cook"), KeyCode.R, new Color(0.54f, 0.24f, 0.10f, 0.96f));
            }
        }

        private void DrawMobileActionButton(Rect rect, string key, string label, KeyCode keyCode, Color color)
        {
            Event current = Event.current;
            if (current != null && current.type == EventType.MouseDown && current.button == 0 && rect.Contains(current.mousePosition))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                PrototypeInput.QueueKeyDown(keyCode);
                current.Use();
            }

            DrawRect(new Rect(rect.x + 3f, rect.y + 4f, rect.width, rect.height), Shadow);
            DrawRect(rect, color);
            DrawBorder(rect, new Color(0.02f, 0.015f, 0.01f, 0.96f), 2f);
            DrawBorder(new Rect(rect.x + 3f, rect.y + 3f, rect.width - 6f, rect.height - 6f), GoldDim, 1f);
            GUI.Label(new Rect(rect.x, rect.y + 3f, rect.width, 18f), key, centerStyle);
            GUI.Label(new Rect(rect.x, rect.y + 21f, rect.width, 18f), label, captionStyle);
        }

        private void DrawOverlay()
        {
            if (overlayMode == OverlayMode.None) return;

            DrawRect(new Rect(0, 0, Screen.width, Screen.height), new Color(0f, 0f, 0f, 0.58f));

            if (overlayMode == OverlayMode.Inventory) DrawInventoryOverlay();
            if (overlayMode == OverlayMode.BuildCatalog) DrawBuildCatalogOverlay();
            if (overlayMode == OverlayMode.Map) DrawMapOverlay();
            if (overlayMode == OverlayMode.Journal) DrawJournalOverlay();
            if (overlayMode == OverlayMode.MerchantShop) DrawMerchantShopOverlay();
        }

        private void HandleBuildInput()
        {
            if (placementController != null && placementController.IsPlacementMode)
            {
                placementController.CancelPlacement();
                overlayMode = OverlayMode.None;
                return;
            }

            ToggleOverlay(OverlayMode.BuildCatalog);
        }

        private void DrawInventoryOverlay()
        {
            float width = Mathf.Min(980f, Screen.width - 30f);
            float height = Mathf.Min(560f, Screen.height - 40f);
            Rect panel = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
            DrawCard(panel, Ink);
            DrawCornerAccents(panel, Gold);
            DrawHeaderStrip(new Rect(panel.x, panel.y, panel.width, 44f));

            GUI.Label(new Rect(panel.x + 24f, panel.y + 9f, panel.width - 48f, 28f), LocalizationRuntime.T("pack_title"), gameTitleStyle);
            GUI.Label(new Rect(panel.x + panel.width - 210f, panel.y + 14f, 190f, 20f), LocalizationRuntime.T("inventory_close"), smallStyle);

            float detailWidth = Mathf.Min(290f, panel.width * 0.32f);
            float contentY = panel.y + 54f;
            float contentHeight = panel.height - 68f;

            Rect gridRect = new Rect(panel.x + 16f, contentY, panel.width - detailWidth - 36f, contentHeight);
            Rect detailRect = new Rect(gridRect.xMax + 12f, contentY, detailWidth, contentHeight);

            DrawInventoryGrid(gridRect);
            DrawInventoryDetailPane(detailRect);
        }

        private void DrawInventoryGrid(Rect rect)
        {
            DrawRect(rect, new Color(0.025f, 0.023f, 0.02f, 0.75f));
            DrawBorder(rect, new Color(0.22f, 0.18f, 0.12f, 1f), 2f);

            PrototypeItemInfo[] items = PrototypeItemCatalog.All;

            const float slotSize = 68f;
            const float gap = 8f;
            int columns = Mathf.Max(1, Mathf.FloorToInt((rect.width - 20f + gap) / (slotSize + gap)));

            for (int i = 0; i < items.Length; i++)
            {
                PrototypeItemInfo item = items[i];
                int column = i % columns;
                int row = i / columns;
                Rect slot = new Rect(rect.x + 10f + column * (slotSize + gap), rect.y + 10f + row * (slotSize + gap), slotSize, slotSize);
                DrawInventorySlot(slot, item);
            }
        }

        private void DrawInventorySlot(Rect slot, PrototypeItemInfo item)
        {
            Event current = Event.current;
            if (current != null && current.type == EventType.MouseDown && current.button == 0 && slot.Contains(current.mousePosition))
            {
                selectedInventoryItemId = item.ItemId;
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                current.Use();
            }

            bool isSelected = string.Equals(selectedInventoryItemId, item.ItemId, StringComparison.Ordinal);
            int quantity = GetQuantity(item.ItemId);
            bool hasItem = quantity > 0;

            // Highlight effect for selected item
            if (isSelected)
            {
                float pulse = 0.6f + Mathf.PingPong(UnityEngine.Time.unscaledTime * 3f, 0.4f);
                DrawRect(new Rect(slot.x - 3f, slot.y - 3f, slot.width + 6f, slot.height + 6f), new Color(1f, 0.85f, 0.25f, pulse * 0.45f));
                DrawRect(slot, new Color(0.40f, 0.26f, 0.09f, 0.98f));
                DrawBorder(slot, Gold, 2.5f);
                // Top-right golden indicator notch
                DrawRect(new Rect(slot.xMax - 6f, slot.y + 2f, 4f, 4f), Gold);
            }
            else
            {
                DrawRect(slot, hasItem ? InkSoft : new Color(0.045f, 0.041f, 0.037f, 0.88f));
                DrawBorder(slot, hasItem ? GoldDim : new Color(0.20f, 0.18f, 0.15f, 1f), 1f);
            }

            Rect icon = new Rect(slot.x + 11f, slot.y + 6f, slot.width - 22f, 34f);
            Color previous = GUI.color;
            GUI.color = hasItem ? Color.white : new Color(1f, 1f, 1f, 0.40f);
            DrawItemGlyph(icon, new HotbarItem(item.ItemId, item.DisplayName, item.Icon, quantity, item.Color));
            GUI.color = previous;

            Rect quantityBadge = new Rect(slot.xMax - 30f, slot.yMax - 22f, 26f, 16f);
            DrawRect(quantityBadge, hasItem ? new Color(0.02f, 0.018f, 0.015f, 0.92f) : new Color(0.02f, 0.018f, 0.015f, 0.55f));
            DrawBorder(quantityBadge, isSelected ? Gold : (hasItem ? GoldDim : new Color(0.18f, 0.16f, 0.14f, 1f)), 1f);
            GUI.Label(quantityBadge, quantity.ToString(), centerStyle);

            GUI.Label(new Rect(slot.x + 2f, slot.yMax - 38f, slot.width - 4f, 16f), GetShortItemName(LocalizeItemName(item.ItemId, item.DisplayName)), centerStyle);
        }

        private void DrawInventoryDetailPane(Rect rect)
        {
            DrawCard(rect, new Color(0.06f, 0.045f, 0.04f, 0.96f));
            DrawBorder(rect, Gold, 2f);
            DrawHeaderStrip(new Rect(rect.x, rect.y, rect.width, 36f));

            PrototypeItemInfo item = PrototypeItemCatalog.Get(selectedInventoryItemId);
            int quantity = GetQuantity(item.ItemId);
            bool hasItem = quantity > 0;

            // Header Item Name
            GUI.Label(new Rect(rect.x + 10f, rect.y + 7f, rect.width - 20f, 22f), LocalizationRuntime.ItemName(item.ItemId), titleStyle);

            // Category Badge
            Rect catBadge = new Rect(rect.x + 16f, rect.y + 44f, rect.width - 32f, 22f);
            DrawRect(catBadge, new Color(0.16f, 0.11f, 0.07f, 0.92f));
            DrawBorder(catBadge, GoldDim, 1f);
            GUI.Label(catBadge, LocalizationRuntime.ItemCategory(item.ItemId), smallStyle);

            // Large Icon Preview Box
            float boxSz = 68f;
            Rect previewBox = new Rect(rect.x + (rect.width - boxSz) * 0.5f, rect.y + 72f, boxSz, boxSz);
            DrawRect(new Rect(previewBox.x - 2f, previewBox.y - 2f, boxSz + 4f, boxSz + 4f), new Color(0.35f, 0.24f, 0.10f, 0.55f));
            DrawRect(previewBox, new Color(0.12f, 0.09f, 0.07f, 1f));
            DrawBorder(previewBox, Gold, 2f);

            Rect previewIcon = new Rect(previewBox.x + 10f, previewBox.y + 10f, previewBox.width - 20f, previewBox.height - 20f);
            Color prevColor = GUI.color;
            GUI.color = hasItem ? Color.white : new Color(1f, 1f, 1f, 0.6f);
            DrawItemGlyph(previewIcon, new HotbarItem(item.ItemId, item.DisplayName, item.Icon, quantity, item.Color));
            GUI.color = prevColor;

            // Quantity Label
            string countText = LocalizationRuntime.IsVietnamese ? ("Sở hữu:  <b>" + quantity + "</b>") : ("In Bag:  <b>" + quantity + "</b>");
            GUI.Label(new Rect(rect.x + 16f, rect.y + 148f, rect.width - 32f, 24f), countText, labelStyle);

            // Divider
            DrawRect(new Rect(rect.x + 16f, rect.y + 176f, rect.width - 32f, 1f), GoldDim);

            // Full Item Description
            GUI.Label(new Rect(rect.x + 14f, rect.y + 184f, rect.width - 28f, 170f), LocalizationRuntime.ItemDescription(item.ItemId), subtitleStyle);

            // Bottom Usage Tip / Action Box
            Rect actionBox = new Rect(rect.x + 12f, rect.yMax - 82f, rect.width - 24f, 70f);
            DrawRect(actionBox, new Color(0.10f, 0.07f, 0.05f, 0.90f));
            DrawBorder(actionBox, new Color(0.38f, 0.28f, 0.14f, 1f), 1f);

            if (item.ItemId == "item.wild-berries" || item.ItemId == "item.mushroom" || item.ItemId == "item.cooked-meal")
            {
                if (hasItem && GUI.Button(new Rect(actionBox.x + 10f, actionBox.y + 8f, actionBox.width - 20f, 32f), "♥  " + (LocalizationRuntime.IsVietnamese ? "Ăn Hồi Máu (Q)" : "Eat (Q)"), buttonStyle))
                {
                    TryConsumeSelectedItem();
                }
                GUI.Label(new Rect(actionBox.x + 4f, actionBox.y + 44f, actionBox.width - 8f, 20f), LocalizationRuntime.IsVietnamese ? "Hồi phục sinh lực cho người chơi" : "Restores player vitality", smallStyle);
            }
            else
            {
                string usageText = LocalizationRuntime.IsVietnamese
                    ? "Dùng phím số 1-9 để chọn trên thanh công cụ"
                    : "Assign to Hotbar 1-9 to use in world";
                GUI.Label(new Rect(actionBox.x + 6f, actionBox.y + 14f, actionBox.width - 12f, 44f), usageText, smallStyle);
            }
        }

        private void DrawBuildCatalogOverlay()
        {
            float panelWidth = Mathf.Min(Screen.width - 40f, 1080f);
            float panelHeight = Mathf.Min(Screen.height - 50f, 680f);
            Rect panel = new Rect((Screen.width - panelWidth) * 0.5f, (Screen.height - panelHeight) * 0.5f, panelWidth, panelHeight);
            DrawCard(panel, Ink);
            DrawCornerAccents(panel, Gold);
            DrawHeaderStrip(new Rect(panel.x, panel.y, panel.width, 50f));

            GUI.Label(new Rect(panel.x + 24f, panel.y + 10f, panel.width - 48f, 28f), LocalizationRuntime.T("construction_catalog"), gameTitleStyle);
            GUI.Label(new Rect(panel.x + panel.width - 260f, panel.y + 16f, 230f, 20f), LocalizationRuntime.T("build_close"), smallStyle);

            Rect sidebar = new Rect(panel.x + 18f, panel.y + 64f, 210f, panel.height - 82f);
            Rect content = new Rect(sidebar.xMax + 14f, sidebar.y, panel.xMax - sidebar.xMax - 32f, sidebar.height);

            DrawBuildCategorySidebar(sidebar);
            DrawBuildCatalogContent(content);
        }

        private void DrawBuildCategorySidebar(Rect rect)
        {
            DrawRect(rect, new Color(0.025f, 0.023f, 0.02f, 0.76f));
            DrawBorder(rect, GoldDim, 1f);
            GUI.Label(new Rect(rect.x + 14f, rect.y + 10f, rect.width - 28f, 24f), LocalizationRuntime.T("categories"), titleStyle);

            DrawBuildCategoryButton(new Rect(rect.x + 10f, rect.y + 44f, rect.width - 20f, 40f), 0, LocalizationRuntime.T("housing"), LocalizationRuntime.T("housing_desc"));
            DrawBuildCategoryButton(new Rect(rect.x + 10f, rect.y + 88f, rect.width - 20f, 40f), 1, LocalizationRuntime.T("fire_light"), LocalizationRuntime.T("fire_light_desc"));
            DrawBuildCategoryButton(new Rect(rect.x + 10f, rect.y + 132f, rect.width - 20f, 40f), 2, LocalizationRuntime.T("animal_pens"), LocalizationRuntime.T("animal_pens_desc"));
            DrawBuildCategoryButton(new Rect(rect.x + 10f, rect.y + 176f, rect.width - 20f, 40f), 3, LocalizationRuntime.T("fences_security"), LocalizationRuntime.T("fences_security_desc"));
            DrawBuildCategoryButton(new Rect(rect.x + 10f, rect.y + 220f, rect.width - 20f, 40f), 4, LocalizationRuntime.T("paths_decor"), LocalizationRuntime.T("paths_decor_desc"));

            // Demolish / Recycle button
            Rect demolishRect = new Rect(rect.x + 10f, rect.y + 270f, rect.width - 20f, 44f);
            DrawRect(demolishRect, new Color(0.40f, 0.08f, 0.06f, 0.95f));
            DrawBorder(demolishRect, new Color(0.98f, 0.35f, 0.25f, 1f), 1.5f);

            Event cur = Event.current;
            if (cur != null && cur.type == EventType.MouseDown && cur.button == 0 && demolishRect.Contains(cur.mousePosition))
            {
                if (placementController != null)
                {
                    placementController.BeginDemolish();
                    overlayMode = OverlayMode.None;
                }
                cur.Use();
            }

            GUI.Label(new Rect(demolishRect.x + 8f, demolishRect.y + 4f, demolishRect.width - 16f, 18f), LocalizationRuntime.T("demolish_btn"), labelStyle);
            GUI.Label(new Rect(demolishRect.x + 8f, demolishRect.y + 22f, demolishRect.width - 16f, 16f), LocalizationRuntime.T("demolish_btn_desc"), smallStyle);

            GUI.Label(new Rect(rect.x + 14f, rect.yMax - 54f, rect.width - 28f, 44f), LocalizationRuntime.T("build_select_hint"), smallStyle);
        }

        private void DrawBuildCategoryButton(Rect rect, int categoryIndex, string title, string subtitle)
        {
            bool selected = selectedBuildCategory == categoryIndex;
            DrawRect(rect, selected ? new Color(0.42f, 0.25f, 0.10f, 0.96f) : InkSoft);
            DrawBorder(rect, selected ? Gold : GoldDim, selected ? 2f : 1f);

            Event current = Event.current;
            if (current != null && current.type == EventType.MouseDown && current.button == 0 && rect.Contains(current.mousePosition))
            {
                selectedBuildCategory = categoryIndex;
                buildCatalogScrollPosition = Vector2.zero;
                current.Use();
            }

            GUI.Label(new Rect(rect.x + 10f, rect.y + 4f, rect.width - 20f, 18f), title, labelStyle);
            GUI.Label(new Rect(rect.x + 10f, rect.y + 22f, rect.width - 20f, 14f), subtitle, smallStyle);
        }

        private void DrawBuildCatalogContent(Rect rect)
        {
            DrawRect(rect, new Color(0.025f, 0.023f, 0.02f, 0.64f));
            DrawBorder(rect, new Color(0.18f, 0.15f, 0.11f, 1f), 2f);

            string heading = selectedBuildCategory == 0 ? LocalizationRuntime.T("housing") :
                             selectedBuildCategory == 1 ? LocalizationRuntime.T("fire_light") :
                             selectedBuildCategory == 2 ? LocalizationRuntime.T("animal_pens") :
                             selectedBuildCategory == 3 ? LocalizationRuntime.T("fences_security") :
                                                          LocalizationRuntime.T("paths_decor");
            GUI.Label(new Rect(rect.x + 18f, rect.y + 10f, rect.width - 36f, 24f), heading, titleStyle);
            if (!string.IsNullOrWhiteSpace(buildCatalogMessage) && UnityEngine.Time.unscaledTime <= buildCatalogMessageHideTime)
            {
                Rect message = new Rect(rect.x + 150f, rect.y + 8f, rect.width - 168f, 26f);
                DrawRect(message, new Color(0.20f, 0.055f, 0.035f, 0.88f));
                DrawBorder(message, new Color(0.84f, 0.28f, 0.18f, 1f), 1f);
                GUI.Label(message, buildCatalogMessage, centerStyle);
            }

            Rect scrollOuter = new Rect(rect.x + 8f, rect.y + 40f, rect.width - 16f, rect.height - 48f);
            const float cardWidth = 220f;
            const float cardHeight = 210f;
            const float gap = 12f;
            int columns = Mathf.Max(1, Mathf.FloorToInt((scrollOuter.width - 20f + gap) / (cardWidth + gap)));

            int totalCards = selectedBuildCategory == 0 ? 6 :
                             selectedBuildCategory == 1 ? 2 :
                             selectedBuildCategory == 2 ? 2 :
                             selectedBuildCategory == 3 ? 7 : 3;

            int totalRows = Mathf.CeilToInt((float)totalCards / columns);
            float viewHeight = Mathf.Max(scrollOuter.height, totalRows * (cardHeight + gap) + 12f);

            buildCatalogScrollPosition = GUI.BeginScrollView(scrollOuter, buildCatalogScrollPosition, new Rect(0, 0, scrollOuter.width - 20f, viewHeight));

            if (selectedBuildCategory == 0)
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_cabin"), LocalizationRuntime.T("building_cabin_desc"), LocalizationRuntime.T("housing"), GetBuildingDefinition("building.cabin"), "Cabin", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_cottage"), LocalizationRuntime.T("building_cottage_desc"), LocalizationRuntime.T("housing"), GetBuildingDefinition("building.stone-cottage"), "Cottage", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_barn"), LocalizationRuntime.T("building_barn_desc"), LocalizationRuntime.T("housing"), GetBuildingDefinition("building.farm-barn"), "Barn", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 3), LocalizationRuntime.T("building_shed"), LocalizationRuntime.T("building_shed_desc"), LocalizationRuntime.T("housing"), GetBuildingDefinition("building.storage-shed"), "Shed", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 4), LocalizationRuntime.T("building_herbalist"), LocalizationRuntime.T("building_herbalist_desc"), LocalizationRuntime.T("housing"), GetBuildingDefinition("building.herbalist-hut"), "Herbalist", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 5), LocalizationRuntime.T("building_tower"), LocalizationRuntime.T("building_tower_desc"), LocalizationRuntime.T("housing"), GetBuildingDefinition("building.lookout-tower"), "Lookout", true);
            }
            else if (selectedBuildCategory == 1)
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_campfire"), LocalizationRuntime.T("building_campfire_desc"), LocalizationRuntime.T("fire_light"), GetBuildingDefinition("building.campfire"), "Campfire", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_hearth"), LocalizationRuntime.T("building_hearth_desc"), LocalizationRuntime.T("fire_light"), GetBuildingDefinition("building.cooking-hearth"), "Hearth", true);
            }
            else if (selectedBuildCategory == 2)
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_pen_small"), LocalizationRuntime.T("building_pen_small_desc"), LocalizationRuntime.T("animal_pens"), GetBuildingDefinition("building.animal-pen-small"), "PenSquare", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_pen_long"), LocalizationRuntime.T("building_pen_long_desc"), LocalizationRuntime.T("animal_pens"), GetBuildingDefinition("building.animal-pen-long"), "PenLong", true);
            }
            else if (selectedBuildCategory == 3)
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_fence_drag"), LocalizationRuntime.T("building_fence_drag_desc"), LocalizationRuntime.T("fences_security"), GetBuildingDefinition("building.perimeter-fence-drag"), "FenceDrag", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_fence_small"), LocalizationRuntime.T("building_fence_small_desc"), LocalizationRuntime.T("fences_security"), GetBuildingDefinition("building.perimeter-fence-small"), "FenceSmall", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_fence_med"), LocalizationRuntime.T("building_fence_med_desc"), LocalizationRuntime.T("fences_security"), GetBuildingDefinition("building.perimeter-fence-medium"), "FenceMedium", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 3), LocalizationRuntime.T("building_fence_lrg"), LocalizationRuntime.T("building_fence_lrg_desc"), LocalizationRuntime.T("fences_security"), GetBuildingDefinition("building.perimeter-fence-large"), "FenceLarge", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 4), LocalizationRuntime.T("building_fence_grd"), LocalizationRuntime.T("building_fence_grd_desc"), LocalizationRuntime.T("fences_security"), GetBuildingDefinition("building.perimeter-fence-grand"), "FenceGrand", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 5), LocalizationRuntime.T("building_fence"), LocalizationRuntime.T("building_fence_desc"), LocalizationRuntime.T("fences_security"), GetBuildingDefinition("building.fence"), "Fence", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 6), LocalizationRuntime.T("building_gate"), LocalizationRuntime.T("building_gate_desc"), LocalizationRuntime.T("fences_security"), GetBuildingDefinition("building.gate"), "Gate", true);
            }
            else
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_path_dirt"), LocalizationRuntime.T("building_path_dirt_desc"), LocalizationRuntime.T("paths_decor"), GetBuildingDefinition("building.path-dirt"), "PathDirt", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_path_cobble"), LocalizationRuntime.T("building_path_cobble_desc"), LocalizationRuntime.T("paths_decor"), GetBuildingDefinition("building.path-cobblestone"), "PathCobble", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_scarecrow"), LocalizationRuntime.T("building_scarecrow_desc"), LocalizationRuntime.T("paths_decor"), GetBuildingDefinition("building.scarecrow"), "Scarecrow", true);
            }

            GUI.EndScrollView();
        }

        private BuildingDefinition GetBuildingDefinition(string buildingId)
        {
            if (sliceController == null) return placementController != null && placementController.BuildingDefinition != null && placementController.BuildingDefinition.BuildingId == buildingId
                ? placementController.BuildingDefinition
                : null;

            return sliceController.GetBuildingDefinition(buildingId);
        }

        private static Rect GetScrollCardRect(float cardWidth, float cardHeight, float gap, int columns, int index)
        {
            int column = index % columns;
            int row = index / columns;
            return new Rect(column * (cardWidth + gap) + 4f, row * (cardHeight + gap) + 4f, cardWidth, cardHeight);
        }

        private void DrawBuildCatalogCard(Rect rect, string name, string description, string category, BuildingDefinition definition, string glyph, bool buildable)
        {
            bool hasMaterials = HasBuildMaterials(definition);
            bool canBuild = buildable && definition != null && placementController != null && hasMaterials;
            DrawRect(rect, buildable ? InkSoft : new Color(0.055f, 0.052f, 0.048f, 0.86f));
            DrawBorder(rect, canBuild ? GoldDim : new Color(0.34f, 0.22f, 0.16f, 1f), canBuild ? 1f : 2f);

            Rect icon = new Rect(rect.x + 10f, rect.y + 10f, 52f, 52f);
            DrawBuildingGlyph(icon, glyph);

            GUI.Label(new Rect(rect.x + 68f, rect.y + 8f, rect.width - 74f, 18f), name, labelStyle);
            GUI.Label(new Rect(rect.x + 68f, rect.y + 26f, rect.width - 74f, 15f), category, smallStyle);
            GUI.Label(new Rect(rect.x + 68f, rect.y + 42f, rect.width - 74f, 26f), description, smallStyle);

            Rect requirements = new Rect(rect.x + 10f, rect.y + 70f, rect.width - 20f, 88f);
            DrawRect(requirements, new Color(0.030f, 0.026f, 0.022f, 0.78f));
            DrawBorder(requirements, new Color(0.16f, 0.13f, 0.10f, 1f), 1f);
            GUI.Label(new Rect(requirements.x + 8f, requirements.y + 3f, requirements.width - 16f, 16f), LocalizationRuntime.T("required_items"), smallStyle);

            if (definition != null)
            {
                DrawBuildRequirements(requirements, definition.ConstructionCosts);
            }
            else
            {
                GUI.Label(new Rect(requirements.x + 8f, requirements.y + 24f, requirements.width - 16f, 34f), LocalizationRuntime.T("requirements_unfinalized"), smallStyle);
            }

            Rect action = new Rect(rect.x + 10f, rect.y + 166f, rect.width - 20f, 34f);
            if (canBuild)
            {
                if (GUI.Button(action, LocalizationRuntime.T("select_place")))
                {
                    TheOldRoad.Audio.AudioManager.PlayUiClick();
                    buildCatalogMessage = string.Empty;
                    placementController.BeginPlacement(definition);
                    overlayMode = OverlayMode.None;
                    activePromptText = name + " " + LocalizationRuntime.T("selected_place_hint");
                    promptHideTime = UnityEngine.Time.unscaledTime + PromptVisibleSeconds;
                }
            }
            else if (buildable && definition != null && placementController != null)
            {
                if (GUI.Button(action, LocalizationRuntime.T("not_enough_items")))
                {
                    ShowBuildCatalogMessage(LocalizationRuntime.T("cannot_build") + " " + name + ". " + GetMissingBuildMaterialsText(definition));
                }
            }
            else
            {
                DrawRect(action, new Color(0.11f, 0.10f, 0.09f, 0.90f));
                DrawBorder(action, new Color(0.22f, 0.20f, 0.18f, 1f), 1f);
                GUI.Label(action, buildable ? LocalizationRuntime.T("missing_definition") : LocalizationRuntime.T("coming_soon"), centerStyle);
            }
        }

        private void DrawBuildRequirements(Rect rect, BuildCostEntry[] costs)
        {
            if (costs == null || costs.Length == 0)
            {
                GUI.Label(new Rect(rect.x + 10f, rect.y + 30f, rect.width - 20f, 20f), LocalizationRuntime.T("no_material_cost"), smallStyle);
                return;
            }

            for (int i = 0; i < costs.Length && i < 3; i++)
            {
                BuildCostEntry cost = costs[i];
                PrototypeItemInfo item = PrototypeItemCatalog.Get(cost.itemId);
                int owned = GetQuantity(cost.itemId);
                bool hasEnough = owned >= cost.quantity;
                Rect row = new Rect(rect.x + 10f, rect.y + 29f + i * 18f, rect.width - 20f, 17f);
                DrawRect(new Rect(row.x, row.y + 4f, 9f, 9f), item.Color);

                Color previous = GUI.color;
                GUI.color = hasEnough ? new Color(0.72f, 0.95f, 0.60f, 1f) : new Color(0.95f, 0.48f, 0.40f, 1f);
                GUI.Label(new Rect(row.x + 16f, row.y - 1f, row.width - 16f, row.height), LocalizeItemName(item.ItemId, item.DisplayName) + " " + owned + "/" + cost.quantity, smallStyle);
                GUI.color = previous;
            }
        }

        private bool HasBuildMaterials(BuildingDefinition definition)
        {
            if (definition == null) return false;
            BuildCostEntry[] costs = definition.ConstructionCosts;
            if (costs == null || costs.Length == 0) return true;

            for (int i = 0; i < costs.Length; i++)
            {
                BuildCostEntry cost = costs[i];
                if (GetQuantity(cost.itemId) < cost.quantity) return false;
            }

            return true;
        }

        private string GetMissingBuildMaterialsText(BuildingDefinition definition)
        {
            if (definition == null || definition.ConstructionCosts == null) return LocalizationRuntime.T("missing_building_definition");

            string message = LocalizationRuntime.T("missing") + ": ";
            bool hasMissing = false;
            for (int i = 0; i < definition.ConstructionCosts.Length; i++)
            {
                BuildCostEntry cost = definition.ConstructionCosts[i];
                int owned = GetQuantity(cost.itemId);
                if (owned >= cost.quantity) continue;

                PrototypeItemInfo item = PrototypeItemCatalog.Get(cost.itemId);
                if (hasMissing) message += ", ";
                message += LocalizeItemName(item.ItemId, item.DisplayName) + " " + owned + "/" + cost.quantity;
                hasMissing = true;
            }

            return hasMissing ? message : LocalizationRuntime.T("materials_ready");
        }

        private void ShowBuildCatalogMessage(string message)
        {
            buildCatalogMessage = message;
            buildCatalogMessageHideTime = UnityEngine.Time.unscaledTime + PromptVisibleSeconds;
            activePromptText = message;
            promptHideTime = UnityEngine.Time.unscaledTime + PromptVisibleSeconds;
            PlayerSpeechBubble.Say("speech.build_blocked");
        }

        private void DrawBuildingGlyph(Rect rect, string glyph)
        {
            DrawRect(rect, new Color(0.025f, 0.022f, 0.018f, 1f));
            DrawBorder(rect, Color.black, 1f);

            if (glyph == "Herbalist")
            {
                DrawSprite(PrototypePixelArtFactory.HerbalistHut(), new Rect(rect.x + 5f, rect.y + 5f, rect.width - 10f, rect.height - 10f));
                return;
            }
            if (glyph == "Lookout")
            {
                DrawSprite(PrototypePixelArtFactory.LookoutTower(), new Rect(rect.x + 10f, rect.y + 4f, rect.width - 20f, rect.height - 8f));
                return;
            }
            if (glyph == "Fence")
            {
                DrawSprite(PrototypePixelArtFactory.WoodFence(), new Rect(rect.x + 6f, rect.y + 12f, rect.width - 12f, rect.height - 24f));
                return;
            }
            if (glyph == "Gate")
            {
                DrawSprite(PrototypePixelArtFactory.WoodGate(false), new Rect(rect.x + 6f, rect.y + 12f, rect.width - 12f, rect.height - 24f));
                return;
            }
            if (glyph == "Cottage")
            {
                DrawSprite(PrototypePixelArtFactory.StoneCottage(), new Rect(rect.x + 5f, rect.y + 5f, rect.width - 10f, rect.height - 10f));
                return;
            }

            if (glyph == "Campfire" || glyph == "Hearth")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.20f, rect.y + rect.height * 0.66f, rect.width * 0.60f, rect.height * 0.10f), new Color(0.30f, 0.24f, 0.18f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.30f, rect.y + rect.height * 0.50f, rect.width * 0.40f, rect.height * 0.14f), new Color(0.55f, 0.33f, 0.14f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.28f, rect.width * 0.28f, rect.height * 0.30f), new Color(0.96f, 0.30f, 0.08f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.44f, rect.y + rect.height * 0.20f, rect.width * 0.14f, rect.height * 0.28f), new Color(1f, 0.78f, 0.22f, 1f));
                return;
            }

            if (glyph == "Scarecrow")
            {
                DrawSprite(PrototypePixelArtFactory.Scarecrow(), new Rect(rect.x + 10f, rect.y + 4f, rect.width - 20f, rect.height - 8f));
                return;
            }
            if (glyph == "PathDirt")
            {
                DrawSprite(PrototypePixelArtFactory.PathDirtTile(), new Rect(rect.x + 14f, rect.y + 14f, rect.width - 28f, rect.height - 28f));
                return;
            }
            if (glyph == "PathCobble")
            {
                DrawSprite(PrototypePixelArtFactory.PathCobblestoneTile(), new Rect(rect.x + 14f, rect.y + 14f, rect.width - 28f, rect.height - 28f));
                return;
            }
            if (glyph == "FenceDrag" || glyph == "FenceSmall" || glyph == "FenceMedium" || glyph == "FenceLarge" || glyph == "FenceGrand")
            {
                DrawSprite(PrototypePixelArtFactory.WoodFenceCorner(), new Rect(rect.x + 4f, rect.y + 4f, 16f, 20f));
                DrawSprite(PrototypePixelArtFactory.WoodFenceHorizontal(), new Rect(rect.x + 18f, rect.y + 6f, rect.width - 22f, 16f));
                DrawSprite(PrototypePixelArtFactory.WoodFenceVertical(), new Rect(rect.x + 4f, rect.y + 24f, 16f, 20f));
                DrawSprite(PrototypePixelArtFactory.WoodGate(false), new Rect(rect.x + 22f, rect.y + 24f, 24f, 16f));
                return;
            }

            if (glyph == "Shed")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.24f, rect.y + rect.height * 0.46f, rect.width * 0.52f, rect.height * 0.30f), new Color(0.42f, 0.27f, 0.14f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.34f, rect.width * 0.64f, rect.height * 0.16f), new Color(0.26f, 0.16f, 0.10f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.42f, rect.y + rect.height * 0.56f, rect.width * 0.16f, rect.height * 0.20f), new Color(0.08f, 0.05f, 0.035f, 1f));
                return;
            }

            DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.46f, rect.width * 0.64f, rect.height * 0.34f), new Color(0.65f, 0.38f, 0.18f, 1f));
            DrawRect(new Rect(rect.x + rect.width * 0.12f, rect.y + rect.height * 0.30f, rect.width * 0.76f, rect.height * 0.18f), new Color(0.44f, 0.16f, 0.10f, 1f));
            DrawRect(new Rect(rect.x + rect.width * 0.42f, rect.y + rect.height * 0.57f, rect.width * 0.16f, rect.height * 0.23f), new Color(0.10f, 0.06f, 0.04f, 1f));
            DrawRect(new Rect(rect.x + rect.width * 0.27f, rect.y + rect.height * 0.53f, rect.width * 0.14f, rect.height * 0.12f), new Color(0.40f, 0.62f, 0.76f, 1f));
        }

        private void DrawMapOverlay()
        {
            float panelWidth = Mathf.Min(Screen.width - 90f, 980f);
            float panelHeight = Mathf.Min(Screen.height - 90f, 720f);
            Rect panel = new Rect((Screen.width - panelWidth) * 0.5f, (Screen.height - panelHeight) * 0.5f, panelWidth, panelHeight);
            DrawCard(panel, Ink);
            DrawCornerAccents(panel, Gold);
            DrawHeaderStrip(new Rect(panel.x, panel.y, panel.width, 50));

            GUI.Label(new Rect(panel.x + 24, panel.y + 10, panel.width - 48, 28), LocalizationRuntime.T("map_title"), gameTitleStyle);
            GUI.Label(new Rect(panel.x + panel.width - 210, panel.y + 16, 190, 20), LocalizationRuntime.T("map_close"), smallStyle);

            float legendWidth = panel.width > 760f ? 220f : 0f;
            Rect mapRect = new Rect(panel.x + 26, panel.y + 72, panel.width - 52 - legendWidth, panel.height - 102);
            DrawRect(new Rect(mapRect.x - 6, mapRect.y - 6, mapRect.width + 12, mapRect.height + 12), new Color(0.02f, 0.018f, 0.015f, 1f));
            HandleMapPinInput(mapRect);
            DrawMinimap(mapRect);
            GUI.Label(new Rect(mapRect.x, mapRect.yMax + 8f, mapRect.width, 18f), LocalizationRuntime.T("map_pin_hint"), centerStyle);

            if (legendWidth > 0f)
            {
                Rect legend = new Rect(mapRect.xMax + 20f, mapRect.y, legendWidth - 20f, mapRect.height);
                DrawMapLegend(legend);
            }
        }

        private void DrawJournalOverlay()
        {
            float width = Mathf.Min(960f, Screen.width - 80f);
            float height = Mathf.Min(620f, Screen.height - 100f);
            Rect panel = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
            DrawCard(panel, Ink);
            DrawCornerAccents(panel, Gold);
            DrawHeaderStrip(new Rect(panel.x, panel.y, panel.width, 48));

            GUI.Label(new Rect(panel.x + 24, panel.y + 10, panel.width - 48, 28), LocalizationRuntime.T("journal"), gameTitleStyle);
            GUI.Label(new Rect(panel.x + panel.width - 210, panel.y + 16, 190, 20), LocalizationRuntime.T("esc_close"), smallStyle);

            string status = sliceController != null
                ? sliceController.LastDiscoveryStatus + "  " + sliceController.DiscoveredLandmarkCount + "/" + sliceController.TotalLandmarkCount
                : LocalizationRuntime.T("inspect_landmarks");
            GUI.Label(new Rect(panel.x + 26, panel.y + 60, panel.width - 52, 22), status, labelStyle);

            Rect storyList = new Rect(panel.x + 24, panel.y + 92, panel.width * 0.48f - 30f, panel.height - 120);
            Rect landmarkList = new Rect(storyList.xMax + 18f, storyList.y, panel.xMax - storyList.xMax - 42f, storyList.height);
            DrawRect(storyList, new Color(0.025f, 0.023f, 0.02f, 0.64f));
            DrawRect(landmarkList, new Color(0.025f, 0.023f, 0.02f, 0.64f));
            DrawBorder(storyList, new Color(0.18f, 0.15f, 0.11f, 1f), 2f);
            DrawBorder(landmarkList, new Color(0.18f, 0.15f, 0.11f, 1f), 2f);

            GUI.Label(new Rect(storyList.x + 16f, storyList.y + 12f, storyList.width - 32f, 22f), LocalizationRuntime.T("story_arc"), titleStyle);
            if (sliceController != null)
            {
                GUI.Label(new Rect(storyList.x + 16f, storyList.y + 42f, storyList.width - 32f, 22f), sliceController.CurrentStoryTitle, labelStyle);
                GUI.Label(new Rect(storyList.x + 16f, storyList.y + 66f, storyList.width - 32f, 42f), sliceController.CurrentStoryDetail, smallStyle);

                string[] storyLines = sliceController.StoryJournalLines;
                float storyY = storyList.y + 118f;
                if (storyLines.Length == 0)
                {
                    GUI.Label(new Rect(storyList.x + 16f, storyY, storyList.width - 32f, 40f), LocalizationRuntime.T("story_no_entries"), smallStyle);
                }
                else
                {
                    for (int i = Mathf.Max(0, storyLines.Length - 5); i < storyLines.Length; i++)
                    {
                        Rect row = new Rect(storyList.x + 14f, storyY, storyList.width - 28f, 68f);
                        DrawRect(row, InkSoft);
                        DrawBorder(row, GoldDim, 1f);
                        GUI.Label(new Rect(row.x + 12f, row.y + 8f, row.width - 24f, 52f), storyLines[i], smallStyle);
                        storyY += 76f;
                        if (storyY > storyList.yMax - 68f) break;
                    }
                }
            }

            DiscoverableLandmark[] landmarks = cachedLandmarks;
            GUI.Label(new Rect(landmarkList.x + 16f, landmarkList.y + 12f, landmarkList.width - 32f, 22f), LocalizationRuntime.T("landmark_records"), titleStyle);
            if (landmarks.Length == 0)
            {
                GUI.Label(new Rect(landmarkList.x + 18, landmarkList.y + 46, landmarkList.width - 36, 24), LocalizationRuntime.T("no_landmarks"), smallStyle);
                return;
            }

            float rowY = landmarkList.y + 46f;
            for (int i = 0; i < landmarks.Length; i++)
            {
                DiscoverableLandmark landmark = landmarks[i];
                if (landmark == null) continue;

                Rect row = new Rect(landmarkList.x + 14f, rowY, landmarkList.width - 28f, 72f);
                DrawRect(row, landmark.IsDiscovered ? InkSoft : new Color(0.045f, 0.04f, 0.035f, 0.72f));
                DrawBorder(row, landmark.IsDiscovered ? GoldDim : new Color(0.16f, 0.14f, 0.12f, 1f), 1f);
                DrawRect(new Rect(row.x + 12f, row.y + 18f, 34f, 34f), landmark.IsDiscovered ? new Color(0.36f, 0.58f, 0.68f, 1f) : new Color(0.16f, 0.15f, 0.14f, 1f));
                DrawBorder(new Rect(row.x + 12f, row.y + 18f, 34f, 34f), Color.black, 1f);
                GUI.Label(new Rect(row.x + 58f, row.y + 10f, row.width - 72f, 22f), landmark.IsDiscovered ? landmark.Title : LocalizationRuntime.T("unknown_landmark"), labelStyle);
                GUI.Label(
                    new Rect(row.x + 58f, row.y + 34f, row.width - 72f, 28f),
                    landmark.IsDiscovered ? landmark.JournalText : LocalizationRuntime.T("journal_hint"),
                    smallStyle);
                rowY += 82f;
                if (rowY > landmarkList.yMax - 72f) break;
            }
        }

        private void DrawMinimap(Rect map)
        {
            DrawRect(map, new Color(0.08f, 0.17f, 0.10f, 1f));
            DrawRect(new Rect(map.x, map.y, map.width, map.height * 0.18f), new Color(0.05f, 0.11f, 0.08f, 1f));
            DrawRect(new Rect(map.x, map.y + map.height * 0.86f, map.width, map.height * 0.14f), new Color(0.05f, 0.11f, 0.08f, 1f));
            DrawRiver(map);
            DrawRoad(map);
            DrawBorder(map, new Color(0.01f, 0.012f, 0.01f, 1f), 2f);

            foreach (ConstructionSite site in cachedConstructionSites)
            {
                if (site == null) continue;
                Color color = IsHomeBuilding(site.BuildingId)
                    ? new Color(1f, 0.82f, 0.24f, 1f)
                    : new Color(0.95f, 0.62f, 0.22f, 1f);
                DrawMapDot(map, site.transform.position, color, map.width > 220f ? 10f : 6f);
            }

            if (hasWaypoint) DrawWaypointMarker(map, waypointWorldPosition, map.width > 220f ? 14f : 9f);

            PlayerMovement player = cachedPlayer;
            if (player != null) DrawMapDot(map, player.transform.position, new Color(0.25f, 0.62f, 1f, 1f), map.width > 220f ? 11f : 7f);
        }

        private void DrawRoad(Rect map)
        {
            int segments = Mathf.Max(28, Mathf.RoundToInt(map.width / 8f));
            float roadHeight = map.height * 0.07f;

            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)(segments - 1);
                float nextT = Mathf.Min(1f, (i + 1) / (float)(segments - 1));
                Vector3 center = GetMapCenter();
                float mapRange = GetMapRange();
                float worldX = Mathf.Lerp(center.x - mapRange * 0.5f, center.x + mapRange * 0.5f, t);
                float roadY = 1.4f * Mathf.Sin(worldX * 0.34f) + 0.8f * Mathf.Sin(worldX * 0.11f);
                Vector2 mapPoint = WorldToMap(map, new Vector3(worldX, roadY, 0f));
                float segmentWidth = Mathf.Max(3f, map.width * (nextT - t) + 2f);
                Color color = i % 2 == 0 ? new Color(0.52f, 0.36f, 0.16f, 1f) : new Color(0.64f, 0.45f, 0.20f, 1f);
                DrawRect(new Rect(mapPoint.x - segmentWidth * 0.5f, mapPoint.y - roadHeight * 0.5f, segmentWidth, roadHeight), color);
            }
        }

        private void DrawRiver(Rect map)
        {
            int segments = Mathf.Max(18, Mathf.RoundToInt(map.width / 11f));
            float riverHeight = map.height * 0.045f;

            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)(segments - 1);
                Vector3 center = GetMapCenter();
                float mapRange = GetMapRange();
                float worldX = Mathf.Lerp(center.x - mapRange * 0.5f, center.x + mapRange * 0.5f, t);
                float riverY = -12.5f - Mathf.Sin(worldX * 0.16f) * 2.0f;
                Vector2 mapPoint = WorldToMap(map, new Vector3(worldX, riverY, 0f));
                DrawRect(new Rect(mapPoint.x - 4f, mapPoint.y - riverHeight * 0.5f, 8f, riverHeight), new Color(0.16f, 0.35f, 0.45f, 1f));
            }
        }

        private void DrawLandmarkDots(Rect map)
        {
            bool isLargeMap = map.width > 220f;
            foreach (DiscoverableLandmark landmark in cachedLandmarks)
            {
                if (landmark == null) continue;
                Vector2 mapPoint = WorldToMap(map, landmark.transform.position);

                if (landmark.IsDiscovered)
                {
                    // Discovered landmark: distinct gold marker
                    float sz = isLargeMap ? 10f : 6f;
                    DrawMapDot(map, landmark.transform.position, new Color(0.95f, 0.78f, 0.25f, 1f), sz);
                }
                else
                {
                    // Undiscovered landmark: prominent "?" question mark badge so players can easily locate and travel to it
                    float sz = isLargeMap ? 18f : 13f;
                    Rect qRect = new Rect(mapPoint.x - sz * 0.5f, mapPoint.y - sz * 0.5f, sz, sz);
                    // Outer glow
                    DrawRect(new Rect(qRect.x - 1f, qRect.y - 1f, sz + 2f, sz + 2f), new Color(1f, 0.25f, 0.25f, 0.45f));
                    // Badge background
                    DrawRect(qRect, new Color(0.78f, 0.14f, 0.14f, 0.96f));
                    DrawBorder(qRect, Gold, 1.5f);
                    GUI.Label(new Rect(qRect.x, qRect.y - 1f, qRect.width, qRect.height), "?", numberStyle);
                }
            }
        }

        private string BuildPromptText()
        {
            string prompt = string.Empty;

            PlayerGatheringInteractor gathering = cachedGathering;
            if (gathering != null && !string.IsNullOrWhiteSpace(gathering.InteractionHint)) prompt = gathering.InteractionHint;

            PlayerCraftingInteractor crafting = cachedCrafting;
            if (crafting != null && !string.IsNullOrWhiteSpace(crafting.CraftingHint)) AppendPrompt(ref prompt, crafting.CraftingHint);

            PlayerCookingInteractor cooking = cachedCooking;
            if (cooking != null && !string.IsNullOrWhiteSpace(cooking.CookingHint)) AppendPrompt(ref prompt, cooking.CookingHint);

            PlayerLandmarkInteractor landmark = cachedLandmarkInteractor;
            if (landmark != null && !string.IsNullOrWhiteSpace(landmark.InteractionHint)) AppendPrompt(ref prompt, landmark.InteractionHint);

            PlayerLootInteractor loot = cachedLootInteractor;
            if (loot != null && !string.IsNullOrWhiteSpace(loot.InteractionHint)) AppendPrompt(ref prompt, loot.InteractionHint);

            PlayerCabinInteractor cabin = cachedCabinInteractor;
            if (cabin != null && !string.IsNullOrWhiteSpace(cabin.InteractionHint)) AppendPrompt(ref prompt, cabin.InteractionHint);

            PlayerNpcInteractor npc = cachedNpcInteractor;
            if (npc != null && !string.IsNullOrWhiteSpace(npc.InteractionHint)) AppendPrompt(ref prompt, npc.InteractionHint);

            if (placementController != null && !string.IsNullOrWhiteSpace(placementController.LastStatus)) AppendPrompt(ref prompt, placementController.LastStatus);

            foreach (AnimalPenController pen in cachedAnimalPens)
            {
                if (pen != null && !string.IsNullOrWhiteSpace(pen.Status))
                {
                    AppendPrompt(ref prompt, pen.Status);
                    break;
                }
            }

            return prompt;
        }

        private static void AppendPrompt(ref string prompt, string addition)
        {
            if (string.IsNullOrEmpty(prompt)) prompt = addition;
            else prompt += "    |    " + addition;
        }

        private static string BuildControlsText()
        {
            return "Space " + LocalizationRuntime.T("attack")
                + "  •  F " + LocalizationRuntime.T("gather") + "/" + LocalizationRuntime.T("use")
                + "  •  Shift " + (LocalizationRuntime.IsVietnamese ? "Chạy" : "Sprint")
                + "  •  Tab " + LocalizationRuntime.T("bag")
                + "  •  C " + LocalizationRuntime.T("craft")
                + "  •  B " + LocalizationRuntime.T("build")
                + "  •  Q " + LocalizationRuntime.T("eat");
        }

        private void TryConsumeSelectedItem()
        {
            HotbarItem item = GetHotbarItem(selectedSlot);
            if (string.IsNullOrEmpty(item.ItemId) || item.Count <= 0) return;

            PlayerVitals vitals = cachedVitals;
            if (vitals == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player == null) player = GameObject.Find("Player");
                if (player != null) vitals = player.GetComponent<PlayerVitals>();
            }

            if (vitals != null && vitals.TryConsumeFood(item.ItemId, out int healed))
            {
                if (inventorySession != null && inventorySession.Runtime != null)
                {
                    inventorySession.Runtime.TryRemove(item.ItemId, 1);
                }
                Combat.FloatingTextController.SpawnHeal(healed, vitals.transform.position);
            }
        }

        private static bool IsFoodItem(string itemId)
        {
            switch (itemId)
            {
                case "item.wild-berries":
                case "item.medicinal-herb":
                case "item.cooked-meal":
                case "item.egg":
                case "item.milk":
                    return true;
                default:
                    return false;
            }
        }

        private static string LocalizeObjectiveLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return line;

            string prefix = string.Empty;
            string text = line;
            if (line.StartsWith("[x] ") || line.StartsWith("[ ] "))
            {
                prefix = line.Substring(0, 4);
                text = line.Substring(4);
            }

            return prefix + LocalizationRuntime.Objective(text);
        }

        private static string LocalizeActionLabel(string label)
        {
            if (string.IsNullOrWhiteSpace(label)) return label;

            switch (label)
            {
                case "Use": return LocalizationRuntime.T("use");
                case "Enter": return LocalizationRuntime.T("enter");
                case "Exit": return LocalizationRuntime.T("exit");
                case "Sleep": return LocalizationRuntime.T("sleep");
                default: return label;
            }
        }

        private static string LocalizeItemName(string itemId, string fallback)
        {
            string localized = LocalizationRuntime.T(itemId);
            return localized == itemId ? fallback : localized;
        }

        private static string LocalizeItemName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName)) return string.Empty;

            switch (displayName)
            {
                case "Wood": return LocalizationRuntime.T("item.wood");
                case "Stone": return LocalizationRuntime.T("item.stone");
                case "Plank":
                case "Cabin Plank": return LocalizationRuntime.T("item.cabin-plank");
                case "Worn Axe": return LocalizationRuntime.T("item.tool-axe");
                case "Stone Pick": return LocalizationRuntime.T("item.tool-pickaxe");
                case "Wild Berries": return LocalizationRuntime.T("item.wild-berries");
                case "Medicinal Herb": return LocalizationRuntime.T("item.medicinal-herb");
                case "Mushroom": return LocalizationRuntime.T("item.mushroom");
                case "Iron Ore": return LocalizationRuntime.T("item.iron-ore");
                case "Torch": return LocalizationRuntime.T("item.torch");
                case "Bell Fragment": return LocalizationRuntime.T("item.bell-fragment");
                case "Old Coin": return LocalizationRuntime.T("item.old-coin");
                default: return displayName;
            }
        }

        private HotbarItem GetHotbarItem(int index)
        {
            switch (index)
            {
                case 0: return new HotbarItem("item.wood", "Wood", "W", GetQuantity("item.wood"), new Color(0.47f, 0.29f, 0.12f, 1f));
                case 1: return new HotbarItem("item.stone", "Stone", "S", GetQuantity("item.stone"), new Color(0.45f, 0.48f, 0.52f, 1f));
                case 2: return new HotbarItem("item.cabin-plank", "Plank", "P", GetQuantity("item.cabin-plank"), new Color(0.74f, 0.50f, 0.25f, 1f));
                case 3: return ToHotbarItem("item.tool-axe");
                case 4: return ToHotbarItem("item.tool-pickaxe");
                case 5: return ToHotbarItem("item.wild-berries");
                case 6: return ToHotbarItem("item.iron-ore");
                case 7: return ToHotbarItem("item.torch");
                case 8: return ToHotbarItem("item.bell-fragment");
                default: return HotbarItem.Empty;
            }
        }

        private HotbarItem ToHotbarItem(string itemId)
        {
            PrototypeItemInfo item = PrototypeItemCatalog.Get(itemId);
            return new HotbarItem(item.ItemId, item.DisplayName, item.Icon, GetQuantity(item.ItemId), item.Color);
        }

        private static string GetShortItemName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName)) return string.Empty;
            if (displayName.Length <= 10) return displayName;

            string[] words = displayName.Split(' ');
            if (words.Length > 1)
            {
                string initials = string.Empty;
                for (int i = 0; i < words.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(words[i])) initials += words[i][0];
                }

                return initials.ToUpperInvariant();
            }

            return displayName.Substring(0, 9) + ".";
        }

        private int GetQuantity(string itemId)
        {
            if (inventorySession == null || inventorySession.Runtime == null) return 0;
            return inventorySession.Runtime.GetQuantity(itemId);
        }

        private int GetForageQuantity()
        {
            return GetQuantity("item.wild-berries")
                + GetQuantity("item.medicinal-herb")
                + GetQuantity("item.mushroom");
        }

        private void DrawHealthBar(Rect rect, int currentHealth, int maxHealth)
        {
            // Ornate Dark Iron & Gold Bezel
            DrawRect(new Rect(rect.x - 2f, rect.y - 2f, rect.width + 4f, rect.height + 4f), new Color(0.06f, 0.05f, 0.04f, 0.95f));
            DrawBorder(new Rect(rect.x - 2f, rect.y - 2f, rect.width + 4f, rect.height + 4f), GoldDim, 1f);

            // Dark Blood Bed
            DrawRect(rect, new Color(0.18f, 0.03f, 0.03f, 1f));
            float fill = maxHealth <= 0 ? 0f : Mathf.Clamp01(currentHealth / (float)maxHealth);

            // Liquid Blood Bar
            if (fill > 0f)
            {
                Rect bloodRect = new Rect(rect.x, rect.y, rect.width * fill, rect.height);
                DrawRect(bloodRect, new Color(0.84f, 0.12f, 0.10f, 1f));
                // Specular highlight line along top of health
                DrawRect(new Rect(rect.x, rect.y, rect.width * fill, 2f), new Color(1f, 0.48f, 0.38f, 1f));
                // Bottom shadow
                DrawRect(new Rect(rect.x, rect.y + rect.height - 2f, rect.width * fill, 2f), new Color(0.45f, 0.05f, 0.05f, 1f));
            }

            // Health Notch Dividers
            int pips = Mathf.CeilToInt(maxHealth / 2f);
            for (int i = 1; i < pips; i++)
            {
                float x = rect.x + rect.width * (i / (float)pips);
                DrawRect(new Rect(x, rect.y, 1f, rect.height), new Color(0.04f, 0.01f, 0.01f, 0.75f));
            }
        }

        private string BuildTinyStatus()
        {
            GameTimeController gameTime = cachedGameTime;
            string time = gameTime != null ? gameTime.ClockText : "Day 1  06:00";
            return time + "  -  " + (sliceController != null ? sliceController.SaveStatus : "Save pending");
        }

        private static string GetItemUseText(string itemId)
        {
            return PrototypeItemCatalog.Get(itemId).UseText;
        }

        private void DrawScreenVignette()
        {
            DrawRect(new Rect(0, 0, Screen.width, 82), new Color(0f, 0f, 0f, 0.24f));
            DrawRect(new Rect(0, Screen.height - 126, Screen.width, 126), new Color(0f, 0f, 0f, 0.26f));
        }

        private void DrawPlayerBadge(Rect rect)
        {
            // Knight Portrait: Steel Greathelm with Visor, Specular Glint, Crimson Mantle & Gorget
            DrawRect(rect, new Color(0.12f, 0.09f, 0.08f, 1f));

            // Steel Helmet Dome
            DrawRect(new Rect(rect.x + 6f, rect.y + 4f, 16f, 13f), new Color(0.65f, 0.70f, 0.78f, 1f));
            DrawRect(new Rect(rect.x + 8f, rect.y + 3f, 12f, 2f), new Color(0.65f, 0.70f, 0.78f, 1f));
            // Helmet Specular Highlight
            DrawRect(new Rect(rect.x + 9f, rect.y + 5f, 6f, 3f), new Color(0.92f, 0.95f, 0.98f, 1f));
            // Visor Brow & Dark Eye Slit
            DrawRect(new Rect(rect.x + 5f, rect.y + 11f, 18f, 2f), new Color(0.20f, 0.22f, 0.28f, 1f));
            DrawRect(new Rect(rect.x + 7f, rect.y + 12f, 14f, 2f), new Color(0.06f, 0.07f, 0.09f, 1f));
            // Golden brow crest
            DrawRect(new Rect(rect.x + 13f, rect.y + 10f, 2f, 2f), Gold);

            // Crimson Scarf / Mantle around neck
            DrawRect(new Rect(rect.x + 4f, rect.y + 17f, 20f, 6f), new Color(0.72f, 0.12f, 0.14f, 1f));
            DrawRect(new Rect(rect.x + 7f, rect.y + 18f, 14f, 3f), new Color(0.92f, 0.22f, 0.22f, 1f));

            // Steel Gorget / Breastplate base
            DrawRect(new Rect(rect.x + 5f, rect.y + 23f, 18f, 4f), new Color(0.55f, 0.60f, 0.68f, 1f));
            DrawRect(new Rect(rect.x + 12f, rect.y + 23f, 4f, 4f), new Color(0.88f, 0.92f, 0.96f, 1f));
        }

        private void DrawResourceChip(Rect rect, string name, int quantity, Color color)
        {
            DrawRect(rect, new Color(0.04f, 0.032f, 0.024f, 0.86f));
            DrawBorder(rect, new Color(0.26f, 0.18f, 0.08f, 1f), 1f);
            DrawRect(new Rect(rect.x + 5, rect.y + 5, 8, 8), color);
            GUI.Label(new Rect(rect.x + 17, rect.y + 1, rect.width - 22, rect.height), name + " " + quantity, smallStyle);
        }

        private void DrawControlPill(Rect rect, string key, string label)
        {
            Event current = Event.current;
            if (current != null && current.type == EventType.MouseDown && current.button == 0 && rect.Contains(current.mousePosition))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                if (key == "M") overlayMode = overlayMode == OverlayMode.Map ? OverlayMode.None : OverlayMode.Map;
                else if (key == "Tab" || key == "I") overlayMode = overlayMode == OverlayMode.Inventory ? OverlayMode.None : OverlayMode.Inventory;
                else if (key == "J") overlayMode = overlayMode == OverlayMode.Journal ? OverlayMode.None : OverlayMode.Journal;
                current.Use();
            }

            DrawRect(rect, new Color(0.045f, 0.035f, 0.025f, 0.90f));
            DrawBorder(rect, GoldDim, 1f);
            GUI.Label(rect, $"{key} {label}", centerStyle);
        }

        private void DrawItemGlyph(Rect rect, HotbarItem item)
        {
            DrawRect(rect, new Color(0.025f, 0.022f, 0.018f, 1f));
            DrawBorder(rect, new Color(0f, 0f, 0f, 1f), 1f);

            string targetId = !string.IsNullOrEmpty(item.ItemId) ? item.ItemId : GetItemIdFromName(item.Name);
            if (!string.IsNullOrEmpty(targetId))
            {
                Texture2D iconTex = PrototypePixelArtFactory.ItemIconTexture(targetId);
                if (iconTex != null)
                {
                    float size = Mathf.Min(rect.width - 4f, rect.height - 4f);
                    Rect iconRect = new Rect(rect.x + (rect.width - size) * 0.5f, rect.y + (rect.height - size) * 0.5f, size, size);
                    GUI.DrawTexture(iconRect, iconTex, ScaleMode.ScaleToFit);
                    return;
                }
            }

            if (item.Name == "Wood")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.30f, rect.y + rect.height * 0.18f, rect.width * 0.42f, rect.height * 0.62f), item.Color);
                DrawRect(new Rect(rect.x + rect.width * 0.44f, rect.y + rect.height * 0.18f, 2f, rect.height * 0.62f), new Color(0.32f, 0.18f, 0.08f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.24f, rect.y + rect.height * 0.28f, rect.width * 0.52f, 3f), new Color(0.63f, 0.40f, 0.18f, 1f));
                return;
            }

            if (item.Name == "Stone")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.20f, rect.y + rect.height * 0.40f, rect.width * 0.62f, rect.height * 0.34f), item.Color);
                DrawRect(new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.28f, rect.width * 0.40f, rect.height * 0.22f), new Color(0.62f, 0.64f, 0.67f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y + rect.height * 0.46f, rect.width * 0.20f, 3f), new Color(0.78f, 0.78f, 0.76f, 1f));
                return;
            }

            if (item.Name == "Plank" || item.Name == "Cabin Plank")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.28f, rect.width * 0.64f, rect.height * 0.18f), item.Color);
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.52f, rect.width * 0.64f, rect.height * 0.18f), item.Color);
                DrawRect(new Rect(rect.x + rect.width * 0.24f, rect.y + rect.height * 0.34f, rect.width * 0.46f, 2f), new Color(0.86f, 0.60f, 0.31f, 1f));
                return;
            }

            if (item.Name == "Cabin")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.46f, rect.width * 0.64f, rect.height * 0.34f), item.Color);
                DrawRect(new Rect(rect.x + rect.width * 0.12f, rect.y + rect.height * 0.30f, rect.width * 0.76f, rect.height * 0.18f), new Color(0.44f, 0.16f, 0.10f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.42f, rect.y + rect.height * 0.57f, rect.width * 0.16f, rect.height * 0.23f), new Color(0.10f, 0.06f, 0.04f, 1f));
                return;
            }

            if (item.Name == "Worn Axe")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.42f, rect.y + rect.height * 0.30f, rect.width * 0.12f, rect.height * 0.52f), new Color(0.44f, 0.24f, 0.10f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.26f, rect.y + rect.height * 0.22f, rect.width * 0.34f, rect.height * 0.22f), item.Color);
                DrawRect(new Rect(rect.x + rect.width * 0.22f, rect.y + rect.height * 0.30f, rect.width * 0.12f, rect.height * 0.18f), new Color(0.80f, 0.84f, 0.86f, 1f));
                return;
            }

            if (item.Name == "Stone Pick")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.46f, rect.y + rect.height * 0.30f, rect.width * 0.11f, rect.height * 0.52f), new Color(0.42f, 0.24f, 0.10f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.20f, rect.y + rect.height * 0.22f, rect.width * 0.62f, rect.height * 0.17f), item.Color);
                DrawRect(new Rect(rect.x + rect.width * 0.26f, rect.y + rect.height * 0.18f, rect.width * 0.18f, rect.height * 0.10f), new Color(0.74f, 0.76f, 0.74f, 1f));
                return;
            }

            if (item.Name == "Bell Fragment")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.28f, rect.y + rect.height * 0.26f, rect.width * 0.44f, rect.height * 0.42f), item.Color);
                DrawRect(new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.18f, rect.width * 0.28f, rect.height * 0.12f), new Color(0.86f, 0.90f, 1f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.44f, rect.y + rect.height * 0.48f, rect.width * 0.13f, rect.height * 0.22f), new Color(0.42f, 0.52f, 0.70f, 1f));
                return;
            }

            if (item.Name == "Wild Berries")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.22f, rect.y + rect.height * 0.48f, rect.width * 0.56f, rect.height * 0.28f), new Color(0.18f, 0.48f, 0.20f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.30f, rect.y + rect.height * 0.26f, rect.width * 0.13f, rect.height * 0.13f), item.Color);
                DrawRect(new Rect(rect.x + rect.width * 0.50f, rect.y + rect.height * 0.32f, rect.width * 0.13f, rect.height * 0.13f), item.Color);
                DrawRect(new Rect(rect.x + rect.width * 0.62f, rect.y + rect.height * 0.46f, rect.width * 0.13f, rect.height * 0.13f), item.Color);
                return;
            }

            if (item.Name == "Medicinal Herb")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.46f, rect.y + rect.height * 0.22f, rect.width * 0.08f, rect.height * 0.60f), item.Color);
                DrawRect(new Rect(rect.x + rect.width * 0.26f, rect.y + rect.height * 0.40f, rect.width * 0.24f, rect.height * 0.10f), new Color(0.50f, 0.90f, 0.42f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.52f, rect.y + rect.height * 0.30f, rect.width * 0.25f, rect.height * 0.10f), new Color(0.62f, 1f, 0.52f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.58f, rect.width * 0.30f, rect.height * 0.10f), new Color(0.38f, 0.78f, 0.34f, 1f));
                return;
            }

            if (item.Name == "Mushroom")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.48f, rect.width * 0.12f, rect.height * 0.30f), new Color(0.86f, 0.76f, 0.58f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.52f, rect.y + rect.height * 0.52f, rect.width * 0.10f, rect.height * 0.25f), new Color(0.82f, 0.70f, 0.52f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.23f, rect.y + rect.height * 0.34f, rect.width * 0.34f, rect.height * 0.18f), item.Color);
                DrawRect(new Rect(rect.x + rect.width * 0.46f, rect.y + rect.height * 0.40f, rect.width * 0.28f, rect.height * 0.16f), new Color(0.62f, 0.22f, 0.18f, 1f));
                return;
            }

            if (item.Name == "Iron Ore")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.18f, rect.y + rect.height * 0.40f, rect.width * 0.64f, rect.height * 0.34f), item.Color);
                DrawRect(new Rect(rect.x + rect.width * 0.34f, rect.y + rect.height * 0.30f, rect.width * 0.40f, rect.height * 0.20f), new Color(0.30f, 0.33f, 0.38f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.30f, rect.y + rect.height * 0.48f, rect.width * 0.18f, 3f), new Color(0.72f, 0.56f, 0.36f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.56f, rect.y + rect.height * 0.42f, rect.width * 0.18f, 3f), new Color(0.86f, 0.68f, 0.44f, 1f));
                return;
            }

            if (item.Name == "Old Coin")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.30f, rect.y + rect.height * 0.24f, rect.width * 0.40f, rect.height * 0.46f), item.Color);
                DrawBorder(new Rect(rect.x + rect.width * 0.30f, rect.y + rect.height * 0.24f, rect.width * 0.40f, rect.height * 0.46f), new Color(0.46f, 0.28f, 0.07f, 1f), 2f);
                DrawRect(new Rect(rect.x + rect.width * 0.44f, rect.y + rect.height * 0.38f, rect.width * 0.12f, rect.height * 0.18f), new Color(1f, 0.89f, 0.48f, 1f));
                return;
            }

            if (item.Name == "Torch")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.45f, rect.y + rect.height * 0.40f, rect.width * 0.12f, rect.height * 0.38f), new Color(0.45f, 0.24f, 0.10f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.24f, rect.width * 0.30f, rect.height * 0.22f), new Color(0.92f, 0.24f, 0.08f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.43f, rect.y + rect.height * 0.18f, rect.width * 0.16f, rect.height * 0.24f), item.Color);
                DrawRect(new Rect(rect.x + rect.width * 0.48f, rect.y + rect.height * 0.22f, rect.width * 0.06f, rect.height * 0.12f), new Color(1f, 0.93f, 0.40f, 1f));
                return;
            }

            DrawRect(new Rect(rect.x + 7, rect.y + 7, rect.width - 14, rect.height - 14), item.Color);
            GUI.Label(rect, item.Icon, centerStyle);
        }

        private void DrawMapLegend(Rect rect)
        {
            DrawRect(rect, new Color(0.03f, 0.026f, 0.022f, 0.84f));
            DrawBorder(rect, GoldDim, 1f);
            GUI.Label(new Rect(rect.x + 14, rect.y + 10, rect.width - 28, 22), LocalizationRuntime.T("legend"), titleStyle);
            DrawLegendRow(rect.x + 16, rect.y + 44, new Color(0.25f, 0.62f, 1f, 1f), LocalizationRuntime.T("legend_player"));
            DrawLegendRow(rect.x + 16, rect.y + 68, new Color(1f, 0.82f, 0.24f, 1f), LocalizationRuntime.T("legend_home"));
            DrawLegendRow(rect.x + 16, rect.y + 92, new Color(0.95f, 0.62f, 0.22f, 1f), LocalizationRuntime.T("legend_building"));
            DrawLegendRow(rect.x + 16, rect.y + 116, new Color(0.86f, 0.42f, 1f, 1f), LocalizationRuntime.T("legend_waypoint"));
            DrawLegendRow(rect.x + 16, rect.y + 140, new Color(0.85f, 0.18f, 0.18f, 1f), LocalizationRuntime.IsVietnamese ? "?  Địa danh chưa khám phá" : "?  Undiscovered Area");
            DrawLegendRow(rect.x + 16, rect.y + 164, new Color(0.95f, 0.78f, 0.25f, 1f), LocalizationRuntime.IsVietnamese ? "★  Địa danh đã khám phá" : "★  Discovered Landmark");

            GUI.Label(new Rect(rect.x + 14, rect.y + 192f, rect.width - 28, 54f), LocalizationRuntime.T("map_pin_hint"), smallStyle);
            if (hasWaypoint && GUI.Button(new Rect(rect.x + 22f, rect.y + 250f, rect.width - 44f, 30f), LocalizationRuntime.T("clear_waypoint")))
            {
                hasWaypoint = false;
            }

            GUI.Label(new Rect(rect.x + 14, rect.yMax - 54, rect.width - 28, 44), LocalizationRuntime.T("legend_clean_map_hint"), smallStyle);
        }

        private void HandleMapPinInput(Rect mapRect)
        {
            Event current = Event.current;
            if (current == null || current.type != EventType.MouseDown || !mapRect.Contains(current.mousePosition)) return;

            if (current.button == 0)
            {
                waypointWorldPosition = MapToWorld(mapRect, current.mousePosition);
                hasWaypoint = true;
                activePromptText = LocalizationRuntime.T("waypoint_set");
                promptHideTime = UnityEngine.Time.unscaledTime + PromptVisibleSeconds;
                current.Use();
                return;
            }

            if (current.button == 1)
            {
                hasWaypoint = false;
                activePromptText = LocalizationRuntime.T("waypoint_cleared");
                promptHideTime = UnityEngine.Time.unscaledTime + PromptVisibleSeconds;
                current.Use();
            }
        }

        private void RefreshHudCache(bool force)
        {
            if (!force && UnityEngine.Time.unscaledTime < nextCacheRefreshTime) return;
            nextCacheRefreshTime = UnityEngine.Time.unscaledTime + 0.35f;

            cachedVitals = cachedVitals != null ? cachedVitals : FindAnyObjectByType<PlayerVitals>();
            cachedPlayer = cachedPlayer != null ? cachedPlayer : FindAnyObjectByType<PlayerMovement>();
            cachedGameTime = cachedGameTime != null ? cachedGameTime : FindAnyObjectByType<GameTimeController>();
            cachedGathering = cachedGathering != null ? cachedGathering : FindAnyObjectByType<PlayerGatheringInteractor>();
            cachedCrafting = cachedCrafting != null ? cachedCrafting : FindAnyObjectByType<PlayerCraftingInteractor>();
            cachedCooking = cachedCooking != null ? cachedCooking : FindAnyObjectByType<PlayerCookingInteractor>();
            cachedLandmarkInteractor = cachedLandmarkInteractor != null ? cachedLandmarkInteractor : FindAnyObjectByType<PlayerLandmarkInteractor>();
            cachedLootInteractor = cachedLootInteractor != null ? cachedLootInteractor : FindAnyObjectByType<PlayerLootInteractor>();
            cachedCabinInteractor = cachedCabinInteractor != null ? cachedCabinInteractor : FindAnyObjectByType<PlayerCabinInteractor>();
            cachedNpcInteractor = cachedNpcInteractor != null ? cachedNpcInteractor : FindAnyObjectByType<PlayerNpcInteractor>();

            cachedLandmarks = FindObjectsByType<DiscoverableLandmark>(FindObjectsInactive.Exclude);
            cachedLootChests = FindObjectsByType<LootChest>(FindObjectsInactive.Exclude);
            cachedResourceNodes = FindObjectsByType<ResourceNode>(FindObjectsInactive.Exclude);
            cachedConstructionSites = FindObjectsByType<ConstructionSite>(FindObjectsInactive.Exclude);
            cachedNpcs = FindObjectsByType<VillagerNpcController>(FindObjectsInactive.Exclude);
            cachedAnimalNpcs = FindObjectsByType<AnimalNpcController>(FindObjectsInactive.Exclude);
            cachedAnimalPens = FindObjectsByType<AnimalPenController>(FindObjectsInactive.Exclude);
        }

        private static Color GetResourceMapColor(string itemId)
        {
            switch (itemId)
            {
                case "item.wood": return new Color(0.34f, 0.90f, 0.30f, 1f);
                case "item.stone": return new Color(0.68f, 0.70f, 0.73f, 1f);
                case "item.wild-berries": return new Color(0.92f, 0.24f, 0.30f, 1f);
                case "item.medicinal-herb": return new Color(0.38f, 0.96f, 0.42f, 1f);
                case "item.mushroom": return new Color(0.84f, 0.58f, 0.36f, 1f);
                case "item.iron-ore": return new Color(0.82f, 0.66f, 0.42f, 1f);
                default: return PrototypeItemCatalog.Get(itemId).Color;
            }
        }

        private void DrawLegendRow(float x, float y, Color color, string text)
        {
            Rect marker = new Rect(x, y + 4, 13, 13);
            DrawRect(marker, color);
            DrawBorder(marker, Color.black, 1f);
            GUI.Label(new Rect(x + 24, y, 150, 22), text, smallStyle);
        }

        private ConstructionSite FindHomeSite()
        {
            ConstructionSite bestCompleted = null;
            ConstructionSite bestAny = null;
            float completedDistance = float.MaxValue;
            float anyDistance = float.MaxValue;
            Vector3 playerPosition = cachedPlayer != null ? cachedPlayer.transform.position : Vector3.zero;

            foreach (ConstructionSite site in cachedConstructionSites)
            {
                if (site == null || !IsHomeBuilding(site.BuildingId)) continue;

                float distance = Vector2.Distance(playerPosition, site.transform.position);
                if (site.IsCompleted && distance < completedDistance)
                {
                    completedDistance = distance;
                    bestCompleted = site;
                }

                if (distance < anyDistance)
                {
                    anyDistance = distance;
                    bestAny = site;
                }
            }

            return bestCompleted != null ? bestCompleted : bestAny;
        }

        private static bool IsHomeBuilding(string buildingId)
        {
            return buildingId == "building.cabin" || buildingId == "building.stone-cottage";
        }

        private static string GetDirectionArrow(Vector2 delta)
        {
            if (delta.sqrMagnitude <= 0.01f) return "●";

            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            if (angle < 0f) angle += 360f;
            int sector = Mathf.RoundToInt(angle / 45f) % 8;
            switch (sector)
            {
                case 0: return "→";
                case 1: return "↗";
                case 2: return "↑";
                case 3: return "↖";
                case 4: return "←";
                case 5: return "↙";
                case 6: return "↓";
                default: return "↘";
            }
        }

        private void DrawMapDot(Rect map, Vector3 worldPosition, Color color, float size)
        {
            Vector2 mapPoint = WorldToMap(map, worldPosition);

            Rect dot = new Rect(
                mapPoint.x - size * 0.5f,
                mapPoint.y - size * 0.5f,
                size,
                size);

            DrawRect(dot, color);
            DrawBorder(dot, Color.black, 1f);
        }

        private void DrawWaypointMarker(Rect map, Vector3 worldPosition, float size)
        {
            Vector2 mapPoint = WorldToMap(map, worldPosition);
            float half = size * 0.5f;
            DrawRect(new Rect(mapPoint.x - half, mapPoint.y - 2f, size, 4f), new Color(0.86f, 0.42f, 1f, 1f));
            DrawRect(new Rect(mapPoint.x - 2f, mapPoint.y - half, 4f, size), new Color(0.86f, 0.42f, 1f, 1f));
            DrawRect(new Rect(mapPoint.x - 3f, mapPoint.y - 3f, 6f, 6f), new Color(1f, 0.88f, 0.34f, 1f));
            DrawBorder(new Rect(mapPoint.x - half, mapPoint.y - half, size, size), new Color(0.07f, 0.04f, 0.08f, 0.9f), 1f);
        }

        private Vector2 WorldToMap(Rect map, Vector3 worldPosition)
        {
            Vector3 center = GetMapCenter();
            float mapRange = GetMapRange();
            Vector2 min = new Vector2(center.x - mapRange * 0.5f, center.y - mapRange * 0.5f);
            Vector2 max = new Vector2(center.x + mapRange * 0.5f, center.y + mapRange * 0.5f);
            Vector2 normalized = new Vector2(
                Mathf.InverseLerp(min.x, max.x, worldPosition.x),
                Mathf.InverseLerp(min.y, max.y, worldPosition.y));

            return new Vector2(
                map.x + Mathf.Clamp01(normalized.x) * map.width,
                map.y + (1f - Mathf.Clamp01(normalized.y)) * map.height);
        }

        private Vector3 MapToWorld(Rect map, Vector2 mapPoint)
        {
            Vector3 center = GetMapCenter();
            float mapRange = GetMapRange();
            float normalizedX = Mathf.Clamp01((mapPoint.x - map.x) / map.width);
            float normalizedY = 1f - Mathf.Clamp01((mapPoint.y - map.y) / map.height);
            float worldX = Mathf.Lerp(center.x - mapRange * 0.5f, center.x + mapRange * 0.5f, normalizedX);
            float worldY = Mathf.Lerp(center.y - mapRange * 0.5f, center.y + mapRange * 0.5f, normalizedY);
            return new Vector3(worldX, worldY, 0f);
        }

        private Vector3 GetMapCenter()
        {
            return cachedPlayer != null ? cachedPlayer.transform.position : Vector3.zero;
        }

        private static float GetMapRange() => 96f;

        private void DrawCard(Rect rect, Color color)
        {
            DrawRect(new Rect(rect.x + 5, rect.y + 6, rect.width, rect.height), new Color(0f, 0f, 0f, 0.62f));
            DrawRect(rect, color);
            DrawBorder(rect, new Color(0.04f, 0.035f, 0.03f, 0.98f), 3f);
            DrawBorder(new Rect(rect.x + 3, rect.y + 3, rect.width - 6, rect.height - 6), Gold, 1.5f);
            DrawBorder(new Rect(rect.x + 6, rect.y + 6, rect.width - 12, rect.height - 12), GoldDim, 1f);
        }

        private void DrawCornerAccents(Rect rect, Color color)
        {
            const float length = 18f;
            const float thick = 3f;
            DrawRect(new Rect(rect.x + 7, rect.y + 7, length, thick), color);
            DrawRect(new Rect(rect.x + 7, rect.y + 7, thick, length), color);
            DrawRect(new Rect(rect.xMax - 7 - length, rect.y + 7, length, thick), color);
            DrawRect(new Rect(rect.xMax - 10, rect.y + 7, thick, length), color);
            DrawRect(new Rect(rect.x + 7, rect.yMax - 10, length, thick), color);
            DrawRect(new Rect(rect.x + 7, rect.yMax - 7 - length, thick, length), color);
            DrawRect(new Rect(rect.xMax - 7 - length, rect.yMax - 10, length, thick), color);
            DrawRect(new Rect(rect.xMax - 10, rect.yMax - 7 - length, thick, length), color);
        }

        private void DrawInset(Rect rect, Color color)
        {
            DrawRect(new Rect(rect.x + 4, rect.y + 4, rect.width - 8, 1), color);
            DrawRect(new Rect(rect.x + 4, rect.y + 4, 1, rect.height - 8), color);
        }

        private void DrawHeaderStrip(Rect rect)
        {
            DrawRect(rect, new Color(0.14f, 0.09f, 0.05f, 0.98f));
            DrawRect(new Rect(rect.x + 4f, rect.y + 3f, 6f, rect.height - 6f), new Color(0.78f, 0.14f, 0.12f, 1f));
            DrawRect(new Rect(rect.x, rect.yMax - 2f, rect.width, 2f), Gold);
        }

        private void DrawBorder(Rect rect, Color color, float thickness)
        {
            DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
            DrawRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color);
            DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
            DrawRect(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color);
        }

        private void DrawRect(Rect rect, Color color)
        {
            Color previousColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, pixel);
            GUI.color = previousColor;
        }

        private void DrawSprite(Sprite sprite, Rect rect)
        {
            if (sprite == null || sprite.texture == null) return;
            GUI.DrawTexture(rect, sprite.texture, ScaleMode.ScaleToFit);
        }

        private void EnsureStyles()
        {
            if (pixel == null)
            {
                pixel = new Texture2D(1, 1, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Point,
                    hideFlags = HideFlags.DontSave
                };
                pixel.SetPixel(0, 0, Color.white);
                pixel.Apply(false, true);
            }

            if (titleStyle != null) return;

            gameTitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Gold }
            };

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Gold }
            };

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Parchment }
            };

            smallStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = MutedText }
            };

            centerStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                clipping = TextClipping.Clip,
                normal = { textColor = Parchment }
            };

            numberStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = new Color(0.95f, 0.88f, 0.65f, 1f) }
            };

            promptStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Parchment }
            };

            captionStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 9,
                fontStyle = FontStyle.Bold,
                normal = { textColor = MutedText }
            };
        }

        private struct HotbarItem
        {
            public static readonly HotbarItem Empty = new HotbarItem(string.Empty, string.Empty, string.Empty, 0, Color.clear);

            public HotbarItem(string name, string icon, int count, Color color)
                : this(string.Empty, name, icon, count, color)
            {
            }

            public HotbarItem(string itemId, string name, string icon, int count, Color color)
            {
                ItemId = itemId;
                Name = name;
                Icon = icon;
                Count = count;
                Color = color;
            }

            public string ItemId { get; }
            public string Name { get; }
            public string Icon { get; }
            public int Count { get; }
            public Color Color { get; }
            public bool IsEmpty => string.IsNullOrEmpty(Name);
        }

        private static string GetItemIdFromName(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            switch (name)
            {
                case "Wood": return "item.wood";
                case "Stone": return "item.stone";
                case "Plank":
                case "Cabin Plank": return "item.cabin-plank";
                case "Wild Berries": return "item.wild-berries";
                case "Medicinal Herb": return "item.medicinal-herb";
                case "Mushroom": return "item.mushroom";
                case "Iron Ore": return "item.iron-ore";
                case "Old Coin": return "item.old-coin";
                case "Torch": return "item.torch";
                case "Worn Axe": return "item.tool-axe";
                case "Stone Pick": return "item.tool-pickaxe";
                case "Journal Page":
                case "Roadwarden Page": return "item.roadwarden-page";
                case "Bell Fragment": return "item.bell-fragment";
                case "Cooked Meal": return "item.cooked-meal";
                case "Egg": return "item.egg";
                case "Wool": return "item.wool";
                case "Milk": return "item.milk";
                case "Silver Coin": return "item.silver-coin";
                case "Watering Can": return "item.watering-can";
                case "Wheat Seeds": return "item.seed-wheat";
                case "Corn Seeds": return "item.seed-corn";
                case "Carrot Seeds": return "item.seed-carrot";
                case "Wheat":
                case "Golden Wheat": return "item.wheat";
                case "Corn":
                case "Sweet Corn": return "item.corn";
                case "Carrot":
                case "Crisp Carrot": return "item.carrot";
                case "Wood Fence": return "item.fence-wood";
                case "Wood Gate": return "item.gate-wood";
                case "Pineapple Seeds": return "item.seed-pineapple";
                case "Tomato Seeds": return "item.seed-tomato";
                case "Pineapple":
                case "Sweet Pineapple": return "item.pineapple";
                case "Tomato":
                case "Ripe Tomato": return "item.tomato";
                default: return string.Empty;
            }
        }

        private void DrawCurrencyBadge()
        {
            if (overlayMode != OverlayMode.None || inventorySession == null || inventorySession.Runtime == null) return;

            int coins = inventorySession.Runtime.GetQuantity("item.silver-coin");
            const float width = 180f;
            Rect pill = new Rect(Screen.width - width - 14f - 96f, 14f, 86f, 28f);
            DrawCard(pill, new Color(0.045f, 0.038f, 0.030f, 0.92f));
            DrawCornerAccents(pill, Gold);

            Rect iconRect = new Rect(pill.x + 5f, pill.y + 4f, 20f, 20f);
            DrawSprite(PrototypePixelArtFactory.SilverCoinIcon(), iconRect);

            GUI.Label(new Rect(pill.x + 28f, pill.y + 4f, pill.width - 32f, 20f), $"{coins:N0}", titleStyle);
        }

        private int selectedMerchantTab;
        private string merchantMessage = string.Empty;
        private float merchantMessageHideTime;

        private struct MerchantItem
        {
            public string itemId;
            public int price;
            public MerchantItem(string itemId, int price)
            {
                this.itemId = itemId;
                this.price = price;
            }
        }

        private static readonly MerchantItem[] MerchantBuyList = new MerchantItem[]
        {
            new MerchantItem("item.seed-wheat", 3),
            new MerchantItem("item.seed-corn", 4),
            new MerchantItem("item.seed-carrot", 4),
            new MerchantItem("item.seed-pineapple", 6),
            new MerchantItem("item.seed-tomato", 5),
            new MerchantItem("item.watering-can", 8),
            new MerchantItem("item.cooked-meal", 12),
            new MerchantItem("item.torch", 5),
            new MerchantItem("item.cabin-plank", 6),
            new MerchantItem("item.tool-axe", 10),
            new MerchantItem("item.tool-pickaxe", 12)
        };

        private static readonly MerchantItem[] MerchantSellList = new MerchantItem[]
        {
            new MerchantItem("item.wheat", 5),
            new MerchantItem("item.corn", 7),
            new MerchantItem("item.carrot", 6),
            new MerchantItem("item.pineapple", 12),
            new MerchantItem("item.tomato", 8),
            new MerchantItem("item.egg", 4),
            new MerchantItem("item.wool", 8),
            new MerchantItem("item.milk", 9),
            new MerchantItem("item.iron-ore", 10),
            new MerchantItem("item.wild-berries", 2),
            new MerchantItem("item.medicinal-herb", 4),
            new MerchantItem("item.mushroom", 3),
            new MerchantItem("item.wood", 2),
            new MerchantItem("item.stone", 2)
        };

        private void DrawMerchantShopOverlay()
        {
            float width = Mathf.Min(760f, Screen.width - 70f);
            float height = Mathf.Min(500f, Screen.height - 110f);
            Rect panel = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
            DrawCard(panel, Ink);
            DrawCornerAccents(panel, Gold);
            DrawHeaderStrip(new Rect(panel.x, panel.y, panel.width, 44));

            GUI.Label(new Rect(panel.x + 18f, panel.y + 10f, 320f, 24f), LocalizationRuntime.T("merchant_shop"), titleStyle);

            int playerCoins = GetQuantity("item.silver-coin");
            Rect coinBadge = new Rect(panel.xMax - 140f, panel.y + 10f, 86f, 24f);
            DrawRect(coinBadge, new Color(0.045f, 0.038f, 0.030f, 0.92f));
            DrawBorder(coinBadge, GoldDim, 1f);
            DrawSprite(PrototypePixelArtFactory.SilverCoinIcon(), new Rect(coinBadge.x + 4f, coinBadge.y + 2f, 20f, 20f));
            GUI.Label(new Rect(coinBadge.x + 28f, coinBadge.y + 2f, coinBadge.width - 30f, 20f), $"{playerCoins:N0}", labelStyle);

            if (GUI.Button(new Rect(panel.xMax - 44f, panel.y + 8f, 28f, 28f), "X"))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                overlayMode = OverlayMode.None;
            }

            Rect tabBuy = new Rect(panel.x + 18f, panel.y + 54f, 140f, 34f);
            Rect tabSell = new Rect(panel.x + 168f, panel.y + 54f, 140f, 34f);

            bool isBuy = selectedMerchantTab == 0;
            DrawRect(tabBuy, isBuy ? new Color(0.42f, 0.25f, 0.10f, 0.96f) : InkSoft);
            DrawBorder(tabBuy, isBuy ? Gold : GoldDim, isBuy ? 2f : 1f);
            if (GUI.Button(tabBuy, LocalizationRuntime.T("buy_tab"), isBuy ? labelStyle : smallStyle))
            {
                selectedMerchantTab = 0;
                TheOldRoad.Audio.AudioManager.PlayUiClick();
            }

            DrawRect(tabSell, !isBuy ? new Color(0.42f, 0.25f, 0.10f, 0.96f) : InkSoft);
            DrawBorder(tabSell, !isBuy ? Gold : GoldDim, !isBuy ? 2f : 1f);
            if (GUI.Button(tabSell, LocalizationRuntime.T("sell_tab"), !isBuy ? labelStyle : smallStyle))
            {
                selectedMerchantTab = 1;
                TheOldRoad.Audio.AudioManager.PlayUiClick();
            }

            if (!string.IsNullOrEmpty(merchantMessage) && UnityEngine.Time.unscaledTime <= merchantMessageHideTime)
            {
                Rect msgRect = new Rect(panel.x + 320f, panel.y + 58f, panel.width - 340f, 26f);
                DrawRect(msgRect, new Color(0.12f, 0.32f, 0.18f, 0.90f));
                DrawBorder(msgRect, Gold, 1f);
                GUI.Label(msgRect, merchantMessage, centerStyle);
            }

            Rect gridRect = new Rect(panel.x + 18f, panel.y + 98f, panel.width - 36f, panel.height - 114f);
            DrawRect(gridRect, new Color(0.025f, 0.023f, 0.02f, 0.64f));
            DrawBorder(gridRect, new Color(0.18f, 0.15f, 0.11f, 1f), 1f);

            MerchantItem[] items = isBuy ? MerchantBuyList : MerchantSellList;
            const float itemWidth = 224f;
            const float itemHeight = 64f;
            const float gapX = 12f;
            const float gapY = 8f;
            int cols = 3;

            for (int i = 0; i < items.Length; i++)
            {
                int c = i % cols;
                int r = i / cols;
                Rect card = new Rect(gridRect.x + 12f + c * (itemWidth + gapX), gridRect.y + 12f + r * (itemHeight + gapY), itemWidth, itemHeight);

                string itemId = items[i].itemId;
                int price = items[i].price;
                int owned = GetQuantity(itemId);
                PrototypeItemInfo info = PrototypeItemCatalog.Get(itemId);

                DrawCard(card, InkSoft);
                DrawBorder(card, GoldDim, 1f);

                Rect iconR = new Rect(card.x + 8f, card.y + 8f, 48f, 48f);
                DrawRect(iconR, new Color(0.02f, 0.018f, 0.015f, 1f));
                DrawBorder(iconR, Color.black, 1f);
                DrawSprite(PrototypePixelArtFactory.ItemIcon(itemId), new Rect(iconR.x + 8f, iconR.y + 8f, 32f, 32f));

                string displayName = LocalizeItemName(itemId, info.DisplayName);
                GUI.Label(new Rect(card.x + 60f, card.y + 6f, 100f, 18f), displayName, labelStyle);
                GUI.Label(new Rect(card.x + 60f, card.y + 24f, 100f, 16f), $"{(isBuy ? "Giá" : "Thu")}: {price} 🪙 (Có: {owned})", smallStyle);

                Rect btnRect = new Rect(card.xMax - 58f, card.y + 16f, 50f, 32f);
                if (isBuy)
                {
                    bool canAfford = playerCoins >= price;
                    Color prev = GUI.color;
                    GUI.color = canAfford ? Color.white : new Color(0.6f, 0.6f, 0.6f, 1f);
                    if (GUI.Button(btnRect, LocalizationRuntime.IsVietnamese ? "Mua" : "Buy"))
                    {
                        if (canAfford && inventorySession != null && inventorySession.Runtime != null)
                        {
                            inventorySession.Runtime.TryRemove("item.silver-coin", price);
                            inventorySession.Runtime.Add(itemId, 1);
                            TheOldRoad.Audio.AudioManager.PlayGatherSuccess();
                            merchantMessage = (LocalizationRuntime.IsVietnamese ? "Đã mua: " : "Purchased: ") + displayName;
                            merchantMessageHideTime = UnityEngine.Time.unscaledTime + 2.5f;
                        }
                    }
                    GUI.color = prev;
                }
                else
                {
                    bool hasItem = owned > 0;
                    Color prev = GUI.color;
                    GUI.color = hasItem ? Color.white : new Color(0.6f, 0.6f, 0.6f, 1f);
                    if (GUI.Button(btnRect, LocalizationRuntime.IsVietnamese ? "Bán" : "Sell"))
                    {
                        if (hasItem && inventorySession != null && inventorySession.Runtime != null)
                        {
                            inventorySession.Runtime.TryRemove(itemId, 1);
                            inventorySession.Runtime.Add("item.silver-coin", price);
                            TheOldRoad.Audio.AudioManager.PlayChestOpen();
                            merchantMessage = (LocalizationRuntime.IsVietnamese ? "Đã bán: " : "Sold: ") + displayName + $" (+{price} 🪙)";
                            merchantMessageHideTime = UnityEngine.Time.unscaledTime + 2.5f;
                        }
                    }
                    GUI.color = prev;
                }
            }
        }

        private enum OverlayMode
        {
            None,
            Inventory,
            BuildCatalog,
            Map,
            Journal,
            MerchantShop
        }

        public void ToggleInventoryOverlay()
        {
            ToggleOverlay(OverlayMode.Inventory);
        }

        public void ToggleMerchantOverlay()
        {
            ToggleOverlay(OverlayMode.MerchantShop);
        }

        private void ToggleOverlay(OverlayMode mode)
        {
            overlayMode = overlayMode == mode ? OverlayMode.None : mode;
        }
    }
}
