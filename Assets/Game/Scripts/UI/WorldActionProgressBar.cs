using System;
using UnityEngine;

namespace TheOldRoad.UI
{
    /// <summary>Runtime prototype world-space progress bar for short player interactions.</summary>
    public sealed class WorldActionProgressBar : MonoBehaviour
    {
        private const float DefaultYOffset = 1.35f;
        private const float BarWidth = 96f;
        private const float BarHeight = 16f;

        private static WorldActionProgressBar activeTimedAction;

        private Camera worldCamera;
        private Transform target;
        private string label;
        private float durationSeconds;
        private float elapsedSeconds;
        private Action onComplete;
        private Action onCancel;
        private bool finished;

        private static Texture2D pixel;
        private static GUIStyle labelStyle;

        public static bool HasActiveTimedAction => activeTimedAction != null;
        public float Progress => durationSeconds <= 0f ? 1f : Mathf.Clamp01(elapsedSeconds / durationSeconds);

        public static bool TryStart(
            GameObject host,
            Camera worldCamera,
            Transform target,
            string label,
            float durationSeconds,
            Action onComplete,
            Action onCancel,
            out WorldActionProgressBar progressBar)
        {
            progressBar = null;
            if (activeTimedAction != null || host == null || target == null) return false;

            progressBar = host.AddComponent<WorldActionProgressBar>();
            progressBar.Configure(worldCamera, target, label, durationSeconds, onComplete, onCancel);
            activeTimedAction = progressBar;
            return true;
        }

        public void Cancel()
        {
            if (finished) return;

            finished = true;
            if (activeTimedAction == this) activeTimedAction = null;
            onCancel?.Invoke();
            Destroy(this);
        }

        private void Configure(
            Camera worldCamera,
            Transform target,
            string label,
            float durationSeconds,
            Action onComplete,
            Action onCancel)
        {
            this.worldCamera = worldCamera != null ? worldCamera : Camera.main;
            this.target = target;
            this.label = string.IsNullOrWhiteSpace(label) ? "Working" : label;
            this.durationSeconds = Mathf.Max(0.05f, durationSeconds);
            this.onComplete = onComplete;
            this.onCancel = onCancel;
        }

        private void Update()
        {
            if (target == null)
            {
                Cancel();
                return;
            }

            elapsedSeconds += UnityEngine.Time.deltaTime;
            if (elapsedSeconds >= durationSeconds) Complete();
        }

        private void Complete()
        {
            if (finished) return;

            finished = true;
            if (activeTimedAction == this) activeTimedAction = null;
            onComplete?.Invoke();
            Destroy(this);
        }

        private void OnGUI()
        {
            if (target == null) return;
            if (worldCamera == null) worldCamera = Camera.main;
            if (worldCamera == null) return;

            EnsureStyles();

            Vector3 screenPosition = worldCamera.WorldToScreenPoint(target.position + Vector3.up * DefaultYOffset);
            if (screenPosition.z < 0f) return;

            float x = screenPosition.x - (BarWidth * 0.5f);
            float y = Screen.height - screenPosition.y - 28f;
            Rect shadow = new Rect(x - 2f, y - 2f, BarWidth + 4f, BarHeight + 4f);
            Rect frame = new Rect(x, y, BarWidth, BarHeight);
            Rect fill = new Rect(x + 2f, y + 2f, (BarWidth - 4f) * Progress, BarHeight - 4f);
            Rect text = new Rect(x - 20f, y - 18f, BarWidth + 40f, 16f);

            DrawRect(shadow, new Color(0.02f, 0.02f, 0.02f, 0.72f));
            DrawRect(frame, new Color(0.14f, 0.10f, 0.07f, 0.92f));
            DrawRect(fill, new Color(1f, 0.76f, 0.28f, 0.96f));
            GUI.Label(text, label + " " + Mathf.CeilToInt(Mathf.Max(0f, durationSeconds - elapsedSeconds)) + "s", labelStyle);
        }

        private void OnDestroy()
        {
            if (activeTimedAction == this) activeTimedAction = null;
        }

        private static void EnsureStyles()
        {
            if (pixel == null)
            {
                pixel = new Texture2D(1, 1) { filterMode = FilterMode.Point };
                pixel.SetPixel(0, 0, Color.white);
                pixel.Apply();
            }

            UiFontHelper.EnsureGlobalSkinFont();
            labelStyle ??= new GUIStyle(GUI.skin.label)
            {
                font = UiFontHelper.CleanFont,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(1f, 0.94f, 0.76f, 1f) }
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
