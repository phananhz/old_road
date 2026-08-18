using System.Collections;
using UnityEngine;
using TheOldRoad.World;

namespace TheOldRoad.Combat
{
    public sealed class SlashVfx : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        public static void Create(Vector3 position, Vector2 direction, Color color, float scale = 1.0f)
        {
            GameObject go = new GameObject("SlashVfx");
            go.transform.position = position + (Vector3)(direction.normalized * 0.45f);

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            go.transform.rotation = Quaternion.Euler(0, 0, angle);
            go.transform.localScale = Vector3.one * scale;

            SlashVfx vfx = go.AddComponent<SlashVfx>();
            vfx.Initialize(color);
        }

        private void Initialize(Color color)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = PrototypePixelArtFactory.SlashArcSprite;
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = 90;

            StartCoroutine(AnimateSlash());
        }

        private IEnumerator AnimateSlash()
        {
            float duration = 0.16f;
            float elapsed = 0f;
            Vector3 baseScale = transform.localScale;
            Color baseColor = spriteRenderer.color;

            while (elapsed < duration)
            {
                elapsed += UnityEngine.Time.deltaTime;
                float t = elapsed / duration;

                // Stretch slightly and fade out
                transform.localScale = baseScale * (1f + t * 0.35f);
                Color c = baseColor;
                c.a = Mathf.Lerp(1f, 0f, t);
                spriteRenderer.color = c;

                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
