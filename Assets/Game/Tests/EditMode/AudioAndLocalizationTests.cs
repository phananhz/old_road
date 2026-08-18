using NUnit.Framework;
using UnityEngine;
using TheOldRoad.Audio;
using TheOldRoad.Inventory;
using TheOldRoad.UI;
using TheOldRoad.World;

namespace TheOldRoad.Tests.EditMode
{
    public class AudioAndLocalizationTests
    {
        [Test]
        public void PrototypeAudioFactory_GeneratesAllSfxClipsSuccessfully()
        {
            Assert.IsNotNull(PrototypeAudioFactory.Footstep);
            Assert.Greater(PrototypeAudioFactory.Footstep.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.ChopWood);
            Assert.Greater(PrototypeAudioFactory.ChopWood.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.MineStone);
            Assert.Greater(PrototypeAudioFactory.MineStone.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.ForageHerb);
            Assert.Greater(PrototypeAudioFactory.ForageHerb.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.LootPickup);
            Assert.Greater(PrototypeAudioFactory.LootPickup.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.CraftHammer);
            Assert.Greater(PrototypeAudioFactory.CraftHammer.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.CookSizzle);
            Assert.Greater(PrototypeAudioFactory.CookSizzle.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.BuildPlace);
            Assert.Greater(PrototypeAudioFactory.BuildPlace.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.BuildComplete);
            Assert.Greater(PrototypeAudioFactory.BuildComplete.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.SwordSlash);
            Assert.Greater(PrototypeAudioFactory.SwordSlash.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.HitImpact);
            Assert.Greater(PrototypeAudioFactory.HitImpact.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.WolfGrowl);
            Assert.Greater(PrototypeAudioFactory.WolfGrowl.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.EnemyDefeated);
            Assert.Greater(PrototypeAudioFactory.EnemyDefeated.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.PlayerHurt);
            Assert.Greater(PrototypeAudioFactory.PlayerHurt.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.DoorLatch);
            Assert.Greater(PrototypeAudioFactory.DoorLatch.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.SleepMorning);
            Assert.Greater(PrototypeAudioFactory.SleepMorning.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.UiClick);
            Assert.Greater(PrototypeAudioFactory.UiClick.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.QuestComplete);
            Assert.Greater(PrototypeAudioFactory.QuestComplete.length, 0f);
        }

        [Test]
        public void PrototypeAudioFactory_GeneratesMusicAndAmbientLoopsSuccessfully()
        {
            Assert.IsNotNull(PrototypeAudioFactory.OverworldMusic);
            Assert.GreaterOrEqual(PrototypeAudioFactory.OverworldMusic.length, 10f);

            Assert.IsNotNull(PrototypeAudioFactory.AmbientDay);
            Assert.GreaterOrEqual(PrototypeAudioFactory.AmbientDay.length, 5f);

            Assert.IsNotNull(PrototypeAudioFactory.AmbientNight);
            Assert.GreaterOrEqual(PrototypeAudioFactory.AmbientNight.length, 5f);
        }

        [Test]
        public void LocalizationRuntime_SwitchesLanguageAndTranslatesCoreKeys()
        {
            // English mode
            LocalizationRuntime.SetLanguage(0);
            Assert.IsFalse(LocalizationRuntime.IsVietnamese);
            Assert.AreEqual("Wood", LocalizationRuntime.T("wood"));
            Assert.AreEqual("Start Journey", LocalizationRuntime.T("start"));
            Assert.AreEqual("Roadwarden Pack", LocalizationRuntime.T("pack_title"));
            Assert.AreEqual("Cabin", LocalizationRuntime.BuildingName("building.cabin"));
            Assert.AreEqual("Forest Wolf", LocalizationRuntime.EnemyName("Forest Wolf"));
            Assert.AreEqual("Miller", LocalizationRuntime.NpcTitle("Miller"));

            // Vietnamese mode
            LocalizationRuntime.SetLanguage(1);
            Assert.IsTrue(LocalizationRuntime.IsVietnamese);
            Assert.AreEqual("Gỗ", LocalizationRuntime.T("wood"));
            Assert.AreEqual("Bắt đầu hành trình", LocalizationRuntime.T("start"));
            Assert.AreEqual("Túi Roadwarden", LocalizationRuntime.T("pack_title"));
            Assert.AreEqual("Nhà gỗ", LocalizationRuntime.BuildingName("building.cabin"));
            Assert.AreEqual("Sói Rừng", LocalizationRuntime.EnemyName("Forest Wolf"));
            Assert.AreEqual("Thợ Xay", LocalizationRuntime.NpcTitle("Miller"));

            // Reset back to English
            LocalizationRuntime.SetLanguage(0);
        }

        [Test]
        public void GameSettingsRuntime_AudioVolumeControls_AdjustAndPersistValues()
        {
            GameSettingsRuntime.SetMasterVolume(0.75f);
            GameSettingsRuntime.SetMusicVolume(0.65f);
            GameSettingsRuntime.SetSfxVolume(0.85f);
            GameSettingsRuntime.SetSoundEnabled(true);

            Assert.AreEqual(0.75f, GameSettingsRuntime.MasterVolume, 0.001f);
            Assert.AreEqual(0.65f, GameSettingsRuntime.MusicVolume, 0.001f);
            Assert.AreEqual(0.85f, GameSettingsRuntime.SfxVolume, 0.001f);
            Assert.IsTrue(GameSettingsRuntime.SoundEnabled);

            Assert.AreEqual(0.75f, AudioManager.MasterVolume, 0.001f);
            Assert.AreEqual(0.65f, AudioManager.MusicVolume, 0.001f);
            Assert.AreEqual(0.85f, AudioManager.SfxVolume, 0.001f);
            Assert.IsFalse(AudioManager.IsMuted);
        }

        [Test]
        public void PrototypeAudioFactory_GeneratesWeatherAndInteractiveClips()
        {
            Assert.IsNotNull(PrototypeAudioFactory.RainLoop);
            Assert.GreaterOrEqual(PrototypeAudioFactory.RainLoop.length, 5f);

            Assert.IsNotNull(PrototypeAudioFactory.Thunder);
            Assert.Greater(PrototypeAudioFactory.Thunder.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.WaterSplash);
            Assert.Greater(PrototypeAudioFactory.WaterSplash.length, 0f);

            Assert.IsNotNull(PrototypeAudioFactory.ChestOpen);
            Assert.Greater(PrototypeAudioFactory.ChestOpen.length, 0f);
        }

        [Test]
        public void HomeStorageChest_DepositAndWithdraw_UpdatesInventoriesCorrectly()
        {
            InventoryRuntime inv = new InventoryRuntime();
            inv.Add("item.wood", 20);

            Assert.AreEqual(20, inv.GetQuantity("item.wood"));
            TheOldRoad.World.HomeStorageChest.Deposit("item.wood", 5, inv);

            Assert.AreEqual(15, inv.GetQuantity("item.wood"));
            Assert.AreEqual(5, TheOldRoad.World.HomeStorageChest.GetStoredQuantity("item.wood"));

            TheOldRoad.World.HomeStorageChest.Withdraw("item.wood", 3, inv);
            Assert.AreEqual(18, inv.GetQuantity("item.wood"));
            Assert.AreEqual(2, TheOldRoad.World.HomeStorageChest.GetStoredQuantity("item.wood"));
        }

        [Test]
        public void CabinInterior_BuildsDistinctLayouts_ForCabinStoneCottageAndStorageShed()
        {
            GameObject obj = new GameObject("TestInterior");
            TheOldRoad.World.CabinInteriorController interior = obj.AddComponent<TheOldRoad.World.CabinInteriorController>();

            // Starter Cabin
            interior.EnsureBuilt("building.cabin");
            Assert.AreEqual("building.cabin", interior.CurrentBuildingType);
            Assert.IsNotNull(interior.BedTransform);

            // Stone Cottage
            interior.EnsureBuilt("building.stone-cottage");
            Assert.AreEqual("building.stone-cottage", interior.CurrentBuildingType);
            Assert.IsNotNull(interior.BedTransform);

            // Storage Shed
            interior.EnsureBuilt("building.storage-shed");
            Assert.AreEqual("building.storage-shed", interior.CurrentBuildingType);
            Assert.IsNotNull(interior.ChestTransform);

            Object.DestroyImmediate(obj);
        }
    }
}
