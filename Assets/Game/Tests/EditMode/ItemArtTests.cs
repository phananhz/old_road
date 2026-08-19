using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using TheOldRoad.Building;
using TheOldRoad.Items;
using TheOldRoad.World;

namespace TheOldRoad.Tests.EditMode
{
    public class ItemArtTests
    {
        private static readonly string[] PrototypeItemIds =
        {
            "item.wood",
            "item.stone",
            "item.cabin-plank",
            "item.wild-berries",
            "item.medicinal-herb",
            "item.mushroom",
            "item.iron-ore",
            "item.old-coin",
            "item.torch",
            "item.tool-axe",
            "item.tool-pickaxe",
            "item.tool-hoe",
            "item.roadwarden-page",
            "item.bell-fragment",
            "item.cooked-meal",
            "item.egg",
            "item.wool",
            "item.milk",
            "item.silver-coin",
            "item.watering-can",
            "item.seed-wheat",
            "item.seed-corn",
            "item.seed-carrot",
            "item.seed-potato",
            "item.wheat",
            "item.corn",
            "item.carrot",
            "item.potato",
            "item.fence-wood",
            "item.gate-wood",
            "item.seed-pineapple",
            "item.seed-tomato",
            "item.pineapple",
            "item.tomato",
            "item.fishing-rod",
            "item.fishing-bait",
            "item.fish-salmon",
            "item.fish-carp",
            "item.fish-golden-perch",
            "item.cooked-fish",
            "item.weapon-sword",
            "item.weapon-bow",
            "item.ammo-arrow",
            "item.shield-wood",
            "item.armor-knight",
            "item.meat-raw",
            "item.leather",
            "item.hay",
            "item.farm-deed"
        };

        [Test]
        public void PrototypePixelArtFactory_GeneratesValid16x16ItemTextures_ForAllCatalogItems()
        {
            foreach (string itemId in PrototypeItemIds)
            {
                Texture2D texture = PrototypePixelArtFactory.ItemIconTexture(itemId);
                Assert.IsNotNull(texture, $"Texture should not be null for item id: {itemId}");
                Assert.AreEqual(16, texture.width, $"Texture width should be 16 for item id: {itemId}");
                Assert.AreEqual(16, texture.height, $"Texture height should be 16 for item id: {itemId}");
            }
        }

        [Test]
        public void PrototypePixelArtFactory_GeneratesValidFarmingAndRuinsSprites()
        {
            Assert.IsNotNull(PrototypePixelArtFactory.TilledSoil(false));
            Assert.IsNotNull(PrototypePixelArtFactory.TilledSoil(true));
            Assert.IsNotNull(PrototypePixelArtFactory.Crop("wheat", 0));
            Assert.IsNotNull(PrototypePixelArtFactory.Crop("wheat", 3));
            Assert.IsNotNull(PrototypePixelArtFactory.Crop("corn", 3));
            Assert.IsNotNull(PrototypePixelArtFactory.Crop("carrot", 3));
            Assert.IsNotNull(PrototypePixelArtFactory.Crop("pineapple", 3));
            Assert.IsNotNull(PrototypePixelArtFactory.Crop("tomato", 3));
            Assert.IsNotNull(PrototypePixelArtFactory.WoodFence());
            Assert.IsNotNull(PrototypePixelArtFactory.WoodFenceHorizontal());
            Assert.IsNotNull(PrototypePixelArtFactory.WoodFenceVertical());
            Assert.IsNotNull(PrototypePixelArtFactory.WoodFenceCorner());
            Assert.IsNotNull(PrototypePixelArtFactory.WoodGate(false));
            Assert.IsNotNull(PrototypePixelArtFactory.WoodGate(true));
            Assert.IsNotNull(PrototypePixelArtFactory.GateLantern(true));
            Assert.IsNotNull(PrototypePixelArtFactory.GateLantern(false));
            Assert.IsNotNull(PrototypePixelArtFactory.FarmSignboard());
            Assert.IsNotNull(PrototypePixelArtFactory.PathDirtTile());
            Assert.IsNotNull(PrototypePixelArtFactory.PathCobblestoneTile());
            Assert.IsNotNull(PrototypePixelArtFactory.Scarecrow());
            Assert.IsNotNull(PrototypePixelArtFactory.SelectionTileHighlight());
            Assert.IsNotNull(PrototypePixelArtFactory.HappyFarmBarn());
            Assert.IsNotNull(PrototypePixelArtFactory.DairyCow());
            Assert.IsNotNull(PrototypePixelArtFactory.StrawNest(true));
            Assert.IsNotNull(PrototypePixelArtFactory.StrawNest(false));
            Assert.IsNotNull(PrototypePixelArtFactory.FarmDog());
            Assert.IsNotNull(PrototypePixelArtFactory.FarmShopSign());
            Assert.IsNotNull(PrototypePixelArtFactory.HerbalistHut());
            Assert.IsNotNull(PrototypePixelArtFactory.LookoutTower());
            Assert.IsNotNull(PrototypePixelArtFactory.NightMonsterSprite(0));
            Assert.IsNotNull(PrototypePixelArtFactory.BellTowerRuins());
            Assert.IsNotNull(PrototypePixelArtFactory.PuzzlePedestal(0, true));
            Assert.IsNotNull(PrototypePixelArtFactory.MerchantCart());
            Assert.IsNotNull(PrototypePixelArtFactory.SilverCoinIcon());
            Assert.IsNotNull(PrototypePixelArtFactory.HeartEmote());

            // Knight Player Walk Sprites (0..3)
            for (int f = 0; f < 4; f++)
            {
                Sprite playerWalk = PrototypePixelArtFactory.PlayerWalk(f);
                Assert.IsNotNull(playerWalk, $"Player walk frame {f} should not be null");
                Assert.AreEqual(16, playerWalk.texture.width);
                Assert.AreEqual(24, playerWalk.texture.height);
            }

            // Title Knight Sunset Panorama Artwork
            Sprite panorama = PrototypePixelArtFactory.TitleKnightSunsetPanorama();
            Assert.IsNotNull(panorama, "Title Knight Sunset Panorama sprite should not be null");
            Assert.AreEqual(512, panorama.texture.width);
            Assert.AreEqual(216, panorama.texture.height);
            Assert.IsNotNull(PrototypePixelArtFactory.TitleKnightSunsetTexture());
        }

        [Test]
        public void ItemDefinition_ExposesIconProperty_WhenConfigured()
        {
            ItemDefinition item = ScriptableObject.CreateInstance<ItemDefinition>();
            Sprite dummySprite = Sprite.Create(new Texture2D(16, 16), new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f));

            item.ConfigureForPrototype("item.test", "Test Item", 99, dummySprite);

            Assert.AreEqual("item.test", item.ItemId);
            Assert.AreEqual("Test Item", item.DisplayName);
            Assert.AreEqual(99, item.MaxStack);
            Assert.AreEqual(dummySprite, item.Icon);
        }

        [Test]
        public void BuildingDefinition_ExposesCompleteAndStageSprites_WhenConfigured()
        {
            BuildingDefinition building = ScriptableObject.CreateInstance<BuildingDefinition>();
            Sprite complete = Sprite.Create(new Texture2D(32, 32), new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
            Sprite[] stages = new[]
            {
                Sprite.Create(new Texture2D(32, 32), new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f)),
                Sprite.Create(new Texture2D(32, 32), new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f))
            };

            building.ConfigureForPrototype(
                "building.test",
                new Vector2Int(2, 2),
                new BuildCostEntry[0],
                10f,
                new[] { "Stage1", "Stage2" },
                complete,
                stages);

            Assert.AreEqual("building.test", building.BuildingId);
            Assert.AreEqual(complete, building.CompleteSprite);
            Assert.AreEqual(stages, building.StageSprites);
        }

        [Test]
        public void PrototypePixelArtFactory_AllExportableSprites_ContainsCompleteAssetCatalog()
        {
            var sprites = PrototypePixelArtFactory.AllExportableSprites;
            Assert.IsNotEmpty(sprites);
            Assert.GreaterOrEqual(sprites.Count(), 30);
        }
    }
}
