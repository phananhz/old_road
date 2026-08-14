using UnityEngine;

namespace TheOldRoad.Building
{
    public readonly struct PlacementArea
    {
        public readonly Vector2Int Min;
        public readonly Vector2Int Max;

        public PlacementArea(Vector2Int min, Vector2Int max)
        {
            Min = min;
            Max = max;
        }

        public bool Contains(Vector2Int origin, Vector2Int footprint)
        {
            return origin.x >= Min.x && origin.y >= Min.y &&
                   origin.x + footprint.x - 1 <= Max.x && origin.y + footprint.y - 1 <= Max.y;
        }
    }

    /// <summary>Grid placement checks independent of preview visuals.</summary>
    public static class GridPlacementValidator
    {
        public static bool IsValid(Vector2Int origin, Vector2Int footprint, PlacementArea area, bool overlapsExisting)
        {
            return footprint.x > 0 && footprint.y > 0 && area.Contains(origin, footprint) && !overlapsExisting;
        }
    }
}
