using System;
using UnityEngine;
using TheOldRoad.Building;
using TheOldRoad.Inventory;
using TheOldRoad.Time;

namespace TheOldRoad.Construction
{
    public static class ConstructionRuntime
    {
        public static ConstructionJob Begin(string constructionId, string buildingId, long durationSeconds, Vector2Int placement, IClock clock)
        {
            if (string.IsNullOrWhiteSpace(constructionId) || string.IsNullOrWhiteSpace(buildingId)) throw new ArgumentException("Stable IDs are required.");
            if (durationSeconds < 0) throw new ArgumentOutOfRangeException(nameof(durationSeconds));
            if (clock == null) throw new ArgumentNullException(nameof(clock));

            return new ConstructionJob
            {
                constructionId = constructionId,
                buildingId = buildingId,
                startUnixSeconds = clock.NowUnixSeconds,
                durationSeconds = durationSeconds,
                gridX = placement.x,
                gridY = placement.y,
                state = durationSeconds == 0 ? ConstructionState.Completed : ConstructionState.Constructing
            };
        }

        public static bool TryBegin(
            string constructionId,
            BuildingDefinition buildingDefinition,
            Vector2Int placement,
            InventoryRuntime inventory,
            IClock clock,
            out ConstructionJob job)
        {
            job = null;
            if (buildingDefinition == null || inventory == null || clock == null) return false;
            if (!inventory.TryRemoveAll(buildingDefinition.ConstructionCosts)) return false;

            job = Begin(
                constructionId,
                buildingDefinition.BuildingId,
                (long)Math.Ceiling(buildingDefinition.ConstructionDurationSeconds),
                placement,
                clock);
            return true;
        }
    }
}
