using System;
using UnityEngine;
using TheOldRoad.Building;
using TheOldRoad.Time;
using TheOldRoad.World;

namespace TheOldRoad.Construction
{
    public sealed class ConstructionSite : MonoBehaviour
    {
        private const float ProgressYOffset = 1.55f;
        private const float BarWidth = 108f;
        private const float BarHeight = 16f;

        [SerializeField] private ConstructionJob job;
        [SerializeField] private BuildingDefinition buildingDefinition;

        private SpriteRenderer siteRenderer;
        private IClock clock;
        private static Texture2D pixel;
        private static GUIStyle labelStyle;

        public ConstructionJob Job => job;
        public BuildingDefinition Definition => buildingDefinition;
        public bool IsCompleted => job != null && job.state == ConstructionState.Completed;
        public string BuildingId => job != null ? job.buildingId : string.Empty;

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
            if (job.buildingId != null && (job.buildingId.Contains("perimeter-fence") || job.buildingId.Contains("animal-pen")))
            {
                siteRenderer.sprite = null;
            }
            else
            {
                siteRenderer.sprite = job.state == ConstructionState.Completed
                    ? PrototypePixelArtFactory.BuildingComplete(job.buildingId)
                    : PrototypePixelArtFactory.BuildingConstruction(job.buildingId, stageIndex);
            }
            siteRenderer.color = Color.white;

            string stageName = GetStageName(now);
            string buildingName = GetBuildingName(job.buildingId);
            gameObject.name = job.state == ConstructionState.Completed
                ? buildingName
                : $"{buildingName} {stageName} {Mathf.RoundToInt(progress * 100f)}%";
        }

        private static string GetBuildingName(string buildingId)
        {
            return TheOldRoad.UI.LocalizationRuntime.BuildingName(buildingId);
        }

        private string GetStageName(long nowUnixSeconds)
        {
            string[] stages = buildingDefinition != null ? buildingDefinition.ConstructionStages : null;
            string stage = (stages == null || stages.Length == 0)
                ? job.GetDefaultVisualStage(nowUnixSeconds).ToString()
                : stages[job.GetStageIndex(nowUnixSeconds, stages.Length)];
            return TheOldRoad.UI.LocalizationRuntime.StageName(stage);
        }

        private void OnGUI()
        {
            if (job == null || job.state == ConstructionState.Completed) return;

            Camera worldCamera = Camera.main;
            if (worldCamera == null) return;

            long now = clock?.NowUnixSeconds ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            float progress = job.GetProgress(now);
            long remainingSeconds = Math.Max(0, job.durationSeconds - Math.Max(0, now - job.startUnixSeconds));

            EnsureStyles();

            Vector3 screenPosition = worldCamera.WorldToScreenPoint(transform.position + Vector3.up * ProgressYOffset);
            if (screenPosition.z < 0f) return;

            float x = screenPosition.x - (BarWidth * 0.5f);
            float y = Screen.height - screenPosition.y - 28f;
            Rect shadow = new Rect(x - 2f, y - 2f, BarWidth + 4f, BarHeight + 4f);
            Rect frame = new Rect(x, y, BarWidth, BarHeight);
            Rect fill = new Rect(x + 2f, y + 2f, (BarWidth - 4f) * progress, BarHeight - 4f);
            Rect text = new Rect(x - 28f, y - 18f, BarWidth + 56f, 16f);

            DrawRect(shadow, new Color(0.02f, 0.02f, 0.02f, 0.72f));
            DrawRect(frame, new Color(0.12f, 0.09f, 0.06f, 0.92f));
            DrawRect(fill, new Color(0.62f, 0.84f, 1f, 0.96f));
            GUI.Label(text, GetStageName(now) + " " + Mathf.RoundToInt(progress * 100f) + "% / " + remainingSeconds + "s", labelStyle);
        }

        private static void EnsureStyles()
        {
            if (pixel == null)
            {
                pixel = new Texture2D(1, 1) { filterMode = FilterMode.Point };
                pixel.SetPixel(0, 0, Color.white);
                pixel.Apply();
            }

            TheOldRoad.UI.UiFontHelper.EnsureGlobalSkinFont();
            labelStyle ??= new GUIStyle(GUI.skin.label)
            {
                font = TheOldRoad.UI.UiFontHelper.CleanFont,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.86f, 0.94f, 1f, 1f) }
            };
        }

        private static void DrawRect(Rect rect, Color color)
        {
            Color previous = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, pixel);
            GUI.color = previous;
        }
    }
}
