using System;
using System.Collections.Generic;
using UnityEngine;
using TheOldRoad.Inventory;
using TheOldRoad.Save;
using TheOldRoad.Audio;

namespace TheOldRoad.Building
{
    [Serializable]
    public sealed class ChestSlot
    {
        public string itemId = string.Empty;
        public int quantity = 0;
        public bool IsEmpty => string.IsNullOrEmpty(itemId) || quantity <= 0;
    }

    /// <summary>
    /// Interactive chest and vault storage container with slot grid, quick stacking, and persistence.
    /// </summary>
    public sealed class ChestStorageController : MonoBehaviour
    {
        public static ChestStorageController ActiveChest { get; private set; }

        [SerializeField] private string chestId = string.Empty;
        [SerializeField] private int capacity = 16;
        [SerializeField] private string chestDisplayNameVi = "Rương Gỗ";
        [SerializeField] private string chestDisplayNameEn = "Wooden Chest";

        private ChestSlot[] slots;

        public string ChestId => chestId;
        public int Capacity => capacity;
        public string DisplayNameVi => chestDisplayNameVi;
        public string DisplayNameEn => chestDisplayNameEn;

        private void Awake()
        {
            EnsureSlots();
            if (string.IsNullOrEmpty(chestId))
            {
                chestId = "chest." + Guid.NewGuid().ToString("N").Substring(0, 8);
            }
        }

        public void Configure(string id, int slotCount, string nameVi, string nameEn)
        {
            if (!string.IsNullOrEmpty(id)) chestId = id;
            capacity = Mathf.Max(8, slotCount);
            chestDisplayNameVi = nameVi;
            chestDisplayNameEn = nameEn;
            EnsureSlots();
        }

        private void EnsureSlots()
        {
            if (slots == null || slots.Length != capacity)
            {
                ChestSlot[] old = slots;
                slots = new ChestSlot[capacity];
                for (int i = 0; i < capacity; i++)
                {
                    slots[i] = (old != null && i < old.Length && old[i] != null)
                        ? old[i]
                        : new ChestSlot();
                }
            }
        }

        public ChestSlot GetSlot(int index)
        {
            EnsureSlots();
            if (index < 0 || index >= slots.Length) return null;
            return slots[index];
        }

        public bool DepositItem(int slotIndex, string itemId, int quantity, InventoryRuntime playerInventory)
        {
            EnsureSlots();
            if (slotIndex < 0 || slotIndex >= slots.Length || string.IsNullOrEmpty(itemId) || quantity <= 0) return false;
            if (playerInventory == null) return false;

            int available = playerInventory.GetQuantity(itemId);
            int transfer = Mathf.Min(available, quantity);
            if (transfer <= 0) return false;

            ChestSlot slot = slots[slotIndex];
            if (slot.IsEmpty)
            {
                if (playerInventory.TryRemove(itemId, transfer))
                {
                    slot.itemId = itemId;
                    slot.quantity = transfer;
                    AudioManager.PlayUiClick();
                    return true;
                }
            }
            else if (slot.itemId == itemId)
            {
                if (playerInventory.TryRemove(itemId, transfer))
                {
                    slot.quantity += transfer;
                    AudioManager.PlayUiClick();
                    return true;
                }
            }

            return false;
        }

        public bool WithdrawItem(int slotIndex, int quantity, InventoryRuntime playerInventory)
        {
            EnsureSlots();
            if (slotIndex < 0 || slotIndex >= slots.Length || playerInventory == null) return false;

            ChestSlot slot = slots[slotIndex];
            if (slot.IsEmpty || quantity <= 0) return false;

            int transfer = Mathf.Min(slot.quantity, quantity);
            if (playerInventory.TryAdd(slot.itemId, transfer))
            {
                slot.quantity -= transfer;
                if (slot.quantity <= 0)
                {
                    slot.itemId = string.Empty;
                    slot.quantity = 0;
                }
                AudioManager.PlayUiClick();
                return true;
            }
            return false;
        }

        public int QuickStack(InventoryRuntime playerInventory)
        {
            EnsureSlots();
            if (playerInventory == null) return 0;

            int stacked = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                ChestSlot slot = slots[i];
                if (!slot.IsEmpty)
                {
                    int invCount = playerInventory.GetQuantity(slot.itemId);
                    if (invCount > 0 && playerInventory.TryRemove(slot.itemId, invCount))
                    {
                        slot.quantity += invCount;
                        stacked += invCount;
                    }
                }
            }

            if (stacked > 0)
            {
                AudioManager.PlayUiClick();
            }
            return stacked;
        }

        public int TakeAll(InventoryRuntime playerInventory)
        {
            EnsureSlots();
            if (playerInventory == null) return 0;

            int taken = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                ChestSlot slot = slots[i];
                if (!slot.IsEmpty)
                {
                    if (playerInventory.TryAdd(slot.itemId, slot.quantity))
                    {
                        taken += slot.quantity;
                        slot.itemId = string.Empty;
                        slot.quantity = 0;
                    }
                }
            }

            if (taken > 0)
            {
                AudioManager.PlayUiClick();
            }
            return taken;
        }

        public ChestSaveEntry Save()
        {
            EnsureSlots();
            var entries = new List<ChestSlotSaveEntry>();
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsEmpty)
                {
                    entries.Add(new ChestSlotSaveEntry
                    {
                        slotIndex = i,
                        itemId = slots[i].itemId,
                        quantity = slots[i].quantity
                    });
                }
            }

            return new ChestSaveEntry
            {
                chestId = chestId,
                slots = entries.ToArray()
            };
        }

        public void Load(ChestSaveEntry save)
        {
            EnsureSlots();
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].itemId = string.Empty;
                slots[i].quantity = 0;
            }

            if (save == null || save.slots == null) return;
            if (!string.IsNullOrEmpty(save.chestId)) chestId = save.chestId;

            foreach (var s in save.slots)
            {
                if (s != null && s.slotIndex >= 0 && s.slotIndex < slots.Length && !string.IsNullOrEmpty(s.itemId))
                {
                    slots[s.slotIndex].itemId = s.itemId;
                    slots[s.slotIndex].quantity = Mathf.Max(0, s.quantity);
                }
            }
        }

        public static void OpenChestUI(ChestStorageController chest)
        {
            ActiveChest = chest;
        }

        public static void CloseChestUI()
        {
            ActiveChest = null;
        }
    }
}
