using UnityEngine;
using TheOldRoad.Construction;
using TheOldRoad.Core;
using TheOldRoad.Input;
using TheOldRoad.Inventory;
using TheOldRoad.Player;
using TheOldRoad.UI;

namespace TheOldRoad.Crafting
{
    /// <summary>Prototype cooking action unlocked by completed campfire or cooking hearth buildings.</summary>
    public sealed class PlayerCookingInteractor : MonoBehaviour
    {
        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private VerticalSliceController sliceController;
        [SerializeField, Min(0.2f)] private float interactionRadius = 1.9f;

        private ConstructionSite nearestCookingStation;
        private float nextScanTime;

        public string CookingHint { get; private set; } = string.Empty;
        public bool CanCookAction { get; private set; }

        public void Configure(InventorySession inventorySession, VerticalSliceController sliceController, float interactionRadius)
        {
            this.inventorySession = inventorySession;
            this.sliceController = sliceController;
            this.interactionRadius = Mathf.Max(0.2f, interactionRadius);
        }

        private void Update()
        {
            if (inventorySession == null) inventorySession = FindAnyObjectByType<InventorySession>();
            if (sliceController == null) sliceController = FindAnyObjectByType<VerticalSliceController>();

            RefreshState(false);
            if (!CanCookAction || !PrototypeInput.GetKeyDown(KeyCode.R)) return;
            Cook();
        }

        private void RefreshState(bool force)
        {
            if (force || UnityEngine.Time.unscaledTime >= nextScanTime)
            {
                nextScanTime = UnityEngine.Time.unscaledTime + 0.25f;
                nearestCookingStation = FindNearestCookingStation();
            }

            CanCookAction = nearestCookingStation != null;
            if (!CanCookAction)
            {
                CookingHint = string.Empty;
                return;
            }

            CookingHint = HasCookingIngredients()
                ? "Press R to cook a meal at the fire."
                : "Need berries plus mushroom or herb to cook.";
        }

        private void Cook()
        {
            if (inventorySession == null || inventorySession.Runtime == null) return;

            InventoryRuntime inventory = inventorySession.Runtime;
            bool usedMushroom = inventory.TryRemoveAll(new[] { ("item.wild-berries", 1), ("item.mushroom", 1) });
            bool usedHerb = !usedMushroom && inventory.TryRemoveAll(new[] { ("item.wild-berries", 1), ("item.medicinal-herb", 1) });
            if (!usedMushroom && !usedHerb)
            {
                CookingHint = "Need berries plus mushroom or herb to cook.";
                PlayerSpeechBubble.Say("speech.cook_blocked");
                return;
            }

            inventory.Add("item.cooked-meal", 1);
            PlayerVitals vitals = GetComponent<PlayerVitals>();
            if (vitals != null) vitals.Heal(4);
            CookingHint = "Cooked meal prepared. Health restored.";
            sliceController?.NotifyPrototypeStateChanged(CookingHint);
            PlayerSpeechBubble.Say("speech.cook_done");
        }

        private bool HasCookingIngredients()
        {
            InventoryRuntime inventory = inventorySession != null ? inventorySession.Runtime : null;
            if (inventory == null) return false;

            return inventory.GetQuantity("item.wild-berries") > 0
                && (inventory.GetQuantity("item.mushroom") > 0 || inventory.GetQuantity("item.medicinal-herb") > 0);
        }

        private ConstructionSite FindNearestCookingStation()
        {
            ConstructionSite nearest = null;
            float nearestDistance = float.MaxValue;
            ConstructionSite[] sites = FindObjectsByType<ConstructionSite>(FindObjectsInactive.Exclude);
            foreach (ConstructionSite site in sites)
            {
                if (site == null || !site.IsCompleted) continue;
                if (site.BuildingId != "building.campfire" && site.BuildingId != "building.cooking-hearth") continue;

                float distance = Vector2.Distance(transform.position, site.transform.position);
                if (distance > interactionRadius || distance >= nearestDistance) continue;

                nearest = site;
                nearestDistance = distance;
            }

            return nearest;
        }
    }
}
