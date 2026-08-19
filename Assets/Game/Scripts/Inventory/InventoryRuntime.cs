using System.Collections.Generic;
using System.Linq;
using TheOldRoad.Building;
using TheOldRoad.Save;

namespace TheOldRoad.Inventory
{
    /// <summary>Small domain inventory API with no UI or scene dependencies.</summary>
    public sealed class InventoryRuntime
    {
        private readonly Dictionary<string, InventoryItem> items = new();

        public IReadOnlyDictionary<string, InventoryItem> Items => items;

        public int GetQuantity(string itemId)
        {
            return items.TryGetValue(itemId, out InventoryItem item) ? item.Quantity : 0;
        }

        public void Add(string itemId, int quantity)
        {
            if (string.IsNullOrWhiteSpace(itemId)) throw new System.ArgumentException("Item ID is required.", nameof(itemId));
            if (quantity <= 0) throw new System.ArgumentOutOfRangeException(nameof(quantity));

            if (items.TryGetValue(itemId, out InventoryItem item)) item.Add(quantity);
            else items.Add(itemId, new InventoryItem(itemId, quantity));
        }

        public bool TryAdd(string itemId, int quantity)
        {
            if (string.IsNullOrWhiteSpace(itemId) || quantity <= 0) return false;
            Add(itemId, quantity);
            return true;
        }

        public bool TryRemove(string itemId, int quantity)
        {
            if (!items.TryGetValue(itemId, out InventoryItem item) || !item.Remove(quantity)) return false;
            if (item.Quantity == 0) items.Remove(itemId);
            return true;
        }

        public bool Has(string itemId, int quantity) => GetQuantity(itemId) >= quantity;

        public bool HasAll(IEnumerable<BuildCostEntry> costs)
        {
            foreach (BuildCostEntry cost in costs)
            {
                if (!Has(cost.itemId, cost.quantity)) return false;
            }

            return true;
        }

        public bool TryRemoveAll(IEnumerable<BuildCostEntry> costs)
        {
            BuildCostEntry[] costArray = costs.ToArray();
            if (!HasAll(costArray)) return false;

            foreach (BuildCostEntry cost in costArray)
            {
                TryRemove(cost.itemId, cost.quantity);
            }

            return true;
        }

        public bool HasAll((string itemId, int quantity)[] costs)
        {
            foreach ((string itemId, int quantity) cost in costs)
            {
                if (!Has(cost.itemId, cost.quantity)) return false;
            }

            return true;
        }

        public bool TryRemoveAll((string itemId, int quantity)[] costs)
        {
            if (!HasAll(costs)) return false;

            foreach ((string itemId, int quantity) cost in costs)
            {
                TryRemove(cost.itemId, cost.quantity);
            }

            return true;
        }

        public InventorySaveEntry[] ToSaveEntries()
        {
            return items.Values
                .Select(item => new InventorySaveEntry { itemId = item.ItemId, quantity = item.Quantity })
                .OrderBy(entry => entry.itemId)
                .ToArray();
        }

        public void LoadFromSaveEntries(IEnumerable<InventorySaveEntry> saveEntries)
        {
            items.Clear();
            if (saveEntries == null) return;

            foreach (InventorySaveEntry entry in saveEntries)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.itemId) || entry.quantity <= 0) continue;
                items[entry.itemId] = new InventoryItem(entry.itemId, entry.quantity);
            }
        }
    }
}
