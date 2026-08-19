using UnityEngine;
using TheOldRoad.Combat;
using TheOldRoad.Core;
using TheOldRoad.UI;

namespace TheOldRoad.World
{
    /// <summary>
    /// Interactive & Auto-Discoverable Landmark / Settlement / NPC POI on the World Map.
    /// When the player visits or gets close, it automatically saves and unlocks on the Map & Minimap.
    /// </summary>
    public sealed class DiscoverableLandmark : MonoBehaviour
    {
        [SerializeField] private string landmarkId;
        [SerializeField] private string title;
        [SerializeField, TextArea] private string journalText;
        [SerializeField] private string mapIconEmoji = "★";
        [SerializeField] private Color mapColor = new Color(0.95f, 0.78f, 0.25f, 1f);
        [SerializeField] private float autoDiscoverRadius = 6.0f;

        private bool discovered;
        private SpriteRenderer spriteRenderer;
        private SpriteRenderer glowRenderer;
        private Vector3 baseScale;
        private Color baseColor = Color.white;
        private Transform playerTransform;

        public string LandmarkId => landmarkId;
        public string Title => TheOldRoad.UI.LocalizationRuntime.T(landmarkId + ".title") != landmarkId + ".title"
            ? TheOldRoad.UI.LocalizationRuntime.T(landmarkId + ".title")
            : (string.IsNullOrWhiteSpace(title) ? gameObject.name : title);
        public string JournalText => TheOldRoad.UI.LocalizationRuntime.T(landmarkId + ".journal") != landmarkId + ".journal"
            ? TheOldRoad.UI.LocalizationRuntime.T(landmarkId + ".journal")
            : journalText;
        public bool IsDiscovered => discovered;
        public string MapIconEmoji => string.IsNullOrWhiteSpace(mapIconEmoji) ? "★" : mapIconEmoji;
        public Color MapColor => mapColor;

        private void Awake()
        {
            CaptureVisualState();
        }

        private void Update()
        {
            if (discovered) return;

            if (playerTransform == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null) playerTransform = player.transform;
                return;
            }

            if (autoDiscoverRadius > 0f)
            {
                float dist = Vector2.Distance(playerTransform.position, transform.position);
                if (dist <= autoDiscoverRadius)
                {
                    DiscoverWithPopup();
                }
            }
        }

        public void Configure(
            string landmarkId,
            string title,
            string journalText,
            bool discovered = false,
            string mapIconEmoji = "★",
            Color? mapColor = null,
            float autoDiscoverRadius = 6.0f)
        {
            this.landmarkId = landmarkId;
            this.title = title;
            this.journalText = journalText;
            this.mapIconEmoji = mapIconEmoji;
            if (mapColor.HasValue) this.mapColor = mapColor.Value;
            this.autoDiscoverRadius = autoDiscoverRadius;
            CaptureVisualState();
            SetDiscovered(discovered);
        }

        public bool Discover()
        {
            if (discovered || string.IsNullOrWhiteSpace(landmarkId)) return false;
            TheOldRoad.Audio.AudioManager.PlayGatherSuccess();
            SetDiscovered(true);
            return true;
        }

        public bool DiscoverWithPopup()
        {
            if (discovered || string.IsNullOrWhiteSpace(landmarkId)) return false;
            TheOldRoad.Audio.AudioManager.PlayGatherSuccess();
            SetDiscovered(true);

            string discMsg = LocalizationRuntime.IsVietnamese
                ? $"🗺️ ĐÃ KHÁM PHÁ: {Title}"
                : $"🗺️ DISCOVERED: {Title}";
            FloatingTextController.Spawn(discMsg, transform.position + Vector3.up * 1.6f, new Color(1f, 0.88f, 0.2f), 2.5f);

            var slice = FindAnyObjectByType<VerticalSliceController>();
            slice?.NotifyLandmarkDiscovered(this);
            return true;
        }

        public void SetDiscovered(bool discovered)
        {
            this.discovered = discovered;
            CaptureVisualState();
            if (spriteRenderer == null) return;

            spriteRenderer.color = discovered
                ? new Color(baseColor.r * 1.12f, baseColor.g * 1.12f, baseColor.b * 1.12f, 1f)
                : baseColor;

            SetGlowVisible(false);
        }

        public void SetHighlighted(bool highlighted)
        {
            CaptureVisualState();
            if (spriteRenderer == null) return;

            transform.localScale = highlighted && !discovered ? baseScale * 1.08f : baseScale;
            if (discovered) return;

            spriteRenderer.color = highlighted
                ? new Color(0.72f, 0.86f, 1f, 1f)
                : baseColor;

            SetGlowVisible(highlighted);
        }

        private void CaptureVisualState()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null) baseColor = spriteRenderer.color;
            }

            if (baseScale == Vector3.zero) baseScale = transform.localScale;
            EnsureGlowRenderer();
        }

        private void EnsureGlowRenderer()
        {
            if (spriteRenderer == null || glowRenderer != null) return;

            GameObject glowObject = new GameObject("Interaction Glow");
            glowObject.transform.SetParent(transform, false);
            glowObject.transform.localPosition = Vector3.zero;
            glowObject.transform.localRotation = Quaternion.identity;
            glowObject.transform.localScale = new Vector3(1.20f, 1.20f, 1f);

            glowRenderer = glowObject.AddComponent<SpriteRenderer>();
            glowRenderer.sprite = spriteRenderer.sprite;
            glowRenderer.color = new Color(0.38f, 0.82f, 1f, 0.45f);
            glowRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
            glowRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
            glowRenderer.enabled = false;
        }

        private void SetGlowVisible(bool visible)
        {
            if (glowRenderer != null)
            {
                glowRenderer.enabled = visible;
            }
        }
    }
}
