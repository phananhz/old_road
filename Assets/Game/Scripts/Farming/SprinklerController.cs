using System;
using UnityEngine;
using TheOldRoad.Audio;
using TheOldRoad.Combat;
using TheOldRoad.UI;

namespace TheOldRoad.Farming
{
    public enum SprinklerTier
    {
        Copper = 0, // Cross 4 adjacent
        Iron = 1,   // 3x3 area
        Gold = 2    // 5x5 area
    }

    /// <summary>
    /// Automatic morning sprinkler system. Waters surrounding farm plots at 06:00 AM daily.
    /// </summary>
    public sealed class SprinklerController : MonoBehaviour
    {
        [SerializeField] private SprinklerTier tier = SprinklerTier.Copper;
        private int lastWateredDay = -1;

        public SprinklerTier Tier => tier;

        public void Configure(SprinklerTier tier)
        {
            this.tier = tier;
        }

        public void ConfigureFromBuildingId(string buildingId)
        {
            if (string.Equals(buildingId, "building.sprinkler-gold", StringComparison.OrdinalIgnoreCase))
            {
                tier = SprinklerTier.Gold;
            }
            else if (string.Equals(buildingId, "building.sprinkler-iron", StringComparison.OrdinalIgnoreCase))
            {
                tier = SprinklerTier.Iron;
            }
            else
            {
                tier = SprinklerTier.Copper;
            }
        }

        public int WaterSurroundingPlots()
        {
            float radius = tier switch
            {
                SprinklerTier.Copper => 1.85f,
                SprinklerTier.Iron => 2.95f,
                SprinklerTier.Gold => 4.50f,
                _ => 1.85f
            };

            int wateredCount = 0;
            Vector3 pos = transform.position;
            FarmPlotController[] allPlots = FindObjectsByType<FarmPlotController>(FindObjectsInactive.Exclude);

            for (int i = 0; i < allPlots.Length; i++)
            {
                FarmPlotController plot = allPlots[i];
                if (plot == null) continue;

                float dist = Vector2.Distance(pos, plot.transform.position);
                if (dist <= radius)
                {
                    if (!plot.IsWatered)
                    {
                        plot.SetWatered(true);
                        wateredCount++;
                    }
                }
            }

            if (wateredCount > 0)
            {
                AudioManager.PlayWaterSplash();
                FloatingTextController.Spawn($"💧 +{wateredCount} {LocalizationRuntime.T("watered")}", pos + Vector3.up * 0.8f, new Color(0.3f, 0.75f, 1f, 1f));
            }

            return wateredCount;
        }

        public void CheckDailyWatering(int absoluteMinute)
        {
            int currentDay = absoluteMinute / 1440;
            int minuteOfDay = absoluteMinute % 1440;

            // Sprinklers trigger at 06:00 AM (360 minutes) or upon sleeping
            if (minuteOfDay >= 360 && lastWateredDay != currentDay)
            {
                lastWateredDay = currentDay;
                WaterSurroundingPlots();
            }
        }

        public static void WaterAllSprinklersInWorld()
        {
            SprinklerController[] sprinklers = FindObjectsByType<SprinklerController>(FindObjectsInactive.Exclude);
            for (int i = 0; i < sprinklers.Length; i++)
            {
                if (sprinklers[i] != null)
                {
                    sprinklers[i].WaterSurroundingPlots();
                }
            }
        }
    }
}
