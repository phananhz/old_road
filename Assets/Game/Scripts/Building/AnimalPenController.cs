using System;
using System.Collections.Generic;
using UnityEngine;
using TheOldRoad.Audio;
using TheOldRoad.Combat;
using TheOldRoad.Construction;
using TheOldRoad.Core;
using TheOldRoad.Farming;
using TheOldRoad.Input;
using TheOldRoad.Inventory;
using TheOldRoad.Time;
using TheOldRoad.UI;
using TheOldRoad.World;

namespace TheOldRoad.Building
{
    /// <summary>
    /// Interactive Animal Pen & Pasture compound (Avatar style).
    /// Enclosed with real wooden fences, working gate, and fully interactive animals
    /// (Dairy Cows, Fluffy Sheep, Poultry Straw Nests, Farm Dog, Troughs & Hay Stacks).
    /// The physical perimeter matches the placement footprint exactly.
    /// </summary>
    public sealed class AnimalPenController : MonoBehaviour
    {
        [SerializeField] private string buildingId = "building.animal-pen-long";
        [SerializeField] private int width = 11;
        [SerializeField] private int height = 6;
        [SerializeField] private bool isGateOpen = false;
        [SerializeField] private ConstructionSite site;
        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private VerticalSliceController sliceController;

        private Transform playerTransform;
        private Transform gateChild;
        private SpriteRenderer gateRenderer;
        private BoxCollider2D gateCollider;
        private float gateX;
        private float gateY;

        public int Width => width;
        public int Height => height;
        public bool IsGateOpen => isGateOpen;
        public string BuildingId => buildingId;
        public string Status => status;
        public string InteractionHint { get; private set; } = string.Empty;

        private string status = string.Empty;

        public void Configure(
            ConstructionSite site,
            InventorySession inventorySession,
            VerticalSliceController sliceController,
            string buildingId,
            float productionSeconds = 45f)
        {
            this.site = site;
            this.inventorySession = inventorySession;
            this.sliceController = sliceController;
            this.buildingId = string.IsNullOrWhiteSpace(buildingId) ? "building.animal-pen-long" : buildingId;

            if (this.buildingId == "building.animal-pen-small")
            {
                this.width = 7;
                this.height = 5;
            }
            else
            {
                this.width = 11;
                this.height = 6;
            }

            BuildEnclosureAndAnimals();
        }

        private void Start()
        {
            if (transform.childCount == 0)
            {
                if (buildingId == "building.animal-pen-small")
                {
                    width = 7;
                    height = 5;
                }
                else
                {
                    width = 11;
                    height = 6;
                }
                BuildEnclosureAndAnimals();
            }
        }

        private void Update()
        {
            if (playerTransform == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null) playerTransform = player.transform;
                return;
            }

            if (gateChild != null)
            {
                float dist = Vector2.Distance(playerTransform.position, gateChild.position);
                if (dist <= 2.2f)
                {
                    InteractionHint = isGateOpen
                        ? (LocalizationRuntime.IsVietnamese ? "[F] Đóng cổng chuồng" : "[F] Close Gate")
                        : (LocalizationRuntime.IsVietnamese ? "[F] Mở cổng chuồng" : "[F] Open Gate");

                    if (PrototypeInput.GetKeyDown(KeyCode.F))
                    {
                        ToggleGate();
                    }
                }
                else
                {
                    InteractionHint = string.Empty;
                }
            }
        }

        public void ToggleGate()
        {
            isGateOpen = !isGateOpen;
            AudioManager.PlayDoorTransition();
            UpdateGateVisuals();
        }

        private void UpdateGateVisuals()
        {
            if (gateRenderer != null)
            {
                gateRenderer.sprite = PrototypePixelArtFactory.WoodGate(isGateOpen);
            }
            if (gateCollider != null)
            {
                gateCollider.enabled = !isGateOpen;
            }
        }

        public void BuildEnclosureAndAnimals()
        {
            // Clear old children if any
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            // Hide root sprite renderer
            var rootSr = GetComponent<SpriteRenderer>();
            if (rootSr != null) rootSr.sprite = null;

            // Remove or adjust root BoxCollider so player doesn't get blocked by the center
            var rootCol = GetComponent<BoxCollider2D>();
            if (rootCol != null) rootCol.enabled = false;

            float halfW = width * 0.5f;
            float halfH = height * 0.5f;

            if (inventorySession == null) inventorySession = FindAnyObjectByType<InventorySession>();

            // Determine gate position
            gateX = buildingId == "building.animal-pen-small" ? 0f : -3.5f;
            gateY = -halfH + 0.5f;

            // =========================================================================
            // 1. PERIMETER FENCES & SOLID COLLIDERS (Exact matching footprint)
            // =========================================================================
            GameObject fenceGroup = new GameObject("Perimeter Fences");
            fenceGroup.transform.SetParent(transform, false);

            // 1a. Top Fence Line
            for (float x = -halfW + 0.5f; x <= halfW - 0.5f; x += 1.0f)
            {
                GameObject seg = new GameObject($"TopFence_{x:F1}");
                seg.transform.SetParent(fenceGroup.transform, false);
                seg.transform.localPosition = new Vector3(x, halfH - 0.5f, 0f);
                var sr = seg.AddComponent<SpriteRenderer>();
                bool isCorner = Mathf.Abs(x - (-halfW + 0.5f)) < 0.2f || Mathf.Abs(x - (halfW - 0.5f)) < 0.2f;
                sr.sprite = isCorner ? PrototypePixelArtFactory.WoodFenceCorner() : PrototypePixelArtFactory.WoodFenceHorizontal();
                sr.sortingOrder = 8;
            }

            // Top Solid Collider
            GameObject topCol = new GameObject("TopCollider");
            topCol.transform.SetParent(fenceGroup.transform, false);
            topCol.transform.localPosition = new Vector3(0f, halfH - 0.5f, 0f);
            var colTop = topCol.AddComponent<BoxCollider2D>();
            colTop.size = new Vector2(width, 0.8f);

            // 1b. Bottom Fence Line (with Gate Gap)
            for (float x = -halfW + 0.5f; x <= halfW - 0.5f; x += 1.0f)
            {
                if (Mathf.Abs(x - gateX) < 0.6f) continue; // Gap for gate

                GameObject seg = new GameObject($"BottomFence_{x:F1}");
                seg.transform.SetParent(fenceGroup.transform, false);
                seg.transform.localPosition = new Vector3(x, -halfH + 0.5f, 0f);
                var sr = seg.AddComponent<SpriteRenderer>();
                bool isCorner = Mathf.Abs(x - (-halfW + 0.5f)) < 0.2f || Mathf.Abs(x - (halfW - 0.5f)) < 0.2f;
                sr.sprite = isCorner ? PrototypePixelArtFactory.WoodFenceCorner() : PrototypePixelArtFactory.WoodFenceHorizontal();
                sr.sortingOrder = 24;
            }

            // Bottom Left Collider
            float leftLen = (gateX - (-halfW));
            if (leftLen > 0.5f)
            {
                GameObject botLeftCol = new GameObject("BotLeftCollider");
                botLeftCol.transform.SetParent(fenceGroup.transform, false);
                botLeftCol.transform.localPosition = new Vector3(-halfW + leftLen * 0.5f, -halfH + 0.5f, 0f);
                var colBL = botLeftCol.AddComponent<BoxCollider2D>();
                colBL.size = new Vector2(leftLen - 0.4f, 0.8f);
            }

            // Bottom Right Collider
            float rightLen = (halfW - gateX);
            if (rightLen > 0.5f)
            {
                GameObject botRightCol = new GameObject("BotRightCollider");
                botRightCol.transform.SetParent(fenceGroup.transform, false);
                botRightCol.transform.localPosition = new Vector3(gateX + rightLen * 0.5f, -halfH + 0.5f, 0f);
                var colBR = botRightCol.AddComponent<BoxCollider2D>();
                colBR.size = new Vector2(rightLen - 0.4f, 0.8f);
            }

            // 1c. Left & Right Fence Lines
            for (float y = -halfH + 1.5f; y <= halfH - 1.5f; y += 1.0f)
            {
                GameObject leftSeg = new GameObject($"LeftFence_{y:F1}");
                leftSeg.transform.SetParent(fenceGroup.transform, false);
                leftSeg.transform.localPosition = new Vector3(-halfW + 0.5f, y, 0f);
                var srL = leftSeg.AddComponent<SpriteRenderer>();
                srL.sprite = PrototypePixelArtFactory.WoodFenceVertical();
                srL.sortingOrder = 12;

                GameObject rightSeg = new GameObject($"RightFence_{y:F1}");
                rightSeg.transform.SetParent(fenceGroup.transform, false);
                rightSeg.transform.localPosition = new Vector3(halfW - 0.5f, y, 0f);
                var srR = rightSeg.AddComponent<SpriteRenderer>();
                srR.sprite = PrototypePixelArtFactory.WoodFenceVertical();
                srR.sortingOrder = 12;
            }

            // Left & Right Solid Colliders
            GameObject leftCol = new GameObject("LeftCollider");
            leftCol.transform.SetParent(fenceGroup.transform, false);
            leftCol.transform.localPosition = new Vector3(-halfW + 0.5f, 0f, 0f);
            var colL = leftCol.AddComponent<BoxCollider2D>();
            colL.size = new Vector2(0.8f, height - 1.2f);

            GameObject rightCol = new GameObject("RightCollider");
            rightCol.transform.SetParent(fenceGroup.transform, false);
            rightCol.transform.localPosition = new Vector3(halfW - 0.5f, 0f, 0f);
            var colR = rightCol.AddComponent<BoxCollider2D>();
            colR.size = new Vector2(0.8f, height - 1.2f);

            // 1d. Gate & Lantern
            GameObject gateObj = new GameObject("Compound Gate");
            gateObj.transform.SetParent(transform, false);
            gateObj.transform.localPosition = new Vector3(gateX, gateY, 0f);
            gateChild = gateObj.transform;

            gateRenderer = gateObj.AddComponent<SpriteRenderer>();
            gateRenderer.sprite = PrototypePixelArtFactory.WoodGate(isGateOpen);
            gateRenderer.sortingOrder = 25;

            gateCollider = gateObj.AddComponent<BoxCollider2D>();
            gateCollider.size = new Vector2(1.2f, 0.8f);
            gateCollider.enabled = !isGateOpen;

            GameObject lanternObj = new GameObject("Gate Lantern");
            lanternObj.transform.SetParent(transform, false);
            lanternObj.transform.localPosition = new Vector3(gateX - 0.8f, gateY + 0.2f, 0f);
            lanternObj.AddComponent<SpriteRenderer>().sprite = PrototypePixelArtFactory.GateLantern(true);
            lanternObj.AddComponent<YSortSprite>().Configure(26);

            // =========================================================================
            // 2. INTERIOR STRUCTURES & ANIMALS (Exact Avatar layout)
            // =========================================================================
            if (buildingId == "building.animal-pen-small")
            {
                // SMALL PEN (7x5): Chicken Coop & Sheep Yard
                GameObject shelter = new GameObject("Poultry Shelter");
                shelter.transform.SetParent(transform, false);
                shelter.transform.localPosition = new Vector3(-1.6f, 1.0f, 0f);
                shelter.AddComponent<SpriteRenderer>().sprite = PrototypePixelArtFactory.AnimalPenSmall();
                shelter.AddComponent<YSortSprite>().Configure(9);
                var sCol = shelter.AddComponent<BoxCollider2D>();
                sCol.size = new Vector2(2.4f, 1.4f);

                // 2 Straw Nests for Eggs
                GameObject nest1 = new GameObject("Straw Nest A");
                nest1.transform.SetParent(transform, false);
                nest1.transform.localPosition = new Vector3(-1.8f, 0.2f, 0f);
                nest1.AddComponent<StrawNestController>().Configure(inventorySession);

                GameObject nest2 = new GameObject("Straw Nest B");
                nest2.transform.SetParent(transform, false);
                nest2.transform.localPosition = new Vector3(-0.6f, 0.2f, 0f);
                nest2.AddComponent<StrawNestController>().Configure(inventorySession);

                // 2 Clucking Hens
                CreateHen("Hen Penny", new Vector3(-1.4f, -0.6f, 0f));
                CreateHen("Hen Clucky", new Vector3(-0.4f, -0.6f, 0f));

                // 1 Fluffy Sheep
                GameObject sheep = new GameObject("Fluffy Sheep Dolly");
                sheep.transform.SetParent(transform, false);
                sheep.transform.localPosition = new Vector3(1.6f, 0.2f, 0f);
                sheep.AddComponent<SheepController>().Configure(inventorySession);

                // Troughs
                CreateProp("Feeding Trough", PrototypePixelArtFactory.FeedingTrough(), new Vector3(1.2f, -1.0f, 0f), 20);
                CreateProp("Water Trough", PrototypePixelArtFactory.WaterTrough(), new Vector3(2.2f, -1.0f, 0f), 20);
                CreateProp("Hay Bale", PrototypePixelArtFactory.HayBalePile(), new Vector3(-2.0f, -1.0f, 0f), 20);
                CreateProp("Scarecrow", PrototypePixelArtFactory.Scarecrow(), new Vector3(-2.4f, 0.8f, 0f), 10);
            }
            else
            {
                // LARGE PEN (11x6): FULL AVATAR ESTATE (Barn, Cows, Sheep, Hens, Dog, Troughs)
                // 2a. Red Dairy Barn
                GameObject barn = new GameObject("Dairy Barn (Happy Farm)");
                barn.transform.SetParent(transform, false);
                barn.transform.localPosition = new Vector3(-2.6f, 1.2f, 0f);
                barn.AddComponent<SpriteRenderer>().sprite = PrototypePixelArtFactory.HappyFarmBarn();
                barn.AddComponent<YSortSprite>().Configure(9);
                var bCol = barn.AddComponent<BoxCollider2D>();
                bCol.size = new Vector2(3.8f, 1.8f);
                bCol.offset = new Vector2(0f, 0.4f);

                // Hay Bale Stacks
                CreateProp("Hay Bale Stack A", PrototypePixelArtFactory.HayBalePile(), new Vector3(-0.6f, 1.2f, 0f), 10);
                CreateProp("Hay Bale Stack B", PrototypePixelArtFactory.HayBalePile(), new Vector3(4.2f, 1.4f, 0f), 10);

                // 2b. Cow Yard (Bella & Daisy) + Troughs
                CreateProp("Cow Feeding Trough", PrototypePixelArtFactory.FeedingTrough(), new Vector3(-2.4f, 0.0f, 0f), 15);
                CreateProp("Cow Water Trough", PrototypePixelArtFactory.WaterTrough(), new Vector3(-1.0f, 0.0f, 0f), 15);

                GameObject cow1 = new GameObject("Dairy Cow Bella");
                cow1.transform.SetParent(transform, false);
                cow1.transform.localPosition = new Vector3(-2.2f, -1.2f, 0f);
                cow1.AddComponent<DairyCowController>().Configure(inventorySession);

                GameObject cow2 = new GameObject("Dairy Cow Daisy");
                cow2.transform.SetParent(transform, false);
                cow2.transform.localPosition = new Vector3(-0.6f, -1.4f, 0f);
                cow2.AddComponent<DairyCowController>().Configure(inventorySession);

                // 2c. Sheep Yard (Dolly & Wooly) + Troughs
                CreateProp("Sheep Feeding Trough", PrototypePixelArtFactory.FeedingTrough(), new Vector3(2.2f, 0.2f, 0f), 15);
                CreateProp("Sheep Water Trough", PrototypePixelArtFactory.WaterTrough(), new Vector3(3.6f, 0.2f, 0f), 15);

                GameObject sheep1 = new GameObject("Fluffy Sheep Dolly");
                sheep1.transform.SetParent(transform, false);
                sheep1.transform.localPosition = new Vector3(2.2f, -1.2f, 0f);
                sheep1.AddComponent<SheepController>().Configure(inventorySession);

                GameObject sheep2 = new GameObject("Fluffy Sheep Wooly");
                sheep2.transform.SetParent(transform, false);
                sheep2.transform.localPosition = new Vector3(3.8f, -1.0f, 0f);
                sheep2.AddComponent<SheepController>().Configure(inventorySession);

                // 2d. Poultry Coop (2 Straw Nests & Hens)
                GameObject nest1 = new GameObject("Straw Nest 1");
                nest1.transform.SetParent(transform, false);
                nest1.transform.localPosition = new Vector3(0.8f, 1.2f, 0f);
                nest1.AddComponent<StrawNestController>().Configure(inventorySession);

                GameObject nest2 = new GameObject("Straw Nest 2");
                nest2.transform.SetParent(transform, false);
                nest2.transform.localPosition = new Vector3(2.0f, 1.2f, 0f);
                nest2.AddComponent<StrawNestController>().Configure(inventorySession);

                CreateHen("Hen Ginger", new Vector3(0.9f, 0.4f, 0f));
                CreateHen("Hen Pepper", new Vector3(2.1f, 0.4f, 0f));

                // 2e. Loyal Farm Dog guarding entrance
                GameObject dog = new GameObject("Farm Dog Buddy");
                dog.transform.SetParent(transform, false);
                dog.transform.localPosition = new Vector3(-4.4f, -1.6f, 0f);
                dog.AddComponent<FarmDogController>();

                // Signboard & Scarecrow
                CreateProp("Farm Signboard", PrototypePixelArtFactory.FarmSignboard(), new Vector3(-4.4f, 0.2f, 0f), 10);
                CreateProp("Scarecrow", PrototypePixelArtFactory.Scarecrow(), new Vector3(-4.4f, 1.4f, 0f), 10);
            }
        }

        private void CreateProp(string name, Sprite sprite, Vector3 localPos, int sortOrder)
        {
            GameObject prop = new GameObject(name);
            prop.transform.SetParent(transform, false);
            prop.transform.localPosition = localPos;
            var sr = prop.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            prop.AddComponent<YSortSprite>().Configure(sortOrder);
        }

        private void CreateHen(string name, Vector3 localPos)
        {
            GameObject hen = new GameObject(name);
            hen.transform.SetParent(transform, false);
            hen.transform.localPosition = localPos;
            var sr = hen.AddComponent<SpriteRenderer>();
            sr.sprite = PrototypePixelArtFactory.HenSprite();
            hen.AddComponent<YSortSprite>().Configure(18);
        }
    }
}
