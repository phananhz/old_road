using System.Collections;
using UnityEngine;
using TheOldRoad.Combat;
using TheOldRoad.Core;
using TheOldRoad.Input;
using TheOldRoad.Inventory;
using TheOldRoad.Player;
using TheOldRoad.UI;

namespace TheOldRoad.Fishing
{
    public sealed class PlayerFishingInteractor : MonoBehaviour
    {
        public enum FishingState
        {
            Idle,
            Casting,
            WaitingForBite,
            Biting,
            Reeling
        }

        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private float maxDistanceToRiver = 3.2f;

        private PlayerMovement playerMovement;
        private FishingState state = FishingState.Idle;
        private FishingBobber currentBobber;
        private Coroutine fishingRoutine;

        public bool IsFishing => state != FishingState.Idle;
        public FishingState State => state;
        public string InteractionHint { get; private set; } = string.Empty;
        public string ActionButtonLabel { get; private set; } = string.Empty;
        public bool CanFishAction { get; private set; }

        public void Configure(InventorySession session)
        {
            inventorySession = session;
        }

        private void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();
        }

        private void Update()
        {
            if (inventorySession == null) inventorySession = FindAnyObjectByType<InventorySession>();

            UpdateInteractionState();

            if (PrototypeInput.GetKeyDown(KeyCode.F))
            {
                OnActionPressed();
            }
        }

        private void UpdateInteractionState()
        {
            if (IsFishing)
            {
                if (state == FishingState.WaitingForBite)
                {
                    InteractionHint = LocalizationRuntime.IsVietnamese ? "Đang chờ cá cắn câu..." : "Waiting for fish to bite...";
                    ActionButtonLabel = LocalizationRuntime.IsVietnamese ? "Thu Cần" : "Cancel";
                }
                else if (state == FishingState.Biting)
                {
                    InteractionHint = LocalizationRuntime.IsVietnamese ? "[F] CÁ CẮN CÂU! GIẬT CẦN NGAY!" : "[F] FISH ON! REEL IN NOW!";
                    ActionButtonLabel = LocalizationRuntime.IsVietnamese ? "Giật Cần!" : "Reel!";
                }
                else if (state == FishingState.Reeling)
                {
                    InteractionHint = LocalizationRuntime.IsVietnamese ? "Đang kéo cá lên bờ..." : "Reeling in the catch...";
                    ActionButtonLabel = string.Empty;
                }
                CanFishAction = true;
                return;
            }

            Vector3 pos = transform.position;
            float riverY = GetRiverY(pos.x);
            float distToRiver = Mathf.Abs(pos.y - riverY);

            bool isNearRiver = distToRiver <= maxDistanceToRiver && pos.x >= -58f && pos.x <= 28f;
            bool hasRod = HasFishingRod();

            if (isNearRiver)
            {
                if (hasRod)
                {
                    bool hasBait = HasBait();
                    string baitText = hasBait
                        ? (LocalizationRuntime.IsVietnamese ? " (Có Mồi)" : " (Baited)")
                        : string.Empty;

                    InteractionHint = LocalizationRuntime.IsVietnamese
                        ? $"[F] Thả câu ven sông{baitText}"
                        : $"[F] Cast Fishing Rod{baitText}";
                    ActionButtonLabel = LocalizationRuntime.IsVietnamese ? "Câu Cá" : "Fish";
                    CanFishAction = true;
                }
                else
                {
                    InteractionHint = LocalizationRuntime.IsVietnamese
                        ? "Bờ sông Valen mát rượi. Cần có Cần câu để câu cá."
                        : "Valen riverbank. Craft a Fishing Rod to catch fish.";
                    ActionButtonLabel = string.Empty;
                    CanFishAction = false;
                }
            }
            else
            {
                InteractionHint = string.Empty;
                ActionButtonLabel = string.Empty;
                CanFishAction = false;
            }
        }

        public void OnActionPressed()
        {
            if (state == FishingState.Idle)
            {
                if (CanFishAction && HasFishingRod())
                {
                    StartFishing();
                }
            }
            else if (state == FishingState.WaitingForBite)
            {
                // Cancel early
                CancelFishing();
            }
            else if (state == FishingState.Biting)
            {
                // Hooked!
                ReelIn();
            }
        }

        private void StartFishing()
        {
            if (fishingRoutine != null) StopCoroutine(fishingRoutine);
            fishingRoutine = StartCoroutine(FishingSequenceRoutine());
        }

        private IEnumerator FishingSequenceRoutine()
        {
            state = FishingState.Casting;
            TheOldRoad.Audio.AudioManager.PlayWaterSplash();

            Vector3 playerPos = transform.position;
            float riverY = GetRiverY(playerPos.x);
            // Spawn bobber inside the river body
            Vector3 bobberPos = new Vector3(playerPos.x + (playerMovement != null ? playerMovement.LastMoveDirection.x * 0.8f : 0f), riverY, 0f);

            currentBobber = FishingBobber.Spawn(bobberPos);

            // Check if player has bait (reduces wait time)
            bool usedBait = TryConsumeBait();
            float waitTime = usedBait ? Random.Range(2.0f, 3.8f) : Random.Range(3.5f, 6.0f);

            state = FishingState.WaitingForBite;
            yield return new WaitForSeconds(waitTime);

            if (currentBobber == null)
            {
                CancelFishing();
                yield break;
            }

            // Trigger Bite Window
            state = FishingState.Biting;
            currentBobber.TriggerBite();
            FloatingTextController.Spawn("!", currentBobber.transform.position + Vector3.up * 0.45f, Color.yellow, 1.2f);

            // Player has 1.8 seconds to react
            float biteWindow = 1.8f;
            float elapsed = 0f;

            while (elapsed < biteWindow && state == FishingState.Biting)
            {
                elapsed += UnityEngine.Time.deltaTime;
                yield return null;
            }

            if (state == FishingState.Biting)
            {
                // Missed the bite!
                FloatingTextController.Spawn(LocalizationRuntime.IsVietnamese ? "Cá chạy mất!" : "Fish got away!", transform.position + Vector3.up * 0.6f, Color.gray, 1.2f);
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                CancelFishing();
            }
        }

        private void ReelIn()
        {
            if (fishingRoutine != null) StopCoroutine(fishingRoutine);
            fishingRoutine = StartCoroutine(ReelInRoutine());
        }

        private IEnumerator ReelInRoutine()
        {
            state = FishingState.Reeling;
            TheOldRoad.Audio.AudioManager.PlayWaterSplash();

            yield return new WaitForSeconds(0.4f);

            // Determine catch loot
            (string itemId, int quantity, string catchName) = RollFishCatch();

            InventoryRuntime inv = inventorySession != null ? inventorySession.Runtime : null;
            if (inv != null)
            {
                inv.Add(itemId, quantity);
                TheOldRoad.Audio.AudioManager.PlayItemPickup();

                string msg = LocalizationRuntime.IsVietnamese
                    ? $"Bắt được {catchName} (+{quantity})!"
                    : $"Caught {catchName} (+{quantity})!";
                FloatingTextController.Spawn(msg, transform.position + Vector3.up * 0.8f, new Color(0.3f, 0.95f, 0.4f, 1f), 1.6f);
            }

            if (currentBobber != null)
            {
                currentBobber.Dismiss();
                currentBobber = null;
            }

            state = FishingState.Idle;
        }

        private (string itemId, int quantity, string catchName) RollFishCatch()
        {
            float roll = Random.value;

            if (roll < 0.08f)
            {
                // 8% Rare Sunken Chest
                return ("item.silver-coin", Random.Range(3, 7), LocalizationRuntime.IsVietnamese ? "Rương Báu Cổ Dưới Sông" : "Sunken River Chest");
            }
            else if (roll < 0.25f)
            {
                // 17% Golden Perch
                return ("item.fish-golden-perch", 1, LocalizationRuntime.IsVietnamese ? "Cá Vược Hoàng Kim" : "Golden Perch");
            }
            else if (roll < 0.60f)
            {
                // 35% River Salmon
                return ("item.fish-salmon", Random.Range(1, 3), LocalizationRuntime.IsVietnamese ? "Cá Hồi Sông" : "River Salmon");
            }
            else
            {
                // 40% Common Carp
                return ("item.fish-carp", Random.Range(1, 3), LocalizationRuntime.IsVietnamese ? "Cá Chép Sông" : "Common Carp");
            }
        }

        public void CancelFishing()
        {
            if (fishingRoutine != null)
            {
                StopCoroutine(fishingRoutine);
                fishingRoutine = null;
            }

            if (currentBobber != null)
            {
                currentBobber.Dismiss();
                currentBobber = null;
            }

            state = FishingState.Idle;
        }

        private bool HasFishingRod()
        {
            InventoryRuntime inv = inventorySession != null ? inventorySession.Runtime : null;
            return inv != null && inv.GetQuantity("item.fishing-rod") > 0;
        }

        private bool HasBait()
        {
            InventoryRuntime inv = inventorySession != null ? inventorySession.Runtime : null;
            return inv != null && inv.GetQuantity("item.fishing-bait") > 0;
        }

        private bool TryConsumeBait()
        {
            InventoryRuntime inv = inventorySession != null ? inventorySession.Runtime : null;
            if (inv != null && inv.GetQuantity("item.fishing-bait") > 0)
            {
                inv.TryRemove("item.fishing-bait", 1);
                return true;
            }
            return false;
        }

        public static float GetRiverY(float worldX)
        {
            return -12.5f - Mathf.Sin(worldX * 0.16f) * 2.0f;
        }

        private void OnDisable()
        {
            CancelFishing();
        }
    }
}
