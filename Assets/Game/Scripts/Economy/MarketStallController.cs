using System;
using System.Collections.Generic;
using UnityEngine;
using TheOldRoad.Inventory;
using TheOldRoad.Audio;
using TheOldRoad.UI;

namespace TheOldRoad.Economy
{
    [Serializable]
    public sealed class MarketSellableItem
    {
        public string itemId;
        public int unitPrice;
        public string category;

        public MarketSellableItem(string itemId, int unitPrice, string category)
        {
            this.itemId = itemId;
            this.unitPrice = unitPrice;
            this.category = category;
        }
    }

    /// <summary>
    /// Interactive controller for the Market Stall / Shipping Bin building.
    /// Allows selling crops, fish, animal products, and artisan goods for Silver Coins.
    /// </summary>
    public sealed class MarketStallController : MonoBehaviour
    {
        public static MarketStallController ActiveStall { get; private set; }
        public static bool IsMarketOpen { get; private set; }

        public static readonly List<MarketSellableItem> SellCatalog = new List<MarketSellableItem>
        {
            // Crops & Farm Produce
            new MarketSellableItem("item.wheat", 3, "Crops"),
            new MarketSellableItem("item.corn", 4, "Crops"),
            new MarketSellableItem("item.carrot", 4, "Crops"),
            new MarketSellableItem("item.potato", 5, "Crops"),
            new MarketSellableItem("item.tomato", 4, "Crops"),
            new MarketSellableItem("item.pineapple", 8, "Crops"),
            new MarketSellableItem("item.strawberry", 6, "Crops"),
            new MarketSellableItem("item.apple", 5, "Crops"),
            new MarketSellableItem("item.grape", 5, "Crops"),
            new MarketSellableItem("item.pumpkin", 12, "Crops"),

            // River Fish
            new MarketSellableItem("item.fish-carp", 6, "Fish"),
            new MarketSellableItem("item.fish-salmon", 10, "Fish"),
            new MarketSellableItem("item.fish-golden-perch", 25, "Fish"),
            new MarketSellableItem("item.cooked-fish", 14, "Food"),

            // Animal Products & Forage
            new MarketSellableItem("item.egg", 2, "Animal"),
            new MarketSellableItem("item.milk", 4, "Animal"),
            new MarketSellableItem("item.wool", 5, "Animal"),
            new MarketSellableItem("item.wild-berries", 2, "Forage"),
            new MarketSellableItem("item.medicinal-herb", 4, "Forage"),
            new MarketSellableItem("item.mushroom", 3, "Forage"),

            // Artisan Products
            new MarketSellableItem("item.cheese", 15, "Artisan"),
            new MarketSellableItem("item.cloth", 14, "Artisan"),
            new MarketSellableItem("item.wine-fruit", 16, "Artisan"),
            new MarketSellableItem("item.juice", 10, "Artisan"),
            new MarketSellableItem("item.flour", 6, "Artisan"),
            new MarketSellableItem("item.iron-bar", 8, "Artisan"),
            new MarketSellableItem("item.armor-knight", 30, "Artisan")
        };

        public static void OpenMarket(MarketStallController stall)
        {
            ActiveStall = stall;
            IsMarketOpen = true;
            AudioManager.PlayMerchantBell();
        }

        public static void CloseMarket()
        {
            IsMarketOpen = false;
            ActiveStall = null;
        }

        public static bool TrySellItem(string itemId, int quantity, InventoryRuntime playerInventory, out int earnedSilver)
        {
            earnedSilver = 0;
            if (string.IsNullOrEmpty(itemId) || quantity <= 0 || playerInventory == null) return false;

            MarketSellableItem entry = SellCatalog.Find(e => string.Equals(e.itemId, itemId, StringComparison.OrdinalIgnoreCase));
            if (entry == null) return false;

            int available = playerInventory.GetQuantity(itemId);
            int sellCount = Mathf.Min(available, quantity);
            if (sellCount <= 0) return false;

            if (playerInventory.TryRemove(itemId, sellCount))
            {
                earnedSilver = sellCount * entry.unitPrice;
                playerInventory.TryAdd("item.silver-coin", earnedSilver);
                AudioManager.PlayMerchantBell();
                return true;
            }

            return false;
        }
    }
}
