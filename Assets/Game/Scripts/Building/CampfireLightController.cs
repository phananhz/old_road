using UnityEngine;
using TheOldRoad.Construction;
using TheOldRoad.Time;
using TheOldRoad.World;

namespace TheOldRoad.Building
{
    /// <summary>Prototype warm light and flame flicker for completed fire buildings.</summary>
    public sealed class CampfireLightController : MonoBehaviour
    {
        [SerializeField] private ConstructionSite site;
        [SerializeField] private GameTimeController gameTime;

        private SpriteRenderer glowRenderer;
        private SpriteRenderer flameRenderer;

        public void Configure(ConstructionSite site, GameTimeController gameTime)
        {
            this.site = site;
            this.gameTime = gameTime;
            EnsureVisuals();
        }

        private void Update()
        {
            if (site == null) site = GetComponent<ConstructionSite>();
            if (gameTime == null) gameTime = FindAnyObjectByType<GameTimeController>();
            EnsureVisuals();

            bool active = site != null && site.IsCompleted;
            if (glowRenderer != null) glowRenderer.enabled = active;
            if (flameRenderer != null) flameRenderer.enabled = active;
            if (!active) return;

            float sunlight = gameTime != null ? gameTime.SunlightIntensity : 1f;
            float nightAmount = 1f - sunlight;
            float flicker = 0.5f + Mathf.Sin(UnityEngine.Time.time * 7.5f) * 0.5f;
            float scale = Mathf.Lerp(1.8f, 3.6f, Mathf.Clamp01(nightAmount + 0.15f)) + flicker * 0.15f;

            glowRenderer.transform.position = transform.position + new Vector3(0f, 0.34f, 0f);
            glowRenderer.transform.localScale = Vector3.one * scale;
            glowRenderer.color = new Color(1f, 0.55f, 0.16f, Mathf.Lerp(0.16f, 0.46f, Mathf.Clamp01(nightAmount + 0.2f)));

            flameRenderer.transform.position = transform.position + new Vector3(0f, 0.34f + flicker * 0.04f, 0f);
            flameRenderer.transform.localScale = Vector3.one * (0.54f + flicker * 0.08f);
            flameRenderer.color = Color.Lerp(new Color(1f, 0.30f, 0.08f, 1f), new Color(1f, 0.82f, 0.18f, 1f), flicker);
        }

        private void EnsureVisuals()
        {
            if (glowRenderer == null)
            {
                GameObject glow = new GameObject("Campfire Warm Glow");
                glow.transform.SetParent(transform, false);
                glowRenderer = glow.AddComponent<SpriteRenderer>();
                glowRenderer.sprite = PrototypePixelArtFactory.TorchGlow();
                glowRenderer.sortingOrder = 9290;
                glowRenderer.enabled = false;
            }

            if (flameRenderer == null)
            {
                GameObject flame = new GameObject("Campfire Flame Flicker");
                flame.transform.SetParent(transform, false);
                flameRenderer = flame.AddComponent<SpriteRenderer>();
                flameRenderer.sprite = PrototypePixelArtFactory.SolidPixel();
                flameRenderer.sortingOrder = 40;
                flameRenderer.enabled = false;
            }
        }
    }
}
