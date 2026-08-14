using UnityEngine;

namespace TheOldRoad.Crafting
{
    [System.Serializable]
    public struct IngredientRequirement
    {
        public string itemId;
        [Min(1)] public int quantity;
    }

    /// <summary>Static recipe data authored as a Unity asset.</summary>
    [CreateAssetMenu(menuName = "The Old Road/Crafting/Recipe Definition")]
    public sealed class RecipeDefinition : ScriptableObject
    {
        [SerializeField] private string recipeId;
        [SerializeField] private IngredientRequirement[] ingredients;
        [SerializeField] private string resultItemId;
        [SerializeField, Min(1)] private int resultQuantity = 1;
        [SerializeField, Min(0f)] private float craftingDurationSeconds;
        [SerializeField] private string workstationId;

        public string RecipeId => recipeId;
        public IngredientRequirement[] Ingredients => ingredients;
        public string ResultItemId => resultItemId;
        public int ResultQuantity => resultQuantity;
        public float CraftingDurationSeconds => craftingDurationSeconds;
        public string WorkstationId => workstationId;

        public void ConfigureForPrototype(
            string recipeId,
            IngredientRequirement[] ingredients,
            string resultItemId,
            int resultQuantity,
            float craftingDurationSeconds,
            string workstationId)
        {
            this.recipeId = recipeId;
            this.ingredients = ingredients ?? System.Array.Empty<IngredientRequirement>();
            this.resultItemId = resultItemId;
            this.resultQuantity = Mathf.Max(1, resultQuantity);
            this.craftingDurationSeconds = Mathf.Max(0f, craftingDurationSeconds);
            this.workstationId = workstationId;
        }
    }
}
