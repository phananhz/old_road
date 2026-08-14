using UnityEngine;

namespace TheOldRoad.World
{
    /// <summary>Small exploration target for the prototype map.</summary>
    public sealed class DiscoverableLandmark : MonoBehaviour
    {
        [SerializeField] private string landmarkId;
        [SerializeField] private string title;
        [SerializeField, TextArea] private string journalText;

        private bool discovered;
        private SpriteRenderer spriteRenderer;
        private SpriteRenderer glowRenderer;
        private Vector3 baseScale;
        private Color baseColor = Color.white;

        public string LandmarkId => landmarkId;
        public string Title => string.IsNullOrWhiteSpace(title) ? gameObject.name : title;
        public string JournalText => journalText;
        public bool IsDiscovered => discovered;

        private void Awake()
        {
            CaptureVisualState();
        }

        public void Configure(string landmarkId, string title, string journalText, bool discovered = false)
        {
            this.landmarkId = landmarkId;
            this.title = title;
            this.journalText = journalText;
            CaptureVisualState();
            SetDiscovered(discovered);
        }

        public bool Discover()
        {
            if (discovered || string.IsNullOrWhiteSpace(landmarkId)) return false;
            SetDiscovered(true);
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
            glowRenderer.color = new Color(0.42f, 0.78f, 1f, 0.52f);
            glowRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
            glowRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
            glowRenderer.enabled = false;
        }

        private void SetGlowVisible(bool visible)
        {
            EnsureGlowRenderer();
            if (glowRenderer == null) return;

            glowRenderer.sprite = spriteRenderer != null ? spriteRenderer.sprite : glowRenderer.sprite;
            if (spriteRenderer != null)
            {
                glowRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
                glowRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
            }
            glowRenderer.enabled = visible && !discovered;
        }
    }
}
