using UnityEngine;

namespace TheOldRoad.Input
{
    /// <summary>Device-independent movement input consumed by gameplay.</summary>
    public interface IPlayerInputSource
    {
        Vector2 Move { get; }
    }
}
