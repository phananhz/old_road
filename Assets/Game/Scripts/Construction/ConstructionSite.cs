using System;
using UnityEngine;
using TheOldRoad.Building;
using TheOldRoad.Time;
using TheOldRoad.World;

namespace TheOldRoad.Construction
{
    public sealed class ConstructionSite : MonoBehaviour
    {
        [SerializeField] private ConstructionJob job;
        [SerializeField] private BuildingDefinition buildingDefinition;

        private SpriteRenderer siteRenderer;
        private IClock clock;

        public ConstructionJob Job => job;

        private void Awake()
        {
            siteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Configure(ConstructionJob job, BuildingDefinition buildingDefinition, IClock clock)
        {
            this.job = job;
            this.buildingDefinition = buildingDefinition;
            this.clock = clock;
            siteRenderer = GetComponent<SpriteRenderer>();
            RefreshVisual();
        }

        private void Update()
        {
            if (job == null) return;

            long now = clock?.NowUnixSeconds ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            job.Refresh(now);
            RefreshVisual();
        }

        private void RefreshVisual()
        {
            if (siteRenderer == null || job == null) return;

            long now = clock?.NowUnixSeconds ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            float progress = job.GetProgress(now);
            int stageIndex = job.GetStageIndex(now, 5);
            siteRenderer.sprite = job.state == ConstructionState.Completed
                ? PrototypePixelArtFactory.CabinComplete()
                : PrototypePixelArtFactory.CabinConstruction(stageIndex);
            siteRenderer.color = Color.white;

            string stageName = GetStageName(now);
            gameObject.name = job.state == ConstructionState.Completed
                ? "Cabin"
                : $"Cabin {stageName} {Mathf.RoundToInt(progress * 100f)}%";
        }

        private string GetStageName(long nowUnixSeconds)
        {
            string[] stages = buildingDefinition != null ? buildingDefinition.ConstructionStages : null;
            if (stages == null || stages.Length == 0) return job.GetDefaultVisualStage(nowUnixSeconds).ToString();
            return stages[job.GetStageIndex(nowUnixSeconds, stages.Length)];
        }
    }
}
