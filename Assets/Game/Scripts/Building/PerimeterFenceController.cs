using System;
using System.Collections.Generic;
using UnityEngine;
using TheOldRoad.Audio;
using TheOldRoad.Combat;
using TheOldRoad.Input;
using TheOldRoad.Time;
using TheOldRoad.UI;
using TheOldRoad.World;

namespace TheOldRoad.Building
{
    /// <summary>
    /// Enclosed multi-directional perimeter fence with 2.5D vertical rails,
    /// horizontal rails, corner posts, functional gate, night lanterns, and safe zone.
    /// Usable for enclosing homesteads, farms, houses, or animal pastures.
    /// </summary>
    public sealed class PerimeterFenceController : MonoBehaviour
    {
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 8;
        [SerializeField] private bool isGateOpen = false;
        [SerializeField] private string farmName = "Trang Trại Valen";

        private Transform playerTransform;
        private Transform gateChild;
        private SpriteRenderer gateRenderer;
        private BoxCollider2D gateCollider;
        private SpriteRenderer lanternLeftRenderer;
        private SpriteRenderer lanternRightRenderer;
        private GameTimeController gameTime;
        private bool hasGreetedPlayer;
        private Vector3 gateWorldPos;

        public int Width => width;
        public int Height => height;
        public bool IsGateOpen => isGateOpen;
        public string FarmName => farmName;

        public void Configure(int width, int height, string farmName = "Trang Trại Valen")
        {
            this.width = Mathf.Max(4, width);
            this.height = Mathf.Max(3, height);
            this.farmName = string.IsNullOrWhiteSpace(farmName) ? (LocalizationRuntime.IsVietnamese ? "Trang Trại Valen" : "Valen Homestead") : farmName;
            RebuildPerimeter();
        }

        private void Start()
        {
            gameTime = FindAnyObjectByType<GameTimeController>();
            if (transform.childCount == 0)
            {
                RebuildPerimeter();
            }
        }

        public void RebuildPerimeter()
        {
            // Clear existing children
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            int gateXIndex = width / 2;

            // 1. Top Horizontal Fence Line (y = height - 1)
            for (int x = 0; x < width; x++)
            {
                GameObject seg = new GameObject($"TopFence_{x}");
                seg.transform.SetParent(transform, false);
                seg.transform.localPosition = new Vector3(x, height - 1, 0f);
                var sr = seg.AddComponent<SpriteRenderer>();
                sr.sprite = (x == 0 || x == width - 1) ? PrototypePixelArtFactory.WoodFenceCorner() : PrototypePixelArtFactory.WoodFenceHorizontal();
                sr.sortingOrder = 14;
            }

            // Top Solid Collider
            GameObject topCol = new GameObject("TopCollider");
            topCol.transform.SetParent(transform, false);
            topCol.transform.localPosition = new Vector3((width - 1) * 0.5f, height - 1, 0f);
            var colTop = topCol.AddComponent<BoxCollider2D>();
            colTop.size = new Vector2(width, 0.8f);

            // 2. Bottom Horizontal Fence Line (y = 0, with Gate Opening at gateXIndex)
            for (int x = 0; x < width; x++)
            {
                if (x == gateXIndex) continue; // Gate opening

                GameObject seg = new GameObject($"BottomFence_{x}");
                seg.transform.SetParent(transform, false);
                seg.transform.localPosition = new Vector3(x, 0f, 0f);
                var sr = seg.AddComponent<SpriteRenderer>();
                sr.sprite = (x == 0 || x == width - 1) ? PrototypePixelArtFactory.WoodFenceCorner() : PrototypePixelArtFactory.WoodFenceHorizontal();
                sr.sortingOrder = 20;
            }

            // Bottom Left & Right Colliders
            int leftLen = gateXIndex;
            int rightLen = width - 1 - gateXIndex;

            if (leftLen > 0)
            {
                GameObject bLeftCol = new GameObject("BottomLeftCollider");
                bLeftCol.transform.SetParent(transform, false);
                bLeftCol.transform.localPosition = new Vector3((leftLen - 1) * 0.5f, 0f, 0f);
                var colBL = bLeftCol.AddComponent<BoxCollider2D>();
                colBL.size = new Vector2(leftLen, 0.8f);
            }

            if (rightLen > 0)
            {
                GameObject bRightCol = new GameObject("BottomRightCollider");
                bRightCol.transform.SetParent(transform, false);
                bRightCol.transform.localPosition = new Vector3((gateXIndex + 1 + width - 1) * 0.5f, 0f, 0f);
                var colBR = bRightCol.AddComponent<BoxCollider2D>();
                colBR.size = new Vector2(rightLen, 0.8f);
            }

            // 3. Left Vertical 2.5D Fence Line (x = 0, y from 1 to height - 2)
            for (int y = 1; y < height - 1; y++)
            {
                GameObject seg = new GameObject($"LeftFence_{y}");
                seg.transform.SetParent(transform, false);
                seg.transform.localPosition = new Vector3(0f, y, 0f);
                var sr = seg.AddComponent<SpriteRenderer>();
                sr.sprite = PrototypePixelArtFactory.WoodFenceVertical();
                sr.sortingOrder = 18;
            }

            GameObject leftCol = new GameObject("LeftCollider");
            leftCol.transform.SetParent(transform, false);
            leftCol.transform.localPosition = new Vector3(0f, (height - 1) * 0.5f, 0f);
            var colLeft = leftCol.AddComponent<BoxCollider2D>();
            colLeft.size = new Vector2(0.8f, height);

            // 4. Right Vertical 2.5D Fence Line (x = width - 1, y from 1 to height - 2)
            for (int y = 1; y < height - 1; y++)
            {
                GameObject seg = new GameObject($"RightFence_{y}");
                seg.transform.SetParent(transform, false);
                seg.transform.localPosition = new Vector3(width - 1, y, 0f);
                var sr = seg.AddComponent<SpriteRenderer>();
                sr.sprite = PrototypePixelArtFactory.WoodFenceVertical();
                sr.sortingOrder = 18;
            }

            GameObject rightCol = new GameObject("RightCollider");
            rightCol.transform.SetParent(transform, false);
            rightCol.transform.localPosition = new Vector3(width - 1, (height - 1) * 0.5f, 0f);
            var colRight = rightCol.AddComponent<BoxCollider2D>();
            colRight.size = new Vector2(0.8f, height);

            // 5. Functional Gate Child & Lanterns at Gate Opening (x = gateXIndex, y = 0)
            gateWorldPos = transform.position + new Vector3(gateXIndex, 0f, 0f);

            GameObject gateObj = new GameObject("GateChild");
            gateObj.transform.SetParent(transform, false);
            gateObj.transform.localPosition = new Vector3(gateXIndex, 0f, 0f);
            gateChild = gateObj.transform;

            gateRenderer = gateObj.AddComponent<SpriteRenderer>();
            gateRenderer.sprite = PrototypePixelArtFactory.WoodGate(isGateOpen);
            gateRenderer.sortingOrder = 22;

            gateCollider = gateObj.AddComponent<BoxCollider2D>();
            gateCollider.size = new Vector2(1.0f, 0.8f);
            gateCollider.enabled = !isGateOpen;

            // Gate Lanterns (Left & Right of gate)
            GameObject lanternL = new GameObject("LanternLeft");
            lanternL.transform.SetParent(transform, false);
            lanternL.transform.localPosition = new Vector3(gateXIndex - 0.45f, 0.2f, 0f);
            lanternLeftRenderer = lanternL.AddComponent<SpriteRenderer>();
            lanternLeftRenderer.sprite = PrototypePixelArtFactory.GateLantern(false);
            lanternLeftRenderer.sortingOrder = 23;

            GameObject lanternR = new GameObject("LanternRight");
            lanternR.transform.SetParent(transform, false);
            lanternR.transform.localPosition = new Vector3(gateXIndex + 0.45f, 0.2f, 0f);
            lanternRightRenderer = lanternR.AddComponent<SpriteRenderer>();
            lanternRightRenderer.sprite = PrototypePixelArtFactory.GateLantern(false);
            lanternRightRenderer.sortingOrder = 23;

            // Signboard near gate
            GameObject signObj = new GameObject("FarmSignboard");
            signObj.transform.SetParent(transform, false);
            float signOffset = (gateXIndex + 1 < width - 1) ? 1.2f : -1.2f;
            signObj.transform.localPosition = new Vector3(gateXIndex + signOffset, 0.1f, 0f);
            var signSr = signObj.AddComponent<SpriteRenderer>();
            signSr.sprite = PrototypePixelArtFactory.FarmSignboard();
            signSr.sortingOrder = 21;
        }

        private void Update()
        {
            if (playerTransform == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null) playerTransform = player.transform;
                return;
            }

            // Update Lantern Light at Night
            if (gameTime != null && lanternLeftRenderer != null && lanternRightRenderer != null)
            {
                bool isNight = gameTime.SunlightIntensity < 0.4f;
                lanternLeftRenderer.sprite = PrototypePixelArtFactory.GateLantern(isNight);
                lanternRightRenderer.sprite = PrototypePixelArtFactory.GateLantern(isNight);
            }

            // Gate interaction check
            float dist = Vector2.Distance(playerTransform.position, transform.position + new Vector3(width / 2, 0f, 0f));
            if (gateChild != null)
            {
                dist = Vector2.Distance(playerTransform.position, gateChild.position);
            }

            if (dist <= 2.2f)
            {
                if (PrototypeInput.GetKeyDown(KeyCode.F) || PrototypeInput.GetKeyDown(KeyCode.E))
                {
                    ToggleGate();
                }
            }

            // Check if player enters pasture yard
            if (isGateOpen && !hasGreetedPlayer && playerTransform != null)
            {
                Vector3 local = transform.InverseTransformPoint(playerTransform.position);
                if (local.x > 0.5f && local.x < width - 1.5f && local.y > 0.5f && local.y < height - 1.5f)
                {
                    hasGreetedPlayer = true;
                    AudioManager.PlayUiClick();
                    PlayerSpeechBubble.Say(LocalizationRuntime.IsVietnamese ? $"Chào mừng đến với {farmName}!" : $"Welcome to {farmName}!");
                }
            }
        }

        public void ToggleGate()
        {
            isGateOpen = !isGateOpen;
            if (gateCollider != null) gateCollider.enabled = !isGateOpen;
            if (gateRenderer != null) gateRenderer.sprite = PrototypePixelArtFactory.WoodGate(isGateOpen);

            AudioManager.PlayDoor();

            string statusText = isGateOpen
                ? (LocalizationRuntime.IsVietnamese ? "Cổng Trang Trại: Đã Mở" : "Farm Gate: Opened")
                : (LocalizationRuntime.IsVietnamese ? "Cổng Trang Trại: Đã Đóng" : "Farm Gate: Closed");

            FloatingTextController.Spawn(statusText, gateChild != null ? gateChild.position + Vector3.up * 1f : transform.position + Vector3.up * 1f, new Color(0.95f, 0.85f, 0.4f, 1f));
        }
    }
}
