using System.Collections.Generic;
using NUnit.Framework;
using TheOldRoad.Quest;

namespace TheOldRoad.Tests.EditMode
{
    public class StoryQuestTests
    {
        [Test]
        public void Evaluate_CompletesOpeningSteps_FromGameplayState()
        {
            HashSet<string> completed = new HashSet<string>();
            StoryQuestContext context = new StoryQuestContext
            {
                DiscoveredLandmarkCount = 1,
                OpenedLootChestCount = 1,
                GetItemQuantity = itemId => itemId == "item.roadwarden-page" ? 1 : 0,
                TalkedToVillager = true
            };

            StoryQuestProgress progress = StoryQuestRuntime.Evaluate(context, completed);

            Assert.IsTrue(completed.Contains("story.01.inspect-road"));
            Assert.IsTrue(completed.Contains("story.02.open-cache"));
            Assert.IsTrue(completed.Contains("story.03.father-page"));
            Assert.IsTrue(completed.Contains("story.04.ask-village"));
            Assert.AreEqual("Make a worn axe", progress.ActiveStep.Title);
        }

        [Test]
        public void Evaluate_PreservesPreviouslyCompletedStoryStep()
        {
            HashSet<string> completed = new HashSet<string> { "story.01.inspect-road" };
            StoryQuestContext context = new StoryQuestContext();

            StoryQuestProgress progress = StoryQuestRuntime.Evaluate(context, completed);

            Assert.IsTrue(completed.Contains("story.01.inspect-road"));
            Assert.AreEqual(1, progress.CompletedCount);
            Assert.AreEqual("Search an abandoned cache", progress.ActiveStep.Title);
        }

        [Test]
        public void Evaluate_ChecksChapterFourAndFiveSteps()
        {
            HashSet<string> completed = new HashSet<string>();
            StoryQuestContext context = new StoryQuestContext
            {
                GetItemQuantity = itemId =>
                {
                    if (itemId == "item.fish-salmon") return 2;
                    if (itemId == "item.cooked-fish") return 1;
                    if (itemId == "item.weapon-sword") return 1;
                    if (itemId == "item.wheat") return 5;
                    if (itemId == "item.milk") return 2;
                    if (itemId == "item.silver-coin") return 20;
                    return 0;
                }
            };

            Assert.IsTrue(context.HasAnyFish());
            Assert.IsTrue(context.HasAnyFarmHarvest());
            Assert.IsTrue(context.HasAdvancedGear());
            Assert.IsTrue(context.HasItem("item.silver-coin", 15));
        }
    }
}
