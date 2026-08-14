using UnityEngine;
using TheOldRoad.Core;
using TheOldRoad.Input;
using TheOldRoad.Inventory;

namespace TheOldRoad.Crafting
{
    /// <summary>Development crafting adapter for the vertical slice. Press C to craft the selected prototype recipe.</summary>
    public sealed class PlayerCraftingInteractor : MonoBehaviour
    {
        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private VerticalSliceController sliceController;
        [SerializeField] private RecipeDefinition recipe;

        public string CraftingHint { get; private set; } = "Press C to craft Cabin Plank.";

        public void Configure(InventorySession inventorySession, VerticalSliceController sliceController, RecipeDefinition recipe)
        {
            this.inventorySession = inventorySession;
            this.sliceController = sliceController;
            this.recipe = recipe;
            RefreshHint();
        }

        private void Update()
        {
            RefreshHint();
            if (!PrototypeInput.GetKeyDown(KeyCode.C)) return;

            if (recipe == null || inventorySession == null || inventorySession.Runtime == null)
            {
                CraftingHint = "Crafting is not ready.";
                return;
            }

            if (CraftingRuntime.TryCraft(recipe, inventorySession.Runtime))
            {
                CraftingHint = "Crafted " + recipe.ResultQuantity + " " + recipe.ResultItemId + ".";
                sliceController?.NotifyCrafted(recipe);
            }
            else
            {
                CraftingHint = "Need: " + FormatIngredients(recipe) + ".";
            }
        }

        private void RefreshHint()
        {
            if (recipe == null)
            {
                CraftingHint = "No recipe selected.";
                return;
            }

            if (inventorySession == null || inventorySession.Runtime == null)
            {
                CraftingHint = "Crafting inventory missing.";
                return;
            }

            CraftingHint = inventorySession.Runtime.HasAll(ToCostArray(recipe))
                ? "Press C to craft " + recipe.ResultItemId + "."
                : "Need: " + FormatIngredients(recipe) + ".";
        }

        private static (string itemId, int quantity)[] ToCostArray(RecipeDefinition recipe)
        {
            if (recipe == null || recipe.Ingredients == null) return System.Array.Empty<(string itemId, int quantity)>();

            (string itemId, int quantity)[] costs = new (string itemId, int quantity)[recipe.Ingredients.Length];
            for (int i = 0; i < recipe.Ingredients.Length; i++)
            {
                costs[i] = (recipe.Ingredients[i].itemId, recipe.Ingredients[i].quantity);
            }

            return costs;
        }

        private static string FormatIngredients(RecipeDefinition recipe)
        {
            if (recipe == null || recipe.Ingredients == null || recipe.Ingredients.Length == 0) return "none";

            string text = string.Empty;
            for (int i = 0; i < recipe.Ingredients.Length; i++)
            {
                IngredientRequirement ingredient = recipe.Ingredients[i];
                if (i > 0) text += ", ";
                text += ingredient.quantity + " " + ingredient.itemId;
            }

            return text;
        }
    }
}
