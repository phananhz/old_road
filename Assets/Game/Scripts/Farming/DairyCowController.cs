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

        public void Configure(InventorySession session)
        {
            this.inventorySession = session;
        }

        private void Start()
        {
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
                    TryMilkCow();
                }
            }
        }

        private void TryMilkCow()
        {
            if (UnityEngine.Time.time < nextMilkTime)
            {
                FloatingTextController.Spawn(LocalizationRuntime.IsVietnamese ? "Bò đang nghỉ ngơi..." : "Cow is resting...", transform.position + Vector3.up * 1.2f, Color.white);
                return;
            }

            nextMilkTime = UnityEngine.Time.time + milkCooldownSeconds;
            AudioManager.PlayChestOpen();

            if (inventorySession != null && inventorySession.Runtime != null)
            {
                inventorySession.Runtime.Add("item.milk", 1);
            }

            ShowHeartEmote();
            FloatingTextController.Spawn("+1 " + (LocalizationRuntime.IsVietnamese ? "Bình sữa tươi" : "Fresh Milk"), transform.position + Vector3.up * 1.2f, new Color(0.9f, 0.95f, 1f, 1f));
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
