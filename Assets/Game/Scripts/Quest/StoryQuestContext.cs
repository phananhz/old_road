using System;

namespace TheOldRoad.Quest
{
    /// <summary>Read-only gameplay snapshot used to evaluate authored prototype story quests.</summary>
    public sealed class StoryQuestContext
    {
        public Func<string, int> GetItemQuantity { get; set; } = _ => 0;
        public Func<string, bool> HasStartedBuilding { get; set; } = _ => false;
        public Func<string, bool> HasCompletedBuilding { get; set; } = _ => false;
        public Func<string, bool> HasDiscoveredLandmark { get; set; } = _ => false;
        public bool TalkedToVillager { get; set; }
        public int OpenedLootChestCount { get; set; }
        public int DiscoveredLandmarkCount { get; set; }

        public bool HasItem(string itemId, int quantity = 1)
        {
            return !string.IsNullOrWhiteSpace(itemId) && GetItemQuantity(itemId) >= quantity;
        }

        public bool HasAnyForagedItem()
        {
            return HasItem("item.wild-berries")
                || HasItem("item.medicinal-herb")
                || HasItem("item.mushroom");
        }
    }
}
