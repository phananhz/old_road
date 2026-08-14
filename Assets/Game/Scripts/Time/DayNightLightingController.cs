using UnityEngine;
using TheOldRoad.Inventory;
using TheOldRoad.Player;
using TheOldRoad.World;

namespace TheOldRoad.Time
{
    /// <summary>Prototype day/night tint and portable torch glow. HUD remains unaffected.</summary>
    [RequireComponent(typeof(Camera))]
    public sealed class DayNightLightingController : MonoBehaviour
    {
        [SerializeField] private GameTimeController gameTime;
        [SerializeField] private InventorySession inventorySession;

        private Camera worldCamera;
        private SpriteRenderer nightOverlay;
        private SpriteRenderer torchGlow;
        private Color dayBackground = new Color(0.40f, 0.63f, 0.70f, 1f);
        private Color duskBackground = new Color(0.32f, 0.22f, 0.20f, 1f);
        private Color nightBackground = new Color(0.035f, 0.045f, 0.075f, 1f);

        public void Configure(GameTimeController gameTime, InventorySession inventorySession)
        {
            this.gameTime = gameTime;
            this.inventorySession = inventorySession;
            EnsureVisuals();
        }

        private void Awake()
        {
            worldCamera = GetComponent<Camera>();
            EnsureVisuals();
        }

        private void LateUpdate()
        {
            if (gameTime == null) gameTime = FindAnyObjectByType<GameTimeController>();
            if (inventorySession == null) inventorySession = FindAnyObjectByType<InventorySession>();
            EnsureVisuals();

            float sunlight = gameTime != null ? gameTime.SunlightIntensity : 1f;
            float nightAmount = 1f - sunlight;
            Color background = Color.Lerp(nightBackground, dayBackground, sunlight);
            if (gameTime != null && (gameTime.Hour >= 17 || gameTime.Hour < 6))
            {
                background = Color.Lerp(background, duskBackground, Mathf.Clamp01(nightAmount * 0.55f));
            }

            worldCamera.backgroundColor = background;
            RenderSettings.ambientLight = Color.Lerp(new Color(0.03f, 0.035f, 0.055f, 1f), Color.white, sunlight);

            UpdateNightOverlay(nightAmount);
            UpdateTorchGlow(nightAmount);
        }

        private void UpdateNightOverlay(float nightAmount)
        {
            if (nightOverlay == null) return;

            float height = worldCamera.orthographicSize * 2f;
            float width = height * worldCamera.aspect;
            nightOverlay.transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);
            nightOverlay.transform.localScale = new Vector3(width, height, 1f);
            float darkness = Mathf.Clamp01(Mathf.InverseLerp(0.08f, 0.88f, nightAmount));
            nightOverlay.color = new Color(0.004f, 0.006f, 0.014f, Mathf.Lerp(0.08f, 0.91f, darkness));
        }

        private void UpdateTorchGlow(float nightAmount)
        {
            if (torchGlow == null) return;

            PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
            bool hasTorch = inventorySession != null
                && inventorySession.Runtime != null
                && inventorySession.Runtime.GetQuantity("item.torch") > 0;

            torchGlow.enabled = player != null && hasTorch && nightAmount > 0.12f;
            if (!torchGlow.enabled) return;

            torchGlow.transform.position = player.transform.position + new Vector3(0f, 0.1f, 0f);
            torchGlow.transform.localScale = Vector3.one * Mathf.Lerp(2.2f, 3.8f, nightAmount);
            torchGlow.color = new Color(1f, 0.62f, 0.18f, Mathf.Lerp(0.18f, 0.46f, nightAmount));
        }

        private void EnsureVisuals()
        {
            if (worldCamera == null) worldCamera = GetComponent<Camera>();

            if (nightOverlay == null)
            {
                GameObject overlayObject = new GameObject("Day Night World Tint");
                overlayObject.transform.SetParent(transform, false);
                nightOverlay = overlayObject.AddComponent<SpriteRenderer>();
                nightOverlay.sprite = PrototypePixelArtFactory.SolidPixel();
                nightOverlay.sortingOrder = 9200;
                nightOverlay.color = Color.clear;
            }

            if (torchGlow == null)
            {
                GameObject torchObject = new GameObject("Portable Torch Glow");
                torchGlow = torchObject.AddComponent<SpriteRenderer>();
                torchGlow.sprite = PrototypePixelArtFactory.TorchGlow();
                torchGlow.sortingOrder = 9300;
                torchGlow.enabled = false;
            }
        }
    }
}
