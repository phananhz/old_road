using UnityEngine;
using TheOldRoad.Core;
using TheOldRoad.Input;
using TheOldRoad.UI;

namespace TheOldRoad.NPC
{
    /// <summary>Prototype player-to-villager interaction adapter.</summary>
    public sealed class PlayerNpcInteractor : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float interactionRadius = 1.55f;
        [SerializeField] private VerticalSliceController sliceController;

        private VillagerNpcController nearestVillager;
        private float hintHideTime;
        private float nextScanTime;

        public bool CanTalkAction => nearestVillager != null;
        public string InteractionHint { get; private set; } = string.Empty;

        public void Configure(VerticalSliceController sliceController, float interactionRadius)
        {
            this.sliceController = sliceController;
            this.interactionRadius = Mathf.Max(0.1f, interactionRadius);
        }

        private void Update()
        {
            UpdateNearestVillager(false);

            if (nearestVillager == null)
            {
                if (UnityEngine.Time.unscaledTime > hintHideTime) InteractionHint = string.Empty;
                return;
            }

            InteractionHint = (LocalizationRuntime.IsVietnamese ? "Bấm F/E để nói chuyện với " : "Press F/E to talk with ") + nearestVillager.VillagerName + " (" + LocalizationRuntime.NpcTitle(nearestVillager.JobTitle) + ").";
            hintHideTime = UnityEngine.Time.unscaledTime + 0.35f;

            if (!PrototypeInput.GetKeyDown(KeyCode.F) && !PrototypeInput.GetKeyDown(KeyCode.E)) return;

            string line = nearestVillager.Talk();
            InteractionHint = nearestVillager.VillagerName + ": " + line;
            hintHideTime = UnityEngine.Time.unscaledTime + 5.5f;
            sliceController?.NotifyVillagerTalked(nearestVillager);
        }

        private void UpdateNearestVillager(bool force)
        {
            if (!force && UnityEngine.Time.unscaledTime < nextScanTime) return;
            nextScanTime = UnityEngine.Time.unscaledTime + 0.20f;

            VillagerNpcController previous = nearestVillager;
            nearestVillager = null;
            float nearestDistance = float.MaxValue;

            foreach (VillagerNpcController villager in FindObjectsByType<VillagerNpcController>(FindObjectsInactive.Exclude))
            {
                if (villager == null) continue;

                float distance = Vector2.Distance(transform.position, villager.transform.position);
                if (distance > interactionRadius || distance >= nearestDistance) continue;

                nearestDistance = distance;
                nearestVillager = villager;
            }

            if (previous != null && previous != nearestVillager) previous.SetHighlighted(false);
            if (nearestVillager != null) nearestVillager.SetHighlighted(true);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.78f, 0.24f, 1f);
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}
