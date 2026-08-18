using System.Collections.Generic;
using UnityEngine;
using TheOldRoad.Construction;
using TheOldRoad.Inventory;
using TheOldRoad.Player;
using TheOldRoad.World;

namespace TheOldRoad.Time
{
    /// <summary>
    /// Dynamic Day/Night Lighting System with soft radial light cutouts for Campfires,
    /// Completed Houses, Shrines, and Player Torches.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class DayNightLightingController : MonoBehaviour
    {
        private const int MaskResolution = 64;

        [SerializeField] private GameTimeController gameTime;
        [SerializeField] private InventorySession inventorySession;

        private Camera worldCamera;
        private SpriteRenderer nightOverlay;
        private Texture2D dynamicLightMask;
        private Sprite dynamicMaskSprite;
        private Color32[] pixelBuffer;

        private Color dayBackground = new Color(0.40f, 0.63f, 0.70f, 1f);
        private Color duskBackground = new Color(0.32f, 0.22f, 0.20f, 1f);
        private Color nightBackground = new Color(0.012f, 0.015f, 0.028f, 1f);

        private readonly List<LightSourceData> lightSources = new List<LightSourceData>();
        private float flickerTimer;

        private struct LightSourceData
        {
            public Vector2 WorldPos;
            public float Radius;
            public float Intensity;
            public Color Color;
        }

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
            flickerTimer += UnityEngine.Time.deltaTime * 6f;

            Color background = Color.Lerp(nightBackground, dayBackground, sunlight);
            if (gameTime != null && (gameTime.Hour >= 17 || gameTime.Hour < 6))
            {
                background = Color.Lerp(background, duskBackground, Mathf.Clamp01(nightAmount * 0.55f));
            }

            worldCamera.backgroundColor = background;
            RenderSettings.ambientLight = Color.Lerp(new Color(0.02f, 0.025f, 0.04f, 1f), Color.white, sunlight);
            TheOldRoad.Audio.AudioManager.SetNightBlend(nightAmount);

            UpdateDynamicNightLighting(nightAmount);
        }

        private void UpdateDynamicNightLighting(float nightAmount)
        {
            if (nightOverlay == null || worldCamera == null) return;

            if (nightAmount < 0.04f)
            {
                nightOverlay.enabled = false;
                return;
            }

            nightOverlay.enabled = true;
            float height = worldCamera.orthographicSize * 2f;
            float width = height * worldCamera.aspect;
            Vector3 camPos = worldCamera.transform.position;

            nightOverlay.transform.position = new Vector3(camPos.x, camPos.y, -0.5f);
            nightOverlay.transform.localScale = new Vector3(width, height, 1f);

            CollectActiveLightSources(camPos, width * 0.7f, height * 0.7f);

            float baseDarkness = Mathf.Clamp01(Mathf.InverseLerp(0.06f, 0.90f, nightAmount)) * 0.94f;
            float minX = camPos.x - width * 0.5f;
            float minY = camPos.y - height * 0.5f;

            for (int y = 0; y < MaskResolution; y++)
            {
                float worldY = minY + (y / (float)(MaskResolution - 1)) * height;
                int rowOffset = y * MaskResolution;

                for (int x = 0; x < MaskResolution; x++)
                {
                    float worldX = minX + (x / (float)(MaskResolution - 1)) * width;
                    Vector2 samplePos = new Vector2(worldX, worldY);

                    float lightSum = 0f;
                    float warmR = 0f;
                    float warmG = 0f;
                    float warmB = 0f;

                    for (int i = 0; i < lightSources.Count; i++)
                    {
                        LightSourceData light = lightSources[i];
                        float dx = samplePos.x - light.WorldPos.x;
                        float dy = samplePos.y - light.WorldPos.y;
                        float distSq = dx * dx + dy * dy;
                        float radSq = light.Radius * light.Radius;

                        if (distSq < radSq)
                        {
                            float normDist = Mathf.Sqrt(distSq) / light.Radius;
                            // Smooth cosine falloff
                            float falloff = 0.5f * (1f + Mathf.Cos(normDist * Mathf.PI)) * light.Intensity;
                            lightSum += falloff;
                            warmR += light.Color.r * falloff;
                            warmG += light.Color.g * falloff;
                            warmB += light.Color.b * falloff;
                        }
                    }

                    lightSum = Mathf.Clamp01(lightSum);
                    float finalAlpha = Mathf.Clamp01(baseDarkness * (1f - lightSum * 0.95f));

                    byte r = (byte)Mathf.Clamp(Mathf.Lerp(4f, warmR * 255f, lightSum * 0.4f), 0f, 255f);
                    byte g = (byte)Mathf.Clamp(Mathf.Lerp(6f, warmG * 255f, lightSum * 0.35f), 0f, 255f);
                    byte b = (byte)Mathf.Clamp(Mathf.Lerp(14f, warmB * 255f, lightSum * 0.2f), 0f, 255f);
                    byte a = (byte)Mathf.Clamp(finalAlpha * 255f, 0f, 255f);

                    pixelBuffer[rowOffset + x] = new Color32(r, g, b, a);
                }
            }

            dynamicLightMask.SetPixels32(pixelBuffer);
            dynamicLightMask.Apply(false);
        }

        private void CollectActiveLightSources(Vector3 camPos, float rangeX, float rangeY)
        {
            lightSources.Clear();

            // 1. Player light source
            PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
            if (player != null)
            {
                bool hasTorch = inventorySession != null
                    && inventorySession.Runtime != null
                    && inventorySession.Runtime.GetQuantity("item.torch") > 0;

                float playerRadius = hasTorch ? 7.2f : 2.5f;
                float flicker = (Mathf.PerlinNoise(flickerTimer, 0f) * 2f - 1f) * 0.35f;
                float playerIntensity = hasTorch ? 1.0f + flicker * 0.15f : 0.75f;

                lightSources.Add(new LightSourceData
                {
                    WorldPos = player.transform.position,
                    Radius = playerRadius + (hasTorch ? flicker * 0.5f : 0f),
                    Intensity = playerIntensity,
                    Color = hasTorch ? new Color(1f, 0.72f, 0.32f) : new Color(0.9f, 0.85f, 0.75f)
                });
            }

            // 2. Campfires, Cooking Hearths, and Completed Buildings
            foreach (ConstructionSite site in FindObjectsByType<ConstructionSite>(FindObjectsInactive.Exclude))
            {
                if (site == null) continue;

                Vector3 pos = site.transform.position;
                if (Mathf.Abs(pos.x - camPos.x) > rangeX + 8f || Mathf.Abs(pos.y - camPos.y) > rangeY + 8f) continue;

                if (site.BuildingId == "building.campfire")
                {
                    float flicker = (Mathf.Sin(flickerTimer * 1.5f + pos.x) + Mathf.Cos(flickerTimer * 2.1f + pos.y)) * 0.3f;
                    lightSources.Add(new LightSourceData
                    {
                        WorldPos = pos,
                        Radius = 5.8f + flicker,
                        Intensity = 1.0f,
                        Color = new Color(1f, 0.62f, 0.20f)
                    });
                }
                else if (site.BuildingId == "building.cooking-hearth")
                {
                    float flicker = Mathf.Sin(flickerTimer * 1.8f + pos.y) * 0.25f;
                    lightSources.Add(new LightSourceData
                    {
                        WorldPos = pos,
                        Radius = 6.4f + flicker,
                        Intensity = 1.05f,
                        Color = new Color(1f, 0.70f, 0.28f)
                    });
                }
                else if (site.IsCompleted)
                {
                    // Completed house windows and door light
                    lightSources.Add(new LightSourceData
                    {
                        WorldPos = pos + new Vector3(0f, -0.5f, 0f),
                        Radius = 7.5f,
                        Intensity = 0.95f,
                        Color = new Color(1f, 0.82f, 0.45f)
                    });
                }
            }

            // 3. Shrines and Landmarks
            foreach (DiscoverableLandmark landmark in FindObjectsByType<DiscoverableLandmark>(FindObjectsInactive.Exclude))
            {
                if (landmark == null) continue;

                Vector3 pos = landmark.transform.position;
                if (Mathf.Abs(pos.x - camPos.x) > rangeX + 6f || Mathf.Abs(pos.y - camPos.y) > rangeY + 6f) continue;

                if (landmark.LandmarkId.Contains("camp") || landmark.LandmarkId.Contains("bell") || landmark.LandmarkId.Contains("shrine"))
                {
                    lightSources.Add(new LightSourceData
                    {
                        WorldPos = pos,
                        Radius = 4.8f,
                        Intensity = 0.85f,
                        Color = new Color(0.95f, 0.78f, 0.45f)
                    });
                }
            }
        }

        private void EnsureVisuals()
        {
            if (worldCamera == null) worldCamera = GetComponent<Camera>();

            if (dynamicLightMask == null)
            {
                dynamicLightMask = new Texture2D(MaskResolution, MaskResolution, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Clamp,
                    hideFlags = HideFlags.DontSave
                };

                pixelBuffer = new Color32[MaskResolution * MaskResolution];
                dynamicMaskSprite = Sprite.Create(dynamicLightMask, new Rect(0, 0, MaskResolution, MaskResolution), new Vector2(0.5f, 0.5f), 1f);
            }

            if (nightOverlay == null)
            {
                GameObject overlayObject = new GameObject("Day Night World Tint");
                overlayObject.transform.SetParent(transform, false);
                nightOverlay = overlayObject.AddComponent<SpriteRenderer>();
                nightOverlay.sprite = dynamicMaskSprite;
                nightOverlay.sortingOrder = 9200;
            }
        }

        private void OnDestroy()
        {
            if (dynamicLightMask != null)
            {
                DestroyImmediate(dynamicLightMask);
            }
        }
    }
}
