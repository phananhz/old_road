using System;
using UnityEngine;
using TheOldRoad.Inventory;
using TheOldRoad.World;
using TheOldRoad.Audio;
using TheOldRoad.UI;

namespace TheOldRoad.Farming
{
    /// <summary>
    /// Interactive farming plot for tilling soil, watering crops, planting seeds, and harvesting produce.
    /// Follows classic farm sim mechanics (Harvest Moon / Stardew).
    /// </summary>
    public sealed class FarmPlotController : MonoBehaviour
    {
        [SerializeField] private bool isTilled = true;
        [SerializeField] private bool isWatered;
        [SerializeField] private string plantedCropId = string.Empty;
        [SerializeField] private int growthStage;
        [SerializeField] private float stageDurationSeconds = 15f;

        private SpriteRenderer soilRenderer;
        private SpriteRenderer cropRenderer;
        private float nextStageTime;
        private bool isHarvestReady => !string.IsNullOrEmpty(plantedCropId) && growthStage >= 3;

        public bool IsTilled => isTilled;
        public bool IsWatered => isWatered;
        public string PlantedCropId => plantedCropId;
        public int GrowthStage => growthStage;
        public bool IsHarvestReady => isHarvestReady;

        public void Configure(bool tilled, bool watered, string cropId, int stage)
        {
            isTilled = tilled;
            isWatered = watered;
            plantedCropId = cropId ?? string.Empty;
            growthStage = Mathf.Clamp(stage, 0, 3);
            EnsureRenderers();
            UpdateVisuals();
        }

        private void Awake()
        {
            EnsureRenderers();
            UpdateVisuals();
        }

        private void Update()
        {
            if (string.IsNullOrEmpty(plantedCropId) || growthStage >= 3) return;

            if (UnityEngine.Time.time >= nextStageTime && nextStageTime > 0f)
            {
                growthStage++;
                UpdateVisuals();
                if (growthStage < 3)
                {
                    nextStageTime = UnityEngine.Time.time + stageDurationSeconds;
                }
            }
        }

        public bool TryTillSoil()
        {
            if (isTilled) return false;
            isTilled = true;
            isWatered = false;
            AudioManager.PlayMiningImpact();
            UpdateVisuals();
            return true;
        }

        public bool TryWaterSoil()
        {
            if (!isTilled || isWatered) return false;
            isWatered = true;
            AudioManager.PlayWaterSplash();
            UpdateVisuals();
            if (!string.IsNullOrEmpty(plantedCropId) && growthStage < 3 && nextStageTime <= UnityEngine.Time.time)
            {
                nextStageTime = UnityEngine.Time.time + stageDurationSeconds;
            }
            return true;
        }

        public bool TryPlantSeed(string seedItemId)
        {
            if (!isTilled || !string.IsNullOrEmpty(plantedCropId)) return false;

            string cropType = GetCropTypeFromSeed(seedItemId);
            if (string.IsNullOrEmpty(cropType)) return false;

            plantedCropId = cropType;
            growthStage = 0;
            nextStageTime = UnityEngine.Time.time + stageDurationSeconds;
            AudioManager.PlayUiClick();
            UpdateVisuals();
            return true;
        }

        public bool TryHarvest(InventoryRuntime inventory)
        {
            if (!isHarvestReady || inventory == null) return false;

            string produceId = GetProduceItemId(plantedCropId);
            string seedId = GetSeedItemId(plantedCropId);

            int yieldCount = UnityEngine.Random.Range(2, 4);
            inventory.Add(produceId, yieldCount);
            if (UnityEngine.Random.value > 0.4f)
            {
                inventory.Add(seedId, 1);
            }

            AudioManager.PlayGatherSuccess();
            PlayerSpeechBubble.Say(LocalizationRuntime.IsVietnamese ? $"Thu hoạch {yieldCount} nông sản tươi!" : $"Harvested {yieldCount} fresh crops!");

            plantedCropId = string.Empty;
            growthStage = 0;
            isWatered = false;
            UpdateVisuals();
            return true;
        }

        private static string GetCropTypeFromSeed(string seedId)
        {
            switch (seedId)
            {
                case "item.seed-wheat": return "wheat";
                case "item.seed-corn": return "corn";
                case "item.seed-carrot": return "carrot";
                case "item.seed-pineapple": return "pineapple";
                case "item.seed-tomato": return "tomato";
                default: return string.Empty;
            }
        }

        private static string GetProduceItemId(string cropType)
        {
            switch (cropType)
            {
                case "wheat": return "item.wheat";
                case "corn": return "item.corn";
                case "carrot": return "item.carrot";
                case "pineapple": return "item.pineapple";
                case "tomato": return "item.tomato";
                default: return "item.wheat";
            }
        }

        private static string GetSeedItemId(string cropType)
        {
            switch (cropType)
            {
                case "wheat": return "item.seed-wheat";
                case "corn": return "item.seed-corn";
                case "carrot": return "item.seed-carrot";
                case "pineapple": return "item.seed-pineapple";
                case "tomato": return "item.seed-tomato";
                default: return "item.seed-wheat";
            }
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
        }

        public void UpdateVisuals()
        {
            EnsureRenderers();
            if (!isTilled)
            {
                soilRenderer.enabled = false;
                cropRenderer.enabled = false;
                return;
            }

            soilRenderer.enabled = true;
            soilRenderer.sprite = PrototypePixelArtFactory.TilledSoil(isWatered);

            if (string.IsNullOrEmpty(plantedCropId))
            {
                cropRenderer.enabled = false;
            }
            else
            {
                cropRenderer.enabled = true;
                cropRenderer.sprite = PrototypePixelArtFactory.Crop(plantedCropId, growthStage);
            }
        }
    }
}
