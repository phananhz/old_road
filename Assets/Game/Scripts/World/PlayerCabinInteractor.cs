using UnityEngine;
using TheOldRoad.Construction;
using TheOldRoad.Input;
using TheOldRoad.Player;
using TheOldRoad.Time;
using TheOldRoad.UI;

namespace TheOldRoad.World
{
    /// <summary>Prototype cabin enter/exit and bed sleep interaction.</summary>
    public sealed class PlayerCabinInteractor : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float interactionRadius = 2.25f;
        [SerializeField] private CabinInteriorController interior;
        [SerializeField] private GameTimeController gameTime;

        private ConstructionSite nearestCabin;
        private bool sleepConfirmationOpen;
        private Vector3 lastInteriorValidPosition;
        private bool hasInteriorValidPosition;
        private float nextCabinScanTime;

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

            if (interior != null && interior.IsInside)
            {
                if (!hasInteriorValidPosition)
                {
                    lastInteriorValidPosition = transform.position;
                    hasInteriorValidPosition = true;
                }

                interior.ConstrainActorInside(transform, ref lastInteriorValidPosition);
            }
            else
            {
                hasInteriorValidPosition = false;
            }

            if (sleepConfirmationOpen)
            {
                HandleSleepConfirmationInput();
                return;
            }

            RefreshActionState();
            if (!CanUseAction || (!PrototypeInput.GetKeyDown(KeyCode.F) && !PrototypeInput.GetKeyDown(KeyCode.E))) return;

            ExecuteAction();
        }

        private void RefreshActionState()
        {
            CanUseAction = false;
            InteractionHint = string.Empty;
            ActionButtonLabel = "Use";
            bool vi = LocalizationRuntime.IsVietnamese;

            if (interior != null && interior.IsInside)
            {
                if (interior.IsNearStairs(transform))
                {
                    CanUseAction = true;
                    bool isF1 = interior.CurrentFloor == 1;
                    ActionButtonLabel = isF1 ? (vi ? "Lên Tầng 2" : "Go Up") : (vi ? "Xuống Tầng 1" : "Go Down");
                    InteractionHint = isF1
                        ? (vi ? "Nhấn [F] hoặc [E] để Lên Tầng 2." : "Press [F] or [E] to go up to 2nd Floor.")
                        : (vi ? "Nhấn [F] hoặc [E] để Xuống Tầng 1." : "Press [F] or [E] to go down to 1st Floor.");
                    return;
                }

                if (interior.IsNearBed(transform))
                {
                    CanUseAction = true;
                    ActionButtonLabel = vi ? "Ngủ" : "Sleep";
                    InteractionHint = vi ? "Nhấn [F] để nghỉ ngơi ngủ 8 tiếng." : "Press [F] to sleep for 8 hours.";
                    return;
                }

                if (interior.IsNearChest(transform))
                {
                    CanUseAction = true;
                    ActionButtonLabel = vi ? "Mở Rương" : "Chest";
                    InteractionHint = vi ? "Nhấn [F] để mở rương đồ cá nhân." : "Press [F] to open chest.";
                    return;
                }

                if (interior.IsNearDoor(transform))
                {
                    CanUseAction = true;
                    ActionButtonLabel = vi ? "Ra Ngoài" : "Exit";
                    InteractionHint = vi ? "Nhấn [F] để bước ra ngoài." : "Press [F] to exit.";
                    return;
                }

                return;
            }

            if (UnityEngine.Time.unscaledTime >= nextCabinScanTime)
            {
                nextCabinScanTime = UnityEngine.Time.unscaledTime + 0.25f;
                nearestCabin = FindNearestCompletedCabin();
            }

            if (nearestCabin == null) return;

            CanUseAction = true;
            string bName = LocalizationRuntime.BuildingName(nearestCabin.BuildingId);
            ActionButtonLabel = vi ? "Vào Nhà" : "Enter";
            InteractionHint = vi ? $"Nhấn [F] để vào {bName}." : $"Press [F] to enter {bName}.";
        }

        private void ExecuteAction()
        {
            PlayerMovement player = GetComponent<PlayerMovement>();
            if (player == null) return;

            if (interior != null && interior.IsInside)
            {
                if (interior.IsNearStairs(transform))
                {
                    interior.ToggleFloor(player);
                    hasInteriorValidPosition = false;
                    return;
                }

                if (interior.IsNearBed(transform))
                {
                    OpenSleepConfirmation();
                    return;
                }

                if (interior.IsNearDoor(transform))
                {
                    interior.Exit(player);
                    hasInteriorValidPosition = false;
                    InteractionHint = interior.Status;
                    TheOldRoad.Audio.AudioManager.PlayDoor();
                    PlayerSpeechBubble.Say("speech.exit_home");
                    return;
                }

                return;
            }

            if (nearestCabin == null) nearestCabin = FindNearestCompletedCabin();
            if (nearestCabin == null || interior == null) return;

            interior.Enter(player, nearestCabin.transform.position, nearestCabin.BuildingId);
            lastInteriorValidPosition = player.transform.position;
            hasInteriorValidPosition = true;
            InteractionHint = interior.Status;
            TheOldRoad.Audio.AudioManager.PlayDoor();
            PlayerSpeechBubble.Say("speech.enter_home");
        }

        private void OpenSleepConfirmation()
        {
            sleepConfirmationOpen = true;
            CanUseAction = false;
            ActionButtonLabel = "Sleep";
            InteractionHint = LocalizationRuntime.IsVietnamese
                ? "Ngủ 8 tiếng hồi đầy máu? Nhấn Y để đồng ý hoặc N/Esc để hủy."
                : "Sleep for 8 hours? Press Y to confirm or N/Esc to cancel.";
            PlayerSpeechBubble.Say("speech.sleep_prompt", 3.2f);
        }

        private void HandleSleepConfirmationInput()
        {
            CanUseAction = false;
            ActionButtonLabel = "Sleep";

            if (PrototypeInput.GetKeyDown(KeyCode.Y) || PrototypeInput.GetKeyDown(KeyCode.Return))
            {
                ConfirmSleep();
                return;
            }

            if (PrototypeInput.GetKeyDown(KeyCode.N) || PrototypeInput.GetKeyDown(KeyCode.Escape))
            {
                CancelSleep();
            }
        }

        private void ConfirmSleep()
        {
            sleepConfirmationOpen = false;
            TheOldRoad.Audio.AudioManager.PlaySleepMorning();
            if (interior != null) interior.SleepEightHours(gameTime);
            InteractionHint = interior != null ? interior.Status : "Slept 8 hours.";
            PlayerSpeechBubble.Say("speech.sleep_done");
        }

        private void CancelSleep()
        {
            sleepConfirmationOpen = false;
            InteractionHint = "Sleep cancelled.";
            PlayerSpeechBubble.Say("speech.sleep_cancelled");
        }

        private static bool IsEnterableBuilding(string buildingId)
        {
            return buildingId == "building.cabin" ||
                   buildingId == "building.stone-cottage" ||
                   buildingId == "building.manor" ||
                   buildingId == "building.windmill" ||
                   buildingId == "building.lookout-tower" ||
                   buildingId == "building.herbalist-hut" ||
                   buildingId == "building.greenhouse" ||
                   buildingId == "building.tent" ||
                   buildingId == "building.storage-shed" ||
                   buildingId == "building.farm-barn";
        }

        private ConstructionSite FindNearestCompletedCabin()
        {
            ConstructionSite nearest = null;
            float nearestDistance = float.MaxValue;
            float scanRadius = Mathf.Max(3.5f, interactionRadius);
            ConstructionSite[] sites = FindObjectsByType<ConstructionSite>(FindObjectsInactive.Exclude);
            foreach (ConstructionSite site in sites)
            {
                if (site == null || site.Job == null || site.Job.state != ConstructionState.Completed) continue;
                if (!IsEnterableBuilding(site.Job.buildingId)) continue;

                float distance = Vector2.Distance(transform.position, site.transform.position);
                if (distance > scanRadius || distance >= nearestDistance) continue;

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

        private void OnGUI()
        {
            if (!sleepConfirmationOpen) return;

            const float width = 420f;
            const float height = 190f;
            Rect panel = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);

            Color previousColor = GUI.color;
            GUI.color = new Color(0.02f, 0.018f, 0.014f, 0.94f);
            GUI.DrawTexture(panel, Texture2D.whiteTexture);
            GUI.color = new Color(0.72f, 0.56f, 0.28f, 1f);
            GUI.DrawTexture(new Rect(panel.x, panel.y, panel.width, 3f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(panel.x, panel.yMax - 3f, panel.width, 3f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(panel.x, panel.y, 3f, panel.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(panel.xMax - 3f, panel.y, 3f, panel.height), Texture2D.whiteTexture);
            GUI.color = previousColor;

            UiFontHelper.EnsureGlobalSkinFont();
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                font = UiFontHelper.CleanFont,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 22,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.95f, 0.84f, 0.58f, 1f) }
            };

            GUIStyle bodyStyle = new GUIStyle(GUI.skin.label)
            {
                font = UiFontHelper.CleanFont,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                wordWrap = true,
                normal = { textColor = new Color(0.92f, 0.88f, 0.78f, 1f) }
            };

            GUI.Label(new Rect(panel.x + 24f, panel.y + 20f, panel.width - 48f, 32f), LocalizationRuntime.T("ui.use_bed"), titleStyle);
            GUI.Label(new Rect(panel.x + 36f, panel.y + 62f, panel.width - 72f, 48f), LocalizationRuntime.T("ui.sleep_confirm_question"), bodyStyle);

            if (GUI.Button(new Rect(panel.x + 52f, panel.y + 126f, 140f, 40f), LocalizationRuntime.T("ui.yes_key")))
            {
                ConfirmSleep();
            }

            if (GUI.Button(new Rect(panel.xMax - 192f, panel.y + 126f, 140f, 40f), LocalizationRuntime.T("ui.no_key")))
            {
                CancelSleep();
            }
        }
    }
}
