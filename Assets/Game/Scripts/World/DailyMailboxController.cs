using System;
using System.Collections.Generic;
using UnityEngine;
using TheOldRoad.Audio;
using TheOldRoad.Combat;
using TheOldRoad.Inventory;
using TheOldRoad.UI;
using TheOldRoad.World;

namespace TheOldRoad.World
{
    [Serializable]
    public sealed class DailyStreakReward
    {
        public int dayNumber;
        public string titleVi;
        public string titleEn;
        public string rewardItemId;
        public int rewardItemCount;
        public int rewardCoins;
    }

    /// <summary>
    /// Interactive Daily Countryside Mailbox providing daily login streak gifts,
    /// rare seed bundles, fishing gear, and coin bonuses.
    /// </summary>
    public sealed class DailyMailboxController : MonoBehaviour
    {
        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private int currentStreakDay = 1;
        [SerializeField] private bool hasUnclaimedMail = true;

        private Transform playerTransform;
        private SpriteRenderer mailboxRenderer;
        private bool isMailOpen;
        private List<DailyStreakReward> streakRewards = new List<DailyStreakReward>();

        public bool IsMailOpen => isMailOpen;
        public bool HasUnclaimedMail => hasUnclaimedMail;
        public int CurrentStreakDay => currentStreakDay;
        public IReadOnlyList<DailyStreakReward> StreakRewards => streakRewards;

        public void Configure(InventorySession session, int streakDay = 1, bool unclaimed = true)
        {
            this.inventorySession = session;
            this.currentStreakDay = Mathf.Clamp(streakDay, 1, 7);
            this.hasUnclaimedMail = unclaimed;
            EnsureStreakRewards();
            UpdateVisual();
        }

        private void Awake()
        {
            mailboxRenderer = GetComponent<SpriteRenderer>();
            if (mailboxRenderer == null) mailboxRenderer = gameObject.AddComponent<SpriteRenderer>();
            mailboxRenderer.sortingOrder = 9;

            BoxCollider2D col = GetComponent<BoxCollider2D>();
            if (col == null) col = gameObject.AddComponent<BoxCollider2D>();
            col.size = new Vector2(1.5f, 1.8f);
            col.offset = new Vector2(0f, 0.4f);

            EnsureStreakRewards();
            UpdateVisual();
        }

        private void Update()
        {
            if (playerTransform == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null) playerTransform = player.transform;
                return;
            }

            float dist = Vector2.Distance(playerTransform.position, transform.position);
            if (dist <= 2.0f)
            {
                if (TheOldRoad.Input.PrototypeInput.GetKeyDown(KeyCode.F))
                {
                    ToggleMail();
                }
            }
            else if (isMailOpen && dist > 3.2f)
            {
                isMailOpen = false;
            }

            if (isMailOpen && TheOldRoad.Input.PrototypeInput.GetKeyDown(KeyCode.Escape))
            {
                isMailOpen = false;
            }
        }

        public void ToggleMail()
        {
            isMailOpen = !isMailOpen;
            TheOldRoad.Audio.AudioManager.PlayUiClick();
        }

        public void CloseMail()
        {
            isMailOpen = false;
        }

        public bool TryClaimTodayReward()
        {
            if (!hasUnclaimedMail) return false;

            InventoryRuntime inv = inventorySession != null ? inventorySession.Runtime : null;
            if (inv == null) return false;

            EnsureStreakRewards();
            DailyStreakReward reward = streakRewards.Find(r => r.dayNumber == currentStreakDay);
            if (reward == null) reward = streakRewards[0];

            if (reward.rewardCoins > 0)
            {
                inv.Add("item.silver-coin", reward.rewardCoins);
            }

            if (!string.IsNullOrEmpty(reward.rewardItemId) && reward.rewardItemCount > 0)
            {
                inv.Add(reward.rewardItemId, reward.rewardItemCount);
            }

            hasUnclaimedMail = false;
            UpdateVisual();
            AudioManager.PlayChestOpen();

            string claimMsg = LocalizationRuntime.IsVietnamese 
                ? $"Đã nhận quà Ngày {currentStreakDay}! (+{reward.rewardCoins} 🪙)" 
                : $"Claimed Day {currentStreakDay} Gift! (+{reward.rewardCoins} 🪙)";
            FloatingTextController.Spawn(claimMsg, transform.position + Vector3.up * 1.4f, Color.green);
            return true;
        }

        private void UpdateVisual()
        {
            if (mailboxRenderer != null)
            {
                mailboxRenderer.sprite = PrototypePixelArtFactory.Mailbox(hasUnclaimedMail);
            }
        }

        private void EnsureStreakRewards()
        {
            if (streakRewards.Count > 0) return;

            streakRewards.Add(new DailyStreakReward
            {
                dayNumber = 1,
                titleVi = "Gói Hạt Giống Đồng Quê",
                titleEn = "Starter Crop Seeds",
                rewardItemId = "item.seed-wheat",
                rewardItemCount = 5,
                rewardCoins = 25
            });

            streakRewards.Add(new DailyStreakReward
            {
                dayNumber = 2,
                titleVi = "Hộp Mồi Câu Sông Valen",
                titleEn = "River Fishing Baits",
                rewardItemId = "item.fishing-bait",
                rewardItemCount = 5,
                rewardCoins = 35
            });

            streakRewards.Add(new DailyStreakReward
            {
                dayNumber = 3,
                titleVi = "Túi Hạt Giống Cà Rốt Ngọt",
                titleEn = "Crisp Carrot Seeds",
                rewardItemId = "item.seed-carrot",
                rewardItemCount = 4,
                rewardCoins = 45
            });

            streakRewards.Add(new DailyStreakReward
            {
                dayNumber = 4,
                titleVi = "Giỏ Dứa Nhiệt Đới Mọng",
                titleEn = "Tropical Pineapples",
                rewardItemId = "item.pineapple",
                rewardItemCount = 2,
                rewardCoins = 60
            });

            streakRewards.Add(new DailyStreakReward
            {
                dayNumber = 5,
                titleVi = "Phần Cá Nướng Bổ Dưỡng",
                titleEn = "Grilled Fish Platter",
                rewardItemId = "item.cooked-fish",
                rewardItemCount = 2,
                rewardCoins = 75
            });

            streakRewards.Add(new DailyStreakReward
            {
                dayNumber = 6,
                titleVi = "Thỏi Quặng Sắt Tinh Luyện",
                titleEn = "Iron Ore Bundle",
                rewardItemId = "item.iron-ore",
                rewardItemCount = 5,
                rewardCoins = 90
            });

            streakRewards.Add(new DailyStreakReward
            {
                dayNumber = 7,
                titleVi = "Rương Kho Báu Hiệp Sĩ",
                titleEn = "Knight's Treasury Chest",
                rewardItemId = "item.cooked-meal",
                rewardItemCount = 3,
                rewardCoins = 150
            });
        }
    }
}
