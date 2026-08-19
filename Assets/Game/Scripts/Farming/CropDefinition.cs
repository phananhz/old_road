using System;
using UnityEngine;

namespace TheOldRoad.Farming
{
    /// <summary>
    /// Static definition of a farmable crop.
    /// </summary>
    [Serializable]
    public sealed class CropDefinition
    {
        public string CropId { get; }
        public string DisplayName { get; }
        public string SeedItemId { get; }
        public string HarvestItemId { get; }
        public float GrowthDurationMinutes { get; }
        public int StageCount { get; }
        public int MinYield { get; }
        public int MaxYield { get; }

        public CropDefinition(
            string cropId,
            string displayName,
            string seedItemId,
            string harvestItemId,
            float growthDurationMinutes,
            int stageCount,
            int minYield,
            int maxYield)
        {
            CropId = cropId;
            DisplayName = displayName;
            SeedItemId = seedItemId;
            HarvestItemId = harvestItemId;
            GrowthDurationMinutes = Mathf.Max(0.1f, growthDurationMinutes);
            StageCount = Mathf.Max(2, stageCount);
            MinYield = Mathf.Max(1, minYield);
            MaxYield = Mathf.Max(MinYield, maxYield);
        }
    }
}
