using System;
using System.Collections.Generic;
using UnityEngine;
using TheOldRoad.Audio;
using TheOldRoad.Inventory;
using TheOldRoad.Player;
using TheOldRoad.UI;

namespace TheOldRoad.World
{
    /// <summary>
    /// Interactive storage chest inside buildings for depositing and withdrawing resources.
    /// </summary>
    public sealed class HomeStorageChest : MonoBehaviour
    {
        private static readonly Dictionary<string, int> StoredItems = new Dictionary<string, int>();
        private static bool isChestOpen = false;

        private CabinInteriorController interior;
        private InventorySession inventorySession;

        public static bool IsChestOpen => isChestOpen;
        public static void CloseChest() => isChestOpen = false;

        public static int GetStoredQuantity(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return 0;
            return StoredItems.TryGetValue(itemId, out int qty) ? qty : 0;
        }

        public static void Deposit(string itemId, int count, InventoryRuntime playerInventory)
        {
            if (string.IsNullOrEmpty(itemId) || count <= 0 || playerInventory == null) return;

            int available = playerInventory.GetQuantity(itemId);
            int transfer = Mathf.Min(available, count);
            if (transfer <= 0) return;

            playerInventory.TryRemove(itemId, transfer);
            int current = GetStoredQuantity(itemId);
            StoredItems[itemId] = current + transfer;
            AudioManager.PlayChestOpen();
        }

        public static void Withdraw(string itemId, int count, InventoryRuntime playerInventory)
        {
            if (string.IsNullOrEmpty(itemId) || count <= 0 || playerInventory == null) return;

            int stored = GetStoredQuantity(itemId);
            int transfer = Mathf.Min(stored, count);
            if (transfer <= 0) return;

            StoredItems[itemId] = stored - transfer;
            if (StoredItems[itemId] <= 0) StoredItems.Remove(itemId);

            playerInventory.Add(itemId, transfer);
            AudioManager.PlayChestOpen();
        }

        private void Awake()
        {
            interior = FindAnyObjectByType<CabinInteriorController>();
            inventorySession = FindAnyObjectByType<InventorySession>();
        }

        private void Update()
        {
            if (interior == null) interior = FindAnyObjectByType<CabinInteriorController>();
            if (inventorySession == null) inventorySession = FindAnyObjectByType<InventorySession>();

            PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
            if (player == null || interior == null || !interior.IsInside)
            {
                isChestOpen = false;
                return;
            }

            if (interior.IsNearChest(player.transform))
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.F) || UnityEngine.Input.GetKeyDown(KeyCode.E))
                {
                    isChestOpen = !isChestOpen;
                    AudioManager.PlayChestOpen();
                }
            }
            else
            {
                isChestOpen = false;
            }

            if (isChestOpen && UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                isChestOpen = false;
            }
        }

        private void OnGUI()
        {
            if (!isChestOpen || interior == null || !interior.IsInside || inventorySession == null) return;

            InventoryRuntime playerInv = inventorySession.Runtime;
            if (playerInv == null) return;

            const float width = 560f;
            const float height = 440f;
            Rect rect = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);

            Color prev = GUI.color;
            GUI.color = new Color(0.04f, 0.035f, 0.028f, 0.95f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = new Color(0.78f, 0.62f, 0.32f, 1f);
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 3f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - 3f, rect.width, 3f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, 3f, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - 3f, rect.y, 3f, rect.height), Texture2D.whiteTexture);
            GUI.color = prev;

            UiFontHelper.EnsureGlobalSkinFont();
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                font = UiFontHelper.CleanFont,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.95f, 0.85f, 0.6f, 1f) }
            };

            GUIStyle sectionStyle = new GUIStyle(GUI.skin.label)
            {
                font = UiFontHelper.CleanFont,
                alignment = TextAnchor.MiddleLeft,
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.90f, 0.78f, 0.45f, 1f) }
            };

            GUIStyle itemStyle = new GUIStyle(GUI.skin.label)
            {
                font = UiFontHelper.CleanFont,
                alignment = TextAnchor.MiddleLeft,
                fontSize = 13,
                normal = { textColor = new Color(0.92f, 0.92f, 0.88f, 1f) }
            };

            string title = LocalizationRuntime.IsVietnamese ? "RƯƠNG CẤT ĐỒ TRONG NHÀ" : "HOME STORAGE CHEST";
            GUI.Label(new Rect(rect.x + 20f, rect.y + 16f, rect.width - 40f, 28f), title, titleStyle);

            // Left column: Player Backpack (Deposit)
            float colWidth = (rect.width - 60f) * 0.5f;
            Rect playerCol = new Rect(rect.x + 20f, rect.y + 54f, colWidth, rect.height - 110f);
            GUI.Label(new Rect(playerCol.x, playerCol.y, playerCol.width, 22f), LocalizationRuntime.IsVietnamese ? "Túi đồ của bạn (Bấm để cất)" : "Backpack (Click to Deposit)", sectionStyle);

            float curY = playerCol.y + 28f;
            string[] depositItemIds = { "item.wood", "item.stone", "item.cabin-plank", "item.iron-ore", "item.wild-berries", "item.mushroom", "item.medicinal-herb", "item.wool", "item.egg" };

            foreach (string itemId in depositItemIds)
            {
                int count = playerInv.GetQuantity(itemId);
                if (count <= 0) continue;

                string name = LocalizationRuntime.ItemName(itemId);
                if (GUI.Button(new Rect(playerCol.x, curY, playerCol.width, 26f), $"{name} ({count}) -> Cất 5"))
                {
                    Deposit(itemId, Mathf.Min(5, count), playerInv);
                }
                curY += 30f;
                if (curY > playerCol.yMax - 30f) break;
            }

            // Right column: Stored in Chest (Withdraw)
            Rect chestCol = new Rect(rect.x + 40f + colWidth, rect.y + 54f, colWidth, rect.height - 110f);
            GUI.Label(new Rect(chestCol.x, chestCol.y, chestCol.width, 22f), LocalizationRuntime.IsVietnamese ? "Trong rương (Bấm để lấy)" : "Chest Storage (Click to Withdraw)", sectionStyle);

            curY = chestCol.y + 28f;
            if (StoredItems.Count == 0)
            {
                GUI.Label(new Rect(chestCol.x, curY, chestCol.width, 40f), LocalizationRuntime.IsVietnamese ? "(Rương đang trống)" : "(Chest is empty)", itemStyle);
            }
            else
            {
                List<string> keys = new List<string>(StoredItems.Keys);
                foreach (string itemId in keys)
                {
                    int storedCount = StoredItems[itemId];
                    if (storedCount <= 0) continue;

                    string name = LocalizationRuntime.ItemName(itemId);
                    if (GUI.Button(new Rect(chestCol.x, curY, chestCol.width, 26f), $"<- Lấy 5 ({name}: {storedCount})"))
                    {
                        Withdraw(itemId, Mathf.Min(5, storedCount), playerInv);
                    }
                    curY += 30f;
                    if (curY > chestCol.yMax - 30f) break;
                }
            }

            if (GUI.Button(new Rect(rect.x + (rect.width - 160f) * 0.5f, rect.yMax - 46f, 160f, 32f), LocalizationRuntime.T("close") + " (Esc)"))
            {
                isChestOpen = false;
                AudioManager.PlayUiClick();
            }
        }
    }
}
