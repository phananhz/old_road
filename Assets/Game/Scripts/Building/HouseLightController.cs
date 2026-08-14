using UnityEngine;
using TheOldRoad.Construction;
using TheOldRoad.Time;
using TheOldRoad.World;

namespace TheOldRoad.Building
{
    /// <summary>Prototype warm night light and chimney smoke for completed houses.</summary>
    public sealed class HouseLightController : MonoBehaviour
    {
        [SerializeField] private ConstructionSite site;
        [SerializeField] private GameTimeController gameTime;
        [SerializeField] private Vector3 glowOffset = new Vector3(0f, 0.55f, 0f);
        [SerializeField] private Vector3 chimneyOffset = new Vector3(1.25f, 2.65f, 0f);
        [SerializeField, Min(0.2f)] private float glowScale = 0.95f;

        private SpriteRenderer glowRenderer;
        private SpriteRenderer leftWindow;
        private SpriteRenderer rightWindow;
        private SmokeVfxController smoke;

        public void Configure(ConstructionSite site, GameTimeController gameTime, Vector3 chimneyOffset, float glowScale)
        {
            this.site = site;
            this.gameTime = gameTime;
            this.chimneyOffset = chimneyOffset;
            this.glowScale = Mathf.Max(0.2f, glowScale);
            EnsureVisuals();
        }

        private void Update()
        {
            if (site == null) site = GetComponent<ConstructionSite>();
            if (gameTime == null) gameTime = FindAnyObjectByType<GameTimeController>();
            EnsureVisuals();

            bool completed = site != null && site.IsCompleted;
            float sunlight = gameTime != null ? gameTime.SunlightIntensity : 1f;
            float nightAmount = Mathf.Clamp01(1f - sunlight);
            float warmAmount = completed ? Mathf.Clamp01((nightAmount - 0.12f) / 0.88f) : 0f;
            float flicker = 0.5f + Mathf.Sin(UnityEngine.Time.time * 3.7f) * 0.5f;

            bool lightActive = warmAmount > 0.02f;
            glowRenderer.enabled = lightActive;
            leftWindow.enabled = lightActive;
            rightWindow.enabled = lightActive;
            if (!lightActive) return;

            glowRenderer.transform.position = transform.position + glowOffset;
            glowRenderer.transform.localScale = Vector3.one * (glowScale + flicker * 0.035f);
            glowRenderer.color = new Color(1f, 0.62f, 0.20f, Mathf.Lerp(0.08f, 0.28f, warmAmount));

            Color windowColor = Color.Lerp(new Color(0.88f, 0.38f, 0.08f, 0.70f), new Color(1f, 0.78f, 0.28f, 1f), flicker);
            leftWindow.color = windowColor;
            rightWindow.color = windowColor;
        }

        private void EnsureVisuals()
        {
            if (glowRenderer == null)
            {
                GameObject glow = new GameObject("House Warm Window Glow");
                glow.transform.SetParent(transform, false);
                glowRenderer = glow.AddComponent<SpriteRenderer>();
                glowRenderer.sprite = PrototypePixelArtFactory.TorchGlow();
                glowRenderer.sortingOrder = 9285;
                glowRenderer.enabled = false;
            }

            if (leftWindow == null)
            {
                leftWindow = CreateWindowLight("Left Window Light", new Vector3(-0.55f, 0.58f, 0f));
            }

            if (rightWindow == null)
            {
                rightWindow = CreateWindowLight("Right Window Light", new Vector3(0.72f, 0.58f, 0f));
            }

            if (smoke == null)
            {
                smoke = gameObject.GetComponent<SmokeVfxController>();
                if (smoke == null) smoke = gameObject.AddComponent<SmokeVfxController>();
                smoke.Configure(site, chimneyOffset, 5, 1.65f, 0.42f, 0.34f, 0.30f, 9325);
            }
        }

        private SpriteRenderer CreateWindowLight(string name, Vector3 localOffset)
        {
            GameObject window = new GameObject(name);
            window.transform.SetParent(transform, false);
            window.transform.localPosition = localOffset;
            window.transform.localScale = new Vector3(0.22f, 0.14f, 1f);
            SpriteRenderer renderer = window.AddComponent<SpriteRenderer>();
            renderer.sprite = PrototypePixelArtFactory.SolidPixel();
            renderer.sortingOrder = 9315;
            renderer.enabled = false;
            return renderer;
        }
    }
}
