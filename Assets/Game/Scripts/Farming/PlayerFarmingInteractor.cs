using System.Collections.Generic;
using UnityEngine;
using TheOldRoad.Inventory;
using TheOldRoad.UI;
using TheOldRoad.Input;
using TheOldRoad.Audio;

namespace TheOldRoad.Farming
{
    /// <summary>
    /// Player interactor for the complete farming cycle:
    /// 1. Till soil with hoe (Xới đất)
    /// 2. Plant selected seeds (Gieo hạt - support cycling with [G])
    /// 3. Water soil (Tưới nước)
    /// 4. Wait for multi-stage growth (Đợi cây lớn theo thời gian)
    /// 5. Harvest mature crops (Thu hoạch nông sản chín vàng)
    /// </summary>
    public sealed class PlayerFarmingInteractor : MonoBehaviour
    {
        [SerializeField, Min(0.5f)] private float interactDistance = 2.4f;
        [SerializeField] private InventorySession inventorySession;

        private FarmPlotController nearbyPlot;
        private int selectedSeedIndex = 0;

        public string InteractionHint { get; private set; } = string.Empty;
        public string ActionButtonLabel { get; private set; } = string.Empty;
        public FarmPlotController NearbyPlot => nearbyPlot;
        public bool CanFarmAction => nearbyPlot != null && (nearbyPlot.IsHarvestReady || !nearbyPlot.IsTilled || string.IsNullOrEmpty(nearbyPlot.PlantedCropId) || !nearbyPlot.IsWatered);

        public void Configure(InventorySession session, float distance = 2.4f)
        {
            inventorySession = session;
            interactDistance = Mathf.Max(0.5f, distance);
        }

        private void Update()
        {
            if (inventorySession == null) inventorySession = FindAnyObjectByType<InventorySession>();

            nearbyPlot = FindNearbyPlot();

            // Allow cycling seed selection with [G]
            if (PrototypeInput.GetKeyDown(KeyCode.G))
            {
                CycleNextAvailableSeed();
            }

            UpdateInteractionHint();

            if (nearbyPlot == null) return;

            if (PrototypeInput.GetKeyDown(KeyCode.F) || PrototypeInput.GetKeyDown(KeyCode.E))
            {
                InteractWithPlot();
            }
        }

        private void UpdateInteractionHint()
        {
            if (nearbyPlot == null)
            {
                InteractionHint = string.Empty;
                ActionButtonLabel = string.Empty;
                return;
            }

            // 1. Ready to harvest
            if (nearbyPlot.IsHarvestReady)
            {
                CropDefinition def = PrototypeCropCatalog.Get(nearbyPlot.PlantedCropId);
                string cropName = def != null ? def.DisplayName : "Cây";
                InteractionHint = LocalizationRuntime.IsVietnamese 
                    ? $"[F] Thu hoạch {cropName} (Chín vàng rực rỡ) 🎉" 
                    : $"[F] Harvest {cropName} (Ripe & Ready) 🎉";
                ActionButtonLabel = LocalizationRuntime.IsVietnamese ? "Thu hoạch" : "Harvest";
                return;
            }

            // 2. Untilled plot -> Till soil
            if (!nearbyPlot.IsTilled)
            {
                InteractionHint = LocalizationRuntime.IsVietnamese ? "[F] Xới đất tơi xốp" : "[F] Till Soil (Hoe)";
                ActionButtonLabel = LocalizationRuntime.IsVietnamese ? "Xới đất" : "Till";
                return;
            }

            // 3. Tilled & empty -> Plant seeds
            if (string.IsNullOrEmpty(nearbyPlot.PlantedCropId))
            {
                string activeSeed = GetCurrentSelectedSeed();
                if (!string.IsNullOrEmpty(activeSeed))
                {
                    if (PrototypeCropCatalog.TryGetBySeed(activeSeed, out CropDefinition crop))
                    {
                        int seedQty = inventorySession != null && inventorySession.Runtime != null ? inventorySession.Runtime.GetQuantity(activeSeed) : 0;
                        InteractionHint = LocalizationRuntime.IsVietnamese 
                            ? $"[F] Gieo hạt {crop.DisplayName} (Còn x{seedQty}) | [G] Đổi hạt khác" 
                            : $"[F] Plant {crop.DisplayName} Seeds (x{seedQty}) | [G] Next Seed";
                        ActionButtonLabel = LocalizationRuntime.IsVietnamese ? "Gieo hạt" : "Plant";
                        return;
                    }
                }

                InteractionHint = LocalizationRuntime.IsVietnamese 
                    ? "Đất đã xới (Cần mua hoặc tìm hạt giống để gieo)" 
                    : "Tilled Soil (Needs seeds in inventory to plant)";
                ActionButtonLabel = string.Empty;
                return;
            }

            // 4. Planted & dry -> Water soil
            if (!nearbyPlot.IsWatered)
            {
                CropDefinition growingCrop = PrototypeCropCatalog.Get(nearbyPlot.PlantedCropId);
                string growingName = growingCrop != null ? growingCrop.DisplayName : "Cây";
                InteractionHint = LocalizationRuntime.IsVietnamese 
                    ? $"[F] Tưới nước cho {growingName} (Cần nước để lớn nhanh)" 
                    : $"[F] Water {growingName} (Needs water for 2x growth)";
                ActionButtonLabel = LocalizationRuntime.IsVietnamese ? "Tưới nước" : "Water";
                return;
            }

            // 5. Planted & watered -> Growing status with time & stage
            CropDefinition cropDef = PrototypeCropCatalog.Get(nearbyPlot.PlantedCropId);
            string name = cropDef != null ? cropDef.DisplayName : "Cây";
            int percent = Mathf.RoundToInt(nearbyPlot.GrowthPercent * 100f);
            string stageDesc = nearbyPlot.GetStageDescription();
            InteractionHint = LocalizationRuntime.IsVietnamese 
                ? $"{name}: {stageDesc} ({percent}%) - [💧 Đã tưới ẩm]" 
                : $"{name}: {stageDesc} ({percent}%) - [💧 Watered]";
            ActionButtonLabel = string.Empty;
        }

        public void InteractWithPlot()
        {
            if (nearbyPlot == null || inventorySession == null || inventorySession.Runtime == null) return;

            InventoryRuntime inv = inventorySession.Runtime;

            // 1. Ready to harvest
            if (nearbyPlot.IsHarvestReady)
            {
                nearbyPlot.TryHarvest(inv);
                return;
            }

            // 2. Untilled dirt -> Till with hoe
            if (!nearbyPlot.IsTilled)
            {
                nearbyPlot.TryTillSoil();
                return;
            }

            // 3. Tilled & empty -> Plant seed
            if (string.IsNullOrEmpty(nearbyPlot.PlantedCropId))
            {
                string seedId = GetCurrentSelectedSeed();
                if (!string.IsNullOrEmpty(seedId) && inv.GetQuantity(seedId) > 0)
                {
                    if (nearbyPlot.TryPlantSeed(seedId))
                    {
                        inv.TryRemove(seedId, 1);
                        return;
                    }
                }
                else
                {
                    PlayerSpeechBubble.Say(LocalizationRuntime.IsVietnamese 
                        ? "Cần có hạt giống trong túi để gieo!" 
                        : "Need seeds in inventory to plant!");
                    return;
                }
            }

            // 4. Planted & not watered -> Water with watering can
            if (!nearbyPlot.IsWatered)
            {
                nearbyPlot.TryWaterSoil();
                return;
            }

            // 5. Fertilizer boost if player has fertilizer in inventory
            if (inv.GetQuantity("item.fertilizer") > 0 && nearbyPlot.GrowthStage < 4)
            {
                if (nearbyPlot.TryApplyFertilizer())
                {
                    inv.TryRemove("item.fertilizer", 1);
                }
            }
        }

        public void CycleNextAvailableSeed()
        {
            List<string> availableSeeds = GetAvailableSeedsList();
            if (availableSeeds.Count <= 1) return;

            selectedSeedIndex = (selectedSeedIndex + 1) % availableSeeds.Count;
            string newSeed = availableSeeds[selectedSeedIndex];
            if (PrototypeCropCatalog.TryGetBySeed(newSeed, out CropDefinition def))
            {
                AudioManager.PlayUiClick();
                PlayerSpeechBubble.Say(LocalizationRuntime.IsVietnamese 
                    ? $"Chọn hạt giống: {def.DisplayName}" 
                    : $"Selected seed: {def.DisplayName}");
            }
        }

        private List<string> GetAvailableSeedsList()
        {
            List<string> list = new List<string>();
            if (inventorySession == null || inventorySession.Runtime == null) return list;

            InventoryRuntime inv = inventorySession.Runtime;
            string[] seedIds = { "item.seed-wheat", "item.seed-carrot", "item.seed-potato", "item.seed-corn", "item.seed-tomato", "item.seed-pineapple" };
            for (int i = 0; i < seedIds.Length; i++)
            {
                if (inv.GetQuantity(seedIds[i]) > 0)
                {
                    list.Add(seedIds[i]);
                }
            }
            return list;
        }

        private string GetCurrentSelectedSeed()
        {
            List<string> list = GetAvailableSeedsList();
            if (list.Count == 0) return string.Empty;

            if (selectedSeedIndex >= list.Count) selectedSeedIndex = 0;
            return list[selectedSeedIndex];
        }

        private FarmPlotController FindNearbyPlot()
        {
            FarmPlotController[] plots = FindObjectsByType<FarmPlotController>(FindObjectsInactive.Exclude);
            FarmPlotController best = null;
            float bestDistance = interactDistance;

            for (int i = 0; i < plots.Length; i++)
            {
                if (plots[i] == null) continue;
                float d = Vector2.Distance(transform.position, plots[i].transform.position);
                if (d <= bestDistance)
                {
                    bestDistance = d;
                    best = plots[i];
                }
            }

            return best;
        }
    }
}
