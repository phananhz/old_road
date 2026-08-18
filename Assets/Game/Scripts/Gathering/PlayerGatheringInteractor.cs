using UnityEngine;
using TheOldRoad.Inventory;
using TheOldRoad.Items;
using TheOldRoad.Core;
using TheOldRoad.Input;
using TheOldRoad.UI;

namespace TheOldRoad.Gathering
{
    /// <summary>Small vertical-slice interaction adapter: press E near a resource node.</summary>
    public sealed class PlayerGatheringInteractor : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float interactionRadius = 1.25f;
        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private VerticalSliceController sliceController;

        private const float GatherDurationSeconds = 1.2f;
        private const float CancelDistancePadding = 0.35f;

        private ResourceNode nearestNode;
        private ResourceNode activeNode;
        private WorldActionProgressBar activeProgress;
        private float nextScanTime;

        public string InteractionHint { get; private set; } = "No nearby resource.";

        public void Configure(InventorySession inventorySession, VerticalSliceController sliceController, float interactionRadius)
        {
            this.inventorySession = inventorySession;
            this.sliceController = sliceController;
            this.interactionRadius = Mathf.Max(0.1f, interactionRadius);
        }

        private void Update()
        {
            if (activeProgress != null)
            {
                UpdateActiveGather();
                return;
            }

            UpdateNearestNode(false);

            if ((!PrototypeInput.GetKeyDown(KeyCode.F) && !PrototypeInput.GetKeyDown(KeyCode.E)) || inventorySession == null) return;
            if (nearestNode == null)
            {
                return;
            }

            if (!nearestNode.CanHarvest(inventorySession.Runtime))
            {
                InteractionHint = FormatBlockedHint(nearestNode);
                PlayerSpeechBubble.Say("speech.gather_blocked");
                return;
            }

            BeginGather(nearestNode);
        }

        private void BeginGather(ResourceNode node)
        {
            if (node == null || node.IsHarvested) return;

            activeNode = node;
            activeNode.SetHighlighted(true);
            if (!WorldActionProgressBar.TryStart(
                    gameObject,
                    Camera.main,
                    activeNode.transform,
                    "Gathering",
                    GatherDurationSeconds,
                    CompleteGather,
                    CancelGather,
                    out activeProgress))
            {
                activeNode.SetHighlighted(false);
                activeNode = null;
                InteractionHint = "Finish the current action first.";
                PlayerSpeechBubble.Say("speech.action_busy");
                return;
            }

            InteractionHint = "Gathering " + activeNode.DisplayName + "...";
            PlayerSpeechBubble.Say("speech.gather_start");
        }

        private void UpdateActiveGather()
        {
            if (activeNode == null || activeNode.IsHarvested)
            {
                activeProgress.Cancel();
                return;
            }

            activeNode.SetHighlighted(true);
            float distance = Vector2.Distance(transform.position, activeNode.transform.position);
            if (distance > interactionRadius + CancelDistancePadding)
            {
                activeProgress.Cancel();
            }
        }

        private void CompleteGather()
        {
            ResourceNode node = activeNode;
            activeProgress = null;
            activeNode = null;

            if (node == null || inventorySession == null) return;
            if (node.TryHarvest(inventorySession.Runtime))
            {
                InteractionHint = "Gathered " + node.ResourceAmount + " " + node.ResourceItemId + ".";
                if (node.ResourceItemId == "item.wood") TheOldRoad.Audio.AudioManager.PlayChopWood();
                else if (node.ResourceItemId == "item.stone" || node.ResourceItemId == "item.iron-ore") TheOldRoad.Audio.AudioManager.PlayMineStone();
                else TheOldRoad.Audio.AudioManager.PlayForage();

                sliceController?.NotifyResourceHarvested(node);
                node.SetHighlighted(false);
                if (nearestNode == node) nearestNode = null;
                PlayerSpeechBubble.Say("speech.gather_done");
            }
            else
            {
                InteractionHint = FormatBlockedHint(node);
                node.SetHighlighted(false);
                PlayerSpeechBubble.Say("speech.gather_blocked");
            }
        }

        private void CancelGather()
        {
            if (activeNode != null) activeNode.SetHighlighted(false);
            activeNode = null;
            activeProgress = null;
            InteractionHint = "Gather cancelled.";
            PlayerSpeechBubble.Say("speech.gather_cancelled");
        }

        private void UpdateNearestNode(bool force)
        {
            if (!force && UnityEngine.Time.unscaledTime < nextScanTime) return;
            nextScanTime = UnityEngine.Time.unscaledTime + 0.16f;

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
                InteractionHint = nearestNode.CanHarvest(inventorySession != null ? inventorySession.Runtime : null)
                    ? "Press E to gather " + nearestNode.DisplayName + "."
                    : FormatBlockedHint(nearestNode);
            }
            else if (previousNode != null)
            {
                InteractionHint = "No nearby resource.";
            }
        }

        private static string FormatBlockedHint(ResourceNode node)
        {
            if (node == null) return "No nearby resource.";
            if (node.RequiresTool)
            {
                PrototypeItemInfo tool = PrototypeItemCatalog.Get(node.RequiredToolItemId);
                return "Need " + tool.DisplayName + " to gather " + node.DisplayName + ".";
            }

            return "Cannot gather " + node.DisplayName + " yet.";
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}
