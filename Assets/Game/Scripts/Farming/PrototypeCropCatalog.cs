using System;

namespace TheOldRoad.Farming
{
    /// <summary>
    /// Static catalog of crop definitions for the farming prototype.
    /// </summary>
    public static class PrototypeCropCatalog
    {
        private static readonly CropDefinition[] Crops =
        {
            new CropDefinition("carrot", "Carrot", "item.seed-carrot", "item.carrot", 90f, 5, 2, 4), // 1.5 Hours
            new CropDefinition("wheat", "Wheat", "item.seed-wheat", "item.wheat", 120f, 5, 3, 6), // 2 Hours
            new CropDefinition("potato", "Potato", "item.seed-potato", "item.potato", 180f, 5, 2, 5), // 3 Hours
            new CropDefinition("corn", "Corn", "item.seed-corn", "item.corn", 360f, 5, 2, 4), // 6 Hours
            new CropDefinition("tomato", "Tomato", "item.seed-tomato", "item.tomato", 480f, 5, 2, 4), // 8 Hours
            new CropDefinition("strawberry", "Strawberry", "item.seed-strawberry", "item.strawberry", 720f, 5, 3, 6), // 12 Hours
            new CropDefinition("pineapple", "Pineapple", "item.seed-pineapple", "item.pineapple", 1440f, 5, 1, 2), // 24 Hours (1 Day)
            new CropDefinition("pumpkin", "Giant Pumpkin", "item.seed-pumpkin", "item.pumpkin", 2880f, 5, 1, 1), // 48 Hours (2 Days)
            new CropDefinition("apple-tree", "Apple Tree", "item.sapling-apple", "item.apple", 4320f, 5, 4, 8) // 72 Hours (3 Days)
        };

        public static CropDefinition[] All => Crops;

        public static CropDefinition Get(string cropId)
        {
            if (string.IsNullOrWhiteSpace(cropId)) return null;

            for (int i = 0; i < Crops.Length; i++)
            {
                if (string.Equals(Crops[i].CropId, cropId, StringComparison.OrdinalIgnoreCase))
                {
                    return Crops[i];
                }
            }

            return null;
        }

        public static bool TryGetBySeed(string seedItemId, out CropDefinition crop)
        {
            crop = null;
            if (string.IsNullOrWhiteSpace(seedItemId)) return false;

            for (int i = 0; i < Crops.Length; i++)
            {
                if (string.Equals(Crops[i].SeedItemId, seedItemId, StringComparison.OrdinalIgnoreCase))
                {
                    crop = Crops[i];
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetByHarvestItem(string harvestItemId, out CropDefinition crop)
        {
            crop = null;
            if (string.IsNullOrWhiteSpace(harvestItemId)) return false;

            for (int i = 0; i < Crops.Length; i++)
            {
                if (string.Equals(Crops[i].HarvestItemId, harvestItemId, StringComparison.OrdinalIgnoreCase))
                {
                    crop = Crops[i];
                    return true;
                }
            }

            return false;
        }
    }
}
