using System;
using UnityEngine;
using TheOldRoad.Construction;
using TheOldRoad.Core;
using TheOldRoad.Inventory;

namespace TheOldRoad.Building
{
    /// <summary>Prototype passive animal pen production for completed pen buildings.</summary>
    public sealed class AnimalPenController : MonoBehaviour
    {
        [SerializeField] private ConstructionSite site;
        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private VerticalSliceController sliceController;
        [SerializeField, Min(5f)] private float productionSeconds = 45f;
        [SerializeField] private string productItemId = "item.egg";

        private float nextProductionTime;
        private string status = string.Empty;
        private float statusClearTime;

        public string Status => status;

        public void Configure(
            ConstructionSite site,
            InventorySession inventorySession,
            VerticalSliceController sliceController,
            string productItemId,
            float productionSeconds)
        {
            this.site = site;
            this.inventorySession = inventorySession;
            this.sliceController = sliceController;
            this.productItemId = string.IsNullOrWhiteSpace(productItemId) ? "item.egg" : productItemId;
            this.productionSeconds = Mathf.Max(5f, productionSeconds);
            nextProductionTime = UnityEngine.Time.time + this.productionSeconds;
        }

        private void Update()
        {
            if (site == null) site = GetComponent<ConstructionSite>();
            if (inventorySession == null) inventorySession = FindAnyObjectByType<InventorySession>();
            if (sliceController == null) sliceController = FindAnyObjectByType<VerticalSliceController>();

            if (site == null || !site.IsCompleted)
            {
                status = string.Empty;
                return;
            }

            float remaining = nextProductionTime - UnityEngine.Time.time;
            if (remaining > 0f)
            {
                if (UnityEngine.Time.time > statusClearTime) status = string.Empty;
                return;
            }

            if (inventorySession != null && inventorySession.Runtime != null)
            {
                inventorySession.Runtime.Add(productItemId, 1);
                status = "Animal pen produced 1 " + productItemId + ".";
                statusClearTime = UnityEngine.Time.time + 4f;
                sliceController?.NotifyPrototypeStateChanged(status);
            }

            nextProductionTime = UnityEngine.Time.time + productionSeconds;
        }
    }
}
