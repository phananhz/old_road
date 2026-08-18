using System;
using UnityEngine;
using TheOldRoad.Construction;
using TheOldRoad.Core;
using TheOldRoad.Inventory;
using TheOldRoad.World;
using TheOldRoad.Audio;
using TheOldRoad.UI;
using TheOldRoad.Input;
using TheOldRoad.NPC;

namespace TheOldRoad.Building
{
    /// <summary>
    /// Interactive Animal Pen with openable gate and direct feeding/harvesting.
    /// Features an open timber fence that lets players see all animals inside.
    /// </summary>
    public sealed class AnimalPenController : MonoBehaviour
    {
        [SerializeField] private ConstructionSite site;
        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private VerticalSliceController sliceController;
        [SerializeField, Min(5f)] private float productionSeconds = 45f;
        [SerializeField] private string productItemId = "item.egg";
        [SerializeField] private bool isGateOpen;

        private float nextProductionTime;
        private string status = string.Empty;
        private float statusClearTime;
        private SpriteRenderer penRenderer;
        private SpriteRenderer gateRenderer;
        private BoxCollider2D gateCollider;
        private GameObject heartEmoteObj;
        private float heartHideTime;

        public string Status => status;
        public bool IsGateOpen => isGateOpen;
        public string InteractionHint { get; private set; } = string.Empty;

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
            EnsureComponents();
        }

        private void Awake()
        {
            EnsureComponents();
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

            // Check player proximity for gate and feeding interactions
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                float distance = Vector2.Distance(player.transform.position, transform.position);
                if (distance <= 2.2f)
                {
                    InteractionHint = isGateOpen
                        ? (LocalizationRuntime.IsVietnamese ? "[F] Đóng cổng chuồng / Cho ăn" : "[F] Close Gate / Feed")
                        : (LocalizationRuntime.IsVietnamese ? "[F] Mở cổng chuồng / Cho ăn" : "[F] Open Gate / Feed");

                    if (PrototypeInput.GetKeyDown(KeyCode.F) || PrototypeInput.GetKeyDown(KeyCode.E))
                    {
                        Interact(player);
                    }
                }
                else
                {
                    InteractionHint = string.Empty;
                }
            }

            if (heartEmoteObj != null && UnityEngine.Time.time > heartHideTime)
            {
                heartEmoteObj.SetActive(false);
            }

            // Passive production timer
            float remaining = nextProductionTime - UnityEngine.Time.time;
            if (remaining <= 0f)
            {
                if (inventorySession != null && inventorySession.Runtime != null)
                {
                    inventorySession.Runtime.Add(productItemId, 1);
                    status = LocalizationRuntime.IsVietnamese
                        ? $"Chuồng trại đã sản sinh 1 {LocalizationRuntime.ItemName(productItemId)}."
                        : $"Animal pen produced 1 {productItemId}.";
                    statusClearTime = UnityEngine.Time.time + 4f;
                    sliceController?.NotifyPrototypeStateChanged(status);
                }
                nextProductionTime = UnityEngine.Time.time + productionSeconds;
            }
            else if (UnityEngine.Time.time > statusClearTime)
            {
                status = string.Empty;
            }
        }

        public void ToggleGate()
        {
            isGateOpen = !isGateOpen;
            AudioManager.PlayDoorTransition();
            UpdateGateVisuals();
        }

        private void Interact(GameObject player)
        {
            InventoryRuntime inv = inventorySession != null ? inventorySession.Runtime : null;

            // Check if player has animal feed
            bool hasFeed = inv != null && (inv.GetQuantity("item.wheat") > 0 || inv.GetQuantity("item.wild-berries") > 0 || inv.GetQuantity("item.seed-wheat") > 0);

            if (hasFeed)
            {
                // Consume 1 feed and give product immediately + show heart
                if (inv.GetQuantity("item.wheat") > 0) inv.TryRemove("item.wheat", 1);
                else if (inv.GetQuantity("item.seed-wheat") > 0) inv.TryRemove("item.seed-wheat", 1);
                else inv.TryRemove("item.wild-berries", 1);

                inv.Add(productItemId, 2);
                ShowHeartEmote();
                AudioManager.PlayGatherSuccess();
                PlayerSpeechBubble.Say(LocalizationRuntime.IsVietnamese ? "Động vật rất vui vẻ! (+2 Nông sản)" : "Animals are happy! (+2 Produce)");
                return;
            }

            // Otherwise toggle gate
            ToggleGate();
        }

        private void ShowHeartEmote()
        {
            if (heartEmoteObj == null)
            {
                heartEmoteObj = new GameObject("HeartEmote");
                heartEmoteObj.transform.SetParent(transform, false);
                heartEmoteObj.transform.localPosition = new Vector3(0f, 1.4f, 0f);
                SpriteRenderer hr = heartEmoteObj.AddComponent<SpriteRenderer>();
                hr.sprite = PrototypePixelArtFactory.HeartEmote();
                hr.sortingOrder = 3000;
            }
            heartEmoteObj.SetActive(true);
            heartHideTime = UnityEngine.Time.time + 2.5f;
        }

        private void EnsureComponents()
        {
            if (penRenderer == null) penRenderer = GetComponent<SpriteRenderer>();
            UpdateGateVisuals();
            EnsureAnimalsInside();
        }

        private void EnsureAnimalsInside()
        {
            Transform animalsChild = transform.Find("AnimalsInside");
            if (animalsChild == null)
            {
                GameObject animalsObj = new GameObject("AnimalsInside");
                animalsObj.transform.SetParent(transform, false);
                animalsObj.transform.localPosition = new Vector3(0f, 0.2f, 0f);
                animalsChild = animalsObj.transform;

                var sr = animalsObj.AddComponent<SpriteRenderer>();
                if (productItemId == "item.milk" || productItemId == "item.wool")
                {
                    sr.sprite = PrototypePixelArtFactory.DairyCow();
                    sr.sortingOrder = 18;
                }
                else
                {
                    sr.sprite = PrototypePixelArtFactory.StrawNest(true);
                    sr.sortingOrder = 18;
                }
            }
        }

        private void UpdateGateVisuals()
        {
            Transform gateChild = transform.Find("GateChild");
            if (gateChild == null)
            {
                GameObject gateObj = new GameObject("GateChild");
                gateObj.transform.SetParent(transform, false);
                gateObj.transform.localPosition = new Vector3(0f, -0.65f, 0f);
                gateChild = gateObj.transform;
            }

            if (gateRenderer == null)
            {
                gateRenderer = gateChild.GetComponent<SpriteRenderer>();
                if (gateRenderer == null) gateRenderer = gateChild.gameObject.AddComponent<SpriteRenderer>();
                gateRenderer.sortingOrder = 20;
            }

            if (gateCollider == null)
            {
                gateCollider = gateChild.GetComponent<BoxCollider2D>();
                if (gateCollider == null) gateCollider = gateChild.gameObject.AddComponent<BoxCollider2D>();
                gateCollider.size = new Vector2(1f, 0.5f);
            }

            gateCollider.enabled = !isGateOpen;
            gateRenderer.sprite = PrototypePixelArtFactory.WoodGate(isGateOpen);
        }
    }
}
