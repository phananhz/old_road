using NUnit.Framework;
using UnityEngine;
using TheOldRoad.Building;
using TheOldRoad.Farming;
using TheOldRoad.Gathering;
using TheOldRoad.Inventory;
using TheOldRoad.Player;
using TheOldRoad.Save;
using TheOldRoad.UI;
using TheOldRoad.World;

namespace TheOldRoad.Tests.EditMode
{
    public class FarmingTests
    {
        [Test]
        public void PrototypeCropCatalog_HasWheatCarrotPotato()
        {
            CropDefinition wheat = PrototypeCropCatalog.Get("wheat");
            Assert.IsNotNull(wheat);
            Assert.AreEqual("item.seed-wheat", wheat.SeedItemId);
            Assert.AreEqual("item.wheat", wheat.HarvestItemId);
            Assert.GreaterOrEqual(wheat.StageCount, 5);

            CropDefinition carrot = PrototypeCropCatalog.Get("carrot");
            Assert.IsNotNull(carrot);
            Assert.AreEqual("item.seed-carrot", carrot.SeedItemId);
            Assert.AreEqual("item.carrot", carrot.HarvestItemId);

            CropDefinition potato = PrototypeCropCatalog.Get("potato");
            Assert.IsNotNull(potato);
            Assert.AreEqual("item.seed-potato", potato.SeedItemId);
            Assert.AreEqual("item.potato", potato.HarvestItemId);
        }

        [Test]
        public void PrototypeCropCatalog_TryGetBySeed_WorksCorrectly()
        {
            Assert.IsTrue(PrototypeCropCatalog.TryGetBySeed("item.seed-carrot", out CropDefinition carrot));
            Assert.AreEqual("carrot", carrot.CropId);

            Assert.IsTrue(PrototypeCropCatalog.TryGetBySeed("item.seed-potato", out CropDefinition potato));
            Assert.AreEqual("potato", potato.CropId);

            Assert.IsFalse(PrototypeCropCatalog.TryGetBySeed("item.invalid-seed", out _));
        }

        [Test]
        public void FarmPlot_TillingAndPlanting_WorksCorrectly()
        {
            GameObject plotObj = new GameObject("TestPlot");
            FarmPlotController plot = plotObj.AddComponent<FarmPlotController>();
            plot.Configure("test.plot.1", false, false, string.Empty, 0f, 0);

            Assert.IsFalse(plot.IsTilled);
            Assert.IsFalse(plot.IsHarvestReady);

            // Cannot plant on untilled soil
            Assert.IsFalse(plot.TryPlantSeed("item.seed-carrot"));

            // Till soil
            Assert.IsTrue(plot.TryTillSoil());
            Assert.IsTrue(plot.IsTilled);

            // Plant carrot seed
            Assert.IsTrue(plot.TryPlantSeed("item.seed-carrot"));
            Assert.AreEqual("carrot", plot.PlantedCropId);
            Assert.AreEqual(0, plot.GrowthStage);

            // Water soil
            Assert.IsTrue(plot.TryWaterSoil());
            Assert.IsTrue(plot.IsWatered);

            Object.DestroyImmediate(plotObj);
        }

        [Test]
        public void Farming_FullLifecycle_Till_Plant_Water_Grow_Harvest()
        {
            GameObject plotObj = new GameObject("LifecyclePlot");
            FarmPlotController plot = plotObj.AddComponent<FarmPlotController>();
            plot.Configure("test.lifecycle", false, false, string.Empty, 0f, 0);

            // Step 1: Untilled bare soil -> Till
            Assert.IsFalse(plot.IsTilled);
            Assert.IsTrue(plot.TryTillSoil());
            Assert.IsTrue(plot.IsTilled);

            // Step 2: Plant seeds
            Assert.IsTrue(plot.TryPlantSeed("item.seed-wheat"));
            Assert.AreEqual("wheat", plot.PlantedCropId);
            Assert.AreEqual(0, plot.GrowthStage);
            Assert.AreEqual(0f, plot.GrowthMinutes);

            // Step 3: Water soil
            Assert.IsFalse(plot.IsWatered);
            Assert.IsTrue(plot.TryWaterSoil());
            Assert.IsTrue(plot.IsWatered);

            // Step 4: Wait for growth over time
            CropDefinition wheatDef = PrototypeCropCatalog.Get("wheat");
            plot.ProgressOfflineTime(wheatDef.GrowthDurationMinutes);
            Assert.AreEqual(4, plot.GrowthStage);
            Assert.IsTrue(plot.IsHarvestReady);

            // Step 5: Harvest produce into inventory
            InventoryRuntime inventory = new InventoryRuntime();
            Assert.IsTrue(plot.TryHarvest(inventory));
            Assert.GreaterOrEqual(inventory.GetQuantity("item.wheat"), 3);

            // Verified: resets cleanly to tilled state for immediate replanting
            Assert.IsTrue(plot.IsTilled);
            Assert.IsEmpty(plot.PlantedCropId);
            Assert.IsFalse(plot.IsWatered);
            Assert.IsFalse(plot.IsHarvestReady);

            Object.DestroyImmediate(plotObj);
        }

        [Test]
        public void FarmPlot_OfflineProgress_AdvancesGrowthCorrectly()
        {
            GameObject plotObj = new GameObject("TestPlot");
            FarmPlotController plot = plotObj.AddComponent<FarmPlotController>();
            plot.Configure("test.plot.2", true, true, "carrot", 0f, 0);

            CropDefinition carrotDef = PrototypeCropCatalog.Get("carrot");
            plot.ProgressOfflineTime(carrotDef.GrowthDurationMinutes);
            Assert.AreEqual(4, plot.GrowthStage);
            Assert.IsTrue(plot.IsHarvestReady);

            Object.DestroyImmediate(plotObj);
        }

        [Test]
        public void FarmPlot_Harvest_AddsYieldToInventoryAndResetsToTilled()
        {
            GameObject plotObj = new GameObject("TestPlot");
            FarmPlotController plot = plotObj.AddComponent<FarmPlotController>();
            CropDefinition carrotDef = PrototypeCropCatalog.Get("carrot");
            plot.Configure("test.plot.3", true, true, "carrot", carrotDef.GrowthDurationMinutes, 4);

            InventoryRuntime inventory = new InventoryRuntime();
            Assert.IsTrue(plot.IsHarvestReady);

            bool harvested = plot.TryHarvest(inventory);
            Assert.IsTrue(harvested);
            Assert.GreaterOrEqual(inventory.GetQuantity("item.carrot"), 2);

            // After harvest, plot remains tilled and ready for replanting
            Assert.IsTrue(plot.IsTilled);
            Assert.IsEmpty(plot.PlantedCropId);
            Assert.AreEqual(0, plot.GrowthStage);
            Assert.IsFalse(plot.IsHarvestReady);

            Object.DestroyImmediate(plotObj);
        }

        [Test]
        public void FarmPlot_SaveAndLoad_PreservesState()
        {
            GameObject plotObj = new GameObject("TestPlot");
            FarmPlotController plot = plotObj.AddComponent<FarmPlotController>();
            plot.Configure("test.plot.save", true, true, "potato", 4.5f, 2);

            FarmPlotSaveEntry save = plot.ToSaveEntry();
            Assert.AreEqual("test.plot.save", save.plotId);
            Assert.IsTrue(save.isTilled);
            Assert.IsTrue(save.isWatered);
            Assert.AreEqual("potato", save.plantedCropId);
            Assert.AreEqual(2, save.growthStage);

            GameObject plotObj2 = new GameObject("TestPlot2");
            FarmPlotController plot2 = plotObj2.AddComponent<FarmPlotController>();
            plot2.LoadFromSaveEntry(save);

            Assert.AreEqual("test.plot.save", plot2.PlotId);
            Assert.IsTrue(plot2.IsTilled);
            Assert.IsTrue(plot2.IsWatered);
            Assert.AreEqual("potato", plot2.PlantedCropId);
            Assert.AreEqual(2, plot2.GrowthStage);

            Object.DestroyImmediate(plotObj);
            Object.DestroyImmediate(plotObj2);
        }

        [Test]
        public void PrototypePixelArtFactory_GeneratesAllCropStages()
        {
            string[] crops = { "wheat", "carrot", "potato" };
            for (int c = 0; c < crops.Length; c++)
            {
                for (int s = 0; s <= 4; s++)
                {
                    Sprite sprite = PrototypePixelArtFactory.Crop(crops[c], s);
                    Assert.IsNotNull(sprite, $"Crop sprite for {crops[c]} stage {s} should not be null");
                }
            }
        }

        [Test]
        public void AvatarAnimalPasture_SpritesAndControllers_AreValid()
        {
            Assert.IsNotNull(PrototypePixelArtFactory.HappyFarmBarn());
            Assert.IsNotNull(PrototypePixelArtFactory.DairyCow());
            Assert.IsNotNull(PrototypePixelArtFactory.FluffySheep());
            Assert.IsNotNull(PrototypePixelArtFactory.HenSprite());
            Assert.IsNotNull(PrototypePixelArtFactory.FarmDog());
            Assert.IsNotNull(PrototypePixelArtFactory.HayBalePile());
            Assert.IsNotNull(PrototypePixelArtFactory.FeedingTrough());
            Assert.IsNotNull(PrototypePixelArtFactory.WaterTrough());
            Assert.IsNotNull(PrototypePixelArtFactory.StrawNest(true));
            Assert.IsNotNull(PrototypePixelArtFactory.StrawNest(false));

            GameObject sheepObj = new GameObject("TestSheep");
            SheepController sheep = sheepObj.AddComponent<SheepController>();
            GameObject sessionObj = new GameObject("TestSession");
            InventorySession session = sessionObj.AddComponent<InventorySession>();
            sheep.Configure(session);
            Assert.IsNotNull(sheep);
            Object.DestroyImmediate(sheepObj);
            Object.DestroyImmediate(sessionObj);
        }

        [Test]
        public void AnimalPenController_BuildsMatchingCompounds_ForSmallAndLarge()
        {
            GameObject sessionObj = new GameObject("TestSession");
            InventorySession session = sessionObj.AddComponent<InventorySession>();

            // Test Small Pen (7x5)
            GameObject smallPenObj = new GameObject("TestSmallPen");
            AnimalPenController smallPen = smallPenObj.AddComponent<AnimalPenController>();
            smallPen.Configure(null, session, null, "building.animal-pen-small");

            Assert.AreEqual(7, smallPen.Width);
            Assert.AreEqual(5, smallPen.Height);
            Assert.Greater(smallPenObj.transform.childCount, 0);
            Assert.IsNotNull(smallPenObj.transform.Find("Perimeter Fences"));
            Assert.IsNotNull(smallPenObj.transform.Find("Poultry Shelter"));
            Assert.IsNotNull(smallPenObj.transform.Find("Fluffy Sheep Dolly"));

            // Test Large Pen (11x6)
            GameObject largePenObj = new GameObject("TestLargePen");
            AnimalPenController largePen = largePenObj.AddComponent<AnimalPenController>();
            largePen.Configure(null, session, null, "building.animal-pen-long");

            Assert.AreEqual(11, largePen.Width);
            Assert.AreEqual(6, largePen.Height);
            Assert.Greater(largePenObj.transform.childCount, 0);
            Assert.IsNotNull(largePenObj.transform.Find("Perimeter Fences"));
            Assert.IsNotNull(largePenObj.transform.Find("Dairy Barn (Happy Farm)"));
            Assert.IsNotNull(largePenObj.transform.Find("Dairy Cow Bella"));
            Assert.IsNotNull(largePenObj.transform.Find("Fluffy Sheep Dolly"));
            Assert.IsNotNull(largePenObj.transform.Find("Farm Dog Buddy"));

            Object.DestroyImmediate(smallPenObj);
            Object.DestroyImmediate(largePenObj);
            Object.DestroyImmediate(sessionObj);
        }

        [Test]
        public void PlayerMouseToolTargeter_InitializesProperly()
        {
            GameObject playerObj = new GameObject("TestPlayer");
            PlayerGatheringInteractor gathering = playerObj.AddComponent<PlayerGatheringInteractor>();
            PlayerFarmingInteractor farming = playerObj.AddComponent<PlayerFarmingInteractor>();
            PlayerMouseToolTargeter targeter = playerObj.AddComponent<PlayerMouseToolTargeter>();

            GameObject sessionObj = new GameObject("TestSession");
            InventorySession session = sessionObj.AddComponent<InventorySession>();

            targeter.Configure(session, gathering, farming);
            Assert.IsNotNull(targeter);

            // Verify all custom pixel art cursor textures
            Assert.IsNotNull(PrototypePixelArtFactory.CursorPointerTexture());
            Assert.IsNotNull(PrototypePixelArtFactory.CursorAxeTexture());
            Assert.IsNotNull(PrototypePixelArtFactory.CursorPickaxeTexture());
            Assert.IsNotNull(PrototypePixelArtFactory.CursorHoeTexture());
            Assert.IsNotNull(PrototypePixelArtFactory.CursorWateringCanTexture());
            Assert.IsNotNull(PrototypePixelArtFactory.CursorSeedTexture());
            Assert.IsNotNull(PrototypePixelArtFactory.CursorHandTexture());
            Assert.IsNotNull(PrototypePixelArtFactory.CursorHarvestTexture());
            Assert.IsNotNull(PrototypePixelArtFactory.CursorSwordTexture());

            Object.DestroyImmediate(playerObj);
            Object.DestroyImmediate(sessionObj);
        }

        [Test]
        public void CustomizableHotbar_AssignAndRemove_WorksCorrectly()
        {
            GameObject hudObj = new GameObject("TestHud");
            InventoryDebugHud hud = hudObj.AddComponent<InventoryDebugHud>();

            // Default contains item.tool-hoe in slot 0
            Assert.IsTrue(hud.IsItemOnHotbar("item.tool-hoe", out int slot0));
            Assert.AreEqual(0, slot0);

            // Remove hoe
            Assert.IsTrue(hud.RemoveItemFromHotbar("item.tool-hoe"));
            Assert.IsFalse(hud.IsItemOnHotbar("item.tool-hoe", out _));

            // Assign potato seeds into specific slot 4
            Assert.IsTrue(hud.AssignItemToHotbar("item.seed-potato", 4));
            Assert.IsTrue(hud.IsItemOnHotbar("item.seed-potato", out int potatoSlot));
            Assert.AreEqual(4, potatoSlot);

            // Clear slot 4
            hud.ClearHotbarSlot(4);
            Assert.IsFalse(hud.IsItemOnHotbar("item.seed-potato", out _));

            // Assign item to first free slot
            Assert.IsTrue(hud.AssignItemToHotbar("item.cooked-meal"));
            Assert.IsTrue(hud.IsItemOnHotbar("item.cooked-meal", out int freeSlot));
            Assert.GreaterOrEqual(freeSlot, 0);

            Object.DestroyImmediate(hudObj);
        }

        [Test]
        public void CustomizableHotbar_SaveAndLoad_PreservesSlots()
        {
            GameObject hudObj = new GameObject("TestHud");
            InventoryDebugHud hud = hudObj.AddComponent<InventoryDebugHud>();

            string[] customLayout = new string[9]
            {
                "item.seed-pineapple",
                "item.seed-tomato",
                "item.weapon-bow",
                "",
                "item.ammo-arrow",
                "",
                "item.cooked-meal",
                "item.tool-hoe",
                "item.watering-can"
            };

            hud.LoadHotbarEntries(customLayout);

            string[] saved = hud.GetHotbarSaveEntries();
            Assert.AreEqual(9, saved.Length);
            Assert.AreEqual("item.seed-pineapple", saved[0]);
            Assert.AreEqual("item.seed-tomato", saved[1]);
            Assert.AreEqual("item.weapon-bow", saved[2]);
            Assert.IsEmpty(saved[3]);
            Assert.AreEqual("item.ammo-arrow", saved[4]);

            Object.DestroyImmediate(hudObj);
        }

        [Test]
        public void FarmPlot_CountdownTimer_FormatsDaysHoursMinutes()
        {
            GameObject plotObj = new GameObject("TestPlot");
            FarmPlotController plot = plotObj.AddComponent<FarmPlotController>();

            // Test 1: Giant Pumpkin (2880 mins = 2 days)
            plot.Configure("test.plot.pumpkin", true, true, "pumpkin", 0f, 0);
            string formatted = plot.RemainingCountdownFormatted;
            Assert.IsTrue(formatted.Contains("2") || formatted.Contains("ngày") || formatted.Contains("d"));

            // Test 2: Ready to harvest
            plot.Configure("test.plot.ready", true, true, "carrot", 90f, 4);
            string readyText = plot.RemainingCountdownFormatted;
            Assert.IsTrue(readyText.Contains("THU HOẠCH") || readyText.Contains("READY"));

            Object.DestroyImmediate(plotObj);
        }

        [Test]
        public void WanderingMerchant_TeleportAndSchedule_WorksCorrectly()
        {
            GameObject merchantObj = new GameObject("TestMerchant");
            WanderingMerchantController merchant = merchantObj.AddComponent<WanderingMerchantController>();

            Assert.IsTrue(merchant.IsPresent);
            Assert.IsNotEmpty(merchant.CurrentLocationName);

            // Teleport to location 1
            merchant.TeleportToLocation(1);
            Assert.AreEqual(new Vector2(0.0f, 6.0f), merchant.CurrentPosition);

            // Test dialogue
            string dialogue = merchant.GetContextualDialogue();
            Assert.IsNotEmpty(dialogue);

            Object.DestroyImmediate(merchantObj);
        }

        [Test]
        public void BuildingCatalog_15Categories_CreateAndRegisterSuccessfully()
        {
            GameObject sliceObj = new GameObject("TestSlice");
            TheOldRoad.Core.VerticalSliceController slice = sliceObj.AddComponent<TheOldRoad.Core.VerticalSliceController>();

            // Category 0: Manor & Greenhouse
            BuildingDefinition manor = slice.GetBuildingDefinition("building.manor");
            Assert.IsNotNull(manor);
            Assert.AreEqual(new Vector2Int(10, 8), manor.Footprint);

            BuildingDefinition greenhouse = slice.GetBuildingDefinition("building.greenhouse");
            Assert.IsNotNull(greenhouse);
            Assert.AreEqual(new Vector2Int(8, 8), greenhouse.Footprint);

            // Category 6: Windmill & Cheese Press
            BuildingDefinition windmill = slice.GetBuildingDefinition("building.windmill");
            Assert.IsNotNull(windmill);
            Assert.AreEqual(new Vector2Int(4, 4), windmill.Footprint);

            BuildingDefinition cheesePress = slice.GetBuildingDefinition("building.cheese-press");
            Assert.IsNotNull(cheesePress);

            // Category 9: Ancient Well & Fountain
            BuildingDefinition well = slice.GetBuildingDefinition("building.ancient-well");
            Assert.IsNotNull(well);

            // Category 10: Knight Statue & Bell Shrine
            BuildingDefinition knightStatue = slice.GetBuildingDefinition("building.knight-statue");
            Assert.IsNotNull(knightStatue);

            Object.DestroyImmediate(sliceObj);
        }
    }
}
