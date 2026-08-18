using System;

namespace TheOldRoad.Quest
{
    public sealed class StoryQuestProgress
    {
        public StoryQuestProgress(
            StoryQuestChapter[] chapters,
            StoryQuestStep[] visibleSteps,
            StoryQuestStep[] completedSteps,
            StoryQuestStep activeStep)
        {
            Chapters = chapters ?? Array.Empty<StoryQuestChapter>();
            VisibleSteps = visibleSteps ?? Array.Empty<StoryQuestStep>();
            CompletedSteps = completedSteps ?? Array.Empty<StoryQuestStep>();
            ActiveStep = activeStep;
        }

        public StoryQuestChapter[] Chapters { get; }
        public StoryQuestStep[] VisibleSteps { get; }
        public StoryQuestStep[] CompletedSteps { get; }
        public StoryQuestStep ActiveStep { get; }
        public int CompletedCount => CompletedSteps.Length;
        public int TotalCount => VisibleSteps.Length;
    }
}
