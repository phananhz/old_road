using NUnit.Framework;
using UnityEngine;
using TheOldRoad.Inventory;
using TheOldRoad.UI;
using TheOldRoad.World;

namespace TheOldRoad.Tests.EditMode
{
    public class RetentionTests
    {
        [Test]
        public void PrototypePixelArtFactory_RetentionSprites_AreNotNull()
        {
            Assert.IsNotNull(PrototypePixelArtFactory.BulletinBoard());
            Assert.IsNotNull(PrototypePixelArtFactory.Mailbox(true));
            Assert.IsNotNull(PrototypePixelArtFactory.Mailbox(false));
            Assert.IsNotNull(PrototypePixelArtFactory.MailLetter());
            Assert.IsNotNull(PrototypePixelArtFactory.CompendiumBadge());
            Assert.IsNotNull(PrototypePixelArtFactory.RewardChestIcon());
        }

        [Test]
        public void DailyBulletinBoard_GeneratesOrders_AndAllowsDelivery()
        {
            GameObject boardObj = new GameObject("TestBulletinBoard");
            DailyBulletinBoardController board = boardObj.AddComponent<DailyBulletinBoardController>();

            GameObject sessionObj = new GameObject("TestSession");
            InventorySession session = sessionObj.AddComponent<InventorySession>();
            board.Configure(session);

            Assert.IsNotEmpty(board.DailyOrders);
            Assert.AreEqual(4, board.DailyOrders.Count);

            var firstOrder = board.DailyOrders[0];
            Assert.IsFalse(firstOrder.isCompleted);

            // Add required item to inventory and deliver
            session.Runtime.Add(firstOrder.requiredItemId, firstOrder.requiredAmount);
            Assert.GreaterOrEqual(session.Runtime.GetQuantity(firstOrder.requiredItemId), firstOrder.requiredAmount);

            int startCoins = session.Runtime.GetQuantity("item.silver-coin");
            bool delivered = board.TryDeliverOrder(0);
            Assert.IsTrue(delivered);
            Assert.IsTrue(firstOrder.isCompleted);
            Assert.AreEqual(startCoins + firstOrder.rewardCoins, session.Runtime.GetQuantity("item.silver-coin"));

            // Cannot deliver twice
            Assert.IsFalse(board.TryDeliverOrder(0));

            Object.DestroyImmediate(boardObj);
            Object.DestroyImmediate(sessionObj);
        }

        [Test]
        public void DailyMailbox_ClaimsStreakGift_AddsCoinsAndItems()
        {
            GameObject mailboxObj = new GameObject("TestMailbox");
            DailyMailboxController mailbox = mailboxObj.AddComponent<DailyMailboxController>();

            GameObject sessionObj = new GameObject("TestSession");
            InventorySession session = sessionObj.AddComponent<InventorySession>();
            mailbox.Configure(session, streakDay: 1, unclaimed: true);

            Assert.IsTrue(mailbox.HasUnclaimedMail);
            Assert.AreEqual(1, mailbox.CurrentStreakDay);

            int initialCoins = session.Runtime.GetQuantity("item.silver-coin");
            bool claimed = mailbox.TryClaimTodayReward();
            Assert.IsTrue(claimed);
            Assert.IsFalse(mailbox.HasUnclaimedMail);
            Assert.Greater(session.Runtime.GetQuantity("item.silver-coin"), initialCoins);

            // Cannot claim again today
            Assert.IsFalse(mailbox.TryClaimTodayReward());

            Object.DestroyImmediate(mailboxObj);
            Object.DestroyImmediate(sessionObj);
        }

        [Test]
        public void CompendiumCatalog_TracksAllCategories()
        {
            var all = CompendiumCatalog.GetAll();
            Assert.GreaterOrEqual(all.Count, 15);
            Assert.Greater(CompendiumCatalog.TotalCount, 0);

            InventoryRuntime inventory = new InventoryRuntime();
            inventory.Add("item.wheat", 5);
            inventory.Add("item.fish-salmon", 1);

            int discovered = CompendiumCatalog.GetDiscoveredCount(inventory);
            Assert.Greater(discovered, 0);
        }

        [Test]
        public void DiscoverableLandmark_AutoDiscovery_AndEmojiColor()
        {
            GameObject landmarkObj = new GameObject("TestLandmark");
            DiscoverableLandmark landmark = landmarkObj.AddComponent<DiscoverableLandmark>();
            landmark.Configure(
                "landmark.village.valen",
                "Valen Village",
                "A peaceful village.",
                false,
                "🏘️",
                new Color(0.2f, 0.8f, 0.9f, 1f),
                8.0f);

            Assert.IsFalse(landmark.IsDiscovered);
            Assert.AreEqual("🏘️", landmark.MapIconEmoji);
            Assert.AreEqual("landmark.village.valen", landmark.LandmarkId);

            bool disc = landmark.Discover();
            Assert.IsTrue(disc);
            Assert.IsTrue(landmark.IsDiscovered);

            Object.DestroyImmediate(landmarkObj);
        }
    }
}
