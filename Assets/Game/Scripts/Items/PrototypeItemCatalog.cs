using System;
using UnityEngine;

namespace TheOldRoad.Items
{
    /// <summary>Small static item catalog for the runtime prototype until authored item assets are wired in.</summary>
    public static class PrototypeItemCatalog
    {
        private static readonly PrototypeItemInfo[] Items =
        {
            new PrototypeItemInfo("item.wood", "Wood", "W", "Build material", new Color(0.47f, 0.29f, 0.12f, 1f)),
            new PrototypeItemInfo("item.stone", "Stone", "S", "Foundation material", new Color(0.45f, 0.48f, 0.52f, 1f)),
            new PrototypeItemInfo("item.cabin-plank", "Cabin Plank", "P", "Crafted component", new Color(0.74f, 0.50f, 0.25f, 1f)),
            new PrototypeItemInfo("item.wild-berries", "Wild Berries", "B", "Foraged food", new Color(0.72f, 0.12f, 0.20f, 1f)),
            new PrototypeItemInfo("item.medicinal-herb", "Medicinal Herb", "H", "Apothecary material", new Color(0.30f, 0.72f, 0.30f, 1f)),
            new PrototypeItemInfo("item.mushroom", "Mushroom", "M", "Forest ingredient", new Color(0.75f, 0.56f, 0.38f, 1f)),
            new PrototypeItemInfo("item.iron-ore", "Iron Ore", "I", "Smithing material", new Color(0.38f, 0.42f, 0.48f, 1f)),
            new PrototypeItemInfo("item.old-coin", "Old Coin", "C", "Ancient currency", new Color(0.95f, 0.74f, 0.30f, 1f)),
            new PrototypeItemInfo("item.torch", "Torch", "T", "Portable light source", new Color(1f, 0.58f, 0.18f, 1f)),
            new PrototypeItemInfo("item.cooked-meal", "Cooked Meal", "F", "Cooked food; restores health in the prototype", new Color(0.92f, 0.55f, 0.22f, 1f)),
            new PrototypeItemInfo("item.egg", "Egg", "E", "Prototype animal product", new Color(0.96f, 0.88f, 0.62f, 1f)),
            new PrototypeItemInfo("item.wool", "Wool", "W", "Prototype animal product", new Color(0.86f, 0.84f, 0.76f, 1f)),
            new PrototypeItemInfo("item.milk", "Milk", "M", "Prototype animal product", new Color(0.88f, 0.94f, 0.96f, 1f))
        };

        public static PrototypeItemInfo[] All => Items;

        public static PrototypeItemInfo Get(string itemId)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (string.Equals(Items[i].ItemId, itemId, StringComparison.Ordinal)) return Items[i];
            }

            return new PrototypeItemInfo(itemId, itemId, "?", "Prototype item", Color.gray);
        }
    }

    public readonly struct PrototypeItemInfo
    {
        public PrototypeItemInfo(string itemId, string displayName, string icon, string useText, Color color)
        {
            ItemId = itemId;
            DisplayName = displayName;
            Icon = icon;
            UseText = useText;
            Color = color;
        }

        public string ItemId { get; }
        public string DisplayName { get; }
        public string Icon { get; }
        public string UseText { get; }
        public Color Color { get; }
    }
}
