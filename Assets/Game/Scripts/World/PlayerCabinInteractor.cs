using UnityEngine;
using TheOldRoad.Construction;
using TheOldRoad.Input;
using TheOldRoad.Player;
using TheOldRoad.Time;

namespace TheOldRoad.World
{
    /// <summary>Prototype cabin enter/exit and bed sleep interaction.</summary>
    public sealed class PlayerCabinInteractor : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float interactionRadius = 2.25f;
        [SerializeField] private CabinInteriorController interior;
        [SerializeField] private GameTimeController gameTime;

        private ConstructionSite nearestCabin;

        public string InteractionHint { get; private set; } = string.Empty;
        public bool CanUseAction { get; private set; }
        public string ActionButtonLabel { get; private set; } = "Use";

        public void Configure(CabinInteriorController interior, GameTimeController gameTime, float interactionRadius)
        {
            this.interior = interior;
            this.gameTime = gameTime;
            this.interactionRadius = Mathf.Max(0.1f, interactionRadius);
        }

        private void Update()
        {
            if (interior == null) interior = FindAnyObjectByType<CabinInteriorController>();
            if (gameTime == null) gameTime = FindAnyObjectByType<GameTimeController>();

            RefreshActionState();
            if (!CanUseAction || !PrototypeInput.GetKeyDown(KeyCode.F)) return;

            ExecuteAction();
        }

        private void RefreshActionState()
        {
            CanUseAction = false;
            InteractionHint = string.Empty;
            ActionButtonLabel = "Use";

            if (interior != null && interior.IsInside)
            {
                CanUseAction = true;
                if (interior.IsNearBed(transform))
                {
                    ActionButtonLabel = "Sleep";
                    InteractionHint = "Press F to sleep 8 hours.";
                    return;
                }

                ActionButtonLabel = "Exit";
                InteractionHint = "Press F to exit the cabin.";
                return;
            }

            nearestCabin = FindNearestCompletedCabin();
            if (nearestCabin == null) return;

            CanUseAction = true;
            ActionButtonLabel = "Enter";
            InteractionHint = "Press F to enter the cabin.";
        }

        private void ExecuteAction()
        {
            PlayerMovement player = GetComponent<PlayerMovement>();
            if (player == null) return;

            if (interior != null && interior.IsInside)
            {
                if (interior.IsNearBed(transform))
                {
                    interior.SleepEightHours(gameTime);
                    InteractionHint = interior.Status;
                    return;
                }

                interior.Exit(player);
                InteractionHint = interior.Status;
                return;
            }

            if (nearestCabin == null) nearestCabin = FindNearestCompletedCabin();
            if (nearestCabin == null || interior == null) return;

            interior.Enter(player, nearestCabin.transform.position);
            InteractionHint = interior.Status;
        }

        private ConstructionSite FindNearestCompletedCabin()
        {
            ConstructionSite nearest = null;
            float nearestDistance = float.MaxValue;
            ConstructionSite[] sites = FindObjectsByType<ConstructionSite>(FindObjectsInactive.Exclude);
            foreach (ConstructionSite site in sites)
            {
                if (site == null || site.Job == null || site.Job.state != ConstructionState.Completed) continue;

                float distance = Vector2.Distance(transform.position, site.transform.position);
                if (distance > interactionRadius || distance >= nearestDistance) continue;

                nearest = site;
                nearestDistance = distance;
            }

            return nearest;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}
