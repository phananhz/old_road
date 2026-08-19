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
            new PrototypeItemInfo("item.tool-axe", "Worn Axe", "A", "Basic chopping tool; unlocks faster roadwork progression", new Color(0.64f, 0.70f, 0.74f, 1f)),
            new PrototypeItemInfo("item.tool-pickaxe", "Stone Pick", "K", "Basic mining tool; required for iron veins", new Color(0.56f, 0.60f, 0.62f, 1f)),
            new PrototypeItemInfo("item.roadwarden-page", "Journal Page", "J", "A torn page from your father's Roadwarden journal", new Color(0.82f, 0.72f, 0.50f, 1f)),
            new PrototypeItemInfo("item.bell-fragment", "Bell Fragment", "G", "A silent shard from the old bell network", new Color(0.70f, 0.80f, 0.96f, 1f)),
            new PrototypeItemInfo("item.cooked-meal", "Cooked Meal", "F", "Cooked food; restores health in the prototype", new Color(0.92f, 0.55f, 0.22f, 1f)),
            new PrototypeItemInfo("item.egg", "Egg", "E", "Prototype animal product", new Color(0.96f, 0.88f, 0.62f, 1f)),
            new PrototypeItemInfo("item.wool", "Wool", "W", "Prototype animal product", new Color(0.86f, 0.84f, 0.76f, 1f)),
            new PrototypeItemInfo("item.milk", "Milk", "M", "Prototype animal product", new Color(0.88f, 0.94f, 0.96f, 1f)),
            new PrototypeItemInfo("item.silver-coin", "Silver Coin", "🪙", "Valen silver currency", new Color(0.92f, 0.94f, 0.98f, 1f)),
            new PrototypeItemInfo("item.tool-hoe", "Worn Hoe", "⛏", "Till soil into fertile farm plots", new Color(0.60f, 0.52f, 0.42f, 1f)),
            new PrototypeItemInfo("item.watering-can", "Watering Can", "💧", "Water tilled crops or collect river water", new Color(0.35f, 0.65f, 0.95f, 1f)),
            new PrototypeItemInfo("item.seed-wheat", "Wheat Seeds", "🌱", "Plant in tilled soil", new Color(0.85f, 0.78f, 0.35f, 1f)),
            new PrototypeItemInfo("item.seed-corn", "Corn Seeds", "🌱", "Plant in tilled soil", new Color(0.95f, 0.85f, 0.25f, 1f)),
            new PrototypeItemInfo("item.seed-carrot", "Carrot Seeds", "🌱", "Plant in tilled soil", new Color(0.95f, 0.50f, 0.20f, 1f)),
            new PrototypeItemInfo("item.seed-potato", "Potato Seeds", "🌱", "Plant in tilled soil", new Color(0.78f, 0.68f, 0.54f, 1f)),
            new PrototypeItemInfo("item.wheat", "Golden Wheat", "🌾", "Freshly harvested crop", new Color(0.92f, 0.82f, 0.38f, 1f)),
            new PrototypeItemInfo("item.corn", "Sweet Corn", "🌽", "Freshly harvested crop", new Color(0.98f, 0.88f, 0.28f, 1f)),
            new PrototypeItemInfo("item.carrot", "Crisp Carrot", "🥕", "Freshly harvested crop", new Color(0.98f, 0.48f, 0.15f, 1f)),
            new PrototypeItemInfo("item.potato", "Golden Potato", "🥔", "Freshly harvested crop", new Color(0.84f, 0.70f, 0.48f, 1f)),
            new PrototypeItemInfo("item.fence-wood", "Wood Fence", "🪵", "Enclose property or animals", new Color(0.55f, 0.38f, 0.22f, 1f)),
            new PrototypeItemInfo("item.gate-wood", "Wood Gate", "🚪", "Openable gate for fences", new Color(0.62f, 0.42f, 0.24f, 1f)),
            new PrototypeItemInfo("item.seed-pineapple", "Pineapple Seeds", "🍍", "Plant in tilled soil", new Color(0.95f, 0.78f, 0.22f, 1f)),
            new PrototypeItemInfo("item.seed-tomato", "Tomato Seeds", "🍅", "Plant in tilled soil", new Color(0.92f, 0.25f, 0.20f, 1f)),
            new PrototypeItemInfo("item.pineapple", "Sweet Pineapple", "🍍", "Juicy golden pineapple", new Color(0.96f, 0.82f, 0.20f, 1f)),
            new PrototypeItemInfo("item.tomato", "Ripe Tomato", "🍅", "Fresh farm tomato", new Color(0.92f, 0.22f, 0.18f, 1f)),
            new PrototypeItemInfo("item.fishing-rod", "Bamboo Rod", "🎣", "Cast into the river to catch fish", new Color(0.72f, 0.58f, 0.32f, 1f)),
            new PrototypeItemInfo("item.fishing-bait", "Earthworm Bait", "🪱", "Essential bait for river fishing", new Color(0.82f, 0.45f, 0.35f, 1f)),
            new PrototypeItemInfo("item.fish-salmon", "River Salmon", "🐟", "Fresh salmon caught from Valen river", new Color(0.95f, 0.42f, 0.35f, 1f)),
            new PrototypeItemInfo("item.fish-carp", "Common Carp", "🐟", "Fresh carp caught from Valen river", new Color(0.45f, 0.65f, 0.42f, 1f)),
            new PrototypeItemInfo("item.fish-golden-perch", "Golden Perch", "🐠", "Rare prized river fish", new Color(0.96f, 0.82f, 0.22f, 1f)),
            new PrototypeItemInfo("item.cooked-fish", "Grilled Fish", "🍲", "Steaming hot herb grilled fish", new Color(0.94f, 0.62f, 0.28f, 1f)),
            new PrototypeItemInfo("item.weapon-sword", "Iron Longsword", "⚔️", "Sharp blade for heavy slashing", new Color(0.82f, 0.88f, 0.96f, 1f)),
            new PrototypeItemInfo("item.weapon-bow", "Hunter's Bow", "🏹", "Ranged bow for shooting arrows", new Color(0.65f, 0.42f, 0.20f, 1f)),
            new PrototypeItemInfo("item.ammo-arrow", "Flint Arrow", "🎯", "Ammunition for bows", new Color(0.68f, 0.70f, 0.74f, 1f)),
            new PrototypeItemInfo("item.shield-wood", "Round Shield", "🛡️", "Blocks incoming attacks", new Color(0.55f, 0.38f, 0.22f, 1f)),
            new PrototypeItemInfo("item.fertilizer", "Organic Fertilizer", "🧪", "Accelerates crop growth by 25%", new Color(0.40f, 0.72f, 0.35f, 1f)),
            new PrototypeItemInfo("item.seed-strawberry", "Strawberry Seeds", "🍓", "Plant in tilled soil", new Color(0.95f, 0.28f, 0.38f, 1f)),
            new PrototypeItemInfo("item.seed-pumpkin", "Giant Pumpkin Seeds", "🎃", "Plant in tilled soil", new Color(0.96f, 0.58f, 0.15f, 1f)),
            new PrototypeItemInfo("item.sapling-apple", "Apple Tree Sapling", "🌳", "Plant in tilled soil", new Color(0.38f, 0.78f, 0.28f, 1f)),
            new PrototypeItemInfo("item.strawberry", "Wild Strawberry", "🍓", "Sweet juicy red berry", new Color(0.96f, 0.18f, 0.28f, 1f)),
            new PrototypeItemInfo("item.pumpkin", "Giant Pumpkin", "🎃", "Prized autumn giant crop", new Color(0.96f, 0.52f, 0.10f, 1f)),
            new PrototypeItemInfo("item.apple", "Crisp Red Apple", "🍎", "Sweet orchard apple", new Color(0.92f, 0.18f, 0.18f, 1f)),
            new PrototypeItemInfo("item.cheese", "Aged Cheese", "🧀", "Rich artisan dairy wheel", new Color(0.98f, 0.85f, 0.30f, 1f)),
            new PrototypeItemInfo("item.cloth", "Woven Cloth", "🧵", "Fine loom fabric bolt", new Color(0.85f, 0.82f, 0.78f, 1f)),
            new PrototypeItemInfo("item.wine-fruit", "Fruit Wine", "🍷", "Aged fruit wine bottle", new Color(0.68f, 0.15f, 0.35f, 1f)),
            new PrototypeItemInfo("item.flour", "Wheat Flour", "🍞", "Milled flour for baking", new Color(0.95f, 0.92f, 0.85f, 1f)),
            new PrototypeItemInfo("item.sprinkler-copper", "Copper Sprinkler", "🚿", "Waters 4 adjacent plots at 6AM", new Color(0.85f, 0.52f, 0.28f, 1f)),
            new PrototypeItemInfo("item.sprinkler-iron", "Iron Sprinkler", "🚿", "Waters 3x3 surrounding plots at 6AM", new Color(0.72f, 0.76f, 0.82f, 1f)),
            new PrototypeItemInfo("item.sprinkler-gold", "Gold Sprinkler", "🚿", "Waters 5x5 surrounding plots at 6AM", new Color(0.96f, 0.82f, 0.25f, 1f)),
            new PrototypeItemInfo("item.farm-deed", "Farm Land Deed", "📜", "Unlocks 12 extra farm plots", new Color(0.94f, 0.85f, 0.60f, 1f)),
            new PrototypeItemInfo("item.grape", "Fresh Grapes", "🍇", "Sweet purple vineyard grapes", new Color(0.68f, 0.25f, 0.72f, 1f)),
            new PrototypeItemInfo("item.seed-grape", "Grape Seeds", "🌱", "Plant in tilled soil or trellis", new Color(0.60f, 0.35f, 0.65f, 1f)),
            new PrototypeItemInfo("item.iron-bar", "Iron Ingot", "🧱", "Refined solid iron bar", new Color(0.72f, 0.76f, 0.82f, 1f)),
            new PrototypeItemInfo("item.juice", "Fresh Fruit Juice", "🧃", "Refreshing sweet berry fruit juice", new Color(0.95f, 0.35f, 0.45f, 1f)),
            new PrototypeItemInfo("item.armor-knight", "Knight Armor", "🛡️", "Forged protective iron battle armor", new Color(0.78f, 0.82f, 0.90f, 1f))
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

        public static bool TryGet(string itemId, out PrototypeItemInfo info)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (string.Equals(Items[i].ItemId, itemId, StringComparison.Ordinal))
                {
                    info = Items[i];
                    return true;
                }
            }

            info = default;
            return false;
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
