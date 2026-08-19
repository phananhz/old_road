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

        public bool HasAnyFish()
        {
            return HasItem("item.fish-salmon")
                || HasItem("item.fish-carp")
                || HasItem("item.fish-golden-perch")
                || HasItem("item.cooked-fish");
        }

        public bool HasAnyFarmHarvest()
        {
            return HasItem("item.wheat")
                || HasItem("item.carrot")
                || HasItem("item.potato")
                || HasItem("item.corn")
                || HasItem("item.tomato")
                || HasItem("item.pineapple");
        }

        public bool HasAdvancedGear()
        {
            return HasItem("item.weapon-sword")
                || HasItem("item.weapon-bow")
                || HasItem("item.shield-wood")
                || HasItem("item.armor-knight");
        }
    }
}
