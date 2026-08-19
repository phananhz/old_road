using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using TheOldRoad.Building;
using TheOldRoad.Inventory;
using TheOldRoad.Save;
using TheOldRoad.Combat;

namespace TheOldRoad.Tests.EditMode
{
    public class BuildingInteractivityTests
    {
        [Test]
        public void Silo_DepositAndWithdraw_WorksCorrectly()
        {
            GameObject go = new GameObject("TestSilo");
            var silo = go.AddComponent<SiloStorageController>();
            silo.Configure("silo.test");

            var inv = new InventoryRuntime();
            inv.Add("item.wheat", 20);
            inv.Add("item.seed-carrot", 15);
            inv.Add("item.wood", 10); // Non-crop item

            // Try depositing non-crop item (should fail)
            bool woodDeposit = silo.Deposit("item.wood", 5, inv);
            Assert.IsFalse(woodDeposit, "Silo should reject non-crop items like wood.");
            Assert.AreEqual(0, silo.GetQuantity("item.wood"));

            // Deposit wheat
            bool wheatDeposit = silo.Deposit("item.wheat", 12, inv);
            Assert.IsTrue(wheatDeposit, "Silo should accept wheat deposit.");
            Assert.AreEqual(12, silo.GetQuantity("item.wheat"));
            Assert.AreEqual(8, inv.GetQuantity("item.wheat"));

            // Deposit all produce
            int totalDeposited = silo.DepositAllProduce(inv);
            Assert.AreEqual(8 + 15, totalDeposited, "DepositAllProduce should deposit all remaining crops and seeds.");
            Assert.AreEqual(20, silo.GetQuantity("item.wheat"));
            Assert.AreEqual(15, silo.GetQuantity("item.seed-carrot"));
            Assert.AreEqual(0, inv.GetQuantity("item.wheat"));
            Assert.AreEqual(0, inv.GetQuantity("item.seed-carrot"));
            Assert.AreEqual(10, inv.GetQuantity("item.wood")); // Wood untouched

            // Withdraw from silo
            bool withdraw = silo.Withdraw("item.wheat", 5, inv);
            Assert.IsTrue(withdraw);
            Assert.AreEqual(15, silo.GetQuantity("item.wheat"));
            Assert.AreEqual(5, inv.GetQuantity("item.wheat"));

            // Save and Load roundtrip
            SiloSaveEntry save = silo.Save();
            Assert.IsNotNull(save);
            Assert.AreEqual("silo.test", save.siloId);

            GameObject go2 = new GameObject("TestSilo2");
            var silo2 = go2.AddComponent<SiloStorageController>();
            silo2.Load(save);
            Assert.AreEqual(15, silo2.GetQuantity("item.wheat"));
            Assert.AreEqual(15, silo2.GetQuantity("item.seed-carrot"));

            Object.DestroyImmediate(go);
            Object.DestroyImmediate(go2);
        }

        [Test]
        public void Chest_DepositWithdrawAndQuickStack_WorksCorrectly()
        {
            GameObject go = new GameObject("TestChest");
            var chest = go.AddComponent<ChestStorageController>();
            chest.Configure("chest.test", 16, "Rương Gỗ", "Wood Chest");

            var inv = new InventoryRuntime();
            inv.Add("item.wood", 30);
            inv.Add("item.stone", 20);

            // Deposit wood into slot 0
            bool dep = chest.DepositItem(0, "item.wood", 10, inv);
            Assert.IsTrue(dep);
            Assert.AreEqual("item.wood", chest.GetSlot(0).itemId);
            Assert.AreEqual(10, chest.GetSlot(0).quantity);
            Assert.AreEqual(20, inv.GetQuantity("item.wood"));

            // Quick stack wood
            int stacked = chest.QuickStack(inv);
            Assert.AreEqual(20, stacked);
            Assert.AreEqual(30, chest.GetSlot(0).quantity);
            Assert.AreEqual(0, inv.GetQuantity("item.wood"));
            Assert.AreEqual(20, inv.GetQuantity("item.stone"));

            // Take all
            int taken = chest.TakeAll(inv);
            Assert.AreEqual(30, taken);
            Assert.IsTrue(chest.GetSlot(0).IsEmpty);
            Assert.AreEqual(30, inv.GetQuantity("item.wood"));

            // Save and Load roundtrip
            chest.DepositItem(2, "item.stone", 15, inv);
            ChestSaveEntry save = chest.Save();
            Assert.IsNotNull(save);

            GameObject go2 = new GameObject("TestChest2");
            var chest2 = go2.AddComponent<ChestStorageController>();
            chest2.Configure("chest.test", 16, "Rương Gỗ", "Wood Chest");
            chest2.Load(save);
            Assert.AreEqual("item.stone", chest2.GetSlot(2).itemId);
            Assert.AreEqual(15, chest2.GetSlot(2).quantity);

            Object.DestroyImmediate(go);
            Object.DestroyImmediate(go2);
        }

        [Test]
        public void ArtisanMachines_ProcessRecipesAndProduceOutputs()
        {
            GameObject go = new GameObject("TestWindmill");
            var windmill = go.AddComponent<ArtisanProcessingController>();
            windmill.Configure("wm.test", "building.windmill");

            var recipes = windmill.GetAvailableRecipes();
            Assert.Greater(recipes.Count, 0, "Windmill must have registered recipes.");

            var inv = new InventoryRuntime();
            inv.Add("item.wheat", 4);

            ArtisanRecipe recipe = recipes.Find(r => r.inputItemId == "item.wheat");
            Assert.IsNotNull(recipe);

            Assert.IsTrue(windmill.CanStartRecipe(recipe, inv));
            bool started = windmill.StartRecipe(recipe, inv);
            Assert.IsTrue(started);
            Assert.IsTrue(windmill.IsProcessing);
            Assert.AreEqual(2, inv.GetQuantity("item.wheat")); // 2 wheat consumed

            // Simulate save/load
            ArtisanSaveEntry save = windmill.Save();
            Assert.IsNotNull(save);
            Assert.AreEqual("wm.test", save.machineId);
            Assert.AreEqual("building.windmill", save.machineBuildingId);
            Assert.IsTrue(save.isProcessing);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void CabinInterior_DistinctLayoutsAndTwoFloorNavigation_Works()
        {
            GameObject playerGo = new GameObject("TestPlayer");
            var player = playerGo.AddComponent<TheOldRoad.Player.PlayerMovement>();

            GameObject interiorGo = new GameObject("TestInterior");
            var interior = interiorGo.AddComponent<TheOldRoad.World.CabinInteriorController>();

            // 1. Enter Stone Cottage (2 stories)
            interior.Enter(player, Vector3.zero, "building.stone-cottage");
            Assert.IsTrue(interior.IsInside);
            Assert.AreEqual(1, interior.CurrentFloor);
            Assert.IsNotNull(interior.StairsTransform, "Floor 1 of Stone Cottage must have stairs up.");

            // 2. Toggle to Floor 2
            interior.ToggleFloor(player);
            Assert.AreEqual(2, interior.CurrentFloor);
            Assert.IsNotNull(interior.StairsTransform, "Floor 2 of Stone Cottage must have stairs down.");
            Assert.IsNotNull(interior.BedTransform, "Floor 2 of Stone Cottage must have master bed.");

            // 3. Toggle back to Floor 1
            interior.ToggleFloor(player);
            Assert.AreEqual(1, interior.CurrentFloor);

            // 4. Enter Manor (2 stories)
            interior.Enter(player, Vector3.zero, "building.manor");
            Assert.AreEqual(1, interior.CurrentFloor);
            Assert.IsNotNull(interior.StairsTransform);
            interior.ToggleFloor(player);
            Assert.AreEqual(2, interior.CurrentFloor);
            Assert.IsNotNull(interior.BedTransform);
            Assert.IsNotNull(interior.ChestTransform);

            // 5. Enter Windmill (2 stories)
            interior.Enter(player, Vector3.zero, "building.windmill");
            Assert.AreEqual(1, interior.CurrentFloor);
            Assert.IsNotNull(interior.StairsTransform);
            interior.ToggleFloor(player);
            Assert.AreEqual(2, interior.CurrentFloor);
            Assert.IsNotNull(interior.BedTransform);

            // 6. Enter Greenhouse, Tent, Shed
            interior.Enter(player, Vector3.zero, "building.greenhouse");
            Assert.IsTrue(interior.IsInside);
            interior.Enter(player, Vector3.zero, "building.tent");
            Assert.IsTrue(interior.IsInside);
            Assert.IsNotNull(interior.BedTransform);
            interior.Enter(player, Vector3.zero, "building.storage-shed");
            Assert.IsTrue(interior.IsInside);
            // 7. Test Movement inside room
            Vector3 startPos = player.transform.position;
            player.transform.position += new Vector3(2.5f, 1.5f, 0f);
            Vector3 lastValid = startPos;
            interior.ConstrainActorInside(player.transform, ref lastValid);
            Assert.AreNotEqual(startPos, player.transform.position, "Player must be able to move freely inside the interior.");

            // 8. Exit building
            interior.Exit(player);
            Assert.IsFalse(interior.IsInside);

            Object.DestroyImmediate(playerGo);
            Object.DestroyImmediate(interiorGo);
        }

        [Test]
        public void MarketStall_SellItemsForSilverCoins_Works()
        {
            var inv = new InventoryRuntime();
            inv.Add("item.wheat", 10);
            inv.Add("item.cheese", 4);
            inv.Add("item.fish-salmon", 2);

            // Sell 5 wheat (3 silver each = 15 silver)
            bool soldWheat = TheOldRoad.Economy.MarketStallController.TrySellItem("item.wheat", 5, inv, out int earned1);
            Assert.IsTrue(soldWheat);
            Assert.AreEqual(15, earned1);
            Assert.AreEqual(5, inv.GetQuantity("item.wheat"));
            Assert.AreEqual(15, inv.GetQuantity("item.silver-coin"));

            // Sell all 4 cheese (15 silver each = 60 silver)
            bool soldCheese = TheOldRoad.Economy.MarketStallController.TrySellItem("item.cheese", 4, inv, out int earned2);
            Assert.IsTrue(soldCheese);
            Assert.AreEqual(60, earned2);
            Assert.AreEqual(0, inv.GetQuantity("item.cheese"));
            Assert.AreEqual(75, inv.GetQuantity("item.silver-coin"));
        }

        [Test]
        public void Sprinklers_WaterPlotsCorrectly()
        {
            GameObject plotGo1 = new GameObject("Plot1");
            plotGo1.transform.position = new Vector3(1f, 0f, 0f);
            var plot1 = plotGo1.AddComponent<TheOldRoad.Farming.FarmPlotController>();

            GameObject plotGo2 = new GameObject("Plot2");
            plotGo2.transform.position = new Vector3(0f, 1f, 0f);
            var plot2 = plotGo2.AddComponent<TheOldRoad.Farming.FarmPlotController>();

            GameObject sprinklerGo = new GameObject("CopperSprinkler");
            sprinklerGo.transform.position = Vector3.zero;
            var sprinkler = sprinklerGo.AddComponent<TheOldRoad.Farming.SprinklerController>();
            sprinkler.Configure(TheOldRoad.Farming.SprinklerTier.Copper);

            int watered = sprinkler.WaterSurroundingPlots();
            Assert.AreEqual(2, watered);
            Assert.IsTrue(plot1.IsWatered);
            Assert.IsTrue(plot2.IsWatered);

            Object.DestroyImmediate(plotGo1);
            Object.DestroyImmediate(plotGo2);
            Object.DestroyImmediate(sprinklerGo);
        }

        [Test]
        public void PlayerVitals_EatAllFoods_HealsCorrectly()
        {
            GameObject go = new GameObject("TestVitals");
            var vitals = go.AddComponent<TheOldRoad.Player.PlayerVitals>();

            // Damage player first
            vitals.Damage(50);
            int damagedHp = vitals.CurrentHealth;

            // Eat Grape (+6 HP)
            bool ateGrape = vitals.TryConsumeFood("item.grape", out int healed1);
            Assert.IsTrue(ateGrape);
            Assert.AreEqual(6, healed1);
            Assert.AreEqual(damagedHp + 6, vitals.CurrentHealth);

            // Eat Cheese (+14 HP)
            bool ateCheese = vitals.TryConsumeFood("item.cheese", out int healed2);
            Assert.IsTrue(ateCheese);
            Assert.AreEqual(14, healed2);

            // Eat Pumpkin (+10 HP)
            bool atePumpkin = vitals.TryConsumeFood("item.pumpkin", out int healed3);
            Assert.IsTrue(atePumpkin);
            Assert.AreEqual(10, healed3);

            Object.DestroyImmediate(go);
        }
    }
}
