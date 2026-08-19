using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TheOldRoad.Inventory;
using TheOldRoad.Save;
using TheOldRoad.Audio;
using TheOldRoad.UI;

namespace TheOldRoad.Building
{
    /// <summary>
    /// Dedicated high-capacity granary storage for crops, vegetables, and seeds.
    /// Capacity: 999 per unique crop item, with bulk deposit and withdraw operations.
    /// </summary>
    public sealed class SiloStorageController : MonoBehaviour
    {
        public static SiloStorageController ActiveSilo { get; private set; }

        [SerializeField] private string siloId = string.Empty;
        private readonly Dictionary<string, int> storedItems = new Dictionary<string, int>();

        public static readonly HashSet<string> AllowedCropItemIds = new HashSet<string>
        {
            "item.seed-wheat",
            "item.seed-corn",
            "item.seed-carrot",
            "item.seed-potato",
            "item.seed-pineapple",
            "item.seed-tomato",
            "item.wheat",
            "item.corn",
            "item.carrot",
            "item.potato",
            "item.pineapple",
            "item.tomato",
            "item.wild-berries",
            "item.grape",
            "item.flour",
            "item.hay"
        };

        public string SiloId => siloId;
        public int TotalCount => storedItems.Values.Sum();

        public static bool IsCropOrSeed(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return false;
            return AllowedCropItemIds.Contains(itemId) || itemId.StartsWith("item.seed-");
        }

        private void Awake()
        {
            if (string.IsNullOrEmpty(siloId))
            {
                siloId = "silo." + Guid.NewGuid().ToString("N").Substring(0, 8);
            }
        }

        public void Configure(string id)
        {
            if (!string.IsNullOrEmpty(id)) siloId = id;
        }

        public int GetQuantity(string itemId)
        {
            return storedItems.TryGetValue(itemId, out int count) ? count : 0;
        }

        public bool Deposit(string itemId, int quantity, InventoryRuntime playerInventory)
        {
            if (string.IsNullOrEmpty(itemId) || quantity <= 0 || playerInventory == null) return false;
            if (!IsCropOrSeed(itemId)) return false;

            int available = playerInventory.GetQuantity(itemId);
            int toTransfer = Mathf.Min(available, quantity);
            if (toTransfer <= 0) return false;

            if (playerInventory.TryRemove(itemId, toTransfer))
            {
                if (storedItems.ContainsKey(itemId))
                    storedItems[itemId] += toTransfer;
                else
                    storedItems[itemId] = toTransfer;

                AudioManager.PlayUiClick();
                return true;
            }
            return false;
        }

        public int DepositAllProduce(InventoryRuntime playerInventory)
        {
            if (playerInventory == null) return 0;
            int totalDeposited = 0;

            var itemsToDeposit = new List<string>();
            foreach (var kv in playerInventory.Items)
            {
                if (IsCropOrSeed(kv.Key) && kv.Value.Quantity > 0)
                {
                    itemsToDeposit.Add(kv.Key);
                }
            }

            foreach (string itemId in itemsToDeposit)
            {
                int qty = playerInventory.GetQuantity(itemId);
                if (qty > 0 && playerInventory.TryRemove(itemId, qty))
                {
                    if (storedItems.ContainsKey(itemId))
                        storedItems[itemId] += qty;
                    else
                        storedItems[itemId] = qty;

                    totalDeposited += qty;
                }
            }

            if (totalDeposited > 0)
            {
                AudioManager.PlayUiClick();
            }
            return totalDeposited;
        }

        public bool Withdraw(string itemId, int quantity, InventoryRuntime playerInventory)
        {
            if (string.IsNullOrEmpty(itemId) || quantity <= 0 || playerInventory == null) return false;
            if (!storedItems.TryGetValue(itemId, out int current) || current <= 0) return false;

            int toWithdraw = Mathf.Min(current, quantity);
            if (playerInventory.TryAdd(itemId, toWithdraw))
            {
                storedItems[itemId] -= toWithdraw;
                if (storedItems[itemId] <= 0)
                {
                    storedItems.Remove(itemId);
                }
                AudioManager.PlayUiClick();
                return true;
            }
            return false;
        }

        public Dictionary<string, int> GetStoredItems()
        {
            return new Dictionary<string, int>(storedItems);
        }

        public SiloSaveEntry Save()
        {
            var entries = new List<InventorySaveEntry>();
            foreach (var kv in storedItems)
            {
                if (kv.Value > 0)
                {
                    entries.Add(new InventorySaveEntry { itemId = kv.Key, quantity = kv.Value });
                }
            }
            return new SiloSaveEntry
            {
                siloId = siloId,
                storedItems = entries.ToArray()
            };
        }

        public void Load(SiloSaveEntry save)
        {
            storedItems.Clear();
            if (save == null || save.storedItems == null) return;
            if (!string.IsNullOrEmpty(save.siloId)) siloId = save.siloId;

            foreach (var entry in save.storedItems)
            {
                if (entry != null && !string.IsNullOrEmpty(entry.itemId) && entry.quantity > 0)
                {
                    storedItems[entry.itemId] = entry.quantity;
                }
            }
        }

        public static void OpenSiloUI(SiloStorageController silo)
        {
            ActiveSilo = silo;
        }

        public static void CloseSiloUI()
        {
            ActiveSilo = null;
        }
    }
}
