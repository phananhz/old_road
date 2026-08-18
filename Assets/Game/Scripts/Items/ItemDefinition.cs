using UnityEngine;

namespace TheOldRoad.Items
{
    /// <summary>Static, authored item data. Runtime quantities belong to InventoryItem.</summary>
    [CreateAssetMenu(menuName = "The Old Road/Items/Item Definition")]
    public sealed class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string itemId;
        [SerializeField] private string displayName;
        [SerializeField, Min(1)] private int maxStack = 99;
        [SerializeField] private Sprite icon;

        public string ItemId => itemId;
        public string DisplayName => displayName;
        public int MaxStack => maxStack;
        public Sprite Icon => icon;

        public void ConfigureForPrototype(string itemId, string displayName, int maxStack, Sprite icon = null)
        {
            this.itemId = itemId;
            this.displayName = displayName;
            this.maxStack = Mathf.Max(1, maxStack);
            if (icon != null) this.icon = icon;
        }
    }
}
