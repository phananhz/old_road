using UnityEngine;
using TheOldRoad.Core;
using TheOldRoad.Input;
using TheOldRoad.Inventory;
using TheOldRoad.UI;

namespace TheOldRoad.World
{
    /// <summary>Lets the player open nearby prototype loot chests.</summary>
    public sealed class PlayerLootInteractor : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float interactionRadius = 1.25f;
        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private VerticalSliceController sliceController;

        private const float OpenDurationSeconds = 0.8f;
        private const float CancelDistancePadding = 0.35f;

        private LootChest nearestChest;
        private LootChest activeChest;
        private WorldActionProgressBar activeProgress;
        private float nextScanTime;

        public string InteractionHint { get; private set; } = string.Empty;

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
                UpdateActiveOpen();
                return;
            }

            UpdateNearestChest(false);

            if (!PrototypeInput.GetKeyDown(KeyCode.E) || inventorySession == null) return;
            if (nearestChest == null) return;

            BeginOpen(nearestChest);
        }

        private void BeginOpen(LootChest chest)
        {
            if (chest == null || chest.IsOpened) return;

            activeChest = chest;
            activeChest.SetHighlighted(true);
            if (!WorldActionProgressBar.TryStart(
                    gameObject,
                    Camera.main,
                    activeChest.transform,
                    "Opening",
                    OpenDurationSeconds,
                    CompleteOpen,
                    CancelOpen,
                    out activeProgress))
            {
                activeChest.SetHighlighted(false);
                activeChest = null;
                InteractionHint = "Finish the current action first.";
                PlayerSpeechBubble.Say("speech.action_busy");
                return;
            }

            InteractionHint = "Opening " + activeChest.DisplayName + "...";
            PlayerSpeechBubble.Say("speech.loot_start");
        }

        private void UpdateActiveOpen()
        {
            if (activeChest == null || activeChest.IsOpened)
            {
                activeProgress.Cancel();
                return;
            }

            activeChest.SetHighlighted(true);
            float distance = Vector2.Distance(transform.position, activeChest.transform.position);
            if (distance > interactionRadius + CancelDistancePadding)
            {
                activeProgress.Cancel();
            }
        }

        private void CompleteOpen()
        {
            LootChest chest = activeChest;
            activeProgress = null;
            activeChest = null;

            if (chest == null || inventorySession == null) return;
            if (chest.TryOpen(inventorySession.Runtime))
            {
                InteractionHint = "Opened " + chest.DisplayName + ": +" + chest.Quantity + " " + chest.ItemId + ".";
                sliceController?.NotifyLootChestOpened(chest);
                chest.SetHighlighted(false);
                if (nearestChest == chest) nearestChest = null;
                PlayerSpeechBubble.Say("speech.loot_done");
            }
        }

        private void CancelOpen()
        {
            if (activeChest != null) activeChest.SetHighlighted(false);
            activeChest = null;
            activeProgress = null;
            InteractionHint = "Open cancelled.";
        }

        private void UpdateNearestChest(bool force)
        {
            if (!force && UnityEngine.Time.unscaledTime < nextScanTime) return;
            nextScanTime = UnityEngine.Time.unscaledTime + 0.18f;

            LootChest previous = nearestChest;
            nearestChest = null;
            float nearestDistance = float.MaxValue;

            LootChest[] chests = FindObjectsByType<LootChest>(FindObjectsInactive.Exclude);
            foreach (LootChest chest in chests)
            {
                if (chest == null || chest.IsOpened) continue;

                float distance = Vector2.Distance(transform.position, chest.transform.position);
                if (distance > interactionRadius || distance >= nearestDistance) continue;

                nearestDistance = distance;
                nearestChest = chest;
            }

            if (previous != null && previous != nearestChest) previous.SetHighlighted(false);
            if (nearestChest != null)
            {
                nearestChest.SetHighlighted(true);
                InteractionHint = "Press E to open " + nearestChest.DisplayName + ".";
            }
            else if (previous != null)
            {
                InteractionHint = string.Empty;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}
