using System;
using UnityEngine;
using TheOldRoad.Inventory;
using TheOldRoad.World;
using TheOldRoad.Audio;
using TheOldRoad.UI;
using TheOldRoad.Save;
using TheOldRoad.Time;
using TheOldRoad.Combat;

namespace TheOldRoad.Farming
{
    /// <summary>
    /// Interactive farming plot for tilling soil, planting seeds, watering crops, waiting for multi-stage growth, and harvesting produce.
    /// Supports 5-stage growth (0..4), in-game time calculation, offline progress, rain auto-watering, and persistent saving.
    /// </summary>
    public sealed class FarmPlotController : MonoBehaviour
    {
        [SerializeField] private string plotId = string.Empty;
        [SerializeField] private bool isTilled;
        [SerializeField] private bool isWatered;
        [SerializeField] private string plantedCropId = string.Empty;
        [SerializeField] private float growthMinutes;
        [SerializeField] private int growthStage;
        [SerializeField] private long plantedAtUnixSeconds;
        [SerializeField] private long lastWateredAtUnixSeconds;

        private SpriteRenderer soilRenderer;
        private SpriteRenderer cropRenderer;
        private Transform harvestSparkleChild;
        private SpriteRenderer harvestSparkleRenderer;
        private GameTimeController gameTime;
        private WeatherController weather;
        private float lastRecordedGameMinute = -1f;

        public string PlotId => plotId;
        public bool IsTilled => isTilled;
        public bool IsWatered => isWatered;
        public string PlantedCropId => plantedCropId;
        public float GrowthMinutes => growthMinutes;
        public int GrowthStage => growthStage;
        public bool IsHarvestReady => !string.IsNullOrEmpty(plantedCropId) && growthStage >= 4;

        public float GrowthPercent
        {
            get
            {
                if (string.IsNullOrEmpty(plantedCropId)) return 0f;
                CropDefinition def = PrototypeCropCatalog.Get(plantedCropId);
                if (def == null) return 0f;
                return Mathf.Clamp01(growthMinutes / def.GrowthDurationMinutes);
            }
        }

        public float RemainingMinutes
        {
            get
            {
                if (string.IsNullOrEmpty(plantedCropId)) return 0f;
                CropDefinition def = PrototypeCropCatalog.Get(plantedCropId);
                if (def == null) return 0f;
                return Mathf.Max(0f, def.GrowthDurationMinutes - growthMinutes);
            }
        }

        public string RemainingCountdownFormatted
        {
            get
            {
                if (string.IsNullOrEmpty(plantedCropId)) return string.Empty;
                if (IsHarvestReady) return LocalizationRuntime.IsVietnamese ? "✨ SẴN SÀNG THU HOẠCH" : "✨ READY TO HARVEST";

                float rem = RemainingMinutes;
                if (rem >= 1440f)
                {
                    int days = Mathf.FloorToInt(rem / 1440f);
                    int hours = Mathf.FloorToInt((rem % 1440f) / 60f);
                    return LocalizationRuntime.IsVietnamese ? $"⏳ {days} ngày {hours} giờ" : $"⏳ {days}d {hours}h";
                }
                if (rem >= 60f)
                {
                    int hours = Mathf.FloorToInt(rem / 60f);
                    int mins = Mathf.FloorToInt(rem % 60f);
                    return LocalizationRuntime.IsVietnamese ? $"⏳ {hours} giờ {mins} phút" : $"⏳ {hours}h {mins}m";
                }
                int m = Mathf.Max(1, Mathf.CeilToInt(rem));
                return LocalizationRuntime.IsVietnamese ? $"⏳ {m} phút" : $"⏳ {m}m";
            }
        }

        public string GetStageDescription()
        {
            if (string.IsNullOrEmpty(plantedCropId)) return string.Empty;
            switch (growthStage)
            {
                case 0: return LocalizationRuntime.IsVietnamese ? "Mới gieo hạt / Nảy mầm" : "Sprout";
                case 1: return LocalizationRuntime.IsVietnamese ? "Nhú lá non" : "Seedling";
                case 2: return LocalizationRuntime.IsVietnamese ? "Cây con phát triển" : "Growing";
                case 3: return LocalizationRuntime.IsVietnamese ? "Ra hoa / Trái non" : "Flowering";
                default: return LocalizationRuntime.IsVietnamese ? "Chín rộ (Sẵn sàng thu hoạch)" : "Ready to Harvest";
            }
        }

        public void Configure(string id, bool tilled = false, bool watered = false, string cropId = "", float progress = 0f, int stage = 0)
        {
            plotId = id ?? string.Empty;
            isTilled = tilled;
            isWatered = watered;
            plantedCropId = cropId ?? string.Empty;
            growthMinutes = progress;
            growthStage = Mathf.Clamp(stage, 0, 4);
            EnsureRenderers();
            UpdateVisuals();
        }

        private void Awake()
        {
            EnsureRenderers();
            UpdateVisuals();
        }

        private void Start()
        {
            gameTime = FindAnyObjectByType<GameTimeController>();
            weather = FindAnyObjectByType<WeatherController>();
            if (gameTime != null)
            {
                lastRecordedGameMinute = gameTime.AbsoluteMinute;
            }
        }

        private void Update()
        {
            if (gameTime == null) gameTime = FindAnyObjectByType<GameTimeController>();
            if (weather == null) weather = FindAnyObjectByType<WeatherController>();

            // Rain auto-watering integration: rain waters all outdoor tilled plots
            if (weather != null && isTilled && !isWatered)
            {
                if (weather.CurrentWeather == WeatherType.LightRain || weather.CurrentWeather == WeatherType.Thunderstorm)
                {
                    isWatered = true;
                    UpdateVisuals();
                }
            }

            // Animate ready harvest sparkle
            if (harvestSparkleChild != null && harvestSparkleChild.gameObject.activeSelf)
            {
                float floatY = 0.55f + Mathf.Sin(UnityEngine.Time.time * 4f) * 0.08f;
                harvestSparkleChild.localPosition = new Vector3(0f, floatY, 0f);
            }

            if (string.IsNullOrEmpty(plantedCropId) || growthStage >= 4) return;

            CropDefinition def = PrototypeCropCatalog.Get(plantedCropId);
            if (def == null) return;

            // Calculate delta in-game minutes
            float currentMinute = gameTime != null ? gameTime.AbsoluteMinute : (UnityEngine.Time.time / 60f);
            if (lastRecordedGameMinute < 0f) lastRecordedGameMinute = currentMinute;

            float deltaMinutes = Mathf.Max(0f, currentMinute - lastRecordedGameMinute);
            lastRecordedGameMinute = currentMinute;

            if (deltaMinutes > 0f)
            {
                float speedMultiplier = isWatered ? 1.0f : 0.5f;
                growthMinutes += deltaMinutes * speedMultiplier;

                int calculatedStage = Mathf.Clamp(Mathf.FloorToInt((growthMinutes / def.GrowthDurationMinutes) * def.StageCount), 0, def.StageCount - 1);
                if (calculatedStage != growthStage)
                {
                    growthStage = calculatedStage;
                    UpdateVisuals();
                }
            }
        }

        public void ProgressOfflineTime(float elapsedInGameMinutes)
        {
            if (elapsedInGameMinutes <= 0f || string.IsNullOrEmpty(plantedCropId) || growthStage >= 4) return;

            CropDefinition def = PrototypeCropCatalog.Get(plantedCropId);
            if (def == null) return;

            // Calculate growth with water consideration
            float effectiveGrowth = isWatered
                ? (elapsedInGameMinutes * 1.0f)
                : (elapsedInGameMinutes * 0.5f);

            growthMinutes += effectiveGrowth;
            growthStage = Mathf.Clamp(Mathf.FloorToInt((growthMinutes / def.GrowthDurationMinutes) * def.StageCount), 0, def.StageCount - 1);
            UpdateVisuals();
        }

        public bool TryTillSoil()
        {
            if (isTilled) return false;
            isTilled = true;
            isWatered = false;
            AudioManager.PlayTillSoil();
            FloatingTextController.Spawn(
                LocalizationRuntime.IsVietnamese ? "🌱 Đã xới đất tơi xốp!" : "🌱 Tilled fresh soil!",
                transform.position + Vector3.up * 0.8f,
                new Color(0.45f, 0.95f, 0.45f),
                1.8f);
            UpdateVisuals();
            return true;
        }

        public bool TryWaterSoil()
        {
            if (!isTilled || isWatered) return false;
            isWatered = true;
            lastWateredAtUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            AudioManager.PlayWaterPour();
            FloatingTextController.Spawn(
                LocalizationRuntime.IsVietnamese ? "💧 Đã tưới nước ẩm! (Lớn nhanh x2)" : "💧 Watered soil! (2x Growth Speed)",
                transform.position + Vector3.up * 0.8f,
                new Color(0.35f, 0.85f, 1f),
                1.8f);
            UpdateVisuals();
            return true;
        }

        public void SetWatered(bool watered)
        {
            isWatered = watered;
            if (watered)
            {
                lastWateredAtUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
            UpdateVisuals();
        }

        public bool TryPlantSeed(string seedItemId)
        {
            if (!isTilled || !string.IsNullOrEmpty(plantedCropId)) return false;

            if (!PrototypeCropCatalog.TryGetBySeed(seedItemId, out CropDefinition crop))
            {
                return false;
            }

            plantedCropId = crop.CropId;
            growthMinutes = 0f;
            growthStage = 0;
            plantedAtUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            AudioManager.PlayPlantSeed();
            FloatingTextController.Spawn(
                LocalizationRuntime.IsVietnamese ? $"🌾 Đã gieo: {crop.DisplayName} (-1 Hạt)" : $"🌾 Planted: {crop.DisplayName} (-1 Seed)",
                transform.position + Vector3.up * 0.8f,
                new Color(0.95f, 0.85f, 0.35f),
                2.0f);
            UpdateVisuals();
            return true;
        }

        public bool TryHarvest(InventoryRuntime inventory)
        {
            if (!IsHarvestReady || inventory == null) return false;

            CropDefinition def = PrototypeCropCatalog.Get(plantedCropId);
            if (def == null) return false;

            int yieldCount = UnityEngine.Random.Range(def.MinYield, def.MaxYield + 1);
            inventory.Add(def.HarvestItemId, yieldCount);

            // 40% chance to return a bonus seed for continuous farming
            bool bonusSeed = false;
            if (UnityEngine.Random.value < 0.40f && !string.IsNullOrEmpty(def.SeedItemId))
            {
                inventory.Add(def.SeedItemId, 1);
                bonusSeed = true;
            }

            AudioManager.PlayCropHarvest();
            string cropName = def.DisplayName;
            string bonusText = bonusSeed ? (LocalizationRuntime.IsVietnamese ? " (+1 Hạt giống)" : " (+1 Seed)") : "";
            FloatingTextController.Spawn(
                LocalizationRuntime.IsVietnamese 
                    ? $"🎉 Thu hoạch: +{yieldCount} {cropName}!{bonusText}" 
                    : $"🎉 Harvested: +{yieldCount} {cropName}!{bonusText}",
                transform.position + Vector3.up * 1.0f,
                new Color(1f, 0.88f, 0.25f),
                2.5f);

            PlayerSpeechBubble.Say(LocalizationRuntime.IsVietnamese 
                ? $"Thu hoạch +{yieldCount} {cropName}!" 
                : $"Harvested +{yieldCount} {cropName}!");

            // Reset to clean Tilled state for smooth repeated farming cycle
            plantedCropId = string.Empty;
            growthMinutes = 0f;
            growthStage = 0;
            isWatered = false;
            UpdateVisuals();
            return true;
        }

        public bool TryApplyFertilizer()
        {
            if (string.IsNullOrEmpty(plantedCropId) || growthStage >= 4) return false;

            CropDefinition def = PrototypeCropCatalog.Get(plantedCropId);
            if (def == null) return false;

            // Boost progress by 40% of duration
            growthMinutes += def.GrowthDurationMinutes * 0.40f;
            growthStage = Mathf.Clamp(Mathf.FloorToInt((growthMinutes / def.GrowthDurationMinutes) * def.StageCount), 0, def.StageCount - 1);
            AudioManager.PlayCropFertilize();
            FloatingTextController.Spawn(
                LocalizationRuntime.IsVietnamese ? "✨ Đã bón phân dinh dưỡng! (Tăng vọt sinh trưởng)" : "✨ Applied organic fertilizer! (Growth Boost)",
                transform.position + Vector3.up * 0.8f,
                new Color(0.55f, 1f, 0.35f),
                2.0f);
            UpdateVisuals();
            return true;
        }

        private void OnGUI()
        {
            if (string.IsNullOrEmpty(plantedCropId) || !isTilled) return;

            Camera cam = Camera.main;
            if (cam == null) return;

            Vector3 screenPos = cam.WorldToScreenPoint(transform.position + Vector3.up * 0.70f);
            if (screenPos.z < 0.1f) return;

            float guiY = Screen.height - screenPos.y;
            float dist = Vector2.Distance(new Vector2(screenPos.x, guiY), new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
            if (dist > 500f) return;

            string timeText = RemainingCountdownFormatted;
            float width = Mathf.Max(78f, timeText.Length * 7.2f + 14f);
            float height = 18f;
            Rect timerRect = new Rect(screenPos.x - width * 0.5f, guiY - height * 0.5f, width, height);

            Color bgColor = IsHarvestReady
                ? new Color(0.45f, 0.32f, 0.08f, 0.94f)
                : (isWatered ? new Color(0.08f, 0.22f, 0.38f, 0.92f) : new Color(0.38f, 0.20f, 0.08f, 0.92f));
            Color borderColor = IsHarvestReady ? new Color(1f, 0.88f, 0.25f, 1f) : (isWatered ? new Color(0.35f, 0.85f, 1f, 0.85f) : new Color(0.95f, 0.55f, 0.20f, 0.85f));

            DrawGuiRect(timerRect, bgColor);
            DrawGuiBorder(timerRect, borderColor, 1f);

            if (!IsHarvestReady)
            {
                float barW = (width - 4f) * GrowthPercent;
                DrawGuiRect(new Rect(timerRect.x + 2f, timerRect.yMax - 3f, barW, 2f), isWatered ? new Color(0.35f, 0.95f, 0.45f, 1f) : new Color(0.95f, 0.65f, 0.2f, 1f));
            }

            UiFontHelper.EnsureGlobalSkinFont();
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
            {
                font = UiFontHelper.CleanFont,
                fontSize = 11,
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };
            labelStyle.normal.textColor = Color.white;
            GUI.Label(timerRect, timeText, labelStyle);
        }

        private static Texture2D pixelTexture;
        private static void DrawGuiRect(Rect r, Color c)
        {
            if (pixelTexture == null)
            {
                pixelTexture = new Texture2D(1, 1);
                pixelTexture.SetPixel(0, 0, Color.white);
                pixelTexture.Apply();
            }
            Color prev = GUI.color;
            GUI.color = c;
            GUI.DrawTexture(r, pixelTexture);
            GUI.color = prev;
        }

        private static void DrawGuiBorder(Rect r, Color c, float thickness)
        {
            DrawGuiRect(new Rect(r.x, r.y, r.width, thickness), c);
            DrawGuiRect(new Rect(r.x, r.yMax - thickness, r.width, thickness), c);
            DrawGuiRect(new Rect(r.x, r.y, thickness, r.height), c);
            DrawGuiRect(new Rect(r.xMax - thickness, r.y, thickness, r.height), c);
        }

        public FarmPlotSaveEntry ToSaveEntry()
        {
            return new FarmPlotSaveEntry
            {
                plotId = plotId,
                isTilled = isTilled,
                isWatered = isWatered,
                plantedCropId = plantedCropId,
                growthMinutes = growthMinutes,
                growthStage = growthStage,
                plantedAtUnixSeconds = plantedAtUnixSeconds,
                lastWateredAtUnixSeconds = lastWateredAtUnixSeconds
            };
        }

        public void LoadFromSaveEntry(FarmPlotSaveEntry entry)
        {
            if (entry == null) return;
            plotId = entry.plotId;
            isTilled = entry.isTilled;
            isWatered = entry.isWatered;
            plantedCropId = entry.plantedCropId ?? string.Empty;
            growthMinutes = entry.growthMinutes;
            growthStage = Mathf.Clamp(entry.growthStage, 0, 4);
            plantedAtUnixSeconds = entry.plantedAtUnixSeconds;
            lastWateredAtUnixSeconds = entry.lastWateredAtUnixSeconds;
            EnsureRenderers();
            UpdateVisuals();
        }

        private void EnsureRenderers()
        {
            if (soilRenderer == null)
            {
                soilRenderer = GetComponent<SpriteRenderer>();
                if (soilRenderer == null) soilRenderer = gameObject.AddComponent<SpriteRenderer>();
                soilRenderer.sortingOrder = 10;
            }

            if (cropRenderer == null)
            {
                Transform cropChild = transform.Find("CropChild");
                if (cropChild == null)
                {
                    GameObject childObj = new GameObject("CropChild");
                    childObj.transform.SetParent(transform, false);
                    childObj.transform.localPosition = new Vector3(0f, 0.25f, 0f);
                    cropChild = childObj.transform;
                }
                cropRenderer = cropChild.GetComponent<SpriteRenderer>();
                if (cropRenderer == null) cropRenderer = cropChild.gameObject.AddComponent<SpriteRenderer>();
                cropRenderer.sortingOrder = 15;
            }

            if (harvestSparkleChild == null)
            {
                Transform sparkleChild = transform.Find("HarvestSparkle");
                if (sparkleChild == null)
                {
                    GameObject sparkleObj = new GameObject("HarvestSparkle");
                    sparkleObj.transform.SetParent(transform, false);
                    sparkleObj.transform.localPosition = new Vector3(0f, 0.55f, 0f);
                    sparkleChild = sparkleObj.transform;
                }
                harvestSparkleChild = sparkleChild;
                harvestSparkleRenderer = harvestSparkleChild.GetComponent<SpriteRenderer>();
                if (harvestSparkleRenderer == null) harvestSparkleRenderer = harvestSparkleChild.gameObject.AddComponent<SpriteRenderer>();
                harvestSparkleRenderer.sprite = PrototypePixelArtFactory.RewardChestIcon();
                harvestSparkleRenderer.sortingOrder = 20;
                harvestSparkleRenderer.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            }
        }

        public void UpdateVisuals()
        {
            EnsureRenderers();
            if (!isTilled)
            {
                soilRenderer.enabled = false;
                cropRenderer.enabled = false;
                if (harvestSparkleChild != null) harvestSparkleChild.gameObject.SetActive(false);
                return;
            }

            soilRenderer.enabled = true;
            soilRenderer.sprite = PrototypePixelArtFactory.TilledSoil(isWatered);

            if (string.IsNullOrEmpty(plantedCropId))
            {
                cropRenderer.enabled = false;
                if (harvestSparkleChild != null) harvestSparkleChild.gameObject.SetActive(false);
            }
            else
            {
                cropRenderer.enabled = true;
                cropRenderer.sprite = PrototypePixelArtFactory.Crop(plantedCropId, growthStage);

                bool ready = growthStage >= 4;
                if (harvestSparkleChild != null)
                {
                    harvestSparkleChild.gameObject.SetActive(ready);
                }
            }
        }
    }
}
