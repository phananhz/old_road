using UnityEngine;
using TheOldRoad.Audio;
using TheOldRoad.Combat;
using TheOldRoad.Inventory;
using TheOldRoad.UI;
using TheOldRoad.World;

namespace TheOldRoad.Farming
{
    /// <summary>
    /// Interactive Spotted Dairy Cow that can be fed and milked.
    /// </summary>
    public sealed class DairyCowController : MonoBehaviour
    {
        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private float milkCooldownSeconds = 40f;

        private float nextMilkTime;
        private SpriteRenderer cowRenderer;
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
            cowRenderer = GetComponent<SpriteRenderer>();
            if (cowRenderer == null)
            {
                cowRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
            cowRenderer.sprite = PrototypePixelArtFactory.DairyCow();
            cowRenderer.sortingOrder = 9;

            BoxCollider2D col = GetComponent<BoxCollider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<BoxCollider2D>();
            }
            col.size = new Vector2(1.8f, 1.2f);
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
                nextWanderTime = UnityEngine.Time.time + Random.Range(5f, 10f);
                Vector2 offset = Random.insideUnitCircle * 2.0f;
                wanderTarget = spawnPosition + new Vector3(offset.x, offset.y, 0f);
            }

            if (Vector3.Distance(transform.position, wanderTarget) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, wanderTarget, UnityEngine.Time.deltaTime * 0.35f);
                if (cowRenderer != null)
                {
                    cowRenderer.flipX = (wanderTarget.x - transform.position.x) > 0;
                }
            }

            if (playerTransform == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null) playerTransform = player.transform;
                return;
            }

            if (Vector2.Distance(playerTransform.position, transform.position) <= 2.2f)
            {
                if (TheOldRoad.Input.PrototypeInput.GetKeyDown(KeyCode.F))
                {
                    TryInteractCow();
                }
            }
        }

        private void TryInteractCow()
        {
            InventoryRuntime inv = inventorySession != null ? inventorySession.Runtime : null;
            bool hasFeed = inv != null && (inv.GetQuantity("item.hay") > 0 || inv.GetQuantity("item.wheat") > 0);

            // If player has feed and cow is resting, feed the cow
            if (hasFeed && UnityEngine.Time.time < nextMilkTime)
            {
                string feedItem = inv.GetQuantity("item.hay") > 0 ? "item.hay" : "item.wheat";
                inv.TryRemove(feedItem, 1);
                nextMilkTime = 0f; // Instant reset
                ShowHeartEmote();
                AudioManager.PlayItemPickup();
                FloatingTextController.Spawn(LocalizationRuntime.IsVietnamese ? "Đã cho Bò ăn cỏ thơm! (Sẵn sàng vắt sữa)" : "Fed hay to Cow! (Ready to milk)", transform.position + Vector3.up * 1.3f, Color.green);
                return;
            }

            TryMilkCow();
        }

        private void TryMilkCow()
        {
            if (UnityEngine.Time.time < nextMilkTime)
            {
                FloatingTextController.Spawn(LocalizationRuntime.IsVietnamese ? "Bò đang nghỉ ngơi... (Cho ăn cỏ khô để vắt tiếp)" : "Cow is resting... (Feed hay to refresh)", transform.position + Vector3.up * 1.2f, Color.white);
                return;
            }

            nextMilkTime = UnityEngine.Time.time + milkCooldownSeconds;
            AudioManager.PlayChestOpen();

            int yield = 1;
            if (inventorySession != null && inventorySession.Runtime != null)
            {
                inventorySession.Runtime.Add("item.milk", yield);
            }

            ShowHeartEmote();
            FloatingTextController.Spawn("+" + yield + " " + (LocalizationRuntime.IsVietnamese ? "Bình sữa tươi" : "Fresh Milk"), transform.position + Vector3.up * 1.2f, new Color(0.9f, 0.95f, 1f, 1f));
        }

        private void ShowHeartEmote()
        {
            if (heartObj == null)
            {
                heartObj = new GameObject("HeartEmote");
                heartObj.transform.SetParent(transform, false);
                heartObj.transform.localPosition = new Vector3(0f, 1.3f, 0f);
                var sr = heartObj.AddComponent<SpriteRenderer>();
                sr.sprite = PrototypePixelArtFactory.HeartEmote();
                sr.sortingOrder = 25;
            }
            heartObj.SetActive(true);
            heartHideTime = UnityEngine.Time.time + 2.5f;
        }
    }
}
