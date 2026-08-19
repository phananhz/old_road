using NUnit.Framework;
using UnityEngine;
using TheOldRoad.Fishing;
using TheOldRoad.Items;
using TheOldRoad.World;

namespace TheOldRoad.Tests.EditMode
{
    public class FishingTests
    {
        [Test]
        public void RiverY_CalculatesConsistentWaterCoordinates()
        {
            float yAtOrigin = PlayerFishingInteractor.GetRiverY(0f);
            Assert.AreEqual(-12.5f, yAtOrigin, 0.001f);

            float yAtTen = PlayerFishingInteractor.GetRiverY(10f);
            Assert.IsTrue(yAtTen < 0f, "River should be in the southern region of the world map");
        }

        [Test]
        public void FishingItems_AreDefinedInItemCatalog()
        {
            Assert.IsTrue(PrototypeItemCatalog.TryGet("item.fishing-rod", out PrototypeItemInfo rod));
            Assert.AreEqual("item.fishing-rod", rod.ItemId);

            Assert.IsTrue(PrototypeItemCatalog.TryGet("item.fishing-bait", out PrototypeItemInfo bait));
            Assert.AreEqual("item.fishing-bait", bait.ItemId);

            Assert.IsTrue(PrototypeItemCatalog.TryGet("item.fish-salmon", out PrototypeItemInfo salmon));
            Assert.AreEqual("item.fish-salmon", salmon.ItemId);

            Assert.IsTrue(PrototypeItemCatalog.TryGet("item.fish-carp", out PrototypeItemInfo carp));
            Assert.AreEqual("item.fish-carp", carp.ItemId);

            Assert.IsTrue(PrototypeItemCatalog.TryGet("item.fish-golden-perch", out PrototypeItemInfo perch));
            Assert.AreEqual("item.fish-golden-perch", perch.ItemId);

            Assert.IsTrue(PrototypeItemCatalog.TryGet("item.cooked-fish", out PrototypeItemInfo cooked));
            Assert.AreEqual("item.cooked-fish", cooked.ItemId);
        }

        [Test]
        public void FishingPixelArt_GeneratesValidSprites()
        {
            Sprite bobber = PrototypePixelArtFactory.FishingBobberSprite;
            Assert.IsNotNull(bobber);

            Sprite splash = PrototypePixelArtFactory.WaterSplashSprite;
            Assert.IsNotNull(splash);

            Sprite salmon = PrototypePixelArtFactory.ItemFishSalmon();
            Assert.IsNotNull(salmon);

            Sprite rod = PrototypePixelArtFactory.ItemFishingRod();
            Assert.IsNotNull(rod);
        }
    }
}
