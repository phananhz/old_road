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
        private SpriteRenderer emberRenderer;
        private SmokeVfxController smoke;

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
            if (emberRenderer != null) emberRenderer.enabled = active;
            if (!active) return;

            float sunlight = gameTime != null ? gameTime.SunlightIntensity : 1f;
            float nightAmount = 1f - sunlight;
            float fastFlicker = 0.5f + Mathf.Sin(UnityEngine.Time.time * 11.5f) * 0.5f;
            float slowFlicker = 0.5f + Mathf.Sin(UnityEngine.Time.time * 5.4f + 1.2f) * 0.5f;
            float combinedFlicker = Mathf.Clamp01(fastFlicker * 0.65f + slowFlicker * 0.35f);
            float scale = Mathf.Lerp(0.38f, 0.72f, Mathf.Clamp01(nightAmount + 0.18f)) + combinedFlicker * 0.05f;

            glowRenderer.transform.position = transform.position + new Vector3(0f, 0.34f, 0f);
            glowRenderer.transform.localScale = Vector3.one * scale;
            glowRenderer.color = new Color(1f, 0.55f, 0.16f, Mathf.Lerp(0.12f, 0.38f, Mathf.Clamp01(nightAmount + 0.2f)));

            flameRenderer.transform.position = transform.position + new Vector3(0f, 0.34f + combinedFlicker * 0.08f, 0f);
            flameRenderer.transform.localScale = new Vector3(0.44f + slowFlicker * 0.18f, 0.72f + fastFlicker * 0.22f, 1f);
            flameRenderer.color = Color.Lerp(new Color(1f, 0.24f, 0.05f, 1f), new Color(1f, 0.86f, 0.20f, 1f), combinedFlicker);

            emberRenderer.transform.position = transform.position + new Vector3(0.06f * Mathf.Sin(UnityEngine.Time.time * 8f), 0.22f, 0f);
            emberRenderer.transform.localScale = new Vector3(0.66f + combinedFlicker * 0.15f, 0.24f + slowFlicker * 0.06f, 1f);
            emberRenderer.color = new Color(1f, 0.36f, 0.08f, 0.92f);
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
                flameRenderer.sortingOrder = 9310;
                flameRenderer.enabled = false;
            }

            if (emberRenderer == null)
            {
                GameObject ember = new GameObject("Campfire Ember Pulse");
                ember.transform.SetParent(transform, false);
                emberRenderer = ember.AddComponent<SpriteRenderer>();
                emberRenderer.sprite = PrototypePixelArtFactory.SolidPixel();
                emberRenderer.sortingOrder = 9308;
                emberRenderer.enabled = false;
            }

            if (smoke == null)
            {
                smoke = GetComponent<SmokeVfxController>();
                if (smoke == null) smoke = gameObject.AddComponent<SmokeVfxController>();
                smoke.Configure(site, new Vector3(0f, 0.72f, 0f), 4, 1.15f, 0.26f, 0.25f, 0.42f, 9320);
            }
        }
    }
}
