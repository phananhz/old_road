using UnityEngine;
using TheOldRoad.Inventory;

namespace TheOldRoad.World
{
    /// <summary>Prototype one-time loot container with stable save identity.</summary>
    public sealed class LootChest : MonoBehaviour
    {
        [SerializeField] private string chestId;
        [SerializeField] private string displayName;
        [SerializeField] private string itemId = "item.wood";
        [SerializeField, Min(1)] private int quantity = 1;

        private bool opened;
        private SpriteRenderer spriteRenderer;
        private SpriteRenderer glowRenderer;
        private Vector3 baseScale;
        private Color baseColor = Color.white;

        public string ChestId => chestId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? gameObject.name : displayName;
        public string ItemId => itemId;
        public int Quantity => quantity;
        public bool IsOpened => opened;

        private void Awake()
        {
            CaptureVisualState();
        }

        public void Configure(string chestId, string displayName, string itemId, int quantity, bool opened = false)
        {
            this.chestId = chestId;
            this.displayName = displayName;
            this.itemId = itemId;
            this.quantity = Mathf.Max(1, quantity);
            CaptureVisualState();
            SetOpened(opened);
        }

        public bool TryOpen(InventoryRuntime inventory)
        {
            if (opened || inventory == null || string.IsNullOrWhiteSpace(itemId) || quantity <= 0) return false;

            inventory.Add(itemId, quantity);
            SetOpened(true);
            return true;
        }

        public void SetOpened(bool opened)
        {
            this.opened = opened;
            CaptureVisualState();

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = opened
                    ? PrototypePixelArtFactory.ChestOpen()
                    : PrototypePixelArtFactory.ChestClosed();
                spriteRenderer.color = opened
                    ? new Color(baseColor.r * 0.75f, baseColor.g * 0.75f, baseColor.b * 0.75f, 1f)
                    : baseColor;
            }

            SetHighlighted(false);
        }

        public void SetHighlighted(bool highlighted)
        {
            CaptureVisualState();
            if (spriteRenderer == null) return;

            transform.localScale = highlighted && !opened ? baseScale * 1.08f : baseScale;
            if (!opened)
            {
                spriteRenderer.color = highlighted
                    ? new Color(1f, 0.90f, 0.45f, 1f)
                    : baseColor;
            }

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
            glowObject.transform.localScale = new Vector3(1.24f, 1.24f, 1f);

            glowRenderer = glowObject.AddComponent<SpriteRenderer>();
            glowRenderer.sprite = spriteRenderer.sprite;
            glowRenderer.color = new Color(1f, 0.74f, 0.18f, 0.58f);
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

            glowRenderer.enabled = visible && !opened;
        }
    }
}
