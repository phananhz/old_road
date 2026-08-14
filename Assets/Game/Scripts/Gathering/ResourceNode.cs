using UnityEngine;
using TheOldRoad.Inventory;

namespace TheOldRoad.Gathering
{
    /// <summary>Configurable resource node. It rewards inventory through gameplay API only.</summary>
    public sealed class ResourceNode : MonoBehaviour
    {
        [SerializeField] private string nodeId;
        [SerializeField] private string resourceItemId = "wood";
        [SerializeField, Min(1)] private int resourceAmount = 1;
        [SerializeField] private string requiredToolItemId = string.Empty;
        private bool harvested;
        private SpriteRenderer spriteRenderer;
        private SpriteRenderer glowRenderer;
        private Vector3 baseScale;
        private Color baseColor = Color.white;

        public string NodeId => nodeId;
        public string ResourceItemId => resourceItemId;
        public int ResourceAmount => resourceAmount;
        public string RequiredToolItemId => requiredToolItemId;
        public bool IsHarvested => harvested;
        public string DisplayName => gameObject.name;
        public bool RequiresTool => !string.IsNullOrWhiteSpace(requiredToolItemId);

        private void Awake()
        {
            CaptureVisualState();
        }

        public void Configure(string nodeId, string resourceItemId, int resourceAmount, bool harvested = false, string requiredToolItemId = "")
        {
            this.nodeId = nodeId;
            this.resourceItemId = resourceItemId;
            this.resourceAmount = Mathf.Max(1, resourceAmount);
            this.requiredToolItemId = requiredToolItemId ?? string.Empty;
            CaptureVisualState();
            SetHarvested(harvested);
        }

        public bool CanHarvest(InventoryRuntime inventory)
        {
            return !harvested
                && inventory != null
                && (!RequiresTool || inventory.Has(requiredToolItemId, 1));
        }

        public bool TryHarvest(InventoryRuntime inventory)
        {
            if (!CanHarvest(inventory) || string.IsNullOrWhiteSpace(resourceItemId) || resourceAmount <= 0)
                return false;

            inventory.Add(resourceItemId, resourceAmount);
            SetHarvested(true);
            return true;
        }

        public void SetHarvested(bool harvested)
        {
            this.harvested = harvested;
            CaptureVisualState();
            if (spriteRenderer != null)
            {
                Color color = baseColor;
                color.a = harvested ? 0.35f : 1f;
                spriteRenderer.color = color;
            }

            SetGlowVisible(false);
        }

        public void SetHighlighted(bool highlighted)
        {
            CaptureVisualState();
            if (spriteRenderer == null) return;

            transform.localScale = highlighted && !harvested ? baseScale * 1.08f : baseScale;
            if (harvested) return;

            spriteRenderer.color = highlighted
                ? new Color(1f, 0.95f, 0.62f, 1f)
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
        }

        private void EnsureGlowRenderer()
        {
            if (spriteRenderer == null || glowRenderer != null) return;

            GameObject glowObject = new GameObject("Interaction Glow");
            glowObject.transform.SetParent(transform, false);
            glowObject.transform.localPosition = Vector3.zero;
            glowObject.transform.localRotation = Quaternion.identity;
            glowObject.transform.localScale = new Vector3(1.22f, 1.22f, 1f);

            glowRenderer = glowObject.AddComponent<SpriteRenderer>();
            glowRenderer.sprite = spriteRenderer.sprite;
            glowRenderer.color = new Color(1f, 0.86f, 0.28f, 0.58f);
            glowRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
            glowRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
            glowRenderer.enabled = false;
        }

        private void SetGlowVisible(bool visible)
        {
            if (visible) EnsureGlowRenderer();
            if (glowRenderer == null) return;

            glowRenderer.sprite = spriteRenderer != null ? spriteRenderer.sprite : glowRenderer.sprite;
            if (spriteRenderer != null)
            {
                glowRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
                glowRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
            }
            glowRenderer.enabled = visible && !harvested;
        }
    }
}
