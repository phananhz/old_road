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
}
