using System;
using TheOldRoad.UI;

namespace TheOldRoad.Quest
{
    public sealed class StoryQuestStep
    {
        private readonly Func<StoryQuestContext, bool> completionRule;

        public StoryQuestStep(
            string stepId,
            string title,
            string detail,
            string storyEntry,
            Func<StoryQuestContext, bool> completionRule)
        {
            StepId = stepId;
            RawTitle = title;
            RawDetail = detail;
            RawStoryEntry = storyEntry;
            this.completionRule = completionRule ?? (_ => false);
        }

        public string StepId { get; }
        public string RawTitle { get; }
        public string RawDetail { get; }
        public string RawStoryEntry { get; }

        public string Title => LocalizationRuntime.T(StepId + ".title") != StepId + ".title"
            ? LocalizationRuntime.T(StepId + ".title")
            : RawTitle;

        public string Detail => LocalizationRuntime.T(StepId + ".detail") != StepId + ".detail"
            ? LocalizationRuntime.T(StepId + ".detail")
            : RawDetail;

        public string StoryEntry => LocalizationRuntime.T(StepId + ".lore") != StepId + ".lore"
            ? LocalizationRuntime.T(StepId + ".lore")
            : RawStoryEntry;

        public bool IsComplete(StoryQuestContext context)
        {
            return context != null && completionRule(context);
        }
    }
}
