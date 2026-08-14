using TheOldRoad.Inventory;

namespace TheOldRoad.Crafting
{
    /// <summary>Executes a recipe without depending on UI or scene objects.</summary>
    public static class CraftingRuntime
    {
        public static bool TryCraft(RecipeDefinition recipe, InventoryRuntime inventory)
        {
            if (recipe == null || inventory == null || string.IsNullOrWhiteSpace(recipe.ResultItemId)) return false;
            if (recipe.Ingredients == null || recipe.Ingredients.Length == 0) return false;

            foreach (IngredientRequirement ingredient in recipe.Ingredients)
            {
                if (string.IsNullOrWhiteSpace(ingredient.itemId) || ingredient.quantity <= 0 || !inventory.Has(ingredient.itemId, ingredient.quantity))
                    return false;
            }

            foreach (IngredientRequirement ingredient in recipe.Ingredients)
                inventory.TryRemove(ingredient.itemId, ingredient.quantity);

            inventory.Add(recipe.ResultItemId, recipe.ResultQuantity);
            return true;
        }
    }
}
