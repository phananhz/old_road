using System;
using UnityEngine;
using TheOldRoad.Save;

namespace TheOldRoad.Construction
{
    public enum ConstructionState { Planned, Constructing, Completed }
    public enum ConstructionVisualStage { Foundation, Frame, Walls, Roof, Complete }

    [Serializable]
    public sealed class ConstructionJob
    {
        public string constructionId;
        public string buildingId;
        public long startUnixSeconds;
        public long durationSeconds;
        public int gridX;
        public int gridY;
        public ConstructionState state;

        public Vector2Int Placement => new Vector2Int(gridX, gridY);

        public float GetProgress(long nowUnixSeconds)
        {
            if (state == ConstructionState.Completed) return 1f;
            if (durationSeconds <= 0) return 1f;
            return Mathf.Clamp01((float)Math.Max(0, nowUnixSeconds - startUnixSeconds) / durationSeconds);
        }

        public void Refresh(long nowUnixSeconds)
        {
            if (GetProgress(nowUnixSeconds) >= 1f) state = ConstructionState.Completed;
        }

        public int GetStageIndex(long nowUnixSeconds, int stageCount)
        {
            if (stageCount <= 1) return 0;
            if (state == ConstructionState.Completed) return stageCount - 1;
            return Mathf.Clamp(Mathf.FloorToInt(GetProgress(nowUnixSeconds) * stageCount), 0, stageCount - 1);
        }

        public ConstructionVisualStage GetDefaultVisualStage(long nowUnixSeconds)
        {
            int index = GetStageIndex(nowUnixSeconds, 5);
            return (ConstructionVisualStage)index;
        }

        public ConstructionSaveEntry ToSaveEntry()
        {
            return new ConstructionSaveEntry
            {
                constructionId = constructionId,
                buildingId = buildingId,
                startUnixSeconds = startUnixSeconds,
                durationSeconds = durationSeconds,
                gridX = gridX,
                gridY = gridY,
                state = state.ToString()
            };
        }

        public static ConstructionJob FromSaveEntry(ConstructionSaveEntry entry)
        {
            Enum.TryParse(entry.state, out ConstructionState parsedState);
            return new ConstructionJob
            {
                constructionId = entry.constructionId,
                buildingId = entry.buildingId,
                startUnixSeconds = entry.startUnixSeconds,
                durationSeconds = entry.durationSeconds,
                gridX = entry.gridX,
                gridY = entry.gridY,
                state = parsedState
            };
        }
    }
}
