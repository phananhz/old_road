using UnityEngine;
using TheOldRoad.Audio;
using TheOldRoad.Combat;
using TheOldRoad.Inventory;
using TheOldRoad.UI;
using TheOldRoad.World;

namespace TheOldRoad.Farming
{
    /// <summary>
    /// Interactive Fluffy Sheep that can be fed hay/wheat and sheared for warm wool.
    /// </summary>
    public sealed class SheepController : MonoBehaviour
    {
        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private float woolCooldownSeconds = 45f;

        private float nextWoolTime;
        private SpriteRenderer sheepRenderer;
        private Transform playerTransform;
        private GameObject heartObj;
        private float heartHideTime;
        private Vector3 spawnPosition;
        private float nextWanderTime;
        private Vector3 wanderTarget;

        public void Configure(InventorySession session)
        {
            this.inventorySession = session;
        }

        private void Start()
        {
            spawnPosition = transform.position;
            wanderTarget = spawnPosition;
            sheepRenderer = GetComponent<SpriteRenderer>();
            if (sheepRenderer == null)
            {
                sheepRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
            sheepRenderer.sprite = PrototypePixelArtFactory.FluffySheep();
            sheepRenderer.sortingOrder = 9;

            BoxCollider2D col = GetComponent<BoxCollider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<BoxCollider2D>();
            }
            col.size = new Vector2(1.5f, 1.0f);
            col.offset = new Vector2(0f, 0.2f);
        }

        private void Update()
        {
            if (heartObj != null && UnityEngine.Time.time >= heartHideTime)
            {
                heartObj.SetActive(false);
            }

            // Gentle natural idle wandering
            if (UnityEngine.Time.time >= nextWanderTime)
            {
                nextWanderTime = UnityEngine.Time.time + Random.Range(4f, 8f);
                Vector2 offset = Random.insideUnitCircle * 1.5f;
                wanderTarget = spawnPosition + new Vector3(offset.x, offset.y, 0f);
            }

            if (Vector3.Distance(transform.position, wanderTarget) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, wanderTarget, UnityEngine.Time.deltaTime * 0.4f);
                if (sheepRenderer != null)
                {
                    sheepRenderer.flipX = (wanderTarget.x - transform.position.x) > 0;
                }
            }

            if (playerTransform == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null) playerTransform = player.transform;
                return;
            }

            if (Vector2.Distance(playerTransform.position, transform.position) <= 2.0f)
            {
                if (TheOldRoad.Input.PrototypeInput.GetKeyDown(KeyCode.F))
                {
                    TryInteractSheep();
                }
            }
        }

        private void TryInteractSheep()
        {
            InventoryRuntime inv = inventorySession != null ? inventorySession.Runtime : null;
            bool hasFeed = inv != null && (inv.GetQuantity("item.hay") > 0 || inv.GetQuantity("item.wheat") > 0);

            // If player has feed and sheep is waiting for wool regrowth, feed to reset cooldown
            if (hasFeed && UnityEngine.Time.time < nextWoolTime)
            {
                string feedItem = inv.GetQuantity("item.hay") > 0 ? "item.hay" : "item.wheat";
                inv.TryRemove(feedItem, 1);
                nextWoolTime = 0f; // Instant reset
                ShowHeartEmote();
                AudioManager.PlayItemPickup();
                FloatingTextController.Spawn(LocalizationRuntime.IsVietnamese ? "Đã cho Cừu ăn cỏ khô! (Sẵn sàng lấy len)" : "Fed hay to Sheep! (Ready to shear)", transform.position + Vector3.up * 1.2f, Color.green);
                return;
            }

            TryShearWool();
        }

        private void TryShearWool()
        {
            if (UnityEngine.Time.time < nextWoolTime)
            {
                FloatingTextController.Spawn(LocalizationRuntime.IsVietnamese ? "Cừu đang mọc lại lông... (Cho ăn cỏ để mọc nhanh)" : "Sheep wool is growing... (Feed hay to refresh)", transform.position + Vector3.up * 1.1f, Color.white);
                return;
            }

            nextWoolTime = UnityEngine.Time.time + woolCooldownSeconds;
            AudioManager.PlayChestOpen();

            int yield = 1;
            if (inventorySession != null && inventorySession.Runtime != null)
            {
                inventorySession.Runtime.Add("item.wool", yield);
            }

            ShowHeartEmote();
            FloatingTextController.Spawn("+" + yield + " " + (LocalizationRuntime.IsVietnamese ? "Len Cừu mềm" : "Soft Wool"), transform.position + Vector3.up * 1.1f, new Color(0.95f, 0.95f, 0.90f, 1f));
        }

        private void ShowHeartEmote()
        {
            if (heartObj == null)
            {
                heartObj = new GameObject("HeartEmote");
                heartObj.transform.SetParent(transform, false);
                heartObj.transform.localPosition = new Vector3(0f, 1.2f, 0f);
                var sr = heartObj.AddComponent<SpriteRenderer>();
                sr.sprite = PrototypePixelArtFactory.HeartEmote();
                sr.sortingOrder = 25;
            }
            heartObj.SetActive(true);
            heartHideTime = UnityEngine.Time.time + 2.5f;
        }
    }
}
