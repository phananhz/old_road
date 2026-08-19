using System.Collections.Generic;
using UnityEngine;
using TheOldRoad.UI;

namespace TheOldRoad.Combat
{
    public sealed class FloatingTextController : MonoBehaviour
    {
        private static FloatingTextController instance;
        public static FloatingTextController Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("FloatingTextController");
                    instance = go.AddComponent<FloatingTextController>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private struct FloatingEntry
        {
            public string Text;
            public Vector3 WorldPos;
            public Color Color;
            public float Elapsed;
            public float Duration;
            public float FloatOffset;
        }

        private readonly List<FloatingEntry> entries = new List<FloatingEntry>(32);
        private GUIStyle textStyle;

        private void Awake()
        {
            if (instance == null) instance = this;
            else if (instance != this) Destroy(gameObject);
        }

        public static void Spawn(string text, Vector3 worldPosition, Color color, float duration = 0.9f)
        {
            Instance.AddEntry(text, worldPosition, color, duration);
        }

        public static void SpawnDamage(int amount, Vector3 worldPosition, bool isCritical = false)
        {
            Color c = isCritical ? new Color(1f, 0.35f, 0.15f, 1f) : new Color(0.95f, 0.85f, 0.25f, 1f);
            string prefix = isCritical ? "!" : "-";
            Spawn(prefix + amount, worldPosition + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0.2f, 0.5f), 0f), c, 0.85f);
        }

        public static void SpawnPlayerDamage(int amount, Vector3 worldPosition)
        {
            Color c = new Color(0.95f, 0.22f, 0.22f, 1f);
            Spawn("-" + amount + " HP", worldPosition + new Vector3(0f, 0.6f, 0f), c, 1.0f);
        }

        public static void SpawnHeal(int amount, Vector3 worldPosition)
        {
            Color c = new Color(0.35f, 0.95f, 0.45f, 1f);
            Spawn("+" + amount + " HP", worldPosition + new Vector3(0f, 0.6f, 0f), c, 1.0f);
        }

        private void AddEntry(string text, Vector3 worldPos, Color color, float duration)
        {
            entries.Add(new FloatingEntry
            {
                Text = text,
                WorldPos = worldPos,
                Color = color,
                Elapsed = 0f,
                Duration = duration,
                FloatOffset = Random.Range(0.5f, 0.9f)
            });
        }

        private void Update()
        {
            float dt = UnityEngine.Time.deltaTime;
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                FloatingEntry entry = entries[i];
                entry.Elapsed += dt;
                if (entry.Elapsed >= entry.Duration)
                {
                    entries.RemoveAt(i);
                }
                else
                {
                    entries[i] = entry;
                }
            }
        }

        private void OnGUI()
        {
            if (entries.Count == 0) return;
            Camera cam = Camera.main;
            if (cam == null) return;

            UiFontHelper.EnsureGlobalSkinFont();
            if (textStyle == null)
            {
                textStyle = new GUIStyle(GUI.skin.label)
                {
                    font = UiFontHelper.CleanFont,
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            for (int i = 0; i < entries.Count; i++)
            {
                FloatingEntry entry = entries[i];
                float progress = Mathf.Clamp01(entry.Elapsed / entry.Duration);
                float yOffset = Mathf.Lerp(0f, entry.FloatOffset, Mathf.Sin(progress * Mathf.PI * 0.5f));
                Vector3 currentWorldPos = entry.WorldPos + new Vector3(0f, yOffset, 0f);

                Vector3 screenPos = cam.WorldToScreenPoint(currentWorldPos);
                if (screenPos.z < 0) continue;

                float alpha = 1f - Mathf.Pow(progress, 2f);
                Color color = entry.Color;
                color.a = alpha;

                float guiY = Screen.height - screenPos.y;
                Rect rect = new Rect(screenPos.x - 50f, guiY - 12f, 100f, 24f);

                // Draw shadow
                GUI.color = new Color(0f, 0f, 0f, alpha * 0.8f);
                GUI.Label(new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height), entry.Text, textStyle);

                // Draw text
                GUI.color = color;
                GUI.Label(rect, entry.Text, textStyle);
            }

            GUI.color = Color.white;
        }
    }
}
