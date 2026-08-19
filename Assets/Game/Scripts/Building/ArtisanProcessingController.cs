using System;
using System.Collections.Generic;
using UnityEngine;
using TheOldRoad.Inventory;
using TheOldRoad.Save;
using TheOldRoad.Audio;
using TheOldRoad.UI;

namespace TheOldRoad.Building
{
    [Serializable]
    public sealed class ArtisanRecipe
    {
        public string recipeId;
        public string machineBuildingId;
        public string inputItemId;
        public int inputQuantity;
        public string secondaryInputItemId;
        public int secondaryInputQuantity;
        public string outputItemId;
        public int outputQuantity;
        public float durationSeconds;
        public string displayNameVi;
        public string displayNameEn;
    }

    /// <summary>
    /// Interactive controller for artisan processing machines: Windmill, Blacksmith Forge, Cheese Press, Loom, Keg, Carpenter Bench.
    /// </summary>
    public sealed class ArtisanProcessingController : MonoBehaviour
    {
        public static ArtisanProcessingController ActiveMachine { get; private set; }

        [SerializeField] private string machineId = string.Empty;
        [SerializeField] private string buildingId = string.Empty;

        private string activeRecipeId = string.Empty;
        private string outputItemId = string.Empty;
        private int outputQuantity = 0;
        private float remainingSeconds = 0f;
        private float totalDuration = 0f;
        private bool isProcessing = false;
        private bool isFinished = false;

        public static readonly List<ArtisanRecipe> Recipes = new List<ArtisanRecipe>
        {
            // Windmill (Cối xay gió)
            new ArtisanRecipe { recipeId = "wm_flour_wheat", machineBuildingId = "building.windmill", inputItemId = "item.wheat", inputQuantity = 2, outputItemId = "item.flour", outputQuantity = 1, durationSeconds = 6f, displayNameVi = "Xay Lúa Mì -> Bột Mì", displayNameEn = "Mill Wheat -> Flour" },
            new ArtisanRecipe { recipeId = "wm_flour_corn", machineBuildingId = "building.windmill", inputItemId = "item.corn", inputQuantity = 2, outputItemId = "item.flour", outputQuantity = 1, durationSeconds = 6f, displayNameVi = "Xay Ngô -> Bột Bắp", displayNameEn = "Mill Corn -> Corn Flour" },

            // Cheese Press (Máy ép phô mai)
            new ArtisanRecipe { recipeId = "cp_cheese", machineBuildingId = "building.cheese-press", inputItemId = "item.milk", inputQuantity = 2, outputItemId = "item.cheese", outputQuantity = 1, durationSeconds = 8f, displayNameVi = "Ép Sữa Tươi -> Phô Mai Vàng", displayNameEn = "Press Milk -> Artisan Cheese" },

            // Loom (Khung dệt)
            new ArtisanRecipe { recipeId = "lm_cloth", machineBuildingId = "building.loom", inputItemId = "item.wool", inputQuantity = 2, outputItemId = "item.cloth", outputQuantity = 1, durationSeconds = 6f, displayNameVi = "Dệt Len Cừu -> Cuộn Vải", displayNameEn = "Weave Wool -> Fine Cloth" },

            // Keg (Thùng ủ lên men)
            new ArtisanRecipe { recipeId = "kg_juice", machineBuildingId = "building.keg", inputItemId = "item.wild-berries", inputQuantity = 3, outputItemId = "item.juice", outputQuantity = 1, durationSeconds = 8f, displayNameVi = "Ủ Dâu Rừng -> Nước Ép Trái Cây", displayNameEn = "Ferment Berries -> Fresh Juice" },
            new ArtisanRecipe { recipeId = "kg_wine_grape", machineBuildingId = "building.keg", inputItemId = "item.grape", inputQuantity = 3, outputItemId = "item.wine", outputQuantity = 1, durationSeconds = 10f, displayNameVi = "Ủ Nho Tươi -> Rượu Vang Đỏ", displayNameEn = "Brew Grapes -> Vintage Wine" },
            new ArtisanRecipe { recipeId = "kg_wine_pine", machineBuildingId = "building.keg", inputItemId = "item.pineapple", inputQuantity = 2, outputItemId = "item.wine", outputQuantity = 1, durationSeconds = 10f, displayNameVi = "Ủ Dứa -> Rượu Vang Nhiệt Đới", displayNameEn = "Brew Pineapple -> Tropical Wine" },

            // Blacksmith Forge (Lò rèn)
            new ArtisanRecipe { recipeId = "bf_iron_bar", machineBuildingId = "building.blacksmith-forge", inputItemId = "item.iron-ore", inputQuantity = 2, secondaryInputItemId = "item.wood", secondaryInputQuantity = 1, outputItemId = "item.iron-bar", outputQuantity = 1, durationSeconds = 8f, displayNameVi = "Luyện Quặng Sắt -> Thỏi Sắt", displayNameEn = "Smelt Iron Ore -> Iron Ingot" },
            new ArtisanRecipe { recipeId = "bf_armor", machineBuildingId = "building.blacksmith-forge", inputItemId = "item.iron-bar", inputQuantity = 4, secondaryInputItemId = "item.wool", secondaryInputQuantity = 2, outputItemId = "item.armor-knight", outputQuantity = 1, durationSeconds = 12f, displayNameVi = "Rèn Giáp Hiệp Sĩ", displayNameEn = "Forge Knight Armor" },

            // Carpenter Bench (Bàn thợ mộc)
            new ArtisanRecipe { recipeId = "cb_plank", machineBuildingId = "building.carpenter-bench", inputItemId = "item.wood", inputQuantity = 2, outputItemId = "item.cabin-plank", outputQuantity = 1, durationSeconds = 4f, displayNameVi = "Xẻ Gỗ -> Ván Gỗ Cabin", displayNameEn = "Saw Wood -> Cabin Plank" },
            new ArtisanRecipe { recipeId = "cb_fence", machineBuildingId = "building.carpenter-bench", inputItemId = "item.wood", inputQuantity = 3, outputItemId = "item.fence-wood", outputQuantity = 2, durationSeconds = 4f, displayNameVi = "Đóng Hàng Rào Gỗ", displayNameEn = "Craft Wooden Fence" }
        };

        public string MachineId => machineId;
        public string BuildingId => buildingId;
        public bool IsProcessing => isProcessing;
        public bool IsFinished => isFinished;
        public float RemainingSeconds => remainingSeconds;
        public float TotalDuration => totalDuration;
        public float Progress => totalDuration > 0f ? Mathf.Clamp01(1f - (remainingSeconds / totalDuration)) : 0f;
        public string OutputItemId => outputItemId;
        public int OutputQuantity => outputQuantity;

        private void Awake()
        {
            if (string.IsNullOrEmpty(machineId))
            {
                machineId = "artisan." + Guid.NewGuid().ToString("N").Substring(0, 8);
            }
        }

        public void Configure(string id, string buildingType)
        {
            if (!string.IsNullOrEmpty(id)) machineId = id;
            buildingId = buildingType;
        }

        private void Update()
        {
            if (isProcessing && !isFinished)
            {
                remainingSeconds -= UnityEngine.Time.deltaTime;
                if (remainingSeconds <= 0f)
                {
                    remainingSeconds = 0f;
                    isProcessing = false;
                    isFinished = true;
                    AudioManager.PlayUiClick();
                }
            }
        }

        public List<ArtisanRecipe> GetAvailableRecipes()
        {
            return Recipes.FindAll(r => r.machineBuildingId == buildingId);
        }

        public bool CanStartRecipe(ArtisanRecipe recipe, InventoryRuntime playerInventory)
        {
            if (recipe == null || isProcessing || isFinished || playerInventory == null) return false;

            if (playerInventory.GetQuantity(recipe.inputItemId) < recipe.inputQuantity) return false;
            if (!string.IsNullOrEmpty(recipe.secondaryInputItemId) && recipe.secondaryInputQuantity > 0)
            {
                if (playerInventory.GetQuantity(recipe.secondaryInputItemId) < recipe.secondaryInputQuantity) return false;
            }
            return true;
        }

        public bool StartRecipe(ArtisanRecipe recipe, InventoryRuntime playerInventory)
        {
            if (!CanStartRecipe(recipe, playerInventory)) return false;

            if (!playerInventory.TryRemove(recipe.inputItemId, recipe.inputQuantity)) return false;
            if (!string.IsNullOrEmpty(recipe.secondaryInputItemId) && recipe.secondaryInputQuantity > 0)
            {
                if (!playerInventory.TryRemove(recipe.secondaryInputItemId, recipe.secondaryInputQuantity))
                {
                    // Refund primary input on failure
                    playerInventory.TryAdd(recipe.inputItemId, recipe.inputQuantity);
                    return false;
                }
            }

            activeRecipeId = recipe.recipeId;
            outputItemId = recipe.outputItemId;
            outputQuantity = recipe.outputQuantity;
            totalDuration = recipe.durationSeconds;
            remainingSeconds = recipe.durationSeconds;
            isProcessing = true;
            isFinished = false;

            AudioManager.PlayUiClick();
            return true;
        }

        public bool CollectOutput(InventoryRuntime playerInventory)
        {
            if (!isFinished || string.IsNullOrEmpty(outputItemId) || outputQuantity <= 0 || playerInventory == null) return false;

            if (playerInventory.TryAdd(outputItemId, outputQuantity))
            {
                AudioManager.PlayUiClick();
                string name = LocalizationRuntime.ItemName(outputItemId);
                
                isFinished = false;
                isProcessing = false;
                outputItemId = string.Empty;
                outputQuantity = 0;
                activeRecipeId = string.Empty;
                return true;
            }
            return false;
        }

        public ArtisanSaveEntry Save()
        {
            return new ArtisanSaveEntry
            {
                machineId = machineId,
                machineBuildingId = buildingId,
                inputItemId = activeRecipeId,
                outputItemId = outputItemId,
                outputQuantity = outputQuantity,
                remainingProcessSeconds = remainingSeconds,
                isProcessing = isProcessing,
                isFinished = isFinished
            };
        }

        public void Load(ArtisanSaveEntry save)
        {
            if (save == null) return;
            if (!string.IsNullOrEmpty(save.machineId)) machineId = save.machineId;
            if (!string.IsNullOrEmpty(save.machineBuildingId)) buildingId = save.machineBuildingId;

            activeRecipeId = save.inputItemId;
            outputItemId = save.outputItemId;
            outputQuantity = save.outputQuantity;
            remainingSeconds = save.remainingProcessSeconds;
            isProcessing = save.isProcessing;
            isFinished = save.isFinished;
            totalDuration = Mathf.Max(5f, remainingSeconds);
        }

        public static void OpenMachineUI(ArtisanProcessingController machine)
        {
            ActiveMachine = machine;
        }

        public static void CloseMachineUI()
        {
            ActiveMachine = null;
        }
    }
}
