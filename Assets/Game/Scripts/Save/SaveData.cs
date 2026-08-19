using System;

namespace TheOldRoad.Save
{
    [Serializable]
    public sealed class InventorySaveEntry
    {
        public string itemId;
        public int quantity;
    }

    [Serializable]
    public sealed class SaveData
    {
        public int saveVersion = 1;
        public InventorySaveEntry[] inventory = Array.Empty<InventorySaveEntry>();
        public ConstructionSaveEntry[] constructionJobs = Array.Empty<ConstructionSaveEntry>();
        public ResourceNodeSaveEntry[] resourceNodes = Array.Empty<ResourceNodeSaveEntry>();
        public LandmarkSaveEntry[] landmarks = Array.Empty<LandmarkSaveEntry>();
        public LootChestSaveEntry[] lootChests = Array.Empty<LootChestSaveEntry>();
        public StoryStepSaveEntry[] completedStorySteps = Array.Empty<StoryStepSaveEntry>();
        public FarmPlotSaveEntry[] farmPlots = Array.Empty<FarmPlotSaveEntry>();
        public ChestSaveEntry[] chests = Array.Empty<ChestSaveEntry>();
        public SiloSaveEntry[] silos = Array.Empty<SiloSaveEntry>();
        public ArtisanSaveEntry[] artisanMachines = Array.Empty<ArtisanSaveEntry>();
        public string[] hotbar = Array.Empty<string>();
        public PlayerSaveEntry player;
        public GameTimeSaveEntry gameTime;
        public long lastSavedUnixSeconds;
        public bool talkedToVillager;
    }

    [Serializable]
    public sealed class ChestSlotSaveEntry
    {
        public int slotIndex;
        public string itemId;
        public int quantity;
    }

    [Serializable]
    public sealed class ChestSaveEntry
    {
        public string chestId;
        public ChestSlotSaveEntry[] slots = Array.Empty<ChestSlotSaveEntry>();
    }

    [Serializable]
    public sealed class SiloSaveEntry
    {
        public string siloId;
        public InventorySaveEntry[] storedItems = Array.Empty<InventorySaveEntry>();
    }

    [Serializable]
    public sealed class ArtisanSaveEntry
    {
        public string machineId;
        public string machineBuildingId;
        public string inputItemId;
        public int inputQuantity;
        public string outputItemId;
        public int outputQuantity;
        public float remainingProcessSeconds;
        public bool isProcessing;
        public bool isFinished;
    }

    [Serializable]
    public sealed class FarmPlotSaveEntry
    {
        public string plotId;
        public bool isTilled;
        public bool isWatered;
        public string plantedCropId;
        public float growthMinutes;
        public int growthStage;
        public long plantedAtUnixSeconds;
        public long lastWateredAtUnixSeconds;
    }

    [Serializable]
    public sealed class ConstructionSaveEntry
    {
        public string constructionId;
        public string buildingId;
        public long startUnixSeconds;
        public long durationSeconds;
        public int gridX;
        public int gridY;
        public string state;
    }

    [Serializable]
    public sealed class ResourceNodeSaveEntry
    {
        public string nodeId;
        public bool harvested;
    }

    [Serializable]
    public sealed class LandmarkSaveEntry
    {
        public string landmarkId;
        public bool discovered;
    }

    [Serializable]
    public sealed class LootChestSaveEntry
    {
        public string chestId;
        public bool opened;
    }

    [Serializable]
    public sealed class StoryStepSaveEntry
    {
        public string stepId;
        public bool completed;
    }

    [Serializable]
    public sealed class PlayerSaveEntry
    {
        public float x;
        public float y;
        public bool insideCabin;
    }

    [Serializable]
    public sealed class GameTimeSaveEntry
    {
        public int absoluteMinute;
    }
}
