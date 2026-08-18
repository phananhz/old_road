using TheOldRoad.UI;

namespace TheOldRoad.Quest
{
    public sealed class StoryQuestChapter
    {
        public StoryQuestChapter(string chapterId, string title, string summary, StoryQuestStep[] steps)
        {
            ChapterId = chapterId;
            RawTitle = title;
            RawSummary = summary;
            Steps = steps ?? System.Array.Empty<StoryQuestStep>();
        }

        public string ChapterId { get; }
        public string RawTitle { get; }
        public string RawSummary { get; }
        public StoryQuestStep[] Steps { get; }

        public string Title => LocalizationRuntime.T(ChapterId + ".title") != ChapterId + ".title"
            ? LocalizationRuntime.T(ChapterId + ".title")
            : RawTitle;

        public string Summary => LocalizationRuntime.T(ChapterId + ".summary") != ChapterId + ".summary"
            ? LocalizationRuntime.T(ChapterId + ".summary")
            : RawSummary;
    }
}
