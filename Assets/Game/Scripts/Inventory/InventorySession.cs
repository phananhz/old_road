using UnityEngine;

namespace TheOldRoad.Inventory
{
    /// <summary>Scene-owned composition point for the current player's runtime inventory.</summary>
    public sealed class InventorySession : MonoBehaviour
    {
        private InventoryRuntime runtime;

        public InventoryRuntime Runtime
        {
            get
            {
                if (runtime == null) runtime = new InventoryRuntime();
                return runtime;
            }
        }

        private void Awake()
        {
            if (runtime == null) runtime = new InventoryRuntime();
        }
    }
}
