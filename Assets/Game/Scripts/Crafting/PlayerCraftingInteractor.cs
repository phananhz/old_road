using UnityEngine;
using TheOldRoad.Core;
using TheOldRoad.Input;
using TheOldRoad.Inventory;
using TheOldRoad.Items;
using TheOldRoad.UI;

namespace TheOldRoad.Crafting
{
    /// <summary>Development crafting adapter for the vertical slice. Press C to craft the selected prototype recipe.</summary>
    public sealed class PlayerCraftingInteractor : MonoBehaviour
    {
        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private VerticalSliceController sliceController;
        [SerializeField] private RecipeDefinition[] recipes;

        public string CraftingHint { get; private set; } = "Press C to craft.";

        public void Configure(InventorySession inventorySession, VerticalSliceController sliceController, params RecipeDefinition[] recipes)
        {
            this.inventorySession = inventorySession;
            this.sliceController = sliceController;
            this.recipes = recipes ?? System.Array.Empty<RecipeDefinition>();
            RefreshHint();
        }

        private void Update()
        {
            RefreshHint();
            if (!PrototypeInput.GetKeyDown(KeyCode.C)) return;

            if (recipes == null || recipes.Length == 0 || inventorySession == null || inventorySession.Runtime == null)
            {
                CraftingHint = "Crafting is not ready.";
                PlayerSpeechBubble.Say("speech.craft_blocked");
                return;
            }

            RecipeDefinition recipe = SelectCraftableRecipe();
            if (recipe == null)
            {
                RecipeDefinition blocked = SelectNextProgressRecipe();
                CraftingHint = blocked != null ? "Need: " + FormatIngredients(blocked) + "." : "No craftable recipe.";
                PlayerSpeechBubble.Say("speech.craft_blocked");
                return;
            }

            if (CraftingRuntime.TryCraft(recipe, inventorySession.Runtime))
            {
                CraftingHint = "Crafted " + recipe.ResultQuantity + " " + PrototypeItemCatalog.Get(recipe.ResultItemId).DisplayName + ".";
                TheOldRoad.Audio.AudioManager.PlayCraft();
                sliceController?.NotifyCrafted(recipe);
                PlayerSpeechBubble.Say("speech.craft_done");
            }
            else
            {
                CraftingHint = "Need: " + FormatIngredients(recipe) + ".";
                PlayerSpeechBubble.Say("speech.craft_blocked");
            }
        }

        private void RefreshHint()
        {
            if (recipes == null || recipes.Length == 0)
            {
                CraftingHint = "No recipe selected.";
                return;
            }

            if (inventorySession == null || inventorySession.Runtime == null)
            {
                CraftingHint = "Crafting inventory missing.";
                return;
            }

            RecipeDefinition craftable = SelectCraftableRecipe();
            if (craftable != null)
            {
                CraftingHint = "Press C to craft " + PrototypeItemCatalog.Get(craftable.ResultItemId).DisplayName + ".";
                return;
            }

            RecipeDefinition blocked = SelectNextProgressRecipe();
            CraftingHint = blocked != null
                ? "Need: " + FormatIngredients(blocked) + " for " + PrototypeItemCatalog.Get(blocked.ResultItemId).DisplayName + "."
                : "No craftable recipe.";
        }

        private RecipeDefinition SelectCraftableRecipe()
        {
            if (inventorySession == null || inventorySession.Runtime == null || recipes == null) return null;

            for (int i = 0; i < recipes.Length; i++)
            {
                RecipeDefinition candidate = recipes[i];
                if (candidate == null) continue;
                if (IsSingleToolAlreadyOwned(candidate)) continue;
                if (inventorySession.Runtime.HasAll(ToCostArray(candidate))) return candidate;
            }

            return null;
        }

        private RecipeDefinition SelectNextProgressRecipe()
        {
            if (recipes == null) return null;

            for (int i = 0; i < recipes.Length; i++)
            {
                RecipeDefinition candidate = recipes[i];
                if (candidate == null) continue;
                if (IsSingleToolAlreadyOwned(candidate)) continue;
                return candidate;
            }

            return recipes.Length > 0 ? recipes[recipes.Length - 1] : null;
        }

        private bool IsSingleToolAlreadyOwned(RecipeDefinition recipe)
        {
            return recipe != null
                && !string.IsNullOrWhiteSpace(recipe.ResultItemId)
                && recipe.ResultItemId.StartsWith("item.tool-", System.StringComparison.Ordinal)
                && inventorySession != null
                && inventorySession.Runtime != null
                && inventorySession.Runtime.Has(recipe.ResultItemId, 1);
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
                text += ingredient.quantity + " " + PrototypeItemCatalog.Get(ingredient.itemId).DisplayName;
            }

            return text;
        }
    }
}
