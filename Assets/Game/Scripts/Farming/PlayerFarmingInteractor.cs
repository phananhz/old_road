using UnityEngine;
using TheOldRoad.Inventory;
using TheOldRoad.UI;
using TheOldRoad.Input;

namespace TheOldRoad.Farming
{
    /// <summary>
    /// Player interactor for farm plots (tilling, watering, planting, and harvesting).
    /// </summary>
    public sealed class PlayerFarmingInteractor : MonoBehaviour
    {
        [SerializeField, Min(0.5f)] private float interactDistance = 1.8f;
        [SerializeField] private InventorySession inventorySession;

        private FarmPlotController nearbyPlot;
        public string InteractionHint { get; private set; } = string.Empty;

        public void Configure(InventorySession session, float distance = 1.8f)
        {
            inventorySession = session;
            interactDistance = Mathf.Max(0.5f, distance);
        }

        private void Update()
        {
            if (inventorySession == null) inventorySession = FindAnyObjectByType<InventorySession>();

            nearbyPlot = FindNearbyPlot();
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
                return;
            }

            if (nearbyPlot.IsHarvestReady)
            {
                InteractionHint = LocalizationRuntime.IsVietnamese ? "[F] Thu hoạch nông sản" : "[F] Harvest Crop";
                return;
            }

            if (!nearbyPlot.IsTilled)
            {
                InteractionHint = LocalizationRuntime.IsVietnamese ? "[F] Xới đất trồng" : "[F] Till Soil";
                return;
            }

            if (string.IsNullOrEmpty(nearbyPlot.PlantedCropId))
            {
                string activeSeed = GetActiveSeedInInventory();
                if (!string.IsNullOrEmpty(activeSeed))
                {
                    InteractionHint = LocalizationRuntime.IsVietnamese ? "[F] Gieo hạt giống" : "[F] Plant Seeds";
                    return;
                }
            }

            if (!nearbyPlot.IsWatered)
            {
                InteractionHint = LocalizationRuntime.IsVietnamese ? "[F] Tưới nước" : "[F] Water Soil";
                return;
            }

            InteractionHint = LocalizationRuntime.IsVietnamese ? "Cây đang lớn..." : "Crop is growing...";
        }

        private void InteractWithPlot()
        {
            if (nearbyPlot == null || inventorySession == null || inventorySession.Runtime == null) return;

            InventoryRuntime inv = inventorySession.Runtime;

            if (nearbyPlot.IsHarvestReady)
            {
                nearbyPlot.TryHarvest(inv);
                return;
            }

            if (!nearbyPlot.IsTilled)
            {
                nearbyPlot.TryTillSoil();
                return;
            }

            if (string.IsNullOrEmpty(nearbyPlot.PlantedCropId))
            {
                string seedId = GetActiveSeedInInventory();
                if (!string.IsNullOrEmpty(seedId) && inv.GetQuantity(seedId) > 0)
                {
                    if (nearbyPlot.TryPlantSeed(seedId))
                    {
                        inv.TryRemove(seedId, 1);
                        return;
                    }
                }
            }

            if (!nearbyPlot.IsWatered)
            {
                nearbyPlot.TryWaterSoil();
            }
        }

        private string GetActiveSeedInInventory()
        {
            if (inventorySession == null || inventorySession.Runtime == null) return string.Empty;
            InventoryRuntime inv = inventorySession.Runtime;

            if (inv.GetQuantity("item.seed-wheat") > 0) return "item.seed-wheat";
            if (inv.GetQuantity("item.seed-corn") > 0) return "item.seed-corn";
            if (inv.GetQuantity("item.seed-carrot") > 0) return "item.seed-carrot";
            return string.Empty;
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
