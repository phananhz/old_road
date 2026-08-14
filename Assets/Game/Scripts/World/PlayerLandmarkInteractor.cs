using UnityEngine;
using TheOldRoad.Core;
using TheOldRoad.Input;
using TheOldRoad.UI;

namespace TheOldRoad.World
{
    /// <summary>Lets the player inspect nearby landmarks and add them to the prototype journal.</summary>
    public sealed class PlayerLandmarkInteractor : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float interactionRadius = 1.35f;
        [SerializeField] private VerticalSliceController sliceController;

        private const float InspectDurationSeconds = 0.9f;
        private const float CancelDistancePadding = 0.35f;

        private DiscoverableLandmark nearestLandmark;
        private DiscoverableLandmark activeLandmark;
        private WorldActionProgressBar activeProgress;

        public string InteractionHint { get; private set; } = string.Empty;

        public void Configure(VerticalSliceController sliceController, float interactionRadius)
        {
            this.sliceController = sliceController;
            this.interactionRadius = Mathf.Max(0.1f, interactionRadius);
        }

        private void Update()
        {
            if (activeProgress != null)
            {
                UpdateActiveInspect();
                return;
            }

            UpdateNearestLandmark();

            if (!PrototypeInput.GetKeyDown(KeyCode.E)) return;
            if (nearestLandmark == null) return;

            BeginInspect(nearestLandmark);
        }

        private void BeginInspect(DiscoverableLandmark landmark)
        {
            if (landmark == null || landmark.IsDiscovered) return;

            activeLandmark = landmark;
            activeLandmark.SetHighlighted(true);
            if (!WorldActionProgressBar.TryStart(
                    gameObject,
                    Camera.main,
                    activeLandmark.transform,
                    "Inspecting",
                    InspectDurationSeconds,
                    CompleteInspect,
                    CancelInspect,
                    out activeProgress))
            {
                activeLandmark.SetHighlighted(false);
                activeLandmark = null;
                InteractionHint = "Finish the current action first.";
                return;
            }

            InteractionHint = "Inspecting " + activeLandmark.Title + "...";
        }

        private void UpdateActiveInspect()
        {
            if (activeLandmark == null || activeLandmark.IsDiscovered)
            {
                activeProgress.Cancel();
                return;
            }

            activeLandmark.SetHighlighted(true);
            float distance = Vector2.Distance(transform.position, activeLandmark.transform.position);
            if (distance > interactionRadius + CancelDistancePadding)
            {
                activeProgress.Cancel();
            }
        }

        private void CompleteInspect()
        {
            DiscoverableLandmark landmark = activeLandmark;
            activeProgress = null;
            activeLandmark = null;

            if (landmark == null) return;
            if (landmark.Discover())
            {
                InteractionHint = "Discovered: " + landmark.Title + ".";
                sliceController?.NotifyLandmarkDiscovered(landmark);
                landmark.SetHighlighted(false);
                if (nearestLandmark == landmark) nearestLandmark = null;
            }
        }

        private void CancelInspect()
        {
            if (activeLandmark != null) activeLandmark.SetHighlighted(false);
            activeLandmark = null;
            activeProgress = null;
            InteractionHint = "Inspect cancelled.";
        }

        private void UpdateNearestLandmark()
        {
            DiscoverableLandmark previous = nearestLandmark;
            nearestLandmark = null;
            float nearestDistance = float.MaxValue;

            DiscoverableLandmark[] landmarks = FindObjectsByType<DiscoverableLandmark>(FindObjectsInactive.Exclude);
            foreach (DiscoverableLandmark landmark in landmarks)
            {
                if (landmark == null || landmark.IsDiscovered) continue;

                float distance = Vector2.Distance(transform.position, landmark.transform.position);
                if (distance > interactionRadius || distance >= nearestDistance) continue;

                nearestDistance = distance;
                nearestLandmark = landmark;
            }

            if (previous != null && previous != nearestLandmark) previous.SetHighlighted(false);
            if (nearestLandmark != null)
            {
                nearestLandmark.SetHighlighted(true);
                InteractionHint = "Press E to inspect " + nearestLandmark.Title + ".";
            }
            else if (previous != null)
            {
                InteractionHint = string.Empty;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}
