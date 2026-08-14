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

        private ResourceNode nearestNode;

        public string InteractionHint { get; private set; } = "No nearby resource.";

        public void Configure(InventorySession inventorySession, VerticalSliceController sliceController, float interactionRadius)
        {
            this.inventorySession = inventorySession;
            this.sliceController = sliceController;
            this.interactionRadius = Mathf.Max(0.1f, interactionRadius);
        }

        private void Update()
        {
            UpdateNearestNode();

            if (!PrototypeInput.GetKeyDown(KeyCode.E) || inventorySession == null) return;
            if (nearestNode == null)
            {
                InteractionHint = "No resource in range.";
                return;
            }

            if (nearestNode.TryHarvest(inventorySession.Runtime))
            {
                InteractionHint = "Gathered " + nearestNode.ResourceAmount + " " + nearestNode.ResourceItemId + ".";
                sliceController?.NotifyResourceHarvested(nearestNode);
                nearestNode.SetHighlighted(false);
                nearestNode = null;
            }
        }

        private void UpdateNearestNode()
        {
            ResourceNode previousNode = nearestNode;
            nearestNode = null;
            float nearestDistance = float.MaxValue;

            ResourceNode[] nodes = FindObjectsByType<ResourceNode>(FindObjectsInactive.Exclude);
            foreach (ResourceNode node in nodes)
            {
                if (node == null || node.IsHarvested) continue;

                float distance = Vector2.Distance(transform.position, node.transform.position);
                if (distance > interactionRadius || distance >= nearestDistance) continue;

                nearestDistance = distance;
                nearestNode = node;
            }

            if (previousNode != null && previousNode != nearestNode) previousNode.SetHighlighted(false);
            if (nearestNode != null)
            {
                nearestNode.SetHighlighted(true);
                InteractionHint = "Press E to gather " + nearestNode.DisplayName + ".";
            }
            else if (previousNode != null)
            {
                InteractionHint = "No nearby resource.";
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}
