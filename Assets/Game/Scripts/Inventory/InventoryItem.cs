namespace TheOldRoad.Inventory
{
    /// <summary>Mutable runtime inventory entry represented by a stable item ID.</summary>
    [System.Serializable]
    public sealed class InventoryItem
    {
        public string ItemId { get; private set; }
        public int Quantity { get; private set; }

        public InventoryItem(string itemId, int quantity)
        {
            ItemId = itemId;
            Quantity = quantity;
        }

        public void Add(int amount) => Quantity += amount;
        public bool Remove(int amount)
        {
            if (amount <= 0 || amount > Quantity) return false;
            Quantity -= amount;
            return true;
        }
    }
}
