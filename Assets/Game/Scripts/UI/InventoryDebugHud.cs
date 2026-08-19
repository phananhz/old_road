using System;
using System.Collections.Generic;
using UnityEngine;
using TheOldRoad.Building;
using TheOldRoad.Construction;
using TheOldRoad.Core;
using TheOldRoad.Crafting;
using TheOldRoad.Farming;
using TheOldRoad.Fishing;
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
        private GUIStyle subtitleStyle;
        private GUIStyle labelStyle;
        private GUIStyle smallStyle;
        private GUIStyle centerStyle;
        private GUIStyle numberStyle;
        private GUIStyle promptStyle;
        private GUIStyle captionStyle;
        private GUIStyle buttonStyle;
        private GUIStyle categoryNormalStyle;
        private GUIStyle categorySelectedStyle;
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
        private TheOldRoad.Building.BuildingInteractionController cachedBuildingInteractor;
        private PlayerFarmingInteractor cachedFarming;
        private PlayerFishingInteractor cachedFishing;
        private PlayerNpcInteractor cachedNpcInteractor;
        private DiscoverableLandmark[] cachedLandmarks = System.Array.Empty<DiscoverableLandmark>();
        private LootChest[] cachedLootChests = System.Array.Empty<LootChest>();
        private ResourceNode[] cachedResourceNodes = System.Array.Empty<ResourceNode>();
        private ConstructionSite[] cachedConstructionSites = System.Array.Empty<ConstructionSite>();
        private VillagerNpcController[] cachedNpcs = System.Array.Empty<VillagerNpcController>();
        private AnimalNpcController[] cachedAnimalNpcs = System.Array.Empty<AnimalNpcController>();
        private AnimalPenController[] cachedAnimalPens = System.Array.Empty<AnimalPenController>();
        private DailyBulletinBoardController cachedBulletinBoard;
        private DailyMailboxController cachedMailbox;

        private string[] hotbarSlotItemIds = new string[9]
        {
            "item.tool-hoe",
            "item.watering-can",
            "item.tool-axe",
            "item.tool-pickaxe",
            "item.seed-carrot",
            "item.seed-wheat",
            "item.weapon-sword",
            "item.fishing-rod",
            "item.wood"
        };

        private string lastClickedInventoryItemId = string.Empty;
        private float lastInventoryItemClickTime = -1f;
        private int lastClickedHotbarIndex = -1;
        private float lastHotbarClickTime = -1f;

        public void Configure(
            InventorySession inventorySession,
            BuildingPlacementController placementController = null,
            VerticalSliceController sliceController = null)
        {
            this.inventorySession = inventorySession;
            this.placementController = placementController;
            this.sliceController = sliceController;
        }

        public int SelectedSlot => selectedSlot;
        public string SelectedItemId => GetHotbarItem(selectedSlot).ItemId;
        public bool IsAnyOverlayOpen => overlayMode != OverlayMode.None || GameStartMenuController.IsOpen;

        public bool IsItemOnHotbar(string itemId, out int slotIndex)
        {
            slotIndex = -1;
            if (string.IsNullOrEmpty(itemId)) return false;
            for (int i = 0; i < hotbarSlotItemIds.Length; i++)
            {
                if (string.Equals(hotbarSlotItemIds[i], itemId, StringComparison.OrdinalIgnoreCase))
                {
                    slotIndex = i;
                    return true;
                }
            }
            return false;
        }

        public bool AssignItemToHotbar(string itemId, int targetSlot = -1)
        {
            if (string.IsNullOrEmpty(itemId)) return false;

            if (targetSlot >= 0 && targetSlot < 9)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (i != targetSlot && string.Equals(hotbarSlotItemIds[i], itemId, StringComparison.OrdinalIgnoreCase))
                    {
                        hotbarSlotItemIds[i] = string.Empty;
                    }
                }
                hotbarSlotItemIds[targetSlot] = itemId;
                selectedSlot = targetSlot;
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                string name = LocalizationRuntime.ItemName(itemId);
                ShowMessage(LocalizationRuntime.IsVietnamese 
                    ? $"✨ Đã gán {name} vào ô {targetSlot + 1} trên thanh công cụ!" 
                    : $"✨ Assigned {name} to Hotbar slot {targetSlot + 1}!");
                return true;
            }

            for (int i = 0; i < 9; i++)
            {
                if (string.IsNullOrEmpty(hotbarSlotItemIds[i]))
                {
                    hotbarSlotItemIds[i] = itemId;
                    selectedSlot = i;
                    TheOldRoad.Audio.AudioManager.PlayUiClick();
                    string name = LocalizationRuntime.ItemName(itemId);
                    ShowMessage(LocalizationRuntime.IsVietnamese 
                        ? $"✨ Đã thêm {name} vào ô {i + 1} trên thanh công cụ!" 
                        : $"✨ Added {name} to Hotbar slot {i + 1}!");
                    return true;
                }
            }

            int replaceSlot = Mathf.Clamp(selectedSlot, 0, 8);
            hotbarSlotItemIds[replaceSlot] = itemId;
            TheOldRoad.Audio.AudioManager.PlayUiClick();
            string itemName = LocalizationRuntime.ItemName(itemId);
            ShowMessage(LocalizationRuntime.IsVietnamese 
                ? $"✨ Đã gán {itemName} vào ô {replaceSlot + 1} trên thanh công cụ!" 
                : $"✨ Assigned {itemName} to Hotbar slot {replaceSlot + 1}!");
            return true;
        }

        public bool RemoveItemFromHotbar(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return false;
            bool removed = false;
            for (int i = 0; i < 9; i++)
            {
                if (string.Equals(hotbarSlotItemIds[i], itemId, StringComparison.OrdinalIgnoreCase))
                {
                    hotbarSlotItemIds[i] = string.Empty;
                    removed = true;
                }
            }

            if (removed)
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                string name = LocalizationRuntime.ItemName(itemId);
                ShowMessage(LocalizationRuntime.IsVietnamese 
                    ? $"📦 Đã cất {name} khỏi thanh công cụ!" 
                    : $"📦 Stored {name} from Hotbar!");
            }
            return removed;
        }

        public void ClearHotbarSlot(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < 9)
            {
                string oldItem = hotbarSlotItemIds[slotIndex];
                hotbarSlotItemIds[slotIndex] = string.Empty;
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                if (!string.IsNullOrEmpty(oldItem))
                {
                    string name = LocalizationRuntime.ItemName(oldItem);
                    ShowMessage(LocalizationRuntime.IsVietnamese 
                        ? $"📦 Đã cất {name} khỏi ô {slotIndex + 1}!" 
                        : $"📦 Cleared {name} from slot {slotIndex + 1}!");
                }
            }
        }

        public void ShowMessage(string message)
        {
            activePromptText = message;
            promptHideTime = UnityEngine.Time.unscaledTime + PromptVisibleSeconds;
        }

        public string[] GetHotbarSaveEntries()
        {
            string[] entries = new string[9];
            for (int i = 0; i < 9; i++)
            {
                entries[i] = hotbarSlotItemIds != null && i < hotbarSlotItemIds.Length ? (hotbarSlotItemIds[i] ?? string.Empty) : string.Empty;
            }
            return entries;
        }

        public void LoadHotbarEntries(string[] entries)
        {
            if (entries == null || entries.Length == 0) return;
            if (hotbarSlotItemIds == null || hotbarSlotItemIds.Length != 9) hotbarSlotItemIds = new string[9];

            for (int i = 0; i < 9; i++)
            {
                hotbarSlotItemIds[i] = i < entries.Length ? (entries[i] ?? string.Empty) : string.Empty;
            }
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
            if (PrototypeInput.GetKeyDown(KeyCode.H)) ToggleOverlay(OverlayMode.Guide);
            if (PrototypeInput.GetKeyDown(KeyCode.B)) HandleBuildInput();
            if (PrototypeInput.GetKeyDown(KeyCode.Q)) TryConsumeSelectedItem();
            if (PrototypeInput.GetKeyDown(KeyCode.Escape))
            {
                overlayMode = OverlayMode.None;
                if (cachedBulletinBoard != null) cachedBulletinBoard.CloseBoard();
                if (cachedMailbox != null) cachedMailbox.CloseMail();
            }

            if (cachedBulletinBoard != null && cachedBulletinBoard.IsBoardOpen && overlayMode != OverlayMode.BulletinBoard)
            {
                overlayMode = OverlayMode.BulletinBoard;
            }
            else if (cachedBulletinBoard != null && !cachedBulletinBoard.IsBoardOpen && overlayMode == OverlayMode.BulletinBoard)
            {
                overlayMode = OverlayMode.None;
            }

            if (cachedMailbox != null && cachedMailbox.IsMailOpen && overlayMode != OverlayMode.Mailbox)
            {
                overlayMode = OverlayMode.Mailbox;
            }
            else if (cachedMailbox != null && !cachedMailbox.IsMailOpen && overlayMode == OverlayMode.Mailbox)
            {
                overlayMode = OverlayMode.None;
            }
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
            DrawNewbieHintBanner();
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
            Rect card = new Rect(Screen.width - width - 14f, 14f, width, 246f);
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
            DrawControlPill(new Rect(card.x + 10f, map.yMax + 30f, 160f, 22f), "H", LocalizationRuntime.T("guide_short"));
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

            Rect card = new Rect(Screen.width - width - 14f, 268f, width, 28f);
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

            Rect card = new Rect(Screen.width - width - 14f, 302f, width, 28f);
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
            if (current != null && current.type == EventType.MouseDown && slot.Contains(current.mousePosition))
            {
                if (current.button == 1) // Right click -> Store/Clear
                {
                    ClearHotbarSlot(index);
                    current.Use();
                    return;
                }

                if (current.button == 0) // Left click
                {
                    float now = UnityEngine.Time.unscaledTime;
                    bool isDoubleClick = (now - lastHotbarClickTime <= 0.38f) && (lastClickedHotbarIndex == index);
                    lastClickedHotbarIndex = index;
                    lastHotbarClickTime = now;

                    if (isDoubleClick)
                    {
                        ClearHotbarSlot(index);
                        lastHotbarClickTime = -1f;
                    }
                    else if (overlayMode == OverlayMode.Inventory && !string.IsNullOrEmpty(selectedInventoryItemId))
                    {
                        AssignItemToHotbar(selectedInventoryItemId, index);
                    }
                    else
                    {
                        selectedSlot = index;
                        TheOldRoad.Audio.AudioManager.PlayUiClick();
                    }

                    current.Use();
                    return;
                }
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

            // Contextual actions (Eat / Cabin use / Farm / Cook)
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
                else
                {
                    PlayerFarmingInteractor farming = cachedFarming;
                    if (farming != null && farming.CanFarmAction)
                    {
                        DrawMobileActionButton(new Rect(right - 142f, bottom - 104f, 68f, 46f), "F", farming.ActionButtonLabel, KeyCode.F, new Color(0.24f, 0.42f, 0.20f, 0.96f));
                    }
                    else
                    {
                        PlayerFishingInteractor fishing = cachedFishing;
                        if (fishing != null && fishing.CanFishAction)
                        {
                            DrawMobileActionButton(new Rect(right - 142f, bottom - 104f, 68f, 46f), "F", fishing.ActionButtonLabel, KeyCode.F, new Color(0.18f, 0.38f, 0.58f, 0.96f));
                        }
                    }
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
            if (overlayMode == OverlayMode.Guide) DrawGuideOverlay();
            if (overlayMode == OverlayMode.BulletinBoard) DrawBulletinBoardOverlay();
            if (overlayMode == OverlayMode.Mailbox) DrawMailboxOverlay();
            if (overlayMode == OverlayMode.SiloStorage) DrawSiloOverlay();
            if (overlayMode == OverlayMode.ChestStorage) DrawChestOverlay();
            if (overlayMode == OverlayMode.ArtisanMachine) DrawArtisanOverlay();
            if (overlayMode == OverlayMode.MarketStall) DrawMarketStallOverlay();
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

            List<PrototypeItemInfo> owned = new List<PrototypeItemInfo>();
            PrototypeItemInfo[] all = PrototypeItemCatalog.All;
            for (int i = 0; i < all.Length; i++)
            {
                if (GetQuantity(all[i].ItemId) > 0)
                {
                    owned.Add(all[i]);
                }
            }

            if (owned.Count == 0)
            {
                GUIStyle emptyStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 13,
                    fontStyle = FontStyle.Italic,
                    alignment = TextAnchor.MiddleCenter
                };
                emptyStyle.normal.textColor = new Color(0.70f, 0.65f, 0.58f, 1f);
                GUI.Label(new Rect(rect.x + 20f, rect.y + rect.height * 0.40f, rect.width - 40f, 40f),
                    LocalizationRuntime.IsVietnamese
                        ? "🎒 Túi hành lý hiện đang trống.\nHãy đi chặt gỗ, đào đá hoặc thu hoạch nông sản!"
                        : "🎒 Backpack is currently empty.\nGather wood, stone or harvest farm crops!",
                    emptyStyle);
                return;
            }

            // Keep selected item valid
            if (string.IsNullOrEmpty(selectedInventoryItemId) || GetQuantity(selectedInventoryItemId) <= 0)
            {
                selectedInventoryItemId = owned[0].ItemId;
            }

            const float slotSize = 68f;
            const float gap = 8f;
            int columns = Mathf.Max(1, Mathf.FloorToInt((rect.width - 20f + gap) / (slotSize + gap)));

            for (int i = 0; i < owned.Count; i++)
            {
                PrototypeItemInfo item = owned[i];
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
                float now = UnityEngine.Time.unscaledTime;
                bool isDoubleClick = (now - lastInventoryItemClickTime <= 0.38f) && string.Equals(lastClickedInventoryItemId, item.ItemId, StringComparison.Ordinal);

                selectedInventoryItemId = item.ItemId;
                lastClickedInventoryItemId = item.ItemId;
                lastInventoryItemClickTime = now;

                if (isDoubleClick)
                {
                    if (IsItemOnHotbar(item.ItemId, out _))
                    {
                        RemoveItemFromHotbar(item.ItemId);
                    }
                    else
                    {
                        AssignItemToHotbar(item.ItemId);
                    }
                    lastInventoryItemClickTime = -1f;
                }
                else
                {
                    TheOldRoad.Audio.AudioManager.PlayUiClick();
                }

                current.Use();
            }

            bool isSelected = string.Equals(selectedInventoryItemId, item.ItemId, StringComparison.Ordinal);
            int quantity = GetQuantity(item.ItemId);
            bool hasItem = quantity > 0;
            bool onHotbar = IsItemOnHotbar(item.ItemId, out int hotbarIndex);

            // Highlight effect for selected item
            if (isSelected)
            {
                float pulse = 0.6f + Mathf.PingPong(UnityEngine.Time.unscaledTime * 3f, 0.4f);
                DrawRect(new Rect(slot.x - 3f, slot.y - 3f, slot.width + 6f, slot.height + 6f), new Color(1f, 0.85f, 0.25f, pulse * 0.45f));
                DrawRect(slot, new Color(0.40f, 0.26f, 0.09f, 0.98f));
                DrawBorder(slot, Gold, 2.5f);
                DrawRect(new Rect(slot.xMax - 6f, slot.y + 2f, 4f, 4f), Gold);
            }
            else
            {
                DrawRect(slot, hasItem ? InkSoft : new Color(0.045f, 0.041f, 0.037f, 0.88f));
                DrawBorder(slot, hasItem ? GoldDim : new Color(0.20f, 0.18f, 0.15f, 1f), 1f);
            }

            // Top-left badge if currently equipped on Hotbar
            if (onHotbar)
            {
                Rect hotbarBadge = new Rect(slot.x + 3f, slot.y + 3f, 16f, 14f);
                DrawRect(hotbarBadge, new Color(0.35f, 0.22f, 0.08f, 0.95f));
                DrawBorder(hotbarBadge, Gold, 1f);
                GUI.Label(hotbarBadge, (hotbarIndex + 1).ToString(), numberStyle);
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
            bool onHotbar = IsItemOnHotbar(item.ItemId, out int slotIdx);

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
            GUI.Label(new Rect(rect.x + 14f, rect.y + 182f, rect.width - 28f, 130f), LocalizationRuntime.ItemDescription(item.ItemId), subtitleStyle);

            // Bottom Action & Hotbar Assignment Area
            float actionH = 135f;
            Rect actionBox = new Rect(rect.x + 10f, rect.yMax - actionH - 8f, rect.width - 20f, actionH);
            DrawRect(actionBox, new Color(0.10f, 0.07f, 0.05f, 0.94f));
            DrawBorder(actionBox, new Color(0.38f, 0.28f, 0.14f, 1f), 1f);

            float btnY = actionBox.y + 6f;

            // 1. Food consume button (if applicable)
            if (IsFoodItem(item.ItemId) && hasItem)
            {
                if (GUI.Button(new Rect(actionBox.x + 8f, btnY, actionBox.width - 16f, 26f), "♥  " + (LocalizationRuntime.IsVietnamese ? "Ăn Hồi Máu (Q)" : "Eat (Q)"), buttonStyle))
                {
                    TryConsumeSelectedItem();
                }
                btnY += 30f;
            }
            else if (item.ItemId == "item.farm-deed" && hasItem)
            {
                if (GUI.Button(new Rect(actionBox.x + 8f, btnY, actionBox.width - 16f, 26f), "📜  " + (LocalizationRuntime.IsVietnamese ? "Khai Hoang Mở 12 Ô Đất" : "Expand 12 Farm Plots"), buttonStyle))
                {
                    if (sliceController != null)
                    {
                        sliceController.EnsureFarmExpansion();
                        inventorySession?.Runtime?.TryRemove("item.farm-deed", 1);
                        TheOldRoad.Audio.AudioManager.PlayQuestComplete();
                        PlayerSpeechBubble.Say(LocalizationRuntime.IsVietnamese ? "Đã mở rộng thêm 12 ô đất nông trại Grid B!" : "Expanded 12 new farm plots!");
                    }
                }
                btnY += 30f;
            }

            // 2. Add / Remove Hotbar Toggle Button
            string hotbarBtnText = onHotbar
                ? (LocalizationRuntime.IsVietnamese ? $"📦 Cất khỏi Hotbar (Ô {slotIdx + 1})" : $"📦 Store from Hotbar (Slot {slotIdx + 1})")
                : (LocalizationRuntime.IsVietnamese ? "➕ Thêm vào Hotbar" : "➕ Add to Hotbar");

            if (GUI.Button(new Rect(actionBox.x + 8f, btnY, actionBox.width - 16f, 28f), hotbarBtnText, buttonStyle))
            {
                if (onHotbar) RemoveItemFromHotbar(item.ItemId);
                else AssignItemToHotbar(item.ItemId);
            }
            btnY += 32f;

            // 3. Quick-Assign Slot Buttons [1]..[9]
            GUI.Label(new Rect(actionBox.x + 8f, btnY, actionBox.width - 16f, 16f), LocalizationRuntime.IsVietnamese ? "Gán vào ô số [1-9]:" : "Assign to Slot [1-9]:", smallStyle);
            btnY += 18f;

            float slotBtnW = (actionBox.width - 16f - 8 * 2f) / 9f;
            for (int k = 0; k < 9; k++)
            {
                Rect slotBtnRect = new Rect(actionBox.x + 8f + k * (slotBtnW + 2f), btnY, slotBtnW, 22f);
                bool isCurrentSlot = onHotbar && slotIdx == k;
                Color prevBtnColor = GUI.backgroundColor;
                if (isCurrentSlot) GUI.backgroundColor = new Color(0.95f, 0.70f, 0.20f, 1f);

                if (GUI.Button(slotBtnRect, (k + 1).ToString()))
                {
                    AssignItemToHotbar(item.ItemId, k);
                }
                GUI.backgroundColor = prevBtnColor;
            }
            btnY += 24f;

            // 4. Usage Tip
            GUI.Label(new Rect(actionBox.x + 4f, btnY, actionBox.width - 8f, 16f), LocalizationRuntime.IsVietnamese ? "💡 Nhấp đúp để Thêm / Cất nhanh" : "💡 Double-click item to Add/Store", smallStyle);
        }

        private void DrawBuildCatalogOverlay()
        {
            float panelWidth = Mathf.Min(Screen.width - 30f, 1140f);
            float panelHeight = Mathf.Min(Screen.height - 40f, 700f);
            Rect panel = new Rect((Screen.width - panelWidth) * 0.5f, (Screen.height - panelHeight) * 0.5f, panelWidth, panelHeight);
            DrawCard(panel, Ink);
            DrawCornerAccents(panel, Gold);
            DrawHeaderStrip(new Rect(panel.x, panel.y, panel.width, 50f));

            GUI.Label(new Rect(panel.x + 24f, panel.y + 10f, panel.width - 48f, 28f), LocalizationRuntime.T("construction_catalog"), gameTitleStyle);
            GUI.Label(new Rect(panel.x + panel.width - 260f, panel.y + 16f, 230f, 20f), LocalizationRuntime.T("build_close"), smallStyle);

            Rect sidebar = new Rect(panel.x + 16f, panel.y + 60f, 220f, panel.height - 76f);
            Rect content = new Rect(sidebar.xMax + 14f, sidebar.y, panel.xMax - sidebar.xMax - 30f, sidebar.height);

            DrawBuildCategorySidebar(sidebar);
            DrawBuildCatalogContent(content);
        }

        private Vector2 buildCategoryScrollPosition;

        private void DrawBuildCategorySidebar(Rect rect)
        {
            DrawRect(rect, new Color(0.025f, 0.023f, 0.02f, 0.76f));
            DrawBorder(rect, GoldDim, 1f);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 8f, rect.width - 24f, 20f), LocalizationRuntime.T("categories"), titleStyle);

            // Demolish / Recycle button
            Rect demolishRect = new Rect(rect.x + 6f, rect.y + 30f, rect.width - 12f, 36f);
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

            GUI.Label(new Rect(demolishRect.x + 6f, demolishRect.y + 2f, demolishRect.width - 12f, 16f), LocalizationRuntime.T("demolish_btn"), labelStyle);
            GUI.Label(new Rect(demolishRect.x + 6f, demolishRect.y + 18f, demolishRect.width - 12f, 14f), LocalizationRuntime.T("demolish_btn_desc"), captionStyle);

            Rect listOuter = new Rect(rect.x + 4f, rect.y + 72f, rect.width - 8f, rect.height - 76f);
            float itemH = 34f;
            float gap = 3f;

            string[] catKeys = {
                "housing", "fire_light", "animal_pens", "fences_security", "paths_decor",
                "furniture_living", "artisan_processing", "storage_logistics", "gardening_greenery", "water_irrigation",
                "monuments_shrines", "market_commerce", "defenses_traps", "leisure_camping", "festivals_ornaments"
            };

            float totalH = catKeys.Length * (itemH + gap) + 6f;
            buildCategoryScrollPosition = GUI.BeginScrollView(listOuter, buildCategoryScrollPosition, new Rect(0, 0, listOuter.width - 16f, totalH));

            for (int i = 0; i < catKeys.Length; i++)
            {
                Rect bRect = new Rect(2f, i * (itemH + gap), listOuter.width - 20f, itemH);
                DrawBuildCategoryButton(bRect, i, LocalizationRuntime.T(catKeys[i]));
            }

            GUI.EndScrollView();
        }

        private void DrawBuildCategoryButton(Rect rect, int categoryIndex, string title)
        {
            bool selected = selectedBuildCategory == categoryIndex;
            DrawRect(rect, selected ? new Color(0.10f, 0.08f, 0.07f, 0.90f) : InkSoft);
            DrawBorder(rect, selected ? Gold : new Color(0.20f, 0.16f, 0.12f, 0.8f), selected ? 2f : 1f);

            Event current = Event.current;
            if (current != null && current.type == EventType.MouseDown && current.button == 0 && rect.Contains(current.mousePosition))
            {
                selectedBuildCategory = categoryIndex;
                buildCatalogScrollPosition = Vector2.zero;
                current.Use();
            }

            GUIStyle style = selected ? (categorySelectedStyle ?? smallStyle) : (categoryNormalStyle ?? smallStyle);
            GUI.Label(new Rect(rect.x + 8f, rect.y, rect.width - 16f, rect.height), title, style);
        }

        private void DrawBuildCatalogContent(Rect rect)
        {
            DrawRect(rect, new Color(0.025f, 0.023f, 0.02f, 0.64f));
            DrawBorder(rect, new Color(0.18f, 0.15f, 0.11f, 1f), 2f);

            string[] catKeys = {
                "housing", "fire_light", "animal_pens", "fences_security", "paths_decor",
                "furniture_living", "artisan_processing", "storage_logistics", "gardening_greenery", "water_irrigation",
                "monuments_shrines", "market_commerce", "defenses_traps", "leisure_camping", "festivals_ornaments"
            };
            string heading = selectedBuildCategory >= 0 && selectedBuildCategory < catKeys.Length
                ? LocalizationRuntime.T(catKeys[selectedBuildCategory])
                : LocalizationRuntime.T("housing");

            GUI.Label(new Rect(rect.x + 18f, rect.y + 10f, rect.width - 36f, 24f), heading, titleStyle);
            if (!string.IsNullOrWhiteSpace(buildCatalogMessage) && UnityEngine.Time.unscaledTime <= buildCatalogMessageHideTime)
            {
                Rect message = new Rect(rect.x + 150f, rect.y + 8f, rect.width - 168f, 26f);
                DrawRect(message, new Color(0.20f, 0.055f, 0.035f, 0.88f));
                DrawBorder(message, new Color(0.84f, 0.28f, 0.18f, 1f), 1f);
                GUI.Label(message, buildCatalogMessage, centerStyle);
            }

            Rect scrollOuter = new Rect(rect.x + 8f, rect.y + 40f, rect.width - 16f, rect.height - 48f);
            const float cardWidth = 244f;
            const float cardHeight = 224f;
            const float gap = 12f;
            int columns = Mathf.Max(1, Mathf.FloorToInt((scrollOuter.width - 20f + gap) / (cardWidth + gap)));

            int totalCards = selectedBuildCategory switch
            {
                0 => 10,
                1 => 6,
                2 => 6,
                3 => 10,
                4 => 6,
                5 => 5,
                6 => 6,
                7 => 4,
                8 => 4,
                9 => 4,
                10 => 3,
                11 => 3,
                12 => 3,
                13 => 4,
                14 => 3,
                _ => 6
            };

            int totalRows = Mathf.CeilToInt((float)totalCards / columns);
            float viewHeight = Mathf.Max(scrollOuter.height, totalRows * (cardHeight + gap) + 12f);

            buildCatalogScrollPosition = GUI.BeginScrollView(scrollOuter, buildCatalogScrollPosition, new Rect(0, 0, scrollOuter.width - 20f, viewHeight));

            if (selectedBuildCategory == 0) // Housing & Lodges
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_cabin"), LocalizationRuntime.T("building_cabin_desc"), GetBuildingDefinition("building.cabin"), "Cabin", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_cottage"), LocalizationRuntime.T("building_cottage_desc"), GetBuildingDefinition("building.stone-cottage"), "Cottage", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_barn"), LocalizationRuntime.T("building_barn_desc"), GetBuildingDefinition("building.farm-barn"), "Barn", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 3), LocalizationRuntime.T("building_shed"), LocalizationRuntime.T("building_shed_desc"), GetBuildingDefinition("building.storage-shed"), "Shed", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 4), LocalizationRuntime.T("building_herbalist"), LocalizationRuntime.T("building_herbalist_desc"), GetBuildingDefinition("building.herbalist-hut"), "Herbalist", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 5), LocalizationRuntime.T("building_tower"), LocalizationRuntime.T("building_tower_desc"), GetBuildingDefinition("building.lookout-tower"), "Lookout", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 6), LocalizationRuntime.T("building_tent"), LocalizationRuntime.T("building_tent_desc"), GetBuildingDefinition("building.tent"), "Cabin", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 7), LocalizationRuntime.T("building_manor"), LocalizationRuntime.T("building_manor_desc"), GetBuildingDefinition("building.manor"), "Cottage", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 8), LocalizationRuntime.T("building_greenhouse"), LocalizationRuntime.T("building_greenhouse_desc"), GetBuildingDefinition("building.greenhouse"), "Herbalist", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 9), LocalizationRuntime.T("building_silo"), LocalizationRuntime.T("building_silo_desc"), GetBuildingDefinition("building.silo"), "Barn", true);
            }
            else if (selectedBuildCategory == 1) // Fire & Lighting
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_campfire"), LocalizationRuntime.T("building_campfire_desc"), GetBuildingDefinition("building.campfire"), "Campfire", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_hearth"), LocalizationRuntime.T("building_hearth_desc"), GetBuildingDefinition("building.cooking-hearth"), "Hearth", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_street_lamp"), LocalizationRuntime.T("building_street_lamp_desc"), GetBuildingDefinition("building.street-lamp"), "Campfire", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 3), LocalizationRuntime.T("building_ground_torch"), LocalizationRuntime.T("building_ground_torch_desc"), GetBuildingDefinition("building.ground-torch"), "Campfire", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 4), LocalizationRuntime.T("building_lantern_pole"), LocalizationRuntime.T("building_lantern_pole_desc"), GetBuildingDefinition("building.lantern-pole"), "Campfire", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 5), LocalizationRuntime.T("building_stone_fireplace"), LocalizationRuntime.T("building_stone_fireplace_desc"), GetBuildingDefinition("building.stone-fireplace"), "Hearth", true);
            }
            else if (selectedBuildCategory == 2) // Animal Husbandry
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_pen_small"), LocalizationRuntime.T("building_pen_small_desc"), GetBuildingDefinition("building.animal-pen-small"), "PenSquare", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_pen_long"), LocalizationRuntime.T("building_pen_long_desc"), GetBuildingDefinition("building.animal-pen-long"), "PenLong", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_sheep_pasture"), LocalizationRuntime.T("building_sheep_pasture_desc"), GetBuildingDefinition("building.sheep-pasture"), "PenSquare", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 3), LocalizationRuntime.T("building_hen_coop"), LocalizationRuntime.T("building_hen_coop_desc"), GetBuildingDefinition("building.hen-coop"), "PenSquare", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 4), LocalizationRuntime.T("building_feed_trough"), LocalizationRuntime.T("building_feed_trough_desc"), GetBuildingDefinition("building.feed-trough"), "PenSquare", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 5), LocalizationRuntime.T("building_water_trough"), LocalizationRuntime.T("building_water_trough_desc"), GetBuildingDefinition("building.water-trough"), "PenSquare", true);
            }
            else if (selectedBuildCategory == 3) // Fences & Walls
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_fence_drag"), LocalizationRuntime.T("building_fence_drag_desc"), GetBuildingDefinition("building.perimeter-fence-drag"), "FenceDrag", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_fence_small"), LocalizationRuntime.T("building_fence_small_desc"), GetBuildingDefinition("building.perimeter-fence-small"), "FenceSmall", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_fence_med"), LocalizationRuntime.T("building_fence_med_desc"), GetBuildingDefinition("building.perimeter-fence-medium"), "FenceMedium", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 3), LocalizationRuntime.T("building_fence_lrg"), LocalizationRuntime.T("building_fence_lrg_desc"), GetBuildingDefinition("building.perimeter-fence-large"), "FenceLarge", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 4), LocalizationRuntime.T("building_fence_grd"), LocalizationRuntime.T("building_fence_grd_desc"), GetBuildingDefinition("building.perimeter-fence-grand"), "FenceGrand", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 5), LocalizationRuntime.T("building_fence"), LocalizationRuntime.T("building_fence_desc"), GetBuildingDefinition("building.fence"), "Fence", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 6), LocalizationRuntime.T("building_gate"), LocalizationRuntime.T("building_gate_desc"), GetBuildingDefinition("building.gate"), "Gate", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 7), LocalizationRuntime.T("building_stone_wall"), LocalizationRuntime.T("building_stone_wall_desc"), GetBuildingDefinition("building.stone-wall"), "Fence", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 8), LocalizationRuntime.T("building_iron_gate"), LocalizationRuntime.T("building_iron_gate_desc"), GetBuildingDefinition("building.iron-gate"), "Gate", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 9), LocalizationRuntime.T("building_log_palisade"), LocalizationRuntime.T("building_log_palisade_desc"), GetBuildingDefinition("building.log-palisade"), "Fence", true);
            }
            else if (selectedBuildCategory == 4) // Paths & Landscaping
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_path_dirt"), LocalizationRuntime.T("building_path_dirt_desc"), GetBuildingDefinition("building.path-dirt"), "PathDirt", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_path_cobble"), LocalizationRuntime.T("building_path_cobble_desc"), GetBuildingDefinition("building.path-cobblestone"), "PathCobble", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_path_wood"), LocalizationRuntime.T("building_path_wood_desc"), GetBuildingDefinition("building.path-wood"), "PathDirt", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 3), LocalizationRuntime.T("building_path_stone_tile"), LocalizationRuntime.T("building_path_stone_tile_desc"), GetBuildingDefinition("building.path-stone-tile"), "PathCobble", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 4), LocalizationRuntime.T("building_wood_bridge"), LocalizationRuntime.T("building_wood_bridge_desc"), GetBuildingDefinition("building.wood-bridge"), "FenceDrag", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 5), LocalizationRuntime.T("building_scarecrow"), LocalizationRuntime.T("building_scarecrow_desc"), GetBuildingDefinition("building.scarecrow"), "Scarecrow", true);
            }
            else if (selectedBuildCategory == 5) // Furniture & Living
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_straw_bed"), LocalizationRuntime.T("building_straw_bed_desc"), GetBuildingDefinition("building.straw-bed"), "Cabin", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_oak_table"), LocalizationRuntime.T("building_oak_table_desc"), GetBuildingDefinition("building.oak-table"), "Cabin", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_leather_chair"), LocalizationRuntime.T("building_leather_chair_desc"), GetBuildingDefinition("building.leather-chair"), "Cabin", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 3), LocalizationRuntime.T("building_bookshelf"), LocalizationRuntime.T("building_bookshelf_desc"), GetBuildingDefinition("building.bookshelf"), "Cabin", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 4), LocalizationRuntime.T("building_woven_rug"), LocalizationRuntime.T("building_woven_rug_desc"), GetBuildingDefinition("building.woven-rug"), "Cabin", true);
            }
            else if (selectedBuildCategory == 6) // Artisan & Processing
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_cheese_press"), LocalizationRuntime.T("building_cheese_press_desc"), GetBuildingDefinition("building.cheese-press"), "Shed", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_loom"), LocalizationRuntime.T("building_loom_desc"), GetBuildingDefinition("building.loom"), "Shed", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_keg"), LocalizationRuntime.T("building_keg_desc"), GetBuildingDefinition("building.keg"), "Shed", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 3), LocalizationRuntime.T("building_windmill"), LocalizationRuntime.T("building_windmill_desc"), GetBuildingDefinition("building.windmill"), "Barn", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 4), LocalizationRuntime.T("building_blacksmith_forge"), LocalizationRuntime.T("building_blacksmith_forge_desc"), GetBuildingDefinition("building.blacksmith-forge"), "Hearth", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 5), LocalizationRuntime.T("building_carpenter_bench"), LocalizationRuntime.T("building_carpenter_bench_desc"), GetBuildingDefinition("building.carpenter-bench"), "Shed", true);
            }
            else if (selectedBuildCategory == 7) // Storage & Logistics
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_wood_chest"), LocalizationRuntime.T("building_wood_chest_desc"), GetBuildingDefinition("building.wood-chest"), "Shed", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_stone_vault"), LocalizationRuntime.T("building_stone_vault_desc"), GetBuildingDefinition("building.stone-vault"), "Shed", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_compost_bin"), LocalizationRuntime.T("building_compost_bin_desc"), GetBuildingDefinition("building.compost-bin"), "Shed", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 3), LocalizationRuntime.T("building_barrel_rack"), LocalizationRuntime.T("building_barrel_rack_desc"), GetBuildingDefinition("building.barrel-rack"), "Shed", true);
            }
            else if (selectedBuildCategory == 8) // Gardening & Greenery
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_grape_trellis"), LocalizationRuntime.T("building_grape_trellis_desc"), GetBuildingDefinition("building.grape-trellis"), "Fence", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_pumpkin_patch"), LocalizationRuntime.T("building_pumpkin_patch_desc"), GetBuildingDefinition("building.pumpkin-patch"), "FenceDrag", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_flower_planter"), LocalizationRuntime.T("building_flower_planter_desc"), GetBuildingDefinition("building.flower-planter"), "Herbalist", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 3), LocalizationRuntime.T("building_garden_hedge"), LocalizationRuntime.T("building_garden_hedge_desc"), GetBuildingDefinition("building.garden-hedge"), "Fence", true);
            }
            else if (selectedBuildCategory == 9) // Water & Irrigation
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_ancient_well"), LocalizationRuntime.T("building_ancient_well_desc"), GetBuildingDefinition("building.ancient-well"), "Cottage", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_water_aqueduct"), LocalizationRuntime.T("building_water_aqueduct_desc"), GetBuildingDefinition("building.water-aqueduct"), "Fence", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_stone_fountain"), LocalizationRuntime.T("building_stone_fountain_desc"), GetBuildingDefinition("building.stone-fountain"), "Cottage", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 3), LocalizationRuntime.T("building_hot_bath"), LocalizationRuntime.T("building_hot_bath_desc"), GetBuildingDefinition("building.hot-bath"), "Cabin", true);
            }
            else if (selectedBuildCategory == 10) // Monuments & Shrines
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_knight_statue"), LocalizationRuntime.T("building_knight_statue_desc"), GetBuildingDefinition("building.knight-statue"), "Lookout", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_guardian_shrine"), LocalizationRuntime.T("building_guardian_shrine_desc"), GetBuildingDefinition("building.guardian-shrine"), "Lookout", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_bell_pillar"), LocalizationRuntime.T("building_bell_pillar_desc"), GetBuildingDefinition("building.bell-pillar"), "Lookout", true);
            }
            else if (selectedBuildCategory == 11) // Market & Commerce
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_market_stall"), LocalizationRuntime.T("building_market_stall_desc"), GetBuildingDefinition("building.market-stall"), "Cabin", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_farm_sign"), LocalizationRuntime.T("building_farm_sign_desc"), GetBuildingDefinition("building.farm-sign"), "Scarecrow", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_travel_cart"), LocalizationRuntime.T("building_travel_cart_desc"), GetBuildingDefinition("building.travel-cart"), "Barn", true);
            }
            else if (selectedBuildCategory == 12) // Defenses & Traps
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_spike_trap"), LocalizationRuntime.T("building_spike_trap_desc"), GetBuildingDefinition("building.spike-trap"), "Campfire", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_wooden_barricade"), LocalizationRuntime.T("building_wooden_barricade_desc"), GetBuildingDefinition("building.wooden-barricade"), "Fence", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_alarm_bell"), LocalizationRuntime.T("building_alarm_bell_desc"), GetBuildingDefinition("building.alarm-bell"), "Lookout", true);
            }
            else if (selectedBuildCategory == 13) // Leisure & Camping
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_wood_swing"), LocalizationRuntime.T("building_wood_swing_desc"), GetBuildingDefinition("building.wood-swing"), "Cabin", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_chess_table"), LocalizationRuntime.T("building_chess_table_desc"), GetBuildingDefinition("building.chess-table"), "Cabin", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_hammock"), LocalizationRuntime.T("building_hammock_desc"), GetBuildingDefinition("building.hammock"), "Cabin", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 3), LocalizationRuntime.T("building_bbq_grill"), LocalizationRuntime.T("building_bbq_grill_desc"), GetBuildingDefinition("building.bbq-grill"), "Campfire", true);
            }
            else // Festivals & Ornaments (selectedBuildCategory == 14)
            {
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 0), LocalizationRuntime.T("building_festival_banner"), LocalizationRuntime.T("building_festival_banner_desc"), GetBuildingDefinition("building.festival-banner"), "Lookout", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 1), LocalizationRuntime.T("building_sky_lantern"), LocalizationRuntime.T("building_sky_lantern_desc"), GetBuildingDefinition("building.sky-lantern"), "Campfire", true);
                DrawBuildCatalogCard(GetScrollCardRect(cardWidth, cardHeight, gap, columns, 2), LocalizationRuntime.T("building_firefly_jar"), LocalizationRuntime.T("building_firefly_jar_desc"), GetBuildingDefinition("building.firefly-jar"), "Campfire", true);
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

        private void DrawBuildCatalogCard(Rect rect, string name, string description, BuildingDefinition definition, string glyph, bool buildable)
        {
            bool hasMaterials = HasBuildMaterials(definition);
            bool canBuild = buildable && definition != null && placementController != null && hasMaterials;
            DrawRect(rect, buildable ? InkSoft : new Color(0.055f, 0.052f, 0.048f, 0.86f));
            DrawBorder(rect, canBuild ? GoldDim : new Color(0.34f, 0.22f, 0.16f, 1f), canBuild ? 1.5f : 1f);

            // Icon
            Rect icon = new Rect(rect.x + 10f, rect.y + 10f, 44f, 44f);
            DrawBuildingGlyph(icon, glyph, definition);

            // Title & Footprint
            GUI.Label(new Rect(rect.x + 60f, rect.y + 8f, rect.width - 66f, 20f), name, labelStyle);
            string sizeInfo = definition != null ? $"📐 {definition.Footprint.x}x{definition.Footprint.y}" : "";
            GUI.Label(new Rect(rect.x + 60f, rect.y + 28f, rect.width - 66f, 16f), sizeInfo, numberStyle);

            // Description
            GUI.Label(new Rect(rect.x + 10f, rect.y + 56f, rect.width - 20f, 32f), description, smallStyle);

            // Requirements Box
            Rect requirements = new Rect(rect.x + 10f, rect.y + 90f, rect.width - 20f, 88f);
            DrawRect(requirements, new Color(0.030f, 0.026f, 0.022f, 0.78f));
            DrawBorder(requirements, new Color(0.16f, 0.13f, 0.10f, 1f), 1f);
            GUI.Label(new Rect(requirements.x + 8f, requirements.y + 2f, requirements.width - 16f, 16f), LocalizationRuntime.T("required_items"), numberStyle);

            if (definition != null)
            {
                DrawBuildRequirements(requirements, definition.ConstructionCosts);
            }
            else
            {
                GUI.Label(new Rect(requirements.x + 8f, requirements.y + 22f, requirements.width - 16f, 34f), LocalizationRuntime.T("requirements_unfinalized"), smallStyle);
            }

            // Action Button
            Rect action = new Rect(rect.x + 10f, rect.y + 182f, rect.width - 20f, 34f);
            if (canBuild)
            {
                if (GUI.Button(action, LocalizationRuntime.T("select_place"), buttonStyle))
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
                if (GUI.Button(action, LocalizationRuntime.T("not_enough_items"), buttonStyle))
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
                GUI.Label(new Rect(rect.x + 10f, rect.y + 24f, rect.width - 20f, 20f), LocalizationRuntime.T("no_material_cost"), smallStyle);
                return;
            }

            for (int i = 0; i < costs.Length && i < 3; i++)
            {
                BuildCostEntry cost = costs[i];
                PrototypeItemInfo item = PrototypeItemCatalog.Get(cost.itemId);
                int owned = GetQuantity(cost.itemId);
                bool hasEnough = owned >= cost.quantity;
                Rect row = new Rect(rect.x + 8f, rect.y + 20f + i * 20f, rect.width - 16f, 18f);
                DrawRect(new Rect(row.x, row.y + 4f, 10f, 10f), item.Color);

                Color previous = GUI.color;
                GUI.color = hasEnough ? new Color(0.72f, 0.95f, 0.60f, 1f) : new Color(0.95f, 0.48f, 0.40f, 1f);
                GUI.Label(new Rect(row.x + 16f, row.y, row.width - 16f, row.height), LocalizeItemName(item.ItemId, item.DisplayName) + " " + owned + "/" + cost.quantity, smallStyle);
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

        private void DrawBuildingGlyph(Rect rect, string glyph, BuildingDefinition definition = null)
        {
            DrawRect(rect, new Color(0.025f, 0.022f, 0.018f, 1f));
            DrawBorder(rect, Color.black, 1f);

            string bId = definition != null ? definition.BuildingId : null;
            Sprite spr = null;

            if (!string.IsNullOrEmpty(bId))
            {
                spr = PrototypePixelArtFactory.BuildingCatalogIcon(bId);
            }
            else if (!string.IsNullOrEmpty(glyph))
            {
                if (glyph == "Herbalist") spr = PrototypePixelArtFactory.HerbalistHut();
                else if (glyph == "Lookout") spr = PrototypePixelArtFactory.LookoutTower();
                else if (glyph == "Fence") spr = PrototypePixelArtFactory.WoodFence();
                else if (glyph == "Gate") spr = PrototypePixelArtFactory.WoodGate(false);
                else if (glyph == "Cottage") spr = PrototypePixelArtFactory.StoneCottage();
                else if (glyph == "Campfire") spr = PrototypePixelArtFactory.Campfire();
                else if (glyph == "Hearth") spr = PrototypePixelArtFactory.CookingHearthOutdoor();
                else if (glyph == "Scarecrow") spr = PrototypePixelArtFactory.Scarecrow();
                else if (glyph == "PathDirt") spr = PrototypePixelArtFactory.PathDirtTile();
                else if (glyph == "PathCobble") spr = PrototypePixelArtFactory.PathCobblestoneTile();
                else if (glyph.StartsWith("Fence")) spr = PrototypePixelArtFactory.PerimeterFencePreview();
                else if (glyph == "Shed") spr = PrototypePixelArtFactory.StorageShed();
                else spr = PrototypePixelArtFactory.BuildingCatalogIcon(glyph);
            }

            if (spr == null)
            {
                spr = PrototypePixelArtFactory.CabinComplete();
            }

            DrawSprite(spr, new Rect(rect.x + 3f, rect.y + 3f, rect.width - 6f, rect.height - 6f));
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
            DrawBorder(mapRect, GoldDim, 1.5f);

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
            // Map terrain background
            DrawRect(map, new Color(0.08f, 0.17f, 0.10f, 1f));
            // North forest zone
            DrawRect(new Rect(map.x, map.y, map.width, map.height * 0.18f), new Color(0.05f, 0.11f, 0.08f, 1f));
            // South forest zone
            DrawRect(new Rect(map.x, map.y + map.height * 0.86f, map.width, map.height * 0.14f), new Color(0.05f, 0.11f, 0.08f, 1f));
            DrawRiver(map);
            DrawRoad(map);
            DrawBorder(map, new Color(0.01f, 0.012f, 0.01f, 1f), 2f);

            // Draw Landmarks (Undiscovered "?" and Discovered "★")
            DrawLandmarkDots(map);

            // Draw Construction sites & Home
            foreach (ConstructionSite site in cachedConstructionSites)
            {
                if (site == null) continue;
                Color color = IsHomeBuilding(site.BuildingId)
                    ? new Color(1f, 0.82f, 0.24f, 1f)
                    : new Color(0.95f, 0.62f, 0.22f, 1f);
                DrawMapDot(map, site.transform.position, color, map.width > 220f ? 10f : 6f);
            }

            // Draw Waypoint
            if (hasWaypoint) DrawWaypointMarker(map, waypointWorldPosition, map.width > 220f ? 14f : 9f);

            // Draw Player with glowing aura
            PlayerMovement player = cachedPlayer;
            if (player != null)
            {
                Vector2 mapPoint = WorldToMap(map, player.transform.position);
                float pSz = map.width > 220f ? 12f : 7f;
                DrawRect(new Rect(mapPoint.x - pSz * 0.5f - 2f, mapPoint.y - pSz * 0.5f - 2f, pSz + 4f, pSz + 4f), new Color(0.25f, 0.62f, 1f, 0.45f));
                DrawRect(new Rect(mapPoint.x - pSz * 0.5f, mapPoint.y - pSz * 0.5f, pSz, pSz), new Color(0.25f, 0.62f, 1f, 1f));
                DrawBorder(new Rect(mapPoint.x - pSz * 0.5f, mapPoint.y - pSz * 0.5f, pSz, pSz), Color.white, 1f);
            }
        }

        private void DrawRoad(Rect map)
        {
            int segments = Mathf.Max(28, Mathf.RoundToInt(map.width / 8f));
            float roadHeight = map.height * 0.07f;
            bool isLarge = map.width > 220f;
            Vector3 center = GetMapCenter();
            float startX = isLarge ? WorldMin.x : (center.x - 32f);
            float endX = isLarge ? WorldMax.x : (center.x + 32f);

            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)(segments - 1);
                float nextT = Mathf.Min(1f, (i + 1) / (float)(segments - 1));
                float worldX = Mathf.Lerp(startX, endX, t);
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
            bool isLarge = map.width > 220f;
            Vector3 center = GetMapCenter();
            float startX = isLarge ? WorldMin.x : (center.x - 32f);
            float endX = isLarge ? WorldMax.x : (center.x + 32f);

            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)(segments - 1);
                float worldX = Mathf.Lerp(startX, endX, t);
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
                    // Discovered landmark: distinct colored badge with icon emoji and readable label
                    float sz = isLargeMap ? 22f : 14f;
                    Rect bRect = new Rect(mapPoint.x - sz * 0.5f, mapPoint.y - sz * 0.5f, sz, sz);

                    // Badge background
                    DrawRect(bRect, landmark.MapColor);
                    DrawBorder(bRect, new Color(0.04f, 0.03f, 0.02f, 0.95f), 1.5f);

                    // Inner Emoji icon
                    GUI.Label(new Rect(bRect.x, bRect.y - (isLargeMap ? 2f : 3f), bRect.width, bRect.height), landmark.MapIconEmoji, centerStyle);

                    if (isLargeMap)
                    {
                        // Label name pill with contrast backdrop
                        string title = landmark.Title;
                        float textWidth = Mathf.Max(60f, title.Length * 7.2f + 14f);
                        Rect labelBg = new Rect(mapPoint.x + sz * 0.5f + 4f, mapPoint.y - 9f, textWidth, 18f);
                        DrawRect(labelBg, new Color(0.02f, 0.02f, 0.02f, 0.82f));
                        DrawBorder(labelBg, new Color(landmark.MapColor.r, landmark.MapColor.g, landmark.MapColor.b, 0.6f), 1f);
                        GUI.Label(new Rect(labelBg.x + 4f, labelBg.y + 1f, labelBg.width - 8f, 16f), title, smallStyle);
                    }
                }
                else
                {
                    // Undiscovered landmark: prominent glowing "?" badge prompting exploration
                    float sz = isLargeMap ? 18f : 12f;
                    Rect qRect = new Rect(mapPoint.x - sz * 0.5f, mapPoint.y - sz * 0.5f, sz, sz);
                    // Outer glow
                    DrawRect(new Rect(qRect.x - 1f, qRect.y - 1f, sz + 2f, sz + 2f), new Color(1f, 0.25f, 0.25f, 0.45f));
                    // Badge background
                    DrawRect(qRect, new Color(0.78f, 0.14f, 0.14f, 0.96f));
                    DrawBorder(qRect, Gold, 1.5f);
                    GUI.Label(new Rect(qRect.x, qRect.y - 1f, qRect.width, qRect.height), "?", centerStyle);
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

            PlayerFarmingInteractor farming = cachedFarming;
            if (farming != null && !string.IsNullOrWhiteSpace(farming.InteractionHint)) AppendPrompt(ref prompt, farming.InteractionHint);

            PlayerFishingInteractor fishing = cachedFishing;
            if (fishing != null && !string.IsNullOrWhiteSpace(fishing.InteractionHint)) AppendPrompt(ref prompt, fishing.InteractionHint);

            PlayerNpcInteractor npc = cachedNpcInteractor;
            if (npc != null && !string.IsNullOrWhiteSpace(npc.InteractionHint)) AppendPrompt(ref prompt, npc.InteractionHint);

            if (cachedBuildingInteractor != null && !string.IsNullOrWhiteSpace(cachedBuildingInteractor.ActionPrompt))
                AppendPrompt(ref prompt, cachedBuildingInteractor.ActionPrompt);

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
                case "item.mushroom":
                case "item.cooked-meal":
                case "item.cooked-fish":
                case "item.egg":
                case "item.milk":
                case "item.carrot":
                case "item.potato":
                case "item.corn":
                case "item.tomato":
                case "item.pineapple":
                case "item.strawberry":
                case "item.apple":
                case "item.grape":
                case "item.pumpkin":
                case "item.cheese":
                case "item.wine-fruit":
                case "item.juice":
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
            if (index < 0 || index >= 9 || hotbarSlotItemIds == null || index >= hotbarSlotItemIds.Length)
                return HotbarItem.Empty;

            string itemId = hotbarSlotItemIds[index];
            if (string.IsNullOrEmpty(itemId)) return HotbarItem.Empty;

            return ToHotbarItem(itemId);
        }

        private HotbarItem ToHotbarItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return HotbarItem.Empty;
            PrototypeItemInfo item = PrototypeItemCatalog.Get(itemId);
            if (string.IsNullOrEmpty(item.ItemId)) return HotbarItem.Empty;
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
                else if (key == "H") overlayMode = overlayMode == OverlayMode.Guide ? OverlayMode.None : OverlayMode.Guide;
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
            DrawLegendRow(rect.x + 16, rect.y + 36, new Color(0.25f, 0.62f, 1f, 1f), LocalizationRuntime.T("legend_player"));
            DrawLegendRow(rect.x + 16, rect.y + 58, new Color(1f, 0.82f, 0.24f, 1f), LocalizationRuntime.T("legend_home"));
            DrawLegendRow(rect.x + 16, rect.y + 80, new Color(0.20f, 0.82f, 0.92f, 1f), LocalizationRuntime.IsVietnamese ? "🏘️  Làng Valen & Cư dân" : "🏘️  Valen Village & NPCs");
            DrawLegendRow(rect.x + 16, rect.y + 102, new Color(1.0f, 0.78f, 0.20f, 1f), LocalizationRuntime.IsVietnamese ? "🛒  Thương nhân Eldon" : "🛒  Merchant Eldon");
            DrawLegendRow(rect.x + 16, rect.y + 124, new Color(0.35f, 0.88f, 0.45f, 1f), LocalizationRuntime.IsVietnamese ? "🐄  Trang trại & Chuồng nuôi" : "🐄  Farm & Animal Pasture");
            DrawLegendRow(rect.x + 16, rect.y + 146, new Color(0.95f, 0.60f, 0.20f, 1f), LocalizationRuntime.IsVietnamese ? "📜  Bảng đơn hàng & Hòm thư" : "📜  Bulletin Board & Mail");
            DrawLegendRow(rect.x + 16, rect.y + 168, new Color(0.86f, 0.42f, 1f, 1f), LocalizationRuntime.T("legend_waypoint"));
            DrawLegendRow(rect.x + 16, rect.y + 190, new Color(0.85f, 0.18f, 0.18f, 1f), LocalizationRuntime.IsVietnamese ? "?  Địa điểm chưa đến" : "?  Undiscovered Area");

            GUI.Label(new Rect(rect.x + 14, rect.y + 220f, rect.width - 28, 45f), LocalizationRuntime.T("map_pin_hint"), smallStyle);
            if (hasWaypoint && GUI.Button(new Rect(rect.x + 22f, rect.y + 270f, rect.width - 44f, 28f), LocalizationRuntime.T("clear_waypoint")))
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
            cachedBuildingInteractor = cachedBuildingInteractor != null ? cachedBuildingInteractor : FindAnyObjectByType<TheOldRoad.Building.BuildingInteractionController>();
            cachedFarming = cachedFarming != null ? cachedFarming : FindAnyObjectByType<PlayerFarmingInteractor>();
            cachedFishing = cachedFishing != null ? cachedFishing : FindAnyObjectByType<PlayerFishingInteractor>();
            cachedNpcInteractor = cachedNpcInteractor != null ? cachedNpcInteractor : FindAnyObjectByType<PlayerNpcInteractor>();
            cachedBulletinBoard = cachedBulletinBoard != null ? cachedBulletinBoard : FindAnyObjectByType<DailyBulletinBoardController>();
            cachedMailbox = cachedMailbox != null ? cachedMailbox : FindAnyObjectByType<DailyMailboxController>();

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
            Vector2 min, max;
            if (map.width > 220f)
            {
                min = WorldMin;
                max = WorldMax;
            }
            else
            {
                Vector3 center = GetMapCenter();
                const float radius = 32f;
                min = new Vector2(center.x - radius, center.y - radius);
                max = new Vector2(center.x + radius, center.y + radius);
            }

            Vector2 normalized = new Vector2(
                Mathf.InverseLerp(min.x, max.x, worldPosition.x),
                Mathf.InverseLerp(min.y, max.y, worldPosition.y));

            return new Vector2(
                map.x + Mathf.Clamp01(normalized.x) * map.width,
                map.y + (1f - Mathf.Clamp01(normalized.y)) * map.height);
        }

        private Vector3 MapToWorld(Rect map, Vector2 mapPoint)
        {
            Vector2 min, max;
            if (map.width > 220f)
            {
                min = WorldMin;
                max = WorldMax;
            }
            else
            {
                Vector3 center = GetMapCenter();
                const float radius = 32f;
                min = new Vector2(center.x - radius, center.y - radius);
                max = new Vector2(center.x + radius, center.y + radius);
            }

            float normalizedX = Mathf.Clamp01((mapPoint.x - map.x) / map.width);
            float normalizedY = 1f - Mathf.Clamp01((mapPoint.y - map.y) / map.height);
            float worldX = Mathf.Lerp(min.x, max.x, normalizedX);
            float worldY = Mathf.Lerp(min.y, max.y, normalizedY);
            return new Vector3(worldX, worldY, 0f);
        }

        private Vector3 GetMapCenter()
        {
            return cachedPlayer != null ? cachedPlayer.transform.position : Vector3.zero;
        }

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

            UiFontHelper.EnsureGlobalSkinFont();
            Font clean = UiFontHelper.CleanFont;

            gameTitleStyle = new GUIStyle(GUI.skin.label)
            {
                font = clean,
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Gold }
            };

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                font = clean,
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Gold }
            };

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                font = clean,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = Parchment }
            };

            smallStyle = new GUIStyle(GUI.skin.label)
            {
                font = clean,
                fontSize = 11,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
                normal = { textColor = MutedText }
            };

            centerStyle = new GUIStyle(GUI.skin.label)
            {
                font = clean,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                clipping = TextClipping.Clip,
                normal = { textColor = Parchment }
            };

            numberStyle = new GUIStyle(GUI.skin.label)
            {
                font = clean,
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = new Color(0.95f, 0.88f, 0.65f, 1f) }
            };

            promptStyle = new GUIStyle(GUI.skin.label)
            {
                font = clean,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Parchment }
            };

            captionStyle = new GUIStyle(GUI.skin.label)
            {
                font = clean,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 10,
                normal = { textColor = MutedText }
            };

            subtitleStyle = new GUIStyle(GUI.skin.label)
            {
                font = clean,
                fontSize = 11,
                wordWrap = true,
                alignment = TextAnchor.UpperLeft,
                normal = { textColor = Parchment }
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                font = clean,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Parchment },
                hover = { textColor = Gold },
                active = { textColor = Color.white }
            };

            categoryNormalStyle = new GUIStyle(GUI.skin.label)
            {
                font = clean,
                fontSize = 11,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = false,
                clipping = TextClipping.Clip,
                normal = { textColor = MutedText }
            };

            categorySelectedStyle = new GUIStyle(GUI.skin.label)
            {
                font = clean,
                fontSize = 11,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = false,
                clipping = TextClipping.Clip,
                normal = { textColor = Gold }
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
            new MerchantItem("item.seed-carrot", 4),
            new MerchantItem("item.seed-potato", 4),
            new MerchantItem("item.seed-corn", 4),
            new MerchantItem("item.seed-tomato", 5),
            new MerchantItem("item.seed-pineapple", 6),
            new MerchantItem("item.watering-can", 8),
            new MerchantItem("item.tool-hoe", 8),
            new MerchantItem("item.fishing-rod", 8),
            new MerchantItem("item.fishing-bait", 2),
            new MerchantItem("item.weapon-sword", 18),
            new MerchantItem("item.weapon-bow", 14),
            new MerchantItem("item.ammo-arrow", 2),
            new MerchantItem("item.shield-wood", 12),
            new MerchantItem("item.armor-knight", 25),
            new MerchantItem("item.hay", 3),
            new MerchantItem("item.farm-deed", 35),
            new MerchantItem("item.cooked-meal", 12),
            new MerchantItem("item.cooked-fish", 15),
            new MerchantItem("item.torch", 5),
            new MerchantItem("item.cabin-plank", 6),
            new MerchantItem("item.tool-axe", 10),
            new MerchantItem("item.tool-pickaxe", 12)
        };

        private static readonly MerchantItem[] MerchantSellList = new MerchantItem[]
        {
            new MerchantItem("item.wheat", 5),
            new MerchantItem("item.carrot", 6),
            new MerchantItem("item.potato", 6),
            new MerchantItem("item.corn", 7),
            new MerchantItem("item.tomato", 8),
            new MerchantItem("item.pineapple", 12),
            new MerchantItem("item.fish-carp", 6),
            new MerchantItem("item.fish-salmon", 8),
            new MerchantItem("item.fish-golden-perch", 20),
            new MerchantItem("item.cooked-fish", 14),
            new MerchantItem("item.egg", 4),
            new MerchantItem("item.wool", 8),
            new MerchantItem("item.milk", 9),
            new MerchantItem("item.meat-raw", 5),
            new MerchantItem("item.leather", 7),
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

        private int guideTab = 0;

        private void DrawNewbieHintBanner()
        {
            if (overlayMode != OverlayMode.None) return;

            float bannerW = Mathf.Min(420f, Screen.width - 490f);
            if (bannerW < 220f) return;

            Rect banner = new Rect((Screen.width - bannerW) * 0.5f, 14f, bannerW, 26f);
            DrawCard(banner, new Color(0.045f, 0.038f, 0.028f, 0.88f));
            DrawBorder(banner, GoldDim, 1f);

            if (GUI.Button(banner, GUIContent.none, GUIStyle.none))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                ToggleOverlay(OverlayMode.Guide);
            }

            string text = LocalizationRuntime.IsVietnamese
                ? "📖 [H] Sổ Hướng Dẫn Tân Thủ & Lối Chơi"
                : "📖 [H] Beginner Guide & Gameplay Tips";
            GUI.Label(banner, text, centerStyle);
        }

        private void DrawGuideOverlay()
        {
            float width = Mathf.Min(980f, Screen.width - 40f);
            float height = Mathf.Min(620f, Screen.height - 60f);
            Rect panel = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
            DrawCard(panel, Ink);
            DrawCornerAccents(panel, Gold);
            DrawHeaderStrip(new Rect(panel.x, panel.y, panel.width, 44));

            GUI.Label(new Rect(panel.x + 20f, panel.y + 10f, 400f, 26f), LocalizationRuntime.T("guide"), titleStyle);
            GUI.Label(new Rect(panel.xMax - 140f, panel.y + 12f, 120f, 20f), LocalizationRuntime.T("esc_close"), smallStyle);

            string[] tabs = new string[]
            {
                LocalizationRuntime.T("guide_tab_basics"),
                LocalizationRuntime.T("guide_tab_farming"),
                LocalizationRuntime.T("guide_tab_fishing"),
                LocalizationRuntime.T("guide_tab_combat"),
                LocalizationRuntime.T("guide_tab_expansion"),
                LocalizationRuntime.T("guide_tab_compendium")
            };

            float tabW = (panel.width - 36f) / tabs.Length;
            for (int i = 0; i < tabs.Length; i++)
            {
                Rect tabRect = new Rect(panel.x + 18f + i * tabW, panel.y + 50f, tabW - 4f, 32f);
                bool isActive = guideTab == i;
                DrawRect(tabRect, isActive ? new Color(0.38f, 0.22f, 0.08f, 0.95f) : InkSoft);
                DrawBorder(tabRect, isActive ? Gold : GoldDim, isActive ? 2f : 1f);

                if (GUI.Button(tabRect, tabs[i], isActive ? labelStyle : smallStyle))
                {
                    guideTab = i;
                    TheOldRoad.Audio.AudioManager.PlayUiClick();
                }
            }

            Rect contentArea = new Rect(panel.x + 18f, panel.y + 88f, panel.width - 36f, panel.height - 144f);
            DrawRect(contentArea, new Color(0.02f, 0.016f, 0.012f, 0.75f));
            DrawBorder(contentArea, GoldDim, 1.5f);

            DrawGuideTabContent(contentArea, guideTab);

            // Navigation Footer
            float navY = panel.yMax - 46f;
            if (guideTab > 0)
            {
                Rect prevRect = new Rect(panel.x + 18f, navY, 140f, 34f);
                if (GUI.Button(prevRect, LocalizationRuntime.IsVietnamese ? "◀ Trang Trước" : "◀ Previous", buttonStyle))
                {
                    guideTab--;
                    TheOldRoad.Audio.AudioManager.PlayUiClick();
                }
            }

            if (guideTab < tabs.Length - 1)
            {
                Rect nextRect = new Rect(panel.xMax - 158f, navY, 140f, 34f);
                if (GUI.Button(nextRect, LocalizationRuntime.IsVietnamese ? "Trang Tiếp ▶" : "Next ▶", buttonStyle))
                {
                    guideTab++;
                    TheOldRoad.Audio.AudioManager.PlayUiClick();
                }
            }

            Rect closeBtnRect = new Rect(panel.x + (panel.width - 120f) * 0.5f, navY, 120f, 34f);
            if (GUI.Button(closeBtnRect, LocalizationRuntime.T("esc_close"), buttonStyle))
            {
                overlayMode = OverlayMode.None;
                TheOldRoad.Audio.AudioManager.PlayUiClick();
            }
        }

        private void DrawGuideTabContent(Rect area, int tab)
        {
            float padding = 16f;
            float curY = area.y + padding;

            if (tab == 0) // Điều khiển & Cơ bản
            {
                GUI.Label(new Rect(area.x + padding, curY, area.width - padding * 2, 22f), 
                    LocalizationRuntime.IsVietnamese ? "🔰 HƯỚNG DẪN TÂN THỦ & ĐIỀU KHIỂN CƠ BẢN" : "🔰 BEGINNER BASICS & CONTROLS", titleStyle);
                curY += 28f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 44f),
                    "[W][A][S][D] / Cần Gạt Ảo",
                    LocalizationRuntime.IsVietnamese ? "Di chuyển Hiệp sĩ khám phá thế giới, đi qua các vùng đất, khu rừng và ven sông." : "Move your Knight across the road, forests, and riverbanks.", null);
                curY += 48f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 44f),
                    "[SPACE] / Nút Chém ⚔",
                    LocalizationRuntime.IsVietnamese ? "Tấn công quái vật bằng Kiếm / Bắn Cung tên / Thu thập tài nguyên cây cối, quặng đá gần đó." : "Attack enemies with Sword / Fire Bow / Gather nearby trees and rock nodes.", null);
                curY += 48f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 44f),
                    "[F] / Nút Tương Tác",
                    LocalizationRuntime.IsVietnamese ? "Phím hành động đa năng: Cuốc đất, gieo hạt, tưới nước, thu hoạch cây, câu cá, vắt sữa bò, nhặt đồ, mở rương." : "Universal interact: Till soil, plant seed, water, harvest crop, fish, milk cow, talk NPC, open chest.", null);
                curY += 48f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 44f),
                    "[V] / Chuột Phải",
                    LocalizationRuntime.IsVietnamese ? "Giơ Khiên gỗ tròn để phòng thủ, chặn đứng và giảm 75% sát thương nhận vào." : "Raise Round Shield to block incoming monster attacks (reduces damage by 75%).", null);
                curY += 48f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 44f),
                    "[Q] / [TAB] / [M] / [J] / [B]",
                    LocalizationRuntime.IsVietnamese ? "[Q] Ăn thực phẩm hồi máu | [TAB/I] Túi đồ | [M] Bản đồ toàn cảnh | [J] Nhiệm vụ | [B] Xây dựng." : "[Q] Eat food | [TAB/I] Inventory | [M] Full Map | [J] Quest Log | [B] Build menu.", null);
            }
            else if (tab == 1) // Nông trại & Trồng trọt
            {
                GUI.Label(new Rect(area.x + padding, curY, area.width - padding * 2, 22f), 
                    LocalizationRuntime.IsVietnamese ? "🌾 HỆ THỐNG NÔNG TRẠI & TRỒNG TRỌT ĐỒNG QUÊ" : "🌾 CROP FARMING & HARVESTING SYSTEM", titleStyle);
                curY += 28f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "1. Cuốc Xới Đất" : "1. Till Soil",
                    LocalizationRuntime.IsVietnamese ? "Trang bị Cuốc làm vườn (Worn Hoe) và nhấn [F] vào ô đất hoang để xới luống đất màu mỡ." : "Equip Worn Hoe and press [F] on wild soil plot to till fertile farm land.", "item.tool-hoe");
                curY += 54f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "2. Gieo Hạt Giống" : "2. Plant Seeds",
                    LocalizationRuntime.IsVietnamese ? "Chọn hạt giống (Lúa mì, Bắp ngô, Cà rốt, Khoai tây, Cà chua, Dứa) trên thanh công cụ và nhấn [F] để gieo." : "Select seeds (Wheat, Corn, Carrot, Potato, Tomato, Pineapple) on hotbar and press [F] to plant.", "item.seed-wheat");
                curY += 54f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "3. Tưới Nước & Mưa" : "3. Watering & Rain",
                    LocalizationRuntime.IsVietnamese ? "Dùng Bình tưới nước [F] để đất ẩm giúp cây lớn nhanh gấp 2 lần! Đặc biệt khi trời mưa đất sẽ tự động được tưới ẩm." : "Use Watering Can [F] so soil is watered (grows 2x faster). Rain will automatically water all farm plots!", "item.watering-can");
                curY += 54f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "4. 5 Giai Đoạn & Offline" : "4. 5 Stages & Offline",
                    LocalizationRuntime.IsVietnamese ? "Cây lớn qua 5 giai đoạn hình ảnh sinh động. Cây vẫn tiếp tục lớn ngay cả khi bạn offline tắt game." : "Crops grow across 5 visual pixel art stages. Crops continue to grow even when you quit the game.", "item.wheat");
                curY += 54f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "5. Thu Hoạch Dễ Dàng" : "5. Easy Harvest",
                    LocalizationRuntime.IsVietnamese ? "Nhấn [F] khi cây trĩu hạt để thu hoạch. Ô đất vẫn giữ nguyên trạng thái đã xới, sẵn sàng gieo đợt mới!" : "Press [F] when crops mature to harvest. Soil remains tilled, ready for your next seed!", "item.potato");
            }
            else if (tab == 2) // Câu cá & Nấu ăn
            {
                GUI.Label(new Rect(area.x + padding, curY, area.width - padding * 2, 22f), 
                    LocalizationRuntime.IsVietnamese ? "🎣 CÂU CÁ SÔNG VALEN & ẨM THỰC HỒI MÁU" : "🎣 RIVER FISHING & CAMPFIRE COOKING", titleStyle);
                curY += 28f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 54f),
                    LocalizationRuntime.IsVietnamese ? "1. Thả Phao Câu Ven Sông" : "1. Cast River Line",
                    LocalizationRuntime.IsVietnamese ? "Tiến sát bờ sông Valen ở phía nam. Trang bị Cần câu tre [item.fishing-rod] và bấm [F] để quăng phao nổi dập dềnh trên nước." : "Walk to the southern Valen river. Equip Bamboo Rod and press [F] to cast bobber into the stream.", "item.fishing-rod");
                curY += 58f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 54f),
                    LocalizationRuntime.IsVietnamese ? "2. Dùng Mồi Trùn Đất" : "2. Earthworm Bait",
                    LocalizationRuntime.IsVietnamese ? "Có Mồi trùn đất trong túi đồ sẽ giúp cá cắn câu nhanh hơn nhiều lần và tăng tỉ lệ bắt cá quý hiếm." : "Carrying Earthworm Bait makes fish bite significantly faster and increases chances for prized catches.", "item.fishing-bait");
                curY += 58f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 54f),
                    LocalizationRuntime.IsVietnamese ? "3. Giật Cần Khi Phao Rung [!]" : "3. Reel On Bite [!]",
                    LocalizationRuntime.IsVietnamese ? "Khi phao rung lắc và hiện biểu tượng [!], lập tức bấm [F] trong vòng 1.8 giây để kéo cá lên bờ!" : "When the bobber splashes and shows [!], quickly press [F] within 1.8s to reel in your catch!", "item.fish-salmon");
                curY += 58f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 54f),
                    LocalizationRuntime.IsVietnamese ? "4. Nướng Cá & Bữa Ăn Nóng" : "4. Grilled Fish & Meals",
                    LocalizationRuntime.IsVietnamese ? "Đem cá và nông sản đến Lửa trại hoặc Bếp lò để nấu Cá nướng thảo mộc (+18 HP) và Bữa ăn nóng (+12 HP)." : "Bring fish and crops to campfire or stove to prepare Herb Grilled Fish (+18 HP) and Cooked Meals (+12 HP).", "item.cooked-fish");
            }
            else if (tab == 3) // Chiến đấu & Trang bị
            {
                GUI.Label(new Rect(area.x + padding, curY, area.width - padding * 2, 22f), 
                    LocalizationRuntime.IsVietnamese ? "⚔️ CHIẾN ĐẤU, VŨ KHÍ, CUNG TÊN & PHÒNG THỦ" : "⚔️ COMBAT, WEAPONS, BOW & DEFENSE", titleStyle);
                curY += 28f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "Kiếm Sắt Dài (+7 DMG)" : "Iron Longsword (+7 DMG)",
                    LocalizationRuntime.IsVietnamese ? "Sát thương chém cận chiến mạnh mẽ, tầm quét rộng để tiêu diệt bầy thú dữ và đạo tặc." : "High melee slash damage with wide sweep arc to defeat wild beasts and bandits.", "item.weapon-sword");
                curY += 54f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "Cung Săn Bắn & Mũi Tên" : "Hunter's Bow & Flint Arrows",
                    LocalizationRuntime.IsVietnamese ? "Tấn công từ xa cực kỳ an toàn. Nhấn phím Space để bắn mũi tên bay tiêu diệt mục tiêu từ xa." : "Safe ranged attack. Press Space to fire flint arrows directly at distant monsters.", "item.weapon-bow");
                curY += 54f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "Khiên Gỗ Tròn (-75% DMG)" : "Round Shield (-75% DMG)",
                    LocalizationRuntime.IsVietnamese ? "Giữ phím [V] hoặc Chuột phải để giơ khiên đỡ đòn, giảm 75% sát thương nhận vào." : "Hold [V] or Right-click to raise shield and reduce 75% incoming damage with steel block clank.", "item.shield-wood");
                curY += 54f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "Giáp Ngực Hiệp Sĩ (+10 HP)" : "Knight Cuirass (+10 Max HP)",
                    LocalizationRuntime.IsVietnamese ? "Gia tăng lượng máu tối đa của nhân vật, giúp bạn sống sót qua những cuộc săn đêm nguy hiểm." : "Increases maximum player health, essential for surviving dangerous night hunts.", "item.armor-knight");
                curY += 54f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "Bóng Ma U Tối (Shadow Stalker)" : "Shadow Stalker Monster",
                    LocalizationRuntime.IsVietnamese ? "Quái vật đêm rình rập, khi bị hạ gục sẽ rơi ra Mảnh chuông cổ, Da thú và nhiều Đồng bạc." : "Lurking shadow night creature that drops Bell Fragments, Leather and Silver Coins when slain.", "item.bell-fragment");
            }
            else if (tab == 4) // Chăn nuôi & Mở rộng đất
            {
                GUI.Label(new Rect(area.x + padding, curY, area.width - padding * 2, 22f), 
                    LocalizationRuntime.IsVietnamese ? "🐄 CHĂN NUÔI GIA SÚC & MỞ RỘNG ĐẤT ĐAI" : "🐄 ANIMAL HUSBANDRY & FARM EXPANSION", titleStyle);
                curY += 28f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 54f),
                    LocalizationRuntime.IsVietnamese ? "Bò Sữa & Cho Ăn Cỏ Khô" : "Dairy Cow & Hay Feeding",
                    LocalizationRuntime.IsVietnamese ? "Bò cho Sữa tươi [item.milk]. Đem Bó cỏ khô [item.hay] hoặc Lúa mì cho bò ăn để reset thời gian vắt sữa ngay lập tức!" : "Cows yield fresh Milk. Feeding Dry Hay or Wheat immediately resets milk cooldown and shows love hearts!", "item.hay");
                curY += 58f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 54f),
                    LocalizationRuntime.IsVietnamese ? "Gà & Rải Thóc Ổ Rơm" : "Hens & Straw Nests",
                    LocalizationRuntime.IsVietnamese ? "Gà đẻ trứng tại ổ rơm. Rải hạt giống lúa mì vào ổ rơm để đàn gà đẻ trứng thơm ngon nhanh hơn." : "Hens lay fresh Eggs in straw nests. Scattering wheat seeds into nests speeds up egg laying.", "item.egg");
                curY += 58f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 54f),
                    LocalizationRuntime.IsVietnamese ? "Thư Khai Hoang Đất Đai" : "Farm Land Deed Expansion",
                    LocalizationRuntime.IsVietnamese ? "Sở hữu Thư khai hoang [item.farm-deed] từ thương nhân sẽ mở thêm 12 ô đất Grid B (tổng cộng 24 ô đất nông trại)!" : "Owning a Farm Land Deed unlocks 12 additional farm plots (doubling your field to 24 total plots)!", "item.farm-deed");
                curY += 58f;

                DrawGuideItemRow(new Rect(area.x + padding, curY, area.width - padding * 2, 54f),
                    LocalizationRuntime.IsVietnamese ? "Thương Nhân Eldon" : "Merchant Eldon Trade",
                    LocalizationRuntime.IsVietnamese ? "Bán nông sản, cá sông, quặng sắt để lấy Đồng bạc [item.silver-coin] và mua sắm hạt giống cùng trang bị xịn." : "Sell harvest crops, river fish, iron ore for Silver Coins to buy rare seeds and high tier equipment.", "item.silver-coin");
            }
            else if (tab == 5) // Sổ Bách Khoa Bộ Sưu Tập
            {
                DrawCompendiumContent(area);
            }
        }

        private void DrawCompendiumContent(Rect area)
        {
            float padding = 14f;
            var entries = CompendiumCatalog.GetAll();
            int discovered = CompendiumCatalog.GetDiscoveredCount(inventorySession != null ? inventorySession.Runtime : null);

            GUI.Label(new Rect(area.x + padding, area.y + padding, area.width - padding * 2, 22f), 
                $"🏆 {LocalizationRuntime.T("compendium_title")} ({discovered}/{CompendiumCatalog.TotalCount})", titleStyle);

            float curY = area.y + 42f;
            float rowH = 46f;
            int maxShow = Mathf.Min(6, entries.Count);

            for (int i = 0; i < maxShow; i++)
            {
                var e = entries[i];
                Rect row = new Rect(area.x + padding, curY + i * (rowH + 4f), area.width - padding * 2, rowH);
                DrawRect(row, InkSoft);
                DrawBorder(row, GoldDim, 1f);

                Rect iconBox = new Rect(row.x + 6f, row.y + (row.height - 32f) * 0.5f, 32f, 32f);
                DrawRect(iconBox, new Color(0.02f, 0.015f, 0.01f, 0.8f));
                DrawBorder(iconBox, GoldDim, 1f);
                Sprite icon = PrototypePixelArtFactory.ItemIcon(e.itemId);
                if (icon != null) DrawSprite(icon, iconBox);

                string name = LocalizationRuntime.IsVietnamese ? e.nameVi : e.nameEn;
                string desc = LocalizationRuntime.IsVietnamese ? e.descVi : e.descEn;
                if (e.recordWeightGrams > 0)
                {
                    name += $" ★ (Kỷ lục: {e.recordWeightGrams / 1000f:F1}kg)";
                }

                GUI.Label(new Rect(iconBox.xMax + 10f, row.y + 4f, row.width - 60f, 18f), name, labelStyle);
                GUI.Label(new Rect(iconBox.xMax + 10f, row.y + 22f, row.width - 60f, 18f), desc, smallStyle);
            }
        }

        private void DrawBulletinBoardOverlay()
        {
            if (cachedBulletinBoard == null) cachedBulletinBoard = FindAnyObjectByType<DailyBulletinBoardController>();
            if (cachedBulletinBoard == null) return;

            float width = Mathf.Min(840f, Screen.width - 40f);
            float height = Mathf.Min(540f, Screen.height - 60f);
            Rect panel = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
            DrawCard(panel, Ink);
            DrawCornerAccents(panel, Gold);
            DrawHeaderStrip(new Rect(panel.x, panel.y, panel.width, 44));

            string title = LocalizationRuntime.IsVietnamese 
                ? "📜 BẢNG ĐƠN HÀNG THỊ TRẤN VALEN" 
                : "📜 VALEN TOWN DAILY ORDERS";
            GUI.Label(new Rect(panel.x + 18f, panel.y + 10f, 450f, 24f), title, titleStyle);

            if (GUI.Button(new Rect(panel.xMax - 44f, panel.y + 8f, 28f, 28f), "X"))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                cachedBulletinBoard.CloseBoard();
                overlayMode = OverlayMode.None;
            }

            string subTitle = LocalizationRuntime.IsVietnamese 
                ? "Giao nông sản, cá sông, trứng sữa và quặng sắt cho cư dân làng để nhận thưởng Đồng Bạc và quà giá trị!" 
                : "Deliver fresh crops, fish, dairy produce, and ores to earn bonus Silver Coins & valuable rewards!";
            GUI.Label(new Rect(panel.x + 18f, panel.y + 48f, panel.width - 36f, 20f), subTitle, smallStyle);

            var orders = cachedBulletinBoard.DailyOrders;
            float startY = panel.y + 76f;
            float rowH = (panel.height - 94f) / Mathf.Max(1, orders.Count);

            for (int i = 0; i < orders.Count; i++)
            {
                var order = orders[i];
                Rect rowRect = new Rect(panel.x + 18f, startY + i * rowH, panel.width - 36f, rowH - 6f);
                DrawRect(rowRect, order.isCompleted ? new Color(0.08f, 0.12f, 0.08f, 0.90f) : InkSoft);
                DrawBorder(rowRect, order.isCompleted ? Color.green : GoldDim, 1f);

                // Client & Task text
                string clientName = LocalizationRuntime.IsVietnamese ? order.clientNameVi : order.clientNameEn;
                string taskText = LocalizationRuntime.IsVietnamese ? order.taskVi : order.taskEn;
                GUI.Label(new Rect(rowRect.x + 12f, rowRect.y + 4f, 320f, 20f), $"<b>{clientName}</b>", labelStyle);
                GUI.Label(new Rect(rowRect.x + 12f, rowRect.y + 24f, 340f, rowRect.height - 28f), taskText, smallStyle);

                // Required Item Badge
                int owned = inventorySession != null && inventorySession.Runtime != null ? inventorySession.Runtime.GetQuantity(order.requiredItemId) : 0;
                bool hasEnough = owned >= order.requiredAmount;
                Rect reqBadge = new Rect(rowRect.x + 360f, rowRect.y + 6f, 150f, rowRect.height - 12f);
                DrawRect(reqBadge, new Color(0.02f, 0.015f, 0.01f, 0.8f));
                DrawBorder(reqBadge, hasEnough ? Color.green : GoldDim, 1f);
                Sprite reqIcon = PrototypePixelArtFactory.ItemIcon(order.requiredItemId);
                if (reqIcon != null) DrawSprite(reqIcon, new Rect(reqBadge.x + 4f, reqBadge.y + (reqBadge.height - 24f) * 0.5f, 24f, 24f));
                GUI.color = hasEnough ? Color.green : new Color(1f, 0.6f, 0.6f);
                GUI.Label(new Rect(reqBadge.x + 32f, reqBadge.y + 4f, reqBadge.width - 34f, reqBadge.height - 8f), 
                    $"{(LocalizationRuntime.IsVietnamese ? "Cần" : "Need")}: {order.requiredAmount}\n{(LocalizationRuntime.IsVietnamese ? "Có" : "Have")}: {owned}", smallStyle);
                GUI.color = Color.white;

                // Reward Badge
                Rect rewBadge = new Rect(reqBadge.xMax + 10f, rowRect.y + 6f, 140f, rowRect.height - 12f);
                DrawRect(rewBadge, new Color(0.045f, 0.038f, 0.030f, 0.90f));
                DrawBorder(rewBadge, GoldDim, 1f);
                DrawSprite(PrototypePixelArtFactory.SilverCoinIcon(), new Rect(rewBadge.x + 4f, rewBadge.y + (rewBadge.height - 20f) * 0.5f, 20f, 20f));
                GUI.Label(new Rect(rewBadge.x + 28f, rewBadge.y + 4f, rewBadge.width - 30f, rewBadge.height - 8f), 
                    $"+{order.rewardCoins} 🪙" + (!string.IsNullOrEmpty(order.rewardBonusItemId) ? $"\n+Bonus" : ""), labelStyle);

                // Deliver Button
                Rect btnRect = new Rect(rowRect.xMax - 110f, rowRect.y + (rowRect.height - 34f) * 0.5f, 100f, 34f);
                if (order.isCompleted)
                {
                    Color prev = GUI.color;
                    GUI.color = new Color(0.5f, 0.8f, 0.5f, 0.8f);
                    GUI.Box(btnRect, LocalizationRuntime.IsVietnamese ? "✓ Đã Giao" : "✓ Done", buttonStyle);
                    GUI.color = prev;
                }
                else
                {
                    Color prev = GUI.color;
                    GUI.color = hasEnough ? Color.white : new Color(0.6f, 0.6f, 0.6f, 1f);
                    if (GUI.Button(btnRect, LocalizationRuntime.T("deliver"), buttonStyle))
                    {
                        if (cachedBulletinBoard.TryDeliverOrder(i))
                        {
                            RefreshHudCache(true);
                        }
                    }
                    GUI.color = prev;
                }
            }
        }

        private void DrawMailboxOverlay()
        {
            if (cachedMailbox == null) cachedMailbox = FindAnyObjectByType<DailyMailboxController>();
            if (cachedMailbox == null) return;

            float width = Mathf.Min(820f, Screen.width - 40f);
            float height = Mathf.Min(520f, Screen.height - 60f);
            Rect panel = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
            DrawCard(panel, Ink);
            DrawCornerAccents(panel, Gold);
            DrawHeaderStrip(new Rect(panel.x, panel.y, panel.width, 44));

            string title = LocalizationRuntime.IsVietnamese 
                ? "📬 HÒM THƯ BỒ CÂU ĐỒNG QUÊ (QUÀ TẶNG ĐIỂM DANH)" 
                : "📬 COUNTRYSIDE DAILY GIFT MAILBOX";
            GUI.Label(new Rect(panel.x + 18f, panel.y + 10f, 480f, 24f), title, titleStyle);

            if (GUI.Button(new Rect(panel.xMax - 44f, panel.y + 8f, 28f, 28f), "X"))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                cachedMailbox.CloseMail();
                overlayMode = OverlayMode.None;
            }

            string sub = LocalizationRuntime.IsVietnamese
                ? $"Chuỗi Đăng Nhập: Ngày {cachedMailbox.CurrentStreakDay}/7 — Nhận quà mỗi ngày để phát triển nông trại nhanh chóng!"
                : $"Daily Streak: Day {cachedMailbox.CurrentStreakDay}/7 — Open gifts daily for rapid farm expansion!";
            GUI.Label(new Rect(panel.x + 18f, panel.y + 48f, panel.width - 36f, 20f), sub, smallStyle);

            var rewards = cachedMailbox.StreakRewards;
            float cardW = (panel.width - 36f - (rewards.Count - 1) * 6f) / Mathf.Max(1, rewards.Count);
            float cardH = 240f;
            float startX = panel.x + 18f;
            float startY = panel.y + 80f;

            for (int i = 0; i < rewards.Count; i++)
            {
                var rew = rewards[i];
                bool isToday = rew.dayNumber == cachedMailbox.CurrentStreakDay;
                bool isClaimed = rew.dayNumber < cachedMailbox.CurrentStreakDay || (isToday && !cachedMailbox.HasUnclaimedMail);

                Rect cardRect = new Rect(startX + i * (cardW + 6f), startY, cardW, cardH);
                DrawRect(cardRect, isToday ? new Color(0.25f, 0.18f, 0.08f, 0.96f) : (isClaimed ? new Color(0.06f, 0.08f, 0.06f, 0.90f) : InkSoft));
                DrawBorder(cardRect, isToday ? Gold : (isClaimed ? Color.green : GoldDim), isToday ? 2f : 1f);

                GUI.Label(new Rect(cardRect.x + 4f, cardRect.y + 8f, cardRect.width - 8f, 20f), 
                    $"{(LocalizationRuntime.IsVietnamese ? "Ngày" : "Day")} {rew.dayNumber}", isToday ? labelStyle : smallStyle);

                // Gift Icon
                Rect iconBox = new Rect(cardRect.x + (cardRect.width - 36f) * 0.5f, cardRect.y + 36f, 36f, 36f);
                DrawRect(iconBox, new Color(0.02f, 0.015f, 0.01f, 0.8f));
                DrawBorder(iconBox, GoldDim, 1f);
                Sprite icon = !string.IsNullOrEmpty(rew.rewardItemId) ? PrototypePixelArtFactory.ItemIcon(rew.rewardItemId) : PrototypePixelArtFactory.RewardChestIcon();
                if (icon != null) DrawSprite(icon, iconBox);

                // Reward details
                string rewTitle = LocalizationRuntime.IsVietnamese ? rew.titleVi : rew.titleEn;
                GUI.Label(new Rect(cardRect.x + 4f, cardRect.y + 80f, cardRect.width - 8f, 40f), rewTitle, smallStyle);
                GUI.Label(new Rect(cardRect.x + 4f, cardRect.y + 130f, cardRect.width - 8f, 24f), $"+{rew.rewardCoins} 🪙", labelStyle);

                if (!string.IsNullOrEmpty(rew.rewardItemId) && rew.rewardItemCount > 0)
                {
                    GUI.Label(new Rect(cardRect.x + 4f, cardRect.y + 158f, cardRect.width - 8f, 20f), $"+{rew.rewardItemCount}x", smallStyle);
                }

                // Claim status tag
                Rect statusRect = new Rect(cardRect.x + 4f, cardRect.yMax - 32f, cardRect.width - 8f, 24f);
                if (isClaimed)
                {
                    GUI.color = Color.green;
                    GUI.Label(statusRect, LocalizationRuntime.IsVietnamese ? "✓ Đã Nhận" : "✓ Claimed", centerStyle);
                    GUI.color = Color.white;
                }
                else if (isToday)
                {
                    GUI.color = Gold;
                    GUI.Label(statusRect, LocalizationRuntime.IsVietnamese ? "★ Hôm Nay" : "★ Today", centerStyle);
                    GUI.color = Color.white;
                }
            }

            // Big Claim Button at the bottom
            Rect claimBtnRect = new Rect(panel.x + (panel.width - 240f) * 0.5f, panel.yMax - 54f, 240f, 38f);
            if (cachedMailbox.HasUnclaimedMail)
            {
                if (GUI.Button(claimBtnRect, "🎁 " + LocalizationRuntime.T("claim_gift"), buttonStyle))
                {
                    if (cachedMailbox.TryClaimTodayReward())
                    {
                        RefreshHudCache(true);
                    }
                }
            }
            else
            {
                Color prev = GUI.color;
                GUI.color = new Color(0.6f, 0.6f, 0.6f, 0.8f);
                GUI.Box(claimBtnRect, LocalizationRuntime.IsVietnamese ? "✓ Đã Nhận Quà Hôm Nay" : "✓ Today's Gift Claimed", buttonStyle);
                GUI.color = prev;
            }
        }

        private void DrawGuideItemRow(Rect rect, string title, string description, string itemId)
        {
            DrawRect(rect, InkSoft);
            DrawBorder(rect, GoldDim, 1f);

            float textX = rect.x + 12f;
            if (!string.IsNullOrEmpty(itemId))
            {
                Rect iconRect = new Rect(rect.x + 8f, rect.y + (rect.height - 32f) * 0.5f, 32f, 32f);
                DrawRect(iconRect, new Color(0.02f, 0.015f, 0.01f, 0.8f));
                DrawBorder(iconRect, GoldDim, 1f);
                Sprite icon = PrototypePixelArtFactory.ItemIcon(itemId);
                if (icon != null) DrawSprite(icon, iconRect);
                textX = iconRect.xMax + 12f;
            }

            float textW = rect.xMax - textX - 10f;
            GUI.Label(new Rect(textX, rect.y + 4f, textW, 20f), title, labelStyle);
            GUI.Label(new Rect(textX, rect.y + 22f, textW, rect.height - 24f), description, smallStyle);
        }

        private enum OverlayMode
        {
            None,
            Inventory,
            BuildCatalog,
            Map,
            Journal,
            MerchantShop,
            Guide,
            BulletinBoard,
            Mailbox,
            SiloStorage,
            ChestStorage,
            ArtisanMachine,
            MarketStall
        }

        private Vector2 marketScrollPos;

        public void OpenMarketOverlay()
        {
            overlayMode = OverlayMode.MarketStall;
            RefreshHudCache(true);
            TheOldRoad.Audio.AudioManager.PlayUiClick();
        }

        public void OpenSiloOverlay()
        {
            overlayMode = OverlayMode.SiloStorage;
            RefreshHudCache(true);
            TheOldRoad.Audio.AudioManager.PlayUiClick();
        }

        public void OpenChestOverlay()
        {
            overlayMode = OverlayMode.ChestStorage;
            RefreshHudCache(true);
            TheOldRoad.Audio.AudioManager.PlayUiClick();
        }

        public void OpenArtisanOverlay()
        {
            overlayMode = OverlayMode.ArtisanMachine;
            RefreshHudCache(true);
            TheOldRoad.Audio.AudioManager.PlayUiClick();
        }

        private void DrawMarketStallOverlay()
        {
            var stall = TheOldRoad.Economy.MarketStallController.ActiveStall;
            if (stall == null && !TheOldRoad.Economy.MarketStallController.IsMarketOpen)
            {
                overlayMode = OverlayMode.None;
                return;
            }

            float width = Mathf.Min(840f, Screen.width - 40f);
            float height = Mathf.Min(560f, Screen.height - 60f);
            Rect panel = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);

            DrawCard(panel, Ink);
            DrawCornerAccents(panel, Gold);
            DrawHeaderStrip(new Rect(panel.x, panel.y, panel.width, 44));

            string title = LocalizationRuntime.IsVietnamese 
                ? "🛒 QUẦY NÔNG SẢN & GIAO HÀNG (SHIPPING MARKET)" 
                : "🛒 FARM PRODUCE & SHIPPING MARKET";
            GUI.Label(new Rect(panel.x + 18f, panel.y + 10f, 520f, 24f), title, titleStyle);

            if (GUI.Button(new Rect(panel.xMax - 44f, panel.y + 8f, 28f, 28f), "X"))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                TheOldRoad.Economy.MarketStallController.CloseMarket();
                overlayMode = OverlayMode.None;
            }

            InventoryRuntime inv = inventorySession != null ? inventorySession.Runtime : null;
            int silverCoins = inv != null ? inv.GetQuantity("item.silver-coin") : 0;

            string sub = LocalizationRuntime.IsVietnamese
                ? $"Ví Bạc Hiện Có:  <color=#FFD700><b>{silverCoins} 🪙</b></color>  — Bán nông sản, thực phẩm và hàng thủ công để kiếm Bạc!"
                : $"Wallet:  <color=#FFD700><b>{silverCoins} 🪙</b></color>  — Sell crops, fish, foods, and artisan goods for Silver!";
            GUI.Label(new Rect(panel.x + 18f, panel.y + 48f, panel.width - 36f, 20f), sub, smallStyle);

            var catalog = TheOldRoad.Economy.MarketStallController.SellCatalog;
            Rect scrollArea = new Rect(panel.x + 18f, panel.y + 78f, panel.width - 36f, panel.height - 96f);
            float rowH = 46f;
            float totalH = catalog.Count * rowH;

            marketScrollPos = GUI.BeginScrollView(scrollArea, marketScrollPos, new Rect(0, 0, scrollArea.width - 16f, totalH));

            for (int i = 0; i < catalog.Count; i++)
            {
                var entry = catalog[i];
                int ownedQty = inv != null ? inv.GetQuantity(entry.itemId) : 0;
                bool canSell = ownedQty > 0;

                Rect rowRect = new Rect(0, i * rowH, scrollArea.width - 16f, rowH - 4f);
                DrawRect(rowRect, canSell ? new Color(0.12f, 0.09f, 0.06f, 0.92f) : new Color(0.04f, 0.035f, 0.03f, 0.70f));
                DrawBorder(rowRect, canSell ? GoldDim : new Color(0.20f, 0.18f, 0.15f, 0.6f), 1f);

                // Icon Box
                Rect iconBox = new Rect(rowRect.x + 6f, rowRect.y + 5f, 32f, 32f);
                DrawRect(iconBox, new Color(0.02f, 0.015f, 0.01f, 0.8f));
                Sprite icon = PrototypePixelArtFactory.ItemIcon(entry.itemId);
                if (icon != null) DrawSprite(icon, iconBox);

                // Name & Price
                string itemName = LocalizationRuntime.ItemName(entry.itemId);
                GUI.Label(new Rect(rowRect.x + 46f, rowRect.y + 4f, 220f, 20f), itemName, canSell ? labelStyle : smallStyle);
                GUI.Label(new Rect(rowRect.x + 46f, rowRect.y + 22f, 220f, 16f), $"{(LocalizationRuntime.IsVietnamese ? "Giá bán:" : "Price:")} {entry.unitPrice} 🪙 / cái", smallStyle);

                // Owned count
                string ownedStr = LocalizationRuntime.IsVietnamese ? $"Có: <b>{ownedQty}</b>" : $"Owned: <b>{ownedQty}</b>";
                GUI.Label(new Rect(rowRect.x + 280f, rowRect.y + 11f, 120f, 20f), ownedStr, canSell ? labelStyle : smallStyle);

                // Sell 1 Button
                Rect sell1Btn = new Rect(rowRect.xMax - 180f, rowRect.y + 6f, 84f, 30f);
                Color prevC = GUI.color;
                GUI.color = canSell ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
                if (GUI.Button(sell1Btn, LocalizationRuntime.IsVietnamese ? "Bán 1" : "Sell 1", buttonStyle) && canSell)
                {
                    if (TheOldRoad.Economy.MarketStallController.TrySellItem(entry.itemId, 1, inv, out int earned))
                    {
                        RefreshHudCache(true);
                    }
                }

                // Sell All Button
                Rect sellAllBtn = new Rect(rowRect.xMax - 90f, rowRect.y + 6f, 84f, 30f);
                if (GUI.Button(sellAllBtn, LocalizationRuntime.IsVietnamese ? "Bán Hết" : "Sell All", buttonStyle) && canSell)
                {
                    if (TheOldRoad.Economy.MarketStallController.TrySellItem(entry.itemId, ownedQty, inv, out int earned))
                    {
                        RefreshHudCache(true);
                    }
                }
                GUI.color = prevC;
            }

            GUI.EndScrollView();
        }

        private void DrawSiloOverlay()
        {
            var silo = TheOldRoad.Building.SiloStorageController.ActiveSilo;
            if (silo == null)
            {
                overlayMode = OverlayMode.None;
                return;
            }

            float width = Mathf.Min(880f, Screen.width - 40f);
            float height = Mathf.Min(560f, Screen.height - 60f);
            Rect panel = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
            DrawCard(panel, Ink);
            DrawCornerAccents(panel, Gold);
            DrawHeaderStrip(new Rect(panel.x, panel.y, panel.width, 44));

            string title = LocalizationRuntime.IsVietnamese 
                ? "🌾 THÁP CHỨA HẠT & NÔNG SẢN (GRAIN SILO)" 
                : "🌾 GRAIN SILO & CROP VAULT";
            GUI.Label(new Rect(panel.x + 18f, panel.y + 10f, 500f, 24f), title, titleStyle);

            if (GUI.Button(new Rect(panel.xMax - 44f, panel.y + 8f, 28f, 28f), "X"))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                TheOldRoad.Building.SiloStorageController.CloseSiloUI();
                overlayMode = OverlayMode.None;
            }

            string subTitle = LocalizationRuntime.IsVietnamese 
                ? $"Kho chứa lương thực chuyên dụng. Tổng lưu trữ: {silo.TotalCount} nông sản & hạt giống." 
                : $"Dedicated granary storage. Total stored: {silo.TotalCount} crops & seeds.";
            GUI.Label(new Rect(panel.x + 18f, panel.y + 48f, panel.width - 300f, 20f), subTitle, smallStyle);

            Rect depositAllBtn = new Rect(panel.xMax - 260f, panel.y + 44f, 240f, 28f);
            if (GUI.Button(depositAllBtn, LocalizationRuntime.IsVietnamese ? "⬇️ GỬI TẤT CẢ NÔNG SẢN" : "⬇️ DEPOSIT ALL CROPS"))
            {
                if (inventorySession != null && inventorySession.Runtime != null)
                {
                    int count = silo.DepositAllProduce(inventorySession.Runtime);
                    if (count > 0)
                    {
                        ShowMessage(LocalizationRuntime.IsVietnamese ? $"🌾 Đã cất {count} nông sản vào Silo!" : $"🌾 Deposited {count} crops into Silo!");
                    }
                }
            }

            float splitW = (panel.width - 48f) * 0.55f;
            Rect leftPanel = new Rect(panel.x + 16f, panel.y + 78f, splitW, panel.height - 94f);
            DrawRect(leftPanel, InkSoft);
            DrawBorder(leftPanel, GoldDim, 1f);
            GUI.Label(new Rect(leftPanel.x + 12f, leftPanel.y + 8f, leftPanel.width - 24f, 20f), 
                LocalizationRuntime.IsVietnamese ? "<b>NÔNG SẢN & HẠT GIỐNG TRONG SILO</b>" : "<b>SILO STORED CROPS & SEEDS</b>", labelStyle);

            var stored = silo.GetStoredItems();
            var storedList = new List<KeyValuePair<string, int>>(stored);
            float itemY = leftPanel.y + 32f;
            float rowH = 42f;

            if (storedList.Count == 0)
            {
                GUI.Label(new Rect(leftPanel.x + 12f, leftPanel.y + 60f, leftPanel.width - 24f, 40f), 
                    LocalizationRuntime.IsVietnamese ? "Silo đang trống. Hãy gửi nông sản từ túi đồ bên phải." : "Silo is currently empty. Deposit crops from the right panel.", smallStyle);
            }
            else
            {
                for (int i = 0; i < storedList.Count && i < 10; i++)
                {
                    var kv = storedList[i];
                    Rect row = new Rect(leftPanel.x + 8f, itemY + i * (rowH + 4f), leftPanel.width - 16f, rowH);
                    DrawRect(row, new Color(0.04f, 0.035f, 0.03f, 0.85f));
                    DrawBorder(row, GoldDim, 1f);

                    Sprite icon = PrototypePixelArtFactory.ItemIcon(kv.Key);
                    if (icon != null) DrawSprite(icon, new Rect(row.x + 6f, row.y + 5f, 32f, 32f));

                    string name = LocalizationRuntime.ItemName(kv.Key);
                    GUI.Label(new Rect(row.x + 44f, row.y + 4f, 150f, 18f), name, labelStyle);
                    GUI.Label(new Rect(row.x + 44f, row.y + 22f, 150f, 16f), $"SL: {kv.Value}", smallStyle);

                    Rect take1 = new Rect(row.xMax - 116f, row.y + 6f, 54f, 30f);
                    if (GUI.Button(take1, "Lấy 1"))
                    {
                        if (inventorySession != null && inventorySession.Runtime != null)
                            silo.Withdraw(kv.Key, 1, inventorySession.Runtime);
                    }

                    Rect take10 = new Rect(row.xMax - 58f, row.y + 6f, 52f, 30f);
                    if (GUI.Button(take10, "Lấy 10"))
                    {
                        if (inventorySession != null && inventorySession.Runtime != null)
                            silo.Withdraw(kv.Key, 10, inventorySession.Runtime);
                    }
                }
            }

            float rightX = leftPanel.xMax + 12f;
            float rightW = panel.xMax - rightX - 16f;
            Rect rightPanel = new Rect(rightX, panel.y + 78f, rightW, panel.height - 94f);
            DrawRect(rightPanel, InkSoft);
            DrawBorder(rightPanel, GoldDim, 1f);
            GUI.Label(new Rect(rightPanel.x + 12f, rightPanel.y + 8f, rightPanel.width - 24f, 20f), 
                LocalizationRuntime.IsVietnamese ? "<b>TÚI ĐỒ NÔNG SẢN CỦA BẠN</b>" : "<b>YOUR BAG PRODUCE</b>", labelStyle);

            if (inventorySession != null && inventorySession.Runtime != null)
            {
                var invItems = new List<KeyValuePair<string, int>>();
                foreach (var kv in inventorySession.Runtime.Items)
                {
                    if (TheOldRoad.Building.SiloStorageController.IsCropOrSeed(kv.Key) && kv.Value.Quantity > 0)
                    {
                        invItems.Add(new KeyValuePair<string, int>(kv.Key, kv.Value.Quantity));
                    }
                }

                if (invItems.Count == 0)
                {
                    GUI.Label(new Rect(rightPanel.x + 12f, rightPanel.y + 60f, rightPanel.width - 24f, 40f), 
                        LocalizationRuntime.IsVietnamese ? "Không có nông sản/hạt giống nào trong túi đồ." : "No crops or seeds in your backpack.", smallStyle);
                }
                else
                {
                    for (int i = 0; i < invItems.Count && i < 10; i++)
                    {
                        var kv = invItems[i];
                        Rect row = new Rect(rightPanel.x + 8f, itemY + i * (rowH + 4f), rightPanel.width - 16f, rowH);
                        DrawRect(row, new Color(0.04f, 0.035f, 0.03f, 0.85f));
                        DrawBorder(row, GoldDim, 1f);

                        Sprite icon = PrototypePixelArtFactory.ItemIcon(kv.Key);
                        if (icon != null) DrawSprite(icon, new Rect(row.x + 6f, row.y + 5f, 32f, 32f));

                        string name = LocalizationRuntime.ItemName(kv.Key);
                        GUI.Label(new Rect(row.x + 44f, row.y + 4f, 130f, 18f), name, labelStyle);
                        GUI.Label(new Rect(row.x + 44f, row.y + 22f, 130f, 16f), $"Có: {kv.Value}", smallStyle);

                        Rect dep1 = new Rect(row.xMax - 110f, row.y + 6f, 50f, 30f);
                        if (GUI.Button(dep1, "Gửi 1"))
                        {
                            silo.Deposit(kv.Key, 1, inventorySession.Runtime);
                        }

                        Rect depAll = new Rect(row.xMax - 56f, row.y + 6f, 50f, 30f);
                        if (GUI.Button(depAll, "Hết"))
                        {
                            silo.Deposit(kv.Key, kv.Value, inventorySession.Runtime);
                        }
                    }
                }
            }
        }

        private void DrawChestOverlay()
        {
            var chest = TheOldRoad.Building.ChestStorageController.ActiveChest;
            if (chest == null)
            {
                overlayMode = OverlayMode.None;
                return;
            }

            float width = Mathf.Min(820f, Screen.width - 40f);
            float height = Mathf.Min(540f, Screen.height - 60f);
            Rect panel = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
            DrawCard(panel, Ink);
            DrawCornerAccents(panel, Gold);
            DrawHeaderStrip(new Rect(panel.x, panel.y, panel.width, 44));

            string title = LocalizationRuntime.IsVietnamese 
                ? $"📦 {chest.DisplayNameVi.ToUpperInvariant()} ({chest.Capacity} Ô CHỨA)" 
                : $"📦 {chest.DisplayNameEn.ToUpperInvariant()} ({chest.Capacity} SLOTS)";
            GUI.Label(new Rect(panel.x + 18f, panel.y + 10f, 450f, 24f), title, titleStyle);

            if (GUI.Button(new Rect(panel.xMax - 44f, panel.y + 8f, 28f, 28f), "X"))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                TheOldRoad.Building.ChestStorageController.CloseChestUI();
                overlayMode = OverlayMode.None;
            }

            Rect stackBtn = new Rect(panel.x + 18f, panel.y + 44f, 190f, 28f);
            if (GUI.Button(stackBtn, LocalizationRuntime.IsVietnamese ? "⚡ CẤT NHANH TRÙNG LẶP" : "⚡ QUICK STACK"))
            {
                if (inventorySession != null && inventorySession.Runtime != null)
                {
                    int stacked = chest.QuickStack(inventorySession.Runtime);
                    if (stacked > 0)
                        ShowMessage(LocalizationRuntime.IsVietnamese ? $"📦 Đã cất nhanh {stacked} vật phẩm!" : $"📦 Quick stacked {stacked} items!");
                }
            }

            Rect takeAllBtn = new Rect(stackBtn.xMax + 10f, panel.y + 44f, 160f, 28f);
            if (GUI.Button(takeAllBtn, LocalizationRuntime.IsVietnamese ? "📥 LẤY TẤT CẢ" : "📥 TAKE ALL"))
            {
                if (inventorySession != null && inventorySession.Runtime != null)
                {
                    int taken = chest.TakeAll(inventorySession.Runtime);
                    if (taken > 0)
                        ShowMessage(LocalizationRuntime.IsVietnamese ? $"📥 Đã lấy {taken} vật phẩm về túi!" : $"📥 Took {taken} items!");
                }
            }

            int cols = 8;
            int rows = Mathf.CeilToInt(chest.Capacity / (float)cols);
            float slotSize = 48f;
            float gridStartX = panel.x + 18f;
            float gridStartY = panel.y + 80f;

            for (int i = 0; i < chest.Capacity; i++)
            {
                int r = i / cols;
                int c = i % cols;
                Rect slotRect = new Rect(gridStartX + c * (slotSize + 8f), gridStartY + r * (slotSize + 8f), slotSize, slotSize);
                var slot = chest.GetSlot(i);

                DrawRect(slotRect, new Color(0.02f, 0.015f, 0.01f, 0.85f));
                DrawBorder(slotRect, (slot != null && !slot.IsEmpty) ? Gold : GoldDim, 1f);

                if (slot != null && !slot.IsEmpty)
                {
                    Sprite icon = PrototypePixelArtFactory.ItemIcon(slot.itemId);
                    if (icon != null) DrawSprite(icon, new Rect(slotRect.x + 4f, slotRect.y + 4f, slotSize - 8f, slotSize - 8f));
                    GUI.Label(new Rect(slotRect.x + 2f, slotRect.yMax - 18f, slotSize - 4f, 16f), slot.quantity.ToString(), smallStyle);

                    if (GUI.Button(slotRect, GUIContent.none, GUIStyle.none))
                    {
                        if (inventorySession != null && inventorySession.Runtime != null)
                        {
                            chest.WithdrawItem(i, 1, inventorySession.Runtime);
                        }
                    }
                }
            }

            float invY = gridStartY + rows * (slotSize + 8f) + 16f;
            DrawRect(new Rect(panel.x + 18f, invY, panel.width - 36f, 1f), GoldDim);
            GUI.Label(new Rect(panel.x + 18f, invY + 6f, 400f, 20f), 
                LocalizationRuntime.IsVietnamese ? "<b>BẤM VẬT PHẨM TRONG TÚI ĐỂ CẤT VÀO RƯƠNG:</b>" : "<b>CLICK BAG ITEM TO DEPOSIT:</b>", labelStyle);

            if (inventorySession != null && inventorySession.Runtime != null)
            {
                var invItems = new List<KeyValuePair<string, int>>();
                foreach (var kv in inventorySession.Runtime.Items)
                {
                    if (kv.Value.Quantity > 0)
                    {
                        invItems.Add(new KeyValuePair<string, int>(kv.Key, kv.Value.Quantity));
                    }
                }
                float invSlotX = panel.x + 18f;
                float invSlotY = invY + 28f;

                for (int i = 0; i < invItems.Count && i < 16; i++)
                {
                    var kv = invItems[i];
                    if (kv.Value <= 0) continue;

                    int c = i % 12;
                    int r = i / 12;
                    Rect bSlot = new Rect(invSlotX + c * (slotSize + 6f), invSlotY + r * (slotSize + 6f), slotSize, slotSize);
                    DrawRect(bSlot, InkSoft);
                    DrawBorder(bSlot, GoldDim, 1f);

                    Sprite icon = PrototypePixelArtFactory.ItemIcon(kv.Key);
                    if (icon != null) DrawSprite(icon, new Rect(bSlot.x + 4f, bSlot.y + 4f, slotSize - 8f, slotSize - 8f));
                    GUI.Label(new Rect(bSlot.x + 2f, bSlot.yMax - 18f, slotSize - 4f, 16f), kv.Value.ToString(), smallStyle);

                    if (GUI.Button(bSlot, GUIContent.none, GUIStyle.none))
                    {
                        for (int s = 0; s < chest.Capacity; s++)
                        {
                            var chSlot = chest.GetSlot(s);
                            if (chSlot.IsEmpty || chSlot.itemId == kv.Key)
                            {
                                chest.DepositItem(s, kv.Key, 1, inventorySession.Runtime);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void DrawArtisanOverlay()
        {
            var machine = TheOldRoad.Building.ArtisanProcessingController.ActiveMachine;
            if (machine == null)
            {
                overlayMode = OverlayMode.None;
                return;
            }

            float width = Mathf.Min(840f, Screen.width - 40f);
            float height = Mathf.Min(520f, Screen.height - 60f);
            Rect panel = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
            DrawCard(panel, Ink);
            DrawCornerAccents(panel, Gold);
            DrawHeaderStrip(new Rect(panel.x, panel.y, panel.width, 44));

            string title = LocalizationRuntime.IsVietnamese 
                ? $"🏭 {LocalizationRuntime.BuildingName(machine.BuildingId).ToUpperInvariant()}" 
                : $"🏭 {LocalizationRuntime.BuildingName(machine.BuildingId).ToUpperInvariant()}";
            GUI.Label(new Rect(panel.x + 18f, panel.y + 10f, 500f, 24f), title, titleStyle);

            if (GUI.Button(new Rect(panel.xMax - 44f, panel.y + 8f, 28f, 28f), "X"))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                TheOldRoad.Building.ArtisanProcessingController.CloseMachineUI();
                overlayMode = OverlayMode.None;
            }

            float topY = panel.y + 52f;
            if (machine.IsProcessing)
            {
                Rect procBox = new Rect(panel.x + 18f, topY, panel.width - 36f, 64f);
                DrawRect(procBox, new Color(0.08f, 0.12f, 0.18f, 0.92f));
                DrawBorder(procBox, new Color(0.35f, 0.85f, 1f, 1f), 1.5f);

                string procText = LocalizationRuntime.IsVietnamese 
                    ? $"⚙️ ĐANG CHẾ BIẾN... Còn {machine.RemainingSeconds:F1}s" 
                    : $"⚙️ PROCESSING... Remaining: {machine.RemainingSeconds:F1}s";
                GUI.Label(new Rect(procBox.x + 16f, procBox.y + 8f, 300f, 20f), procText, labelStyle);

                Rect barFrame = new Rect(procBox.x + 16f, procBox.y + 34f, procBox.width - 32f, 16f);
                DrawRect(barFrame, new Color(0.02f, 0.02f, 0.02f, 0.8f));
                DrawRect(new Rect(barFrame.x, barFrame.y, barFrame.width * machine.Progress, barFrame.height), new Color(0.35f, 0.85f, 1f, 1f));
                topY += 76f;
            }
            else if (machine.IsFinished)
            {
                Rect finBox = new Rect(panel.x + 18f, topY, panel.width - 36f, 64f);
                DrawRect(finBox, new Color(0.08f, 0.18f, 0.08f, 0.95f));
                DrawBorder(finBox, Color.green, 2f);

                string finText = LocalizationRuntime.IsVietnamese 
                    ? $"✨ ĐÃ HOÀN TẤT: +{machine.OutputQuantity} {LocalizationRuntime.ItemName(machine.OutputItemId)}" 
                    : $"✨ FINISHED: +{machine.OutputQuantity} {LocalizationRuntime.ItemName(machine.OutputItemId)}";
                GUI.Label(new Rect(finBox.x + 16f, finBox.y + 8f, 400f, 20f), finText, titleStyle);

                Rect collectBtn = new Rect(finBox.xMax - 220f, finBox.y + 14f, 200f, 36f);
                if (GUI.Button(collectBtn, LocalizationRuntime.IsVietnamese ? "🎁 NHẬN THÀNH PHẨM" : "🎁 COLLECT OUTPUT"))
                {
                    if (inventorySession != null && inventorySession.Runtime != null)
                    {
                        machine.CollectOutput(inventorySession.Runtime);
                    }
                }
                topY += 76f;
            }

            var recipes = machine.GetAvailableRecipes();
            float listH = panel.yMax - topY - 16f;
            Rect listRect = new Rect(panel.x + 18f, topY, panel.width - 36f, listH);
            DrawRect(listRect, InkSoft);
            DrawBorder(listRect, GoldDim, 1f);

            GUI.Label(new Rect(listRect.x + 12f, listRect.y + 6f, 300f, 20f), 
                LocalizationRuntime.IsVietnamese ? "<b>CÔNG THỨC CHẾ BIẾN CÓ SẴN</b>" : "<b>AVAILABLE RECIPES</b>", labelStyle);

            float rowY = listRect.y + 30f;
            float rowHeight = 52f;

            for (int i = 0; i < recipes.Count; i++)
            {
                var r = recipes[i];
                Rect row = new Rect(listRect.x + 8f, rowY + i * (rowHeight + 6f), listRect.width - 16f, rowHeight);
                DrawRect(row, new Color(0.035f, 0.03f, 0.025f, 0.85f));
                DrawBorder(row, GoldDim, 1f);

                Sprite inIcon1 = PrototypePixelArtFactory.ItemIcon(r.inputItemId);
                if (inIcon1 != null) DrawSprite(inIcon1, new Rect(row.x + 8f, row.y + 8f, 36f, 36f));
                int owned1 = inventorySession != null && inventorySession.Runtime != null ? inventorySession.Runtime.GetQuantity(r.inputItemId) : 0;
                bool has1 = owned1 >= r.inputQuantity;
                GUI.color = has1 ? Color.white : new Color(1f, 0.5f, 0.5f);
                GUI.Label(new Rect(row.x + 48f, row.y + 8f, 140f, 36f), $"{LocalizationRuntime.ItemName(r.inputItemId)}\n{owned1}/{r.inputQuantity}", smallStyle);
                GUI.color = Color.white;

                float arrowX = row.x + 200f;
                if (!string.IsNullOrEmpty(r.secondaryInputItemId) && r.secondaryInputQuantity > 0)
                {
                    Sprite inIcon2 = PrototypePixelArtFactory.ItemIcon(r.secondaryInputItemId);
                    if (inIcon2 != null) DrawSprite(inIcon2, new Rect(row.x + 200f, row.y + 8f, 36f, 36f));
                    int owned2 = inventorySession != null && inventorySession.Runtime != null ? inventorySession.Runtime.GetQuantity(r.secondaryInputItemId) : 0;
                    bool has2 = owned2 >= r.secondaryInputQuantity;
                    GUI.color = has2 ? Color.white : new Color(1f, 0.5f, 0.5f);
                    GUI.Label(new Rect(row.x + 240f, row.y + 8f, 130f, 36f), $"{LocalizationRuntime.ItemName(r.secondaryInputItemId)}\n{owned2}/{r.secondaryInputQuantity}", smallStyle);
                    GUI.color = Color.white;
                    arrowX = row.x + 380f;
                }

                GUI.Label(new Rect(arrowX, row.y + 16f, 30f, 20f), "➡️", labelStyle);

                Sprite outIcon = PrototypePixelArtFactory.ItemIcon(r.outputItemId);
                if (outIcon != null) DrawSprite(outIcon, new Rect(arrowX + 32f, row.y + 8f, 36f, 36f));
                GUI.Label(new Rect(arrowX + 72f, row.y + 8f, 150f, 36f), $"{LocalizationRuntime.ItemName(r.outputItemId)} (+{r.outputQuantity})\n⏳ {r.durationSeconds}s", smallStyle);

                bool canCraft = !machine.IsProcessing && !machine.IsFinished && machine.CanStartRecipe(r, inventorySession != null ? inventorySession.Runtime : null);
                Rect craftBtn = new Rect(row.xMax - 140f, row.y + 10f, 130f, 32f);

                GUI.enabled = canCraft;
                if (GUI.Button(craftBtn, LocalizationRuntime.IsVietnamese ? "BẮT ĐẦU" : "START"))
                {
                    if (inventorySession != null && inventorySession.Runtime != null)
                    {
                        machine.StartRecipe(r, inventorySession.Runtime);
                    }
                }
                GUI.enabled = true;
            }
        }

        public void ToggleInventoryOverlay()
        {
            ToggleOverlay(OverlayMode.Inventory);
        }

        public void ToggleMerchantOverlay()
        {
            ToggleOverlay(OverlayMode.MerchantShop);
        }

        public void ToggleGuideOverlay(int tab = 0)
        {
            guideTab = Mathf.Clamp(tab, 0, 5);
            ToggleOverlay(OverlayMode.Guide);
        }

        private void ToggleOverlay(OverlayMode mode)
        {
            overlayMode = overlayMode == mode ? OverlayMode.None : mode;
            if (overlayMode != OverlayMode.None)
            {
                RefreshHudCache(true);
                TheOldRoad.Audio.AudioManager.PlayUiClick();
            }
        }
    }
}
