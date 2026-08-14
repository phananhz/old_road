using UnityEngine;

namespace TheOldRoad.World
{
    /// <summary>Sorts top-down sprites so lower objects render in front.</summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class YSortSprite : MonoBehaviour
    {
        [SerializeField] private int sortingOffset;
        private SpriteRenderer spriteRenderer;

        public void Configure(int sortingOffset)
        {
            this.sortingOffset = sortingOffset;
            Apply();
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            Apply();
        }

        private void LateUpdate()
        {
            Apply();
        }

        private void Apply()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return;
            spriteRenderer.sortingOrder = sortingOffset - Mathf.RoundToInt(transform.position.y * 100f);
        }
    }
}
