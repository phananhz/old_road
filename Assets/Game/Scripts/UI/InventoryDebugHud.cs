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
        private string activePromptText = string.Empty;
        private float promptHideTime;
        private string buildCatalogMessage = string.Empty;
        private float buildCatalogMessageHideTime;

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
            for (int i = 0; i < 9; i++)
            {
                if (PrototypeInput.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i))) selectedSlot = i;
            }

            if (PrototypeInput.GetKeyDown(KeyCode.I)) ToggleOverlay(OverlayMode.Inventory);
            if (PrototypeInput.GetKeyDown(KeyCode.M)) ToggleOverlay(OverlayMode.Map);
            if (PrototypeInput.GetKeyDown(KeyCode.J)) ToggleOverlay(OverlayMode.Journal);
            if (PrototypeInput.GetKeyDown(KeyCode.B)) HandleBuildInput();
            if (PrototypeInput.GetKeyDown(KeyCode.Escape)) overlayMode = OverlayMode.None;
        }

        private void OnGUI()
        {
            EnsureStyles();

            DrawScreenVignette();
            DrawStatusCard();
            DrawObjectiveCard();
            DrawMinimapCard();
            DrawPromptRibbon();
            DrawHotbar();
            DrawOverlay();
            DrawMobileActionButtons();
        }

        private void DrawStatusCard()
        {
            Rect card = new Rect(18, 18, 344, 140);
            DrawCard(card, Ink);
            DrawCornerAccents(card, Gold);
            DrawHeaderStrip(new Rect(card.x, card.y, card.width, 32));

            GUI.Label(new Rect(card.x + 16, card.y + 5, card.width - 32, 26), "THE OLD ROAD", gameTitleStyle);
            GUI.Label(new Rect(card.x + 214, card.y + 8, 110, 18), "Valen Outskirts", smallStyle);

            Rect portrait = new Rect(card.x + 16, card.y + 44, 46, 46);
            DrawRect(portrait, new Color(0.18f, 0.13f, 0.09f, 1f));
            DrawBorder(portrait, GoldDim, 2f);
            DrawPlayerBadge(portrait);

            PlayerVitals vitals = FindAnyObjectByType<PlayerVitals>();
            int currentHealth = vitals != null ? vitals.CurrentHealth : 20;
            int maxHealth = vitals != null ? vitals.MaxHealth : 20;
            GUI.Label(new Rect(card.x + 74, card.y + 42, 96, 18), "Roadwarden", labelStyle);
            DrawHealthBar(new Rect(card.x + 74, card.y + 65, 194, 16), currentHealth, maxHealth);
            GUI.Label(new Rect(card.x + 276, card.y + 59, 48, 24), currentHealth + "/" + maxHealth, labelStyle);

            if (sliceController != null)
            {
                string progress = "Landmarks " + sliceController.DiscoveredLandmarkCount + "/" + sliceController.TotalLandmarkCount;
                GUI.Label(new Rect(card.x + 74, card.y + 91, 220, 18), progress, smallStyle);
            }

            float chipY = card.y + 113;
            DrawResourceChip(new Rect(card.x + 16, chipY, 72, 18), "Wood", GetQuantity("item.wood"), PrototypeItemCatalog.Get("item.wood").Color);
            DrawResourceChip(new Rect(card.x + 96, chipY, 74, 18), "Stone", GetQuantity("item.stone"), PrototypeItemCatalog.Get("item.stone").Color);
            DrawResourceChip(new Rect(card.x + 178, chipY, 70, 18), "Food", GetForageQuantity(), PrototypeItemCatalog.Get("item.wild-berries").Color);
            DrawResourceChip(new Rect(card.x + 256, chipY, 70, 18), "Ore", GetQuantity("item.iron-ore"), PrototypeItemCatalog.Get("item.iron-ore").Color);
        }

        private void DrawMinimapCard()
        {
            const float width = 220f;
            Rect card = new Rect(Screen.width - width - 18f, 18f, width, 270f);
            DrawCard(card, Ink);
            DrawCornerAccents(card, GoldDim);

            GameTimeController gameTime = FindAnyObjectByType<GameTimeController>();
            string timeText = gameTime != null ? gameTime.ClockText : "Day 1  06:00";
            GUI.Label(new Rect(card.x + 14, card.y + 9, card.width - 28, 22), timeText, titleStyle);
            GUI.Label(new Rect(card.x + 14, card.y + 31, card.width - 28, 18), "Old road survey", smallStyle);

            Rect map = new Rect(card.x + 14, card.y + 54, card.width - 28, card.width - 28);
            DrawRect(new Rect(map.x - 5, map.y - 5, map.width + 10, map.height + 10), new Color(0.02f, 0.018f, 0.015f, 1f));
            DrawMinimap(map);

            DrawControlPill(new Rect(card.x + 18, map.yMax + 12, 82, 22), "M", "Map");
            DrawControlPill(new Rect(card.x + 106, map.yMax + 12, 44, 22), "I", "Bag");
            DrawControlPill(new Rect(card.x + 156, map.yMax + 12, 44, 22), "J", "Log");
        }

        private void DrawObjectiveCard()
        {
            if (sliceController == null || overlayMode != OverlayMode.None) return;

            string[] objectives = sliceController.ObjectiveDisplayLines;
            if (objectives == null || objectives.Length == 0) return;

            Rect card = new Rect(18f, 170f, 344f, 34f + objectives.Length * 22f);
            DrawCard(card, new Color(0.045f, 0.036f, 0.028f, 0.88f));
            DrawCornerAccents(card, GoldDim);

            GUI.Label(
                new Rect(card.x + 16f, card.y + 8f, card.width - 32f, 20f),
                "Current Roadwarden Tasks  " + sliceController.CompletedObjectiveCount + "/" + sliceController.TotalObjectiveCount,
                labelStyle);

            for (int i = 0; i < objectives.Length; i++)
            {
                bool done = objectives[i].StartsWith("[x]");
                Color previous = GUI.color;
                GUI.color = done ? new Color(0.70f, 0.92f, 0.58f, 1f) : Parchment;
                GUI.Label(new Rect(card.x + 18f, card.y + 34f + i * 22f, card.width - 36f, 20f), objectives[i], smallStyle);
                GUI.color = previous;
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

            float width = Mathf.Min(680f, Screen.width - 80f);
            float y = Screen.width >= 1100f ? 24f : 166f;
            Rect ribbon = new Rect((Screen.width - width) * 0.5f, y, width, 38f);
            DrawRect(new Rect(ribbon.x + 4, ribbon.y + 5, ribbon.width, ribbon.height), Shadow);
            DrawRect(ribbon, new Color(0.055f, 0.038f, 0.025f, 0.88f));
            DrawBorder(ribbon, GoldDim, 2f);
            DrawBorder(new Rect(ribbon.x + 5, ribbon.y + 5, ribbon.width - 10, ribbon.height - 10), new Color(0.22f, 0.15f, 0.08f, 1f), 1f);
            GUI.Label(ribbon, activePromptText, promptStyle);
        }

        private void DrawHotbar()
        {
            const int slotCount = 9;
            float slotSize = Mathf.Clamp((Screen.width - 160f) / slotCount, 54f, 70f);
            const float gap = 5f;
            float totalWidth = slotCount * slotSize + (slotCount - 1) * gap;
            float startX = (Screen.width - totalWidth) * 0.5f;
            float y = Screen.height - slotSize - 24f;

            Rect backing = new Rect(startX - 16f, y - 42f, totalWidth + 32f, slotSize + 64f);
            DrawRect(new Rect(backing.x + 5, backing.y + 7, backing.width, backing.height), Shadow);
            DrawCard(backing, new Color(0.025f, 0.022f, 0.02f, 0.88f));
            DrawCornerAccents(backing, GoldDim);

            for (int i = 0; i < slotCount; i++)
            {
                DrawHotbarSlot(new Rect(startX + i * (slotSize + gap), y, slotSize, slotSize), i);
            }

            GUI.Label(new Rect(startX, y - 33f, totalWidth, 20f), "E Gather/Inspect    F Use/Enter/Sleep    C Craft    B Build    I Bag    M Map    J Journal", centerStyle);
        }

        private void DrawHotbarSlot(Rect slot, int index)
        {
            Event current = Event.current;
            if (current != null && current.type == EventType.MouseDown && current.button == 0 && slot.Contains(current.mousePosition))
            {
                selectedSlot = index;
                current.Use();
            }

            bool selected = index == selectedSlot;
            Color background = selected ? new Color(0.40f, 0.27f, 0.10f, 0.98f) : new Color(0.095f, 0.08f, 0.065f, 0.96f);
            if (selected) DrawRect(new Rect(slot.x - 4, slot.y - 4, slot.width + 8, slot.height + 8), new Color(0.95f, 0.64f, 0.18f, 0.18f));
            DrawRect(slot, background);
            DrawBorder(slot, selected ? Gold : new Color(0.24f, 0.21f, 0.18f, 1f), selected ? 3f : 2f);
            DrawInset(slot, selected ? new Color(0.82f, 0.52f, 0.18f, 0.45f) : new Color(1f, 1f, 1f, 0.06f));

            HotbarItem item = GetHotbarItem(index);
            GUI.Label(new Rect(slot.x + 5, slot.y + 3, 18, 18), (index + 1).ToString(), numberStyle);

            if (item.IsEmpty)
            {
                DrawRect(new Rect(slot.x + slot.width * 0.32f, slot.y + slot.height * 0.50f, slot.width * 0.36f, 2), new Color(0.25f, 0.23f, 0.20f, 1f));
                return;
            }

            Rect icon = new Rect(slot.x + slot.width * 0.5f - 15f, slot.y + 16f, 30f, 26f);
            DrawItemGlyph(icon, item);
            GUI.Label(new Rect(slot.x + 3, slot.y + slot.height - 20f, slot.width - 6, 16), item.Name, captionStyle);
            if (item.Count > 0) GUI.Label(new Rect(slot.x + slot.width - 28f, slot.y + 39f, 23f, 18), item.Count.ToString(), numberStyle);
        }

        private void DrawMobileActionButtons()
        {
            if (overlayMode != OverlayMode.None)
            {
                DrawMobileActionButton(new Rect(Screen.width - 112f, Screen.height - 122f, 82f, 54f), "Esc", "Close", KeyCode.Escape, new Color(0.50f, 0.18f, 0.14f, 0.96f));
                return;
            }

            float right = Screen.width - 28f;
            float bottom = Screen.height - 30f;

            DrawMobileActionButton(new Rect(right - 92f, bottom - 168f, 78f, 58f), "E", "Gather", KeyCode.E, new Color(0.22f, 0.44f, 0.18f, 0.96f));
            DrawMobileActionButton(new Rect(right - 178f, bottom - 112f, 74f, 52f), "C", "Craft", KeyCode.C, new Color(0.45f, 0.31f, 0.12f, 0.96f));
            DrawMobileActionButton(new Rect(right - 92f, bottom - 102f, 78f, 58f), "B", "Build", KeyCode.B, new Color(0.45f, 0.21f, 0.12f, 0.96f));
            PlayerCookingInteractor cooking = FindAnyObjectByType<PlayerCookingInteractor>();
            if (cooking != null && cooking.CanCookAction)
            {
                DrawMobileActionButton(new Rect(right - 178f, bottom - 234f, 74f, 52f), "R", "Cook", KeyCode.R, new Color(0.54f, 0.24f, 0.10f, 0.96f));
            }

            PlayerCabinInteractor cabin = FindAnyObjectByType<PlayerCabinInteractor>();
            if (cabin != null && cabin.CanUseAction)
            {
                DrawMobileActionButton(new Rect(right - 178f, bottom - 176f, 74f, 52f), "F", cabin.ActionButtonLabel, KeyCode.F, new Color(0.36f, 0.20f, 0.42f, 0.96f));
            }
            DrawMobileActionButton(new Rect(right - 260f, bottom - 100f, 68f, 46f), "I", "Bag", KeyCode.I, new Color(0.15f, 0.22f, 0.33f, 0.96f));
            DrawMobileActionButton(new Rect(right - 260f, bottom - 154f, 68f, 46f), "M", "Map", KeyCode.M, new Color(0.18f, 0.27f, 0.23f, 0.96f));
            DrawMobileActionButton(new Rect(right - 260f, bottom - 208f, 68f, 46f), "J", "Log", KeyCode.J, new Color(0.22f, 0.18f, 0.32f, 0.96f));
        }

        private void DrawMobileActionButton(Rect rect, string key, string label, KeyCode keyCode, Color color)
        {
            Event current = Event.current;
            if (current != null && current.type == EventType.MouseDown && current.button == 0 && rect.Contains(current.mousePosition))
            {
                PrototypeInput.QueueKeyDown(keyCode);
                current.Use();
            }

            DrawRect(new Rect(rect.x + 4f, rect.y + 5f, rect.width, rect.height), Shadow);
            DrawRect(rect, color);
            DrawBorder(rect, new Color(0.02f, 0.015f, 0.01f, 0.96f), 2f);
            DrawBorder(new Rect(rect.x + 3f, rect.y + 3f, rect.width - 6f, rect.height - 6f), GoldDim, 1f);
            GUI.Label(new Rect(rect.x, rect.y + 5f, rect.width, 22f), key, titleStyle);
            GUI.Label(new Rect(rect.x, rect.y + 27f, rect.width, 18f), label, centerStyle);
        }

        private void DrawOverlay()
        {
            if (overlayMode == OverlayMode.None) return;

            DrawRect(new Rect(0, 0, Screen.width, Screen.height), new Color(0f, 0f, 0f, 0.58f));

            if (overlayMode == OverlayMode.Inventory) DrawInventoryOverlay();
            if (overlayMode == OverlayMode.BuildCatalog) DrawBuildCatalogOverlay();
            if (overlayMode == OverlayMode.Map) DrawMapOverlay();
            if (overlayMode == OverlayMode.Journal) DrawJournalOverlay();
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
            float width = Mathf.Min(760f, Screen.width - 70f);
            float height = Mathf.Min(500f, Screen.height - 110f);
            Rect panel = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
            DrawCard(panel, Ink);
            DrawCornerAccents(panel, Gold);
            DrawHeaderStrip(new Rect(panel.x, panel.y, panel.width, 44));

            GUI.Label(new Rect(panel.x + 24, panel.y + 9, panel.width - 48, 28), "Roadwarden Pack", gameTitleStyle);
            GUI.Label(new Rect(panel.x + panel.width - 190, panel.y + 14, 170, 20), "I / Esc to close", smallStyle);

            GUI.Label(new Rect(panel.x + 26, panel.y + 54, panel.width - 52, 20), "Materials gathered from Valen Outskirts", smallStyle);
            Rect grid = new Rect(panel.x + 24, panel.y + 82, panel.width - 48, panel.height - 110);
            DrawInventoryGrid(grid);
        }

        private void DrawInventoryGrid(Rect rect)
        {
            DrawRect(rect, new Color(0.025f, 0.023f, 0.02f, 0.64f));
            DrawBorder(rect, new Color(0.18f, 0.15f, 0.11f, 1f), 2f);

            PrototypeItemInfo[] items = PrototypeItemCatalog.All;

            const float slotSize = 76f;
            const float gap = 10f;
            int columns = Mathf.Max(1, Mathf.FloorToInt((rect.width - 24f + gap) / (slotSize + gap)));

            for (int i = 0; i < items.Length; i++)
            {
                PrototypeItemInfo item = items[i];
                int column = i % columns;
                int row = i / columns;
                Rect slot = new Rect(rect.x + 14 + column * (slotSize + gap), rect.y + 14 + row * (slotSize + gap), slotSize, slotSize);
                DrawInventorySlot(slot, item);
            }
        }

        private void DrawInventorySlot(Rect slot, PrototypeItemInfo item)
        {
            int quantity = GetQuantity(item.ItemId);
            bool hasItem = quantity > 0;
            DrawRect(slot, hasItem ? InkSoft : new Color(0.045f, 0.041f, 0.037f, 0.88f));
            DrawBorder(slot, hasItem ? GoldDim : new Color(0.20f, 0.18f, 0.15f, 1f), 1f);

            Rect icon = new Rect(slot.x + 13f, slot.y + 8f, slot.width - 26f, 38f);
            Color previous = GUI.color;
            GUI.color = hasItem ? Color.white : new Color(1f, 1f, 1f, 0.42f);
            DrawItemGlyph(icon, new HotbarItem(item.DisplayName, item.Icon, quantity, item.Color));
            GUI.color = previous;

            Rect quantityBadge = new Rect(slot.xMax - 34f, slot.yMax - 25f, 27f, 18f);
            DrawRect(quantityBadge, hasItem ? new Color(0.02f, 0.018f, 0.015f, 0.92f) : new Color(0.02f, 0.018f, 0.015f, 0.55f));
            DrawBorder(quantityBadge, hasItem ? GoldDim : new Color(0.18f, 0.16f, 0.14f, 1f), 1f);
            GUI.Label(quantityBadge, quantity.ToString(), centerStyle);

            GUI.Label(new Rect(slot.x + 5f, slot.yMax - 43f, slot.width - 10f, 18f), GetShortItemName(item.DisplayName), centerStyle);
        }

        private void DrawBuildCatalogOverlay()
        {
            float panelWidth = Mathf.Min(Screen.width - 70f, 1040f);
            float panelHeight = Mathf.Min(Screen.height - 90f, 650f);
            Rect panel = new Rect((Screen.width - panelWidth) * 0.5f, (Screen.height - panelHeight) * 0.5f, panelWidth, panelHeight);
            DrawCard(panel, Ink);
            DrawCornerAccents(panel, Gold);
            DrawHeaderStrip(new Rect(panel.x, panel.y, panel.width, 50f));

            GUI.Label(new Rect(panel.x + 24f, panel.y + 10f, panel.width - 48f, 28f), "Construction Catalog", gameTitleStyle);
            GUI.Label(new Rect(panel.x + panel.width - 260f, panel.y + 16f, 230f, 20f), "B / Esc to close", smallStyle);

            Rect sidebar = new Rect(panel.x + 22f, panel.y + 72f, 190f, panel.height - 100f);
            Rect content = new Rect(sidebar.xMax + 18f, sidebar.y, panel.xMax - sidebar.xMax - 40f, sidebar.height);

            DrawBuildCategorySidebar(sidebar);
            DrawBuildCatalogContent(content);
        }

        private void DrawBuildCategorySidebar(Rect rect)
        {
            DrawRect(rect, new Color(0.025f, 0.023f, 0.02f, 0.76f));
            DrawBorder(rect, GoldDim, 1f);
            GUI.Label(new Rect(rect.x + 14f, rect.y + 12f, rect.width - 28f, 24f), "Categories", titleStyle);

            DrawBuildCategoryButton(new Rect(rect.x + 14f, rect.y + 52f, rect.width - 28f, 42f), 0, "Housing", "Homes and shelters");
            DrawBuildCategoryButton(new Rect(rect.x + 14f, rect.y + 104f, rect.width - 28f, 42f), 1, "Fire & Light", "Warmth and camp utility");
            DrawBuildCategoryButton(new Rect(rect.x + 14f, rect.y + 156f, rect.width - 28f, 42f), 2, "Animal Pens", "Fenced square or rectangle yards");

            GUI.Label(new Rect(rect.x + 14f, rect.yMax - 72f, rect.width - 28f, 52f), "Select a buildable card to enter placement mode.", smallStyle);
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
                current.Use();
            }

            GUI.Label(new Rect(rect.x + 12f, rect.y + 5f, rect.width - 24f, 20f), title, labelStyle);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 24f, rect.width - 24f, 16f), subtitle, smallStyle);
        }

        private void DrawBuildCatalogContent(Rect rect)
        {
            DrawRect(rect, new Color(0.025f, 0.023f, 0.02f, 0.64f));
            DrawBorder(rect, new Color(0.18f, 0.15f, 0.11f, 1f), 2f);

            string heading = selectedBuildCategory == 0 ? "Housing" : selectedBuildCategory == 1 ? "Fire & Light" : "Animal Pens";
            GUI.Label(new Rect(rect.x + 18f, rect.y + 14f, rect.width - 36f, 24f), heading, titleStyle);
            if (!string.IsNullOrWhiteSpace(buildCatalogMessage) && UnityEngine.Time.unscaledTime <= buildCatalogMessageHideTime)
            {
                Rect message = new Rect(rect.x + 150f, rect.y + 12f, rect.width - 168f, 26f);
                DrawRect(message, new Color(0.20f, 0.055f, 0.035f, 0.88f));
                DrawBorder(message, new Color(0.84f, 0.28f, 0.18f, 1f), 1f);
                GUI.Label(message, buildCatalogMessage, centerStyle);
            }

            Rect grid = new Rect(rect.x + 18f, rect.y + 52f, rect.width - 36f, rect.height - 72f);
            const float cardWidth = 220f;
            const float cardHeight = 242f;
            const float gap = 18f;
            int columns = Mathf.Max(1, Mathf.FloorToInt((grid.width + gap) / (cardWidth + gap)));

            if (selectedBuildCategory == 0)
            {
                DrawBuildCatalogCard(GetBuildCardRect(grid, cardWidth, cardHeight, gap, columns, 0), "Cabin", "Starter home with bed and interior.", "Housing", GetBuildingDefinition("building.cabin"), "Cabin", true);
                DrawBuildCatalogCard(GetBuildCardRect(grid, cardWidth, cardHeight, gap, columns, 1), "Stone Cottage", "Larger stone home prototype.", "Housing", GetBuildingDefinition("building.stone-cottage"), "Cottage", true);
                DrawBuildCatalogCard(GetBuildCardRect(grid, cardWidth, cardHeight, gap, columns, 2), "Storage Shed", "Small utility storage building.", "Housing", GetBuildingDefinition("building.storage-shed"), "Shed", true);
                return;
            }

            if (selectedBuildCategory == 1)
            {
                DrawBuildCatalogCard(GetBuildCardRect(grid, cardWidth, cardHeight, gap, columns, 0), "Campfire", "Small outdoor fire, light source, and cooking spot.", "Fire & Light", GetBuildingDefinition("building.campfire"), "Campfire", true);
                DrawBuildCatalogCard(GetBuildCardRect(grid, cardWidth, cardHeight, gap, columns, 1), "Cooking Hearth", "Stronger cooking station with warm light.", "Fire & Light", GetBuildingDefinition("building.cooking-hearth"), "Hearth", true);
                return;
            }

            DrawBuildCatalogCard(GetBuildCardRect(grid, cardWidth, cardHeight, gap, columns, 0), "Small Animal Pen", "Square fenced yard. Produces eggs in prototype.", "Animal Pens", GetBuildingDefinition("building.animal-pen-small"), "PenSquare", true);
            DrawBuildCatalogCard(GetBuildCardRect(grid, cardWidth, cardHeight, gap, columns, 1), "Long Animal Pen", "Rectangle fenced yard. Produces wool in prototype.", "Animal Pens", GetBuildingDefinition("building.animal-pen-long"), "PenLong", true);
        }

        private BuildingDefinition GetBuildingDefinition(string buildingId)
        {
            if (sliceController == null) return placementController != null && placementController.BuildingDefinition != null && placementController.BuildingDefinition.BuildingId == buildingId
                ? placementController.BuildingDefinition
                : null;

            return sliceController.GetBuildingDefinition(buildingId);
        }

        private static Rect GetBuildCardRect(Rect grid, float cardWidth, float cardHeight, float gap, int columns, int index)
        {
            int column = index % columns;
            int row = index / columns;
            return new Rect(grid.x + column * (cardWidth + gap), grid.y + row * (cardHeight + gap), cardWidth, cardHeight);
        }

        private void DrawBuildCatalogCard(Rect rect, string name, string description, string category, BuildingDefinition definition, string glyph, bool buildable)
        {
            bool hasMaterials = HasBuildMaterials(definition);
            bool canBuild = buildable && definition != null && placementController != null && hasMaterials;
            DrawRect(rect, buildable ? InkSoft : new Color(0.055f, 0.052f, 0.048f, 0.86f));
            DrawBorder(rect, canBuild ? GoldDim : new Color(0.34f, 0.22f, 0.16f, 1f), canBuild ? 1f : 2f);

            Rect icon = new Rect(rect.x + 18f, rect.y + 18f, 70f, 62f);
            DrawBuildingGlyph(icon, glyph);

            GUI.Label(new Rect(rect.x + 100f, rect.y + 15f, rect.width - 114f, 22f), name, labelStyle);
            GUI.Label(new Rect(rect.x + 100f, rect.y + 38f, rect.width - 114f, 18f), category, smallStyle);
            GUI.Label(new Rect(rect.x + 100f, rect.y + 58f, rect.width - 114f, 44f), description, smallStyle);

            Rect requirements = new Rect(rect.x + 16f, rect.y + 104f, rect.width - 32f, 74f);
            DrawRect(requirements, new Color(0.030f, 0.026f, 0.022f, 0.78f));
            DrawBorder(requirements, new Color(0.16f, 0.13f, 0.10f, 1f), 1f);
            GUI.Label(new Rect(requirements.x + 10f, requirements.y + 6f, requirements.width - 20f, 18f), "Required items", smallStyle);

            if (definition != null)
            {
                DrawBuildRequirements(requirements, definition.ConstructionCosts);
            }
            else
            {
                GUI.Label(new Rect(requirements.x + 10f, requirements.y + 30f, requirements.width - 20f, 34f), "Prototype: requirements not finalized yet.", smallStyle);
            }

            Rect action = new Rect(rect.x + 18f, rect.yMax - 48f, rect.width - 36f, 32f);
            if (canBuild)
            {
                if (GUI.Button(action, "Select & Place"))
                {
                    buildCatalogMessage = string.Empty;
                    placementController.BeginPlacement(definition);
                    overlayMode = OverlayMode.None;
                    activePromptText = name + " selected. Move cursor to a valid grid cell, then left click.";
                    promptHideTime = UnityEngine.Time.unscaledTime + PromptVisibleSeconds;
                }
            }
            else if (buildable && definition != null && placementController != null)
            {
                if (GUI.Button(action, "Not enough items"))
                {
                    ShowBuildCatalogMessage("Cannot build " + name + ". " + GetMissingBuildMaterialsText(definition));
                }
            }
            else
            {
                DrawRect(action, new Color(0.11f, 0.10f, 0.09f, 0.90f));
                DrawBorder(action, new Color(0.22f, 0.20f, 0.18f, 1f), 1f);
                GUI.Label(action, buildable ? "Missing definition" : "Coming soon", centerStyle);
            }
        }

        private void DrawBuildRequirements(Rect rect, BuildCostEntry[] costs)
        {
            if (costs == null || costs.Length == 0)
            {
                GUI.Label(new Rect(rect.x + 10f, rect.y + 30f, rect.width - 20f, 20f), "No material cost.", smallStyle);
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
                GUI.Label(new Rect(row.x + 16f, row.y - 1f, row.width - 16f, row.height), item.DisplayName + " " + owned + "/" + cost.quantity, smallStyle);
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
            if (definition == null || definition.ConstructionCosts == null) return "Missing building definition.";

            string message = "Missing: ";
            bool hasMissing = false;
            for (int i = 0; i < definition.ConstructionCosts.Length; i++)
            {
                BuildCostEntry cost = definition.ConstructionCosts[i];
                int owned = GetQuantity(cost.itemId);
                if (owned >= cost.quantity) continue;

                PrototypeItemInfo item = PrototypeItemCatalog.Get(cost.itemId);
                if (hasMissing) message += ", ";
                message += item.DisplayName + " " + owned + "/" + cost.quantity;
                hasMissing = true;
            }

            return hasMissing ? message : "Materials are ready.";
        }

        private void ShowBuildCatalogMessage(string message)
        {
            buildCatalogMessage = message;
            buildCatalogMessageHideTime = UnityEngine.Time.unscaledTime + PromptVisibleSeconds;
            activePromptText = message;
            promptHideTime = UnityEngine.Time.unscaledTime + PromptVisibleSeconds;
        }

        private void DrawBuildingGlyph(Rect rect, string glyph)
        {
            DrawRect(rect, new Color(0.025f, 0.022f, 0.018f, 1f));
            DrawBorder(rect, Color.black, 1f);

            if (glyph == "Campfire" || glyph == "Hearth")
            {
                DrawRect(new Rect(rect.x + rect.width * 0.20f, rect.y + rect.height * 0.66f, rect.width * 0.60f, rect.height * 0.10f), new Color(0.30f, 0.24f, 0.18f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.30f, rect.y + rect.height * 0.50f, rect.width * 0.40f, rect.height * 0.14f), new Color(0.55f, 0.33f, 0.14f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.36f, rect.y + rect.height * 0.28f, rect.width * 0.28f, rect.height * 0.30f), new Color(0.96f, 0.30f, 0.08f, 1f));
                DrawRect(new Rect(rect.x + rect.width * 0.44f, rect.y + rect.height * 0.20f, rect.width * 0.14f, rect.height * 0.28f), new Color(1f, 0.78f, 0.22f, 1f));
                return;
            }

            if (glyph == "PenSquare" || glyph == "PenLong")
            {
                Rect fence = glyph == "PenLong"
                    ? new Rect(rect.x + rect.width * 0.14f, rect.y + rect.height * 0.30f, rect.width * 0.72f, rect.height * 0.42f)
                    : new Rect(rect.x + rect.width * 0.24f, rect.y + rect.height * 0.24f, rect.width * 0.52f, rect.height * 0.52f);
                DrawBorder(fence, new Color(0.58f, 0.34f, 0.16f, 1f), 4f);
                DrawRect(new Rect(fence.x + 5f, fence.y + 5f, fence.width - 10f, fence.height - 10f), new Color(0.12f, 0.28f, 0.12f, 1f));
                DrawRect(new Rect(fence.x + fence.width * 0.46f, fence.yMax - 4f, fence.width * 0.18f, 5f), new Color(0.10f, 0.07f, 0.04f, 1f));
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

            GUI.Label(new Rect(panel.x + 24, panel.y + 10, panel.width - 48, 28), "Valen Outskirts Map", gameTitleStyle);
            GUI.Label(new Rect(panel.x + panel.width - 210, panel.y + 16, 190, 20), "M / Esc to close", smallStyle);

            float legendWidth = panel.width > 760f ? 220f : 0f;
            Rect mapRect = new Rect(panel.x + 26, panel.y + 72, panel.width - 52 - legendWidth, panel.height - 102);
            DrawRect(new Rect(mapRect.x - 6, mapRect.y - 6, mapRect.width + 12, mapRect.height + 12), new Color(0.02f, 0.018f, 0.015f, 1f));
            DrawMinimap(mapRect);

            if (legendWidth > 0f)
            {
                Rect legend = new Rect(mapRect.xMax + 20f, mapRect.y, legendWidth - 20f, mapRect.height);
                DrawMapLegend(legend);
            }
        }

        private void DrawJournalOverlay()
        {
            float width = Mathf.Min(820f, Screen.width - 80f);
            float height = Mathf.Min(560f, Screen.height - 100f);
            Rect panel = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
            DrawCard(panel, Ink);
            DrawCornerAccents(panel, Gold);
            DrawHeaderStrip(new Rect(panel.x, panel.y, panel.width, 48));

            GUI.Label(new Rect(panel.x + 24, panel.y + 10, panel.width - 48, 28), "Roadwarden Journal", gameTitleStyle);
            GUI.Label(new Rect(panel.x + panel.width - 210, panel.y + 16, 190, 20), "J / Esc to close", smallStyle);

            string status = sliceController != null
                ? sliceController.LastDiscoveryStatus + "  " + sliceController.DiscoveredLandmarkCount + "/" + sliceController.TotalLandmarkCount
                : "Inspect landmarks to fill the journal.";
            GUI.Label(new Rect(panel.x + 26, panel.y + 60, panel.width - 52, 22), status, labelStyle);

            Rect list = new Rect(panel.x + 24, panel.y + 92, panel.width - 48, panel.height - 120);
            DrawRect(list, new Color(0.025f, 0.023f, 0.02f, 0.64f));
            DrawBorder(list, new Color(0.18f, 0.15f, 0.11f, 1f), 2f);

            DiscoverableLandmark[] landmarks = FindObjectsByType<DiscoverableLandmark>(FindObjectsInactive.Exclude);
            if (landmarks.Length == 0)
            {
                GUI.Label(new Rect(list.x + 18, list.y + 18, list.width - 36, 24), "No landmarks found in this scene.", smallStyle);
                return;
            }

            float rowY = list.y + 16f;
            for (int i = 0; i < landmarks.Length; i++)
            {
                DiscoverableLandmark landmark = landmarks[i];
                if (landmark == null) continue;

                Rect row = new Rect(list.x + 14f, rowY, list.width - 28f, 72f);
                DrawRect(row, landmark.IsDiscovered ? InkSoft : new Color(0.045f, 0.04f, 0.035f, 0.72f));
                DrawBorder(row, landmark.IsDiscovered ? GoldDim : new Color(0.16f, 0.14f, 0.12f, 1f), 1f);
                DrawRect(new Rect(row.x + 12f, row.y + 18f, 34f, 34f), landmark.IsDiscovered ? new Color(0.36f, 0.58f, 0.68f, 1f) : new Color(0.16f, 0.15f, 0.14f, 1f));
                DrawBorder(new Rect(row.x + 12f, row.y + 18f, 34f, 34f), Color.black, 1f);
                GUI.Label(new Rect(row.x + 58f, row.y + 10f, row.width - 72f, 22f), landmark.IsDiscovered ? landmark.Title : "Unknown landmark", labelStyle);
                GUI.Label(
                    new Rect(row.x + 58f, row.y + 34f, row.width - 72f, 28f),
                    landmark.IsDiscovered ? landmark.JournalText : "Follow the road and inspect this place to record it.",
                    smallStyle);
                rowY += 82f;
                if (rowY > list.yMax - 72f) break;
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

            DrawLandmarkDot(map, "Northern Waystone");
            DrawLandmarkDot(map, "Old Road Sign");
            DrawLandmarkDot(map, "Broken Watch Arch");
            DrawLandmarkDot(map, "River Footbridge");
            DrawLandmarkDot(map, "Abandoned Camp");
            DrawLandmarkDot(map, "Eastern Bell Marker");
            DrawLandmarkDot(map, "Hunter Shrine");
            DrawLandmarkDot(map, "South Ruin Gate");

            foreach (LootChest chest in FindObjectsByType<LootChest>(FindObjectsInactive.Exclude))
            {
                if (chest == null || chest.IsOpened) continue;
                DrawMapDot(map, chest.transform.position, new Color(1f, 0.78f, 0.24f, 1f), map.width > 220f ? 9f : 5f);
            }

            foreach (ResourceNode node in FindObjectsByType<ResourceNode>(FindObjectsInactive.Exclude))
            {
                if (node == null || node.IsHarvested) continue;
                DrawMapDot(map, node.transform.position, GetResourceMapColor(node.ResourceItemId), map.width > 220f ? 9f : 5f);
            }

            foreach (ConstructionSite site in FindObjectsByType<ConstructionSite>(FindObjectsInactive.Exclude))
            {
                if (site == null) continue;
                DrawMapDot(map, site.transform.position, new Color(0.95f, 0.62f, 0.22f, 1f), map.width > 220f ? 10f : 6f);
            }

            foreach (VillagerNpcController npc in FindObjectsByType<VillagerNpcController>(FindObjectsInactive.Exclude))
            {
                if (npc == null) continue;
                DrawMapDot(map, npc.transform.position, new Color(0.92f, 0.78f, 0.42f, 1f), map.width > 220f ? 8f : 5f);
            }

            PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
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

        private void DrawLandmarkDot(Rect map, string objectName)
        {
            GameObject landmark = GameObject.Find(objectName);
            if (landmark == null) return;
            DrawMapDot(map, landmark.transform.position, new Color(0.68f, 0.42f, 0.92f, 1f), map.width > 220f ? 9f : 5f);
        }

        private string BuildPromptText()
        {
            string prompt = string.Empty;

            PlayerGatheringInteractor gathering = FindAnyObjectByType<PlayerGatheringInteractor>();
            if (gathering != null && !string.IsNullOrWhiteSpace(gathering.InteractionHint)) prompt = gathering.InteractionHint;

            PlayerCraftingInteractor crafting = FindAnyObjectByType<PlayerCraftingInteractor>();
            if (crafting != null && !string.IsNullOrWhiteSpace(crafting.CraftingHint)) AppendPrompt(ref prompt, crafting.CraftingHint);

            PlayerCookingInteractor cooking = FindAnyObjectByType<PlayerCookingInteractor>();
            if (cooking != null && !string.IsNullOrWhiteSpace(cooking.CookingHint)) AppendPrompt(ref prompt, cooking.CookingHint);

            PlayerLandmarkInteractor landmark = FindAnyObjectByType<PlayerLandmarkInteractor>();
            if (landmark != null && !string.IsNullOrWhiteSpace(landmark.InteractionHint)) AppendPrompt(ref prompt, landmark.InteractionHint);

            PlayerLootInteractor loot = FindAnyObjectByType<PlayerLootInteractor>();
            if (loot != null && !string.IsNullOrWhiteSpace(loot.InteractionHint)) AppendPrompt(ref prompt, loot.InteractionHint);

            PlayerCabinInteractor cabin = FindAnyObjectByType<PlayerCabinInteractor>();
            if (cabin != null && !string.IsNullOrWhiteSpace(cabin.InteractionHint)) AppendPrompt(ref prompt, cabin.InteractionHint);

            if (placementController != null && !string.IsNullOrWhiteSpace(placementController.LastStatus)) AppendPrompt(ref prompt, placementController.LastStatus);

            foreach (AnimalPenController pen in FindObjectsByType<AnimalPenController>(FindObjectsInactive.Exclude))
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

        private HotbarItem GetHotbarItem(int index)
        {
            switch (index)
            {
                case 0: return new HotbarItem("Wood", "W", GetQuantity("item.wood"), new Color(0.47f, 0.29f, 0.12f, 1f));
                case 1: return new HotbarItem("Stone", "S", GetQuantity("item.stone"), new Color(0.45f, 0.48f, 0.52f, 1f));
                case 2: return new HotbarItem("Plank", "P", GetQuantity("item.cabin-plank"), new Color(0.74f, 0.50f, 0.25f, 1f));
                case 3: return ToHotbarItem("item.wild-berries");
                case 4: return ToHotbarItem("item.medicinal-herb");
                case 5: return ToHotbarItem("item.mushroom");
                case 6: return ToHotbarItem("item.iron-ore");
                case 7: return ToHotbarItem("item.torch");
                case 8: return new HotbarItem("Cabin", "B", 1, new Color(0.65f, 0.38f, 0.18f, 1f));
                default: return HotbarItem.Empty;
            }
        }

        private HotbarItem ToHotbarItem(string itemId)
        {
            PrototypeItemInfo item = PrototypeItemCatalog.Get(itemId);
            return new HotbarItem(item.DisplayName, item.Icon, GetQuantity(item.ItemId), item.Color);
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
            DrawRect(rect, BloodDark);
            float fill = maxHealth <= 0 ? 0f : Mathf.Clamp01(currentHealth / (float)maxHealth);
            DrawRect(new Rect(rect.x, rect.y, rect.width * fill, rect.height), Blood);
            DrawRect(new Rect(rect.x, rect.y, rect.width * fill, 3f), new Color(0.95f, 0.25f, 0.17f, 1f));
            DrawBorder(rect, Color.black, 1f);

            int pips = Mathf.CeilToInt(maxHealth / 2f);
            for (int i = 1; i < pips; i++)
            {
                float x = rect.x + rect.width * (i / (float)pips);
                DrawRect(new Rect(x, rect.y, 1f, rect.height), new Color(0.08f, 0.02f, 0.02f, 0.8f));
            }
        }

        private string BuildTinyStatus()
        {
            GameTimeController gameTime = FindAnyObjectByType<GameTimeController>();
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
            DrawRect(new Rect(rect.x + 17, rect.y + 8, 12, 8), new Color(0.58f, 0.58f, 0.61f, 1f));
            DrawRect(new Rect(rect.x + 15, rect.y + 15, 16, 20), new Color(0.20f, 0.23f, 0.28f, 1f));
            DrawRect(new Rect(rect.x + 18, rect.y + 17, 10, 3), new Color(0.78f, 0.78f, 0.72f, 1f));
            DrawRect(new Rect(rect.x + 12, rect.y + 20, 6, 17), new Color(0.35f, 0.08f, 0.08f, 1f));
            DrawRect(new Rect(rect.x + 29, rect.y + 20, 5, 17), new Color(0.35f, 0.08f, 0.08f, 1f));
            DrawRect(new Rect(rect.x + 23, rect.y + 10, 2, 28), new Color(0.84f, 0.68f, 0.34f, 1f));
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
            DrawRect(rect, new Color(0.045f, 0.035f, 0.025f, 0.90f));
            DrawBorder(rect, GoldDim, 1f);
            DrawRect(new Rect(rect.x + 4, rect.y + 4, 20, rect.height - 8), PanelWarm);
            GUI.Label(new Rect(rect.x + 4, rect.y + 1, 20, rect.height), key, centerStyle);
            GUI.Label(new Rect(rect.x + 28, rect.y + 2, rect.width - 30, rect.height), label, smallStyle);
        }

        private void DrawItemGlyph(Rect rect, HotbarItem item)
        {
            DrawRect(rect, new Color(0.025f, 0.022f, 0.018f, 1f));
            DrawBorder(rect, new Color(0f, 0f, 0f, 1f), 1f);

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
            GUI.Label(new Rect(rect.x + 14, rect.y + 12, rect.width - 28, 24), "Legend", titleStyle);
            DrawLegendRow(rect.x + 16, rect.y + 54, new Color(0.25f, 0.62f, 1f, 1f), "Player");
            DrawLegendRow(rect.x + 16, rect.y + 84, GetResourceMapColor("item.wood"), "Wood nodes");
            DrawLegendRow(rect.x + 16, rect.y + 114, GetResourceMapColor("item.stone"), "Stone nodes");
            DrawLegendRow(rect.x + 16, rect.y + 144, GetResourceMapColor("item.wild-berries"), "Food/herb nodes");
            DrawLegendRow(rect.x + 16, rect.y + 174, GetResourceMapColor("item.iron-ore"), "Ore nodes");
            DrawLegendRow(rect.x + 16, rect.y + 204, new Color(0.95f, 0.62f, 0.22f, 1f), "Cabin site");
            DrawLegendRow(rect.x + 16, rect.y + 234, new Color(0.68f, 0.42f, 0.92f, 1f), "Landmark");
            DrawLegendRow(rect.x + 16, rect.y + 264, new Color(1f, 0.78f, 0.24f, 1f), "Loot chest");
            DrawLegendRow(rect.x + 16, rect.y + 294, new Color(0.92f, 0.78f, 0.42f, 1f), "Villager NPC");
            GUI.Label(new Rect(rect.x + 14, rect.yMax - 58, rect.width - 28, 44), "Explore by following the road. Markers are prototype testing aids.", smallStyle);
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

        private static Vector3 GetMapCenter()
        {
            PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
            return player != null ? player.transform.position : Vector3.zero;
        }

        private static float GetMapRange() => 96f;

        private void DrawCard(Rect rect, Color color)
        {
            DrawRect(new Rect(rect.x + 4, rect.y + 5, rect.width, rect.height), Shadow);
            DrawRect(rect, color);
            DrawBorder(rect, new Color(0.01f, 0.01f, 0.01f, 0.9f), 3f);
            DrawBorder(new Rect(rect.x + 3, rect.y + 3, rect.width - 6, rect.height - 6), GoldDim, 1f);
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
            DrawRect(rect, new Color(0.18f, 0.11f, 0.05f, 0.95f));
            DrawRect(new Rect(rect.x, rect.yMax - 2f, rect.width, 2f), GoldDim);
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
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Gold }
            };

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Gold }
            };

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Parchment }
            };

            smallStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                normal = { textColor = MutedText }
            };

            centerStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                clipping = TextClipping.Clip,
                normal = { textColor = Parchment }
            };

            numberStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 10,
                normal = { textColor = MutedText }
            };

            promptStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Parchment }
            };

            captionStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
                fontStyle = FontStyle.Bold,
                normal = { textColor = MutedText }
            };
        }

        private struct HotbarItem
        {
            public static readonly HotbarItem Empty = new HotbarItem(string.Empty, string.Empty, 0, Color.clear);

            public HotbarItem(string name, string icon, int count, Color color)
            {
                Name = name;
                Icon = icon;
                Count = count;
                Color = color;
            }

            public string Name { get; }
            public string Icon { get; }
            public int Count { get; }
            public Color Color { get; }
            public bool IsEmpty => string.IsNullOrEmpty(Name);
        }

        private enum OverlayMode
        {
            None,
            Inventory,
            BuildCatalog,
            Map,
            Journal
        }

        private void ToggleOverlay(OverlayMode mode)
        {
            overlayMode = overlayMode == mode ? OverlayMode.None : mode;
        }
    }
}
