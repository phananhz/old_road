using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using TheOldRoad.Building;
using TheOldRoad.World;

namespace TheOldRoad.Tests.EditMode
{
    public class BuildingVisualConsistencyTests
    {
        private static readonly string[] AllBuildingIds =
        {
            // Category 0: Housing & Lodges
            "building.cabin",
            "building.stone-cottage",
            "building.farm-barn",
            "building.storage-shed",
            "building.herbalist-hut",
            "building.lookout-tower",
            "building.tent",
            "building.manor",
            "building.greenhouse",
            "building.silo",

            // Category 1: Fire & Lighting
            "building.campfire",
            "building.cooking-hearth",
            "building.street-lamp",
            "building.ground-torch",
            "building.lantern-pole",
            "building.stone-fireplace",

            // Category 2: Animal Husbandry
            "building.animal-pen-small",
            "building.animal-pen-long",
            "building.sheep-pasture",
            "building.hen-coop",
            "building.feed-trough",
            "building.water-trough",

            // Category 3: Fences & Walls
            "building.fence",
            "building.fence-vertical",
            "building.gate",
            "building.stone-wall",
            "building.iron-gate",
            "building.log-palisade",

            // Category 4: Paths & Bridges
            "building.path-dirt",
            "building.path-cobblestone",
            "building.path-wood",
            "building.path-stone-tile",
            "building.wood-bridge",
            "building.scarecrow",

            // Category 5: Furniture & Living
            "building.straw-bed",
            "building.oak-table",
            "building.leather-chair",
            "building.bookshelf",
            "building.woven-rug",

            // Category 6: Artisan & Processing
            "building.cheese-press",
            "building.loom",
            "building.keg",
            "building.windmill",
            "building.blacksmith-forge",
            "building.carpenter-bench",

            // Category 7: Storage & Logistics
            "building.wood-chest",
            "building.stone-vault",
            "building.compost-bin",
            "building.barrel-rack",

            // Category 8: Gardening & Greenery
            "building.grape-trellis",
            "building.pumpkin-patch",
            "building.flower-planter",
            "building.garden-hedge",

            // Category 9: Water & Irrigation
            "building.ancient-well",
            "building.water-aqueduct",
            "building.stone-fountain",
            "building.hot-bath",

            // Category 10: Monuments & Shrines
            "building.knight-statue",
            "building.guardian-shrine",
            "building.bell-pillar",

            // Category 11: Market & Commerce
            "building.market-stall",
            "building.farm-sign",
            "building.travel-cart",

            // Category 12: Defenses & Traps
            "building.spike-trap",
            "building.wooden-barricade",
            "building.alarm-bell",

            // Category 13: Leisure & Camping
            "building.wood-swing",
            "building.chess-table",
            "building.hammock",
            "building.bbq-grill",

            // Category 14: Festivals & Ornaments
            "building.festival-banner",
            "building.sky-lantern",
            "building.firefly-jar"
        };

        [Test]
        public void EveryBuildingDefinition_HasValidAndDistinctCatalogSprite()
        {
            HashSet<Sprite> uniqueSprites = new HashSet<Sprite>();

            foreach (string buildingId in AllBuildingIds)
            {
                Sprite sprite = PrototypePixelArtFactory.BuildingCatalogIcon(buildingId);
                Assert.IsNotNull(sprite, $"BuildingCatalogIcon for '{buildingId}' must not be null.");
                Assert.IsNotNull(sprite.texture, $"Texture for '{buildingId}' sprite must not be null.");
                Assert.Greater(sprite.rect.width, 0, $"Sprite width for '{buildingId}' must be > 0.");
                Assert.Greater(sprite.rect.height, 0, $"Sprite height for '{buildingId}' must be > 0.");

                uniqueSprites.Add(sprite);
            }

            // Verify that we have high visual diversity (over 50 unique dedicated sprites)
            Assert.GreaterOrEqual(uniqueSprites.Count, 50, "Buildings should have dedicated unique pixel art sprites rather than shared fallbacks.");
        }

        [Test]
        public void BuildingComplete_Matches_BuildingCatalogIcon()
        {
            foreach (string buildingId in AllBuildingIds)
            {
                Sprite completeSprite = PrototypePixelArtFactory.BuildingComplete(buildingId);
                Sprite catalogSprite = PrototypePixelArtFactory.BuildingCatalogIcon(buildingId);

                Assert.IsNotNull(completeSprite, $"BuildingComplete for '{buildingId}' must not be null.");
                Assert.AreEqual(completeSprite, catalogSprite, $"World building sprite and catalog icon must match 1:1 for '{buildingId}'.");
            }
        }

        [Test]
        public void AllBuildingDefinitions_HavePositiveDurationAndValidCosts()
        {
            GameObject go = new GameObject("TestSlice");
            var slice = go.AddComponent<TheOldRoad.Core.VerticalSliceController>();

            foreach (string buildingId in AllBuildingIds)
            {
                BuildingDefinition def = slice.GetBuildingDefinition(buildingId);
                if (def == null) continue;

                Assert.Greater(def.ConstructionDurationSeconds, 0f, $"Building '{buildingId}' must have a positive construction duration (> 0s).");
                Assert.Greater(def.Footprint.x, 0, $"Building '{buildingId}' footprint.x must be > 0.");
                Assert.Greater(def.Footprint.y, 0, $"Building '{buildingId}' footprint.y must be > 0.");
                Assert.IsNotNull(def.ConstructionCosts, $"Building '{buildingId}' must have defined construction costs.");
                Assert.Greater(def.ConstructionCosts.Length, 0, $"Building '{buildingId}' must require at least 1 material.");
            }

            Object.DestroyImmediate(go);
        }
    }
}
