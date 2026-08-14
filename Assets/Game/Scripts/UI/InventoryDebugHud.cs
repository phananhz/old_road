using UnityEngine;
using TheOldRoad.Building;
using TheOldRoad.Construction;
using TheOldRoad.Core;
using TheOldRoad.Crafting;
using TheOldRoad.Gathering;
using TheOldRoad.Input;
using TheOldRoad.Inventory;
using TheOldRoad.Player;
using TheOldRoad.Time;

namespace TheOldRoad.UI
{
    /// <summary>Polished prototype HUD: medieval panels, health, minimap, hotbar, overlays, and prompts.</summary>
    public sealed class InventoryDebugHud : MonoBehaviour
    {
        private static readonly Vector2 WorldMin = new Vector2(-30f, -18f);
        private static readonly Vector2 WorldMax = new Vector2(30f, 18f);
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
            if (PrototypeInput.GetKeyDown(KeyCode.Escape)) overlayMode = OverlayMode.None;
        }

        private void OnGUI()
        {
            EnsureStyles();

            DrawScreenVignette();
            DrawStatusCard();
            DrawMinimapCard();
            DrawPromptRibbon();
            DrawHotbar();
            DrawOverlay();
        }

        private void DrawStatusCard()
        {
            Rect card = new Rect(18, 18, 344, 118);
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

            float chipY = card.y + 91;
            DrawResourceChip(new Rect(card.x + 16, chipY, 96, 18), "Wood", GetQuantity("item.wood"), new Color(0.48f, 0.28f, 0.12f, 1f));
            DrawResourceChip(new Rect(card.x + 122, chipY, 96, 18), "Stone", GetQuantity("item.stone"), new Color(0.48f, 0.50f, 0.54f, 1f));
            DrawResourceChip(new Rect(card.x + 228, chipY, 96, 18), "Plank", GetQuantity("item.cabin-plank"), new Color(0.72f, 0.48f, 0.24f, 1f));
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
            DrawControlPill(new Rect(card.x + 112, map.yMax + 12, 82, 22), "I", "Bag");
        }

        private void DrawPromptRibbon()
        {
            if (overlayMode != OverlayMode.None) return;

            string prompt = BuildPromptText();
            if (string.IsNullOrWhiteSpace(prompt)) return;

            float width = Mathf.Min(820f, Screen.width - 60f);
            Rect ribbon = new Rect((Screen.width - width) * 0.5f, Screen.height - 144f, width, 38f);
            DrawRect(new Rect(ribbon.x + 4, ribbon.y + 5, ribbon.width, ribbon.height), Shadow);
            DrawRect(ribbon, new Color(0.055f, 0.038f, 0.025f, 0.88f));
            DrawBorder(ribbon, GoldDim, 2f);
            DrawBorder(new Rect(ribbon.x + 5, ribbon.y + 5, ribbon.width - 10, ribbon.height - 10), new Color(0.22f, 0.15f, 0.08f, 1f), 1f);
            GUI.Label(ribbon, prompt, promptStyle);
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

            GUI.Label(new Rect(startX, y - 33f, totalWidth, 20f), "E Gather    C Craft    B Build    I Inventory    M Map", centerStyle);
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

        private void DrawOverlay()
        {
            if (overlayMode == OverlayMode.None) return;

            DrawRect(new Rect(0, 0, Screen.width, Screen.height), new Color(0f, 0f, 0f, 0.58f));

            if (overlayMode == OverlayMode.Inventory) DrawInventoryOverlay();
            if (overlayMode == OverlayMode.Map) DrawMapOverlay();
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

            string[] itemIds = { "item.wood", "item.stone", "item.cabin-plank" };
            string[] itemNames = { "Wood", "Stone", "Cabin Plank" };
            Color[] colors =
            {
                new Color(0.47f, 0.29f, 0.12f, 1f),
                new Color(0.45f, 0.48f, 0.52f, 1f),
                new Color(0.74f, 0.50f, 0.25f, 1f)
            };

            const float slotWidth = 176f;
            const float slotHeight = 96f;
            const float gap = 14f;
            int columns = Mathf.Max(1, Mathf.FloorToInt((rect.width - 24f) / (slotWidth + gap)));

            for (int i = 0; i < itemIds.Length; i++)
            {
                int column = i % columns;
                int row = i / columns;
                Rect slot = new Rect(rect.x + 14 + column * (slotWidth + gap), rect.y + 14 + row * (slotHeight + gap), slotWidth, slotHeight);
                DrawRect(slot, InkSoft);
                DrawBorder(slot, GoldDim, 1f);
                Rect icon = new Rect(slot.x + 15, slot.y + 18, 44, 40);
                DrawItemGlyph(icon, new HotbarItem(itemNames[i], itemNames[i].Substring(0, 1), GetQuantity(itemIds[i]), colors[i]));
                GUI.Label(new Rect(slot.x + 72, slot.y + 16, slot.width - 86, 24), itemNames[i], labelStyle);
                GUI.Label(new Rect(slot.x + 72, slot.y + 42, slot.width - 86, 26), "x" + GetQuantity(itemIds[i]), titleStyle);
                GUI.Label(new Rect(slot.x + 15, slot.y + 68, slot.width - 30, 18), GetItemUseText(itemIds[i]), smallStyle);
            }
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

            foreach (ResourceNode node in FindObjectsByType<ResourceNode>(FindObjectsInactive.Exclude))
            {
                if (node == null || node.IsHarvested) continue;
                Color color = node.ResourceItemId == "item.wood"
                    ? new Color(0.34f, 0.90f, 0.30f, 1f)
                    : new Color(0.68f, 0.70f, 0.73f, 1f);
                DrawMapDot(map, node.transform.position, color, map.width > 220f ? 9f : 5f);
            }

            foreach (ConstructionSite site in FindObjectsByType<ConstructionSite>(FindObjectsInactive.Exclude))
            {
                if (site == null) continue;
                DrawMapDot(map, site.transform.position, new Color(0.95f, 0.62f, 0.22f, 1f), map.width > 220f ? 10f : 6f);
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
                float worldX = Mathf.Lerp(WorldMin.x, WorldMax.x, t);
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
                float worldX = Mathf.Lerp(WorldMin.x, -5f, t);
                float riverY = -7.5f - Mathf.Sin(worldX * 0.22f) * 1.2f;
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

            if (placementController != null && !string.IsNullOrWhiteSpace(placementController.LastStatus)) AppendPrompt(ref prompt, placementController.LastStatus);

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
                case 3: return new HotbarItem("Cabin", "B", 1, new Color(0.65f, 0.38f, 0.18f, 1f));
                default: return HotbarItem.Empty;
            }
        }

        private int GetQuantity(string itemId)
        {
            if (inventorySession == null || inventorySession.Runtime == null) return 0;
            return inventorySession.Runtime.GetQuantity(itemId);
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
            switch (itemId)
            {
                case "item.wood": return "Build material";
                case "item.stone": return "Foundation material";
                case "item.cabin-plank": return "Crafted component";
                default: return "Prototype item";
            }
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

            DrawRect(new Rect(rect.x + 7, rect.y + 7, rect.width - 14, rect.height - 14), item.Color);
            GUI.Label(rect, item.Icon, centerStyle);
        }

        private void DrawMapLegend(Rect rect)
        {
            DrawRect(rect, new Color(0.03f, 0.026f, 0.022f, 0.84f));
            DrawBorder(rect, GoldDim, 1f);
            GUI.Label(new Rect(rect.x + 14, rect.y + 12, rect.width - 28, 24), "Legend", titleStyle);
            DrawLegendRow(rect.x + 16, rect.y + 54, new Color(0.25f, 0.62f, 1f, 1f), "Player");
            DrawLegendRow(rect.x + 16, rect.y + 84, new Color(0.34f, 0.90f, 0.30f, 1f), "Wood nodes");
            DrawLegendRow(rect.x + 16, rect.y + 114, new Color(0.68f, 0.70f, 0.73f, 1f), "Stone nodes");
            DrawLegendRow(rect.x + 16, rect.y + 144, new Color(0.95f, 0.62f, 0.22f, 1f), "Cabin site");
            DrawLegendRow(rect.x + 16, rect.y + 174, new Color(0.68f, 0.42f, 0.92f, 1f), "Landmark");
            GUI.Label(new Rect(rect.x + 14, rect.yMax - 78, rect.width - 28, 60), "Explore by following the road. The map uses prototype markers for testing.", smallStyle);
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

        private static Vector2 WorldToMap(Rect map, Vector3 worldPosition)
        {
            Vector2 normalized = new Vector2(
                Mathf.InverseLerp(WorldMin.x, WorldMax.x, worldPosition.x),
                Mathf.InverseLerp(WorldMin.y, WorldMax.y, worldPosition.y));

            return new Vector2(
                map.x + Mathf.Clamp01(normalized.x) * map.width,
                map.y + (1f - Mathf.Clamp01(normalized.y)) * map.height);
        }

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
            Map
        }

        private void ToggleOverlay(OverlayMode mode)
        {
            overlayMode = overlayMode == mode ? OverlayMode.None : mode;
        }
    }
}
