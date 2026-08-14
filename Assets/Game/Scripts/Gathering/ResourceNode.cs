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
        private bool harvested;
        private SpriteRenderer spriteRenderer;
        private Vector3 baseScale;
        private Color baseColor = Color.white;

        public string NodeId => nodeId;
        public string ResourceItemId => resourceItemId;
        public int ResourceAmount => resourceAmount;
        public bool IsHarvested => harvested;
        public string DisplayName => gameObject.name;

        private void Awake()
        {
            CaptureVisualState();
        }

        public void Configure(string nodeId, string resourceItemId, int resourceAmount, bool harvested = false)
        {
            this.nodeId = nodeId;
            this.resourceItemId = resourceItemId;
            this.resourceAmount = Mathf.Max(1, resourceAmount);
            CaptureVisualState();
            SetHarvested(harvested);
        }

        public bool TryHarvest(InventoryRuntime inventory)
        {
            if (harvested || inventory == null || string.IsNullOrWhiteSpace(resourceItemId) || resourceAmount <= 0)
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
    }
}
