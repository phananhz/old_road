using UnityEngine;

namespace TheOldRoad.UI
{
    /// <summary>Small world-space speech bubble used for prototype player self-talk.</summary>
    public sealed class PlayerSpeechBubble : MonoBehaviour
    {
        private const float DefaultDurationSeconds = 2.8f;
        private const float FadeSeconds = 0.35f;
        private const float YOffset = 1.45f;

        private static PlayerSpeechBubble primary;
        private static Texture2D pixel;
        private static GUIStyle textStyle;

        [SerializeField] private Camera worldCamera;

        private string currentText = string.Empty;
        private float hideTime;
        private float durationSeconds = DefaultDurationSeconds;

        public static void Say(string localizationKey, float durationSeconds = DefaultDurationSeconds)
        {
            PlayerSpeechBubble bubble = primary != null ? primary : FindAnyObjectByType<PlayerSpeechBubble>();
            if (bubble == null) return;

            bubble.Show(LocalizationRuntime.T(localizationKey), durationSeconds);
        }

        public static void SayText(string text, float durationSeconds = DefaultDurationSeconds)
        {
            PlayerSpeechBubble bubble = primary != null ? primary : FindAnyObjectByType<PlayerSpeechBubble>();
            if (bubble == null) return;

            bubble.Show(text, durationSeconds);
        }

        public void Configure(Camera worldCamera)
        {
            this.worldCamera = worldCamera != null ? worldCamera : Camera.main;
        }

        private void Awake()
        {
            primary = this;
            if (worldCamera == null) worldCamera = Camera.main;
        }

        private void OnDestroy()
        {
            if (primary == this) primary = null;
        }

        private void Show(string text, float seconds)
        {
            if (string.IsNullOrWhiteSpace(text)) return;

            currentText = text;
            durationSeconds = Mathf.Max(0.5f, seconds);
            hideTime = UnityEngine.Time.unscaledTime + durationSeconds;
        }

        private void OnGUI()
        {
            if (string.IsNullOrWhiteSpace(currentText)) return;
            if (GameStartMenuController.IsOpen) return;
            if (worldCamera == null) worldCamera = Camera.main;
            if (worldCamera == null) return;

            float remaining = hideTime - UnityEngine.Time.unscaledTime;
            if (remaining <= 0f)
            {
                currentText = string.Empty;
                return;
            }

            EnsureStyles();

            Vector3 screenPosition = worldCamera.WorldToScreenPoint(transform.position + Vector3.up * YOffset);
            if (screenPosition.z < 0f) return;

            float alpha = remaining < FadeSeconds ? Mathf.Clamp01(remaining / FadeSeconds) : 1f;
            float width = Mathf.Clamp(textStyle.CalcSize(new GUIContent(currentText)).x + 38f, 140f, 280f);
            float height = Mathf.Clamp(textStyle.CalcHeight(new GUIContent(currentText), width - 26f) + 20f, 42f, 88f);
            float x = Mathf.Clamp(screenPosition.x - width * 0.5f, 8f, Screen.width - width - 8f);
            float y = Mathf.Clamp(Screen.height - screenPosition.y - height - 8f, 8f, Screen.height - height - 8f);

            Rect shadow = new Rect(x + 4f, y + 5f, width, height);
            Rect bubble = new Rect(x, y, width, height);
            Rect textRect = new Rect(x + 13f, y + 8f, width - 26f, height - 16f);
            float tailX = Mathf.Clamp(screenPosition.x - 8f, bubble.x + 28f, bubble.xMax - 44f);
            Rect tailShadow = new Rect(tailX + 4f, bubble.yMax + 2f, 16f, 12f);
            Rect tail = new Rect(tailX, bubble.yMax - 1f, 16f, 12f);

            Color previousColor = GUI.color;
            DrawRect(shadow, new Color(0f, 0f, 0f, 0.36f * alpha));
            DrawRect(bubble, new Color(0.96f, 0.90f, 0.72f, 0.96f * alpha));
            DrawRect(new Rect(bubble.x, bubble.y, bubble.width, 3f), new Color(0.29f, 0.18f, 0.08f, alpha));
            DrawRect(new Rect(bubble.x, bubble.yMax - 3f, bubble.width, 3f), new Color(0.29f, 0.18f, 0.08f, alpha));
            DrawRect(new Rect(bubble.x, bubble.y, 3f, bubble.height), new Color(0.29f, 0.18f, 0.08f, alpha));
            DrawRect(new Rect(bubble.xMax - 3f, bubble.y, 3f, bubble.height), new Color(0.29f, 0.18f, 0.08f, alpha));
            DrawRect(tailShadow, new Color(0f, 0f, 0f, 0.24f * alpha));
            DrawRect(tail, new Color(0.96f, 0.90f, 0.72f, 0.96f * alpha));
            DrawRect(new Rect(tail.x, tail.yMax - 3f, tail.width, 3f), new Color(0.29f, 0.18f, 0.08f, alpha));

            GUI.color = new Color(1f, 1f, 1f, alpha);
            GUI.Label(textRect, currentText, textStyle);
            GUI.color = previousColor;
        }

        private static void EnsureStyles()
        {
            if (pixel == null)
            {
                pixel = new Texture2D(1, 1, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Point,
                    hideFlags = HideFlags.DontSave
                };
                pixel.SetPixel(0, 0, Color.white);
                pixel.Apply(false, true);
            }

            textStyle ??= new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                wordWrap = true,
                normal = { textColor = new Color(0.12f, 0.075f, 0.035f, 1f) }
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
