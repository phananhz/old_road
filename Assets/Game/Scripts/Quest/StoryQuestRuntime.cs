using System;
using System.Collections.Generic;
using System.Linq;

namespace TheOldRoad.Quest
{
    /// <summary>Authored opening story arc for the current prototype.</summary>
    public static class StoryQuestRuntime
    {
        private static readonly StoryQuestChapter[] Chapters =
        {
            new StoryQuestChapter(
                "chapter.01.bell",
                "Chapter I - The Bell Beyond Valen",
                "A bell rings where no bell tower should still stand. Recover your father's trail and prove the old road has returned.",
                new[]
                {
                    new StoryQuestStep(
                        "story.01.inspect-road",
                        "Read the first mark on the old road",
                        "Inspect any landmark near Valen to confirm the road is awake.",
                        "The first mark is not weathered by time. Someone, or something, has touched the road recently.",
                        context => context.DiscoveredLandmarkCount > 0),
                    new StoryQuestStep(
                        "story.02.open-cache",
                        "Search an abandoned cache",
                        "Open an old chest and look for Roadwarden supplies.",
                        "The cache still carries Valen resin and Roadwarden twine. It was meant for someone coming back.",
                        context => context.OpenedLootChestCount > 0),
                    new StoryQuestStep(
                        "story.03.father-page",
                        "Recover Father's missing page",
                        "Find the Roadwarden journal page hidden near the starting road.",
                        "Father's page says: If the roads open again, do not believe what they told us about that night.",
                        context => context.HasItem("item.roadwarden-page")),
                    new StoryQuestStep(
                        "story.04.ask-village",
                        "Ask Valen what they heard",
                        "Speak with a villager and collect the first rumour.",
                        "The villagers heard the bell before dawn. No one admits being awake, but every hearth was lit.",
                        context => context.TalkedToVillager)
                }),
            new StoryQuestChapter(
                "chapter.02.roadwarden",
                "Chapter II - Roadwarden's Burden",
                "The road is no longer safe. Prepare tools, shelter, food, and a small fire before following it farther.",
                new[]
                {
                    new StoryQuestStep(
                        "story.05.make-axe",
                        "Make a worn axe",
                        "Craft an axe so the forest does not decide your path for you.",
                        "The axe is crude, but the first Roadwardens started with less.",
                        context => context.HasItem("item.tool-axe")),
                    new StoryQuestStep(
                        "story.06.gather-stonewood",
                        "Gather road materials",
                        "Carry at least 3 wood and 2 stone.",
                        "Wood, stone, and patience: enough to build a place the dark cannot immediately take.",
                        context => context.HasItem("item.wood", 3) && context.HasItem("item.stone", 2)),
                    new StoryQuestStep(
                        "story.07.make-pick",
                        "Make a stone pick",
                        "Craft a pick and prepare to mine old iron.",
                        "The pick rings differently near the road, as if the ground remembers iron below it.",
                        context => context.HasItem("item.tool-pickaxe")),
                    new StoryQuestStep(
                        "story.08.first-iron",
                        "Mine the first iron",
                        "Mine iron ore from an exposed vein.",
                        "The iron is warm in your hand. Father wrote that bell towers were built on warmer metal.",
                        context => context.HasItem("item.iron-ore")),
                    new StoryQuestStep(
                        "story.09.prepare-food",
                        "Forage travel food",
                        "Gather berries, herbs, or mushrooms before nightfall.",
                        "The road is easier to follow when hunger is not deciding for you.",
                        context => context.HasAnyForagedItem())
                }),
            new StoryQuestChapter(
                "chapter.03.shelter",
                "Chapter III - Fire Against The Dark",
                "Build a base near the old road so exploration has a real point of return.",
                new[]
                {
                    new StoryQuestStep(
                        "story.10.cabin-plank",
                        "Cut the first cabin plank",
                        "Craft one cabin plank.",
                        "A single plank is not a home, but it is the first proof you are staying.",
                        context => context.HasItem("item.cabin-plank")),
                    new StoryQuestStep(
                        "story.11.start-home",
                        "Raise a Roadwarden shelter",
                        "Start construction on any cabin or cottage.",
                        "The frame faces the old road like a question waiting for an answer.",
                        context => context.HasStartedBuilding("building.cabin")
                            || context.HasStartedBuilding("building.stone-cottage")),
                    new StoryQuestStep(
                        "story.12.first-bell-fragment",
                        "Find the first bell fragment",
                        "Recover a bell fragment from the eastern road.",
                        "The fragment does not ring, but it hums when held near Father's page.",
                        context => context.HasItem("item.bell-fragment")),
                    new StoryQuestStep(
                        "story.13-light-fire",
                        "Build a fire for the night",
                        "Complete a campfire or cooking hearth.",
                        "The firelight is small. That is why it matters.",
                        context => context.HasCompletedBuilding("building.campfire")
                            || context.HasCompletedBuilding("building.cooking-hearth")),
                    new StoryQuestStep(
                        "story.14-cook-meal",
                        "Cook a warm meal",
                        "Cook one meal at a completed fire building.",
                        "Warm food turns a camp into a place people can return to.",
                        context => context.HasItem("item.cooked-meal")),
                    new StoryQuestStep(
                        "story.15-animal-pen",
                        "Make the first village pen",
                        "Build a small or long animal pen.",
                        "The road needs defenders, but Valen first needs ordinary life to continue.",
                        context => context.HasCompletedBuilding("building.animal-pen-small")
                            || context.HasCompletedBuilding("building.animal-pen-long"))
                }),
            new StoryQuestChapter(
                "chapter.04.river-and-hunt",
                "Chapter IV - Riverbanks and Night Hunt",
                "The road extends past the riverbanks into predator grounds. Master river fishing and arm yourself for the wild trail.",
                new[]
                {
                    new StoryQuestStep(
                        "story.16.fish-river",
                        "Cast a line into Valen River",
                        "Catch any fresh fish (salmon, carp, or perch) from the river.",
                        "The clear river waters provide life and food for those willing to wait patiently.",
                        context => context.HasAnyFish()),
                    new StoryQuestStep(
                        "story.17.cook-fish",
                        "Grill fish over the hearth",
                        "Prepare a savory grilled fish meal.",
                        "A hot grilled fish restores spirit and stamina on long expeditions.",
                        context => context.HasItem("item.cooked-fish")),
                    new StoryQuestStep(
                        "story.18.forge-weapons",
                        "Arm yourself for the road",
                        "Obtain an Iron Longsword, Hunter's Bow, or Knight Cuirass.",
                        "The monsters in the dark will not heed words. Steel and bowstring speak clearly.",
                        context => context.HasAdvancedGear()),
                    new StoryQuestStep(
                        "story.19.blackwood-cave",
                        "Discover the Blackwood Cave",
                        "Scout the cold mouth of the ancient Blackwood caverns.",
                        "The cave breathes cold air. Roadwarden marks warn that the deep tunnels run far below the forest.",
                        context => context.HasDiscoveredLandmark("landmark.cave.blackwood"))
                }),
            new StoryQuestChapter(
                "chapter.05.prosperity",
                "Chapter V - Prosperity of the Old Road",
                "Expand the homestead into a thriving agrarian haven and restore trade across the realm.",
                new[]
                {
                    new StoryQuestStep(
                        "story.20.farm-harvest",
                        "Harvest the bounty of the fields",
                        "Harvest fresh wheat, potatoes, carrots, corn, or tomatoes.",
                        "Golden crops wave across the tilled soil, proving life has returned to the forgotten plots.",
                        context => context.HasAnyFarmHarvest()),
                    new StoryQuestStep(
                        "story.21.livestock-flock",
                        "Nurture the farm animals",
                        "Gather fresh eggs, milk, or wool from well-fed livestock.",
                        "The gentle lowing of cattle and clucking of hens brings genuine warmth to the village.",
                        context => context.HasItem("item.milk") || context.HasItem("item.egg") || context.HasItem("item.wool")),
                    new StoryQuestStep(
                        "story.22.merchant-wealth",
                        "Trade with Eldon the Merchant",
                        "Accumulate at least 15 Silver Coins from trade and bounty.",
                        "Trade caravans begin to speak of Valen once more. The road is alive.",
                        context => context.HasItem("item.silver-coin", 15)),
                    new StoryQuestStep(
                        "story.23.dragon-ridge",
                        "Survey the Dragon Scarred Ridge",
                        "Discover the legendary fused stone ridge.",
                        "The ancient stone bears witness: you have walked the full length of The Old Road and made it home.",
                        context => context.HasDiscoveredLandmark("landmark.dragon.ridge")
                            || context.HasDiscoveredLandmark("landmark.ridge.dragon"))
                })
        };

        public static StoryQuestProgress Evaluate(StoryQuestContext context, ISet<string> completedStepIds)
        {
            completedStepIds ??= new HashSet<string>();

            List<StoryQuestStep> visible = new List<StoryQuestStep>();
            List<StoryQuestStep> completed = new List<StoryQuestStep>();
            StoryQuestStep active = null;

            foreach (StoryQuestChapter chapter in Chapters)
            {
                bool previousStepsComplete = true;
                foreach (StoryQuestStep step in chapter.Steps)
                {
                    visible.Add(step);
                    bool isComplete = completedStepIds.Contains(step.StepId) || step.IsComplete(context);
                    if (isComplete)
                    {
                        completed.Add(step);
                        completedStepIds.Add(step.StepId);
                    }
                    else if (active == null && previousStepsComplete)
                    {
                        active = step;
                    }

                    previousStepsComplete &= isComplete;
                }

                if (active != null) break;
            }

            return new StoryQuestProgress(Chapters, visible.ToArray(), completed.ToArray(), active);
        }

        public static StoryQuestChapter[] GetChapters()
        {
            return Chapters.ToArray();
        }
    }
}
