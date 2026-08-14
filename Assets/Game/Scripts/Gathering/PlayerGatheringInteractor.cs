using UnityEngine;
using TheOldRoad.Inventory;
using TheOldRoad.Core;
using TheOldRoad.Input;

namespace TheOldRoad.Gathering
{
    /// <summary>Small vertical-slice interaction adapter: press E near a resource node.</summary>
    public sealed class PlayerGatheringInteractor : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float interactionRadius = 1.25f;
        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private VerticalSliceController sliceController;

        public void Configure(InventorySession inventorySession, VerticalSliceController sliceController, float interactionRadius)
        {
            this.inventorySession = inventorySession;
            this.sliceController = sliceController;
            this.interactionRadius = Mathf.Max(0.1f, interactionRadius);
        }

        private void Update()
        {
            if (!PrototypeInput.GetKeyDown(KeyCode.E) || inventorySession == null) return;

            ResourceNode[] nodes = FindObjectsByType<ResourceNode>(FindObjectsInactive.Exclude);
            foreach (ResourceNode node in nodes)
            {
                if (Vector2.Distance(transform.position, node.transform.position) <= interactionRadius &&
                    node.TryHarvest(inventorySession.Runtime))
                {
                    sliceController?.NotifyResourceHarvested(node);
                    break;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}
