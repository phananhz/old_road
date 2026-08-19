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
    public sealed class TownDeliveryOrder
    {
        public string orderId;
        public string clientNameVi;
        public string clientNameEn;
        public string taskVi;
        public string taskEn;
        public string requiredItemId;
        public int requiredAmount;
        public int rewardCoins;
        public string rewardBonusItemId;
        public int rewardBonusItemCount;
        public bool isCompleted;
    }

    /// <summary>
    /// Interactive Daily Bulletin Board offering dynamic town delivery orders.
    /// Provides daily goals and rewarding coin/seed/recipe bounties.
    /// </summary>
    public sealed class DailyBulletinBoardController : MonoBehaviour
    {
        [SerializeField] private InventorySession inventorySession;

        private Transform playerTransform;
        private SpriteRenderer boardRenderer;
        private List<TownDeliveryOrder> dailyOrders = new List<TownDeliveryOrder>();
        private bool isBoardOpen;

        public bool IsBoardOpen => isBoardOpen;
        public IReadOnlyList<TownDeliveryOrder> DailyOrders => dailyOrders;

        public void Configure(InventorySession session)
        {
            this.inventorySession = session;
            GenerateDefaultOrders();
        }

        private void Awake()
        {
            boardRenderer = GetComponent<SpriteRenderer>();
            if (boardRenderer == null) boardRenderer = gameObject.AddComponent<SpriteRenderer>();
            boardRenderer.sprite = PrototypePixelArtFactory.BulletinBoard();
            boardRenderer.sortingOrder = 9;

            BoxCollider2D col = GetComponent<BoxCollider2D>();
            if (col == null) col = gameObject.AddComponent<BoxCollider2D>();
            col.size = new Vector2(2.0f, 2.2f);
            col.offset = new Vector2(0f, 0.6f);

            if (dailyOrders.Count == 0) GenerateDefaultOrders();
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
            if (dist <= 2.2f)
            {
                if (TheOldRoad.Input.PrototypeInput.GetKeyDown(KeyCode.F))
                {
                    ToggleBoard();
                }
            }
            else if (isBoardOpen && dist > 3.5f)
            {
                isBoardOpen = false;
            }

            if (isBoardOpen && TheOldRoad.Input.PrototypeInput.GetKeyDown(KeyCode.Escape))
            {
                isBoardOpen = false;
            }
        }

        public void ToggleBoard()
        {
            isBoardOpen = !isBoardOpen;
            TheOldRoad.Audio.AudioManager.PlayUiClick();
        }

        public void CloseBoard()
        {
            isBoardOpen = false;
        }

        public bool TryDeliverOrder(int orderIndex)
        {
            if (orderIndex < 0 || orderIndex >= dailyOrders.Count) return false;
            TownDeliveryOrder order = dailyOrders[orderIndex];
            if (order.isCompleted) return false;

            InventoryRuntime inv = inventorySession != null ? inventorySession.Runtime : null;
            if (inv == null) return false;

            if (inv.GetQuantity(order.requiredItemId) < order.requiredAmount)
            {
                string msg = LocalizationRuntime.IsVietnamese 
                    ? "Chưa đủ số lượng vật phẩm yêu cầu!" 
                    : "Not enough items in inventory!";
                FloatingTextController.Spawn(msg, transform.position + Vector3.up * 1.5f, Color.red);
                return false;
            }

            inv.TryRemove(order.requiredItemId, order.requiredAmount);
            inv.Add("item.silver-coin", order.rewardCoins);

            if (!string.IsNullOrEmpty(order.rewardBonusItemId) && order.rewardBonusItemCount > 0)
            {
                inv.Add(order.rewardBonusItemId, order.rewardBonusItemCount);
            }

            order.isCompleted = true;
            AudioManager.PlayChestOpen();

            string successMsg = LocalizationRuntime.IsVietnamese 
                ? $"Hoàn thành đơn hàng! (+{order.rewardCoins} 🪙)" 
                : $"Delivered order! (+{order.rewardCoins} 🪙)";
            FloatingTextController.Spawn(successMsg, transform.position + Vector3.up * 1.5f, Color.yellow);
            return true;
        }

        private void GenerateDefaultOrders()
        {
            dailyOrders.Clear();

            // Order 1: Chef's Produce
            dailyOrders.Add(new TownDeliveryOrder
            {
                orderId = "order.chef.veggies",
                clientNameVi = "Bếp Trưởng Quán Rượu Valen",
                clientNameEn = "Valen Tavern Chef",
                taskVi = "Cần 3 Cà chua đỏ & 2 Cà rốt tươi để nấu tiệc đêm.",
                taskEn = "Needs 3 Ripe Tomatoes for the evening stew.",
                requiredItemId = "item.tomato",
                requiredAmount = 3,
                rewardCoins = 45,
                rewardBonusItemId = "item.cooked-meal",
                rewardBonusItemCount = 2,
                isCompleted = false
            });

            // Order 2: Fisherman's Bounty
            dailyOrders.Add(new TownDeliveryOrder
            {
                orderId = "order.fisher.catch",
                clientNameVi = "Lão Ngư Dân Bờ Sông",
                clientNameEn = "Old Riverside Angler",
                taskVi = "Thu mua 2 con Cá Hồi tươi ngon vừa câu trên sông Valen.",
                taskEn = "Purchasing 2 Fresh Salmon caught from River Valen.",
                requiredItemId = "item.fish-salmon",
                requiredAmount = 2,
                rewardCoins = 60,
                rewardBonusItemId = "item.fishing-bait",
                rewardBonusItemCount = 4,
                isCompleted = false
            });

            // Order 3: Dairy Delivery
            dailyOrders.Add(new TownDeliveryOrder
            {
                orderId = "order.dairy.bakery",
                clientNameVi = "Tiệm Bánh Mì Làng",
                clientNameEn = "Village Baker",
                taskVi = "Cần 2 Bình Sữa Bò tươi để nướng bánh bơ béo ngậy.",
                taskEn = "Needs 2 Fresh Milk bottles for butter pastries.",
                requiredItemId = "item.milk",
                requiredAmount = 2,
                rewardCoins = 50,
                rewardBonusItemId = "item.seed-wheat",
                rewardBonusItemCount = 5,
                isCompleted = false
            });

            // Order 4: Blacksmith's Ore
            dailyOrders.Add(new TownDeliveryOrder
            {
                orderId = "order.smith.iron",
                clientNameVi = "Bác Thợ Rèn Cổ",
                clientNameEn = "Village Blacksmith",
                taskVi = "Cần 4 Quặng Sắt thô để rèn nông cụ và móng ngựa.",
                taskEn = "Needs 4 Raw Iron Ore to forge sturdy farm tools.",
                requiredItemId = "item.iron-ore",
                requiredAmount = 4,
                rewardCoins = 75,
                rewardBonusItemId = "item.ammo-arrow",
                rewardBonusItemCount = 10,
                isCompleted = false
            });
        }
    }
}
