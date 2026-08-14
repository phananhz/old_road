using UnityEngine;
using TheOldRoad.Inventory;

namespace TheOldRoad.Gathering
{
    /// <summary>Configurable resource node. It rewards inventory through gameplay API only.</summary>
    public sealed class ResourceNode : MonoBehaviour
    {
        [SerializeField] private string nodeId;
        [SerializeField] private string resourceItemId = "wood";
        [SerializeField, Min(1)] private int resourceAmount = 1;
        private bool harvested;

        public string NodeId => nodeId;
        public string ResourceItemId => resourceItemId;
        public int ResourceAmount => resourceAmount;
        public bool IsHarvested => harvested;

        public void Configure(string nodeId, string resourceItemId, int resourceAmount, bool harvested = false)
        {
            this.nodeId = nodeId;
            this.resourceItemId = resourceItemId;
            this.resourceAmount = Mathf.Max(1, resourceAmount);
            this.harvested = harvested;
        }

        public bool TryHarvest(InventoryRuntime inventory)
        {
            if (harvested || inventory == null || string.IsNullOrWhiteSpace(resourceItemId) || resourceAmount <= 0)
                return false;

            inventory.Add(resourceItemId, resourceAmount);
            harvested = true;
            return true;
        }

        public void SetHarvested(bool harvested)
        {
            this.harvested = harvested;
        }
    }
}
