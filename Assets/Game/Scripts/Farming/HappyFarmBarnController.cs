using UnityEngine;
using TheOldRoad.Audio;
using TheOldRoad.Combat;
using TheOldRoad.Inventory;
using TheOldRoad.UI;
using TheOldRoad.World;

namespace TheOldRoad.Farming
{
    /// <summary>
    /// Interactive Happy Farm Barn with open corrugated roof, hay bale storage, and tool room entrance.
    /// </summary>
    public sealed class HappyFarmBarnController : MonoBehaviour
    {
        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private float interactRadius = 2.2f;

        private Transform playerTransform;
        private SpriteRenderer barnRenderer;
        private bool isNearDoor;
        private bool isNearHay;

        public void Configure(InventorySession session)
        {
            this.inventorySession = session;
        }

        private void Start()
        {
            barnRenderer = GetComponent<SpriteRenderer>();
            if (barnRenderer == null)
            {
                barnRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
            barnRenderer.sprite = PrototypePixelArtFactory.HappyFarmBarn();
            barnRenderer.sortingOrder = 15;

            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<BoxCollider2D>();
            }
            collider.size = new Vector2(4.2f, 1.8f);
            collider.offset = new Vector2(0f, 0.4f);
        }

        private void Update()
        {
            if (playerTransform == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null) playerTransform = player.transform;
                return;
            }

            Vector3 doorPos = transform.position + new Vector3(1.2f, -0.6f, 0f);
            Vector3 hayPos = transform.position + new Vector3(-1.2f, -0.6f, 0f);

            float distDoor = Vector2.Distance(playerTransform.position, doorPos);
            float distHay = Vector2.Distance(playerTransform.position, hayPos);

            isNearDoor = distDoor <= interactRadius;
            isNearHay = distHay <= interactRadius;

            if (TheOldRoad.Input.PrototypeInput.GetKeyDown(KeyCode.F))
            {
                if (isNearDoor)
                {
                    AudioManager.PlayDoor();
                    InventoryDebugHud hud = FindAnyObjectByType<InventoryDebugHud>();
                    if (hud != null)
                    {
                        hud.ToggleInventoryOverlay();
                    }
                    FloatingTextController.Spawn(LocalizationRuntime.IsVietnamese ? "Kho Nông Trại" : "Farm Storage", doorPos + Vector3.up * 1.2f, new Color(1f, 0.85f, 0.3f, 1f));
                }
                else if (isNearHay)
                {
                    AudioManager.PlayForage();
                    if (inventorySession != null && inventorySession.Runtime != null)
                    {
                        inventorySession.Runtime.Add("item.wheat", 1);
                        FloatingTextController.Spawn("+1 " + (LocalizationRuntime.IsVietnamese ? "Rơm rạ / Lúa mì" : "Wheat / Hay"), hayPos + Vector3.up * 1.2f, new Color(0.95f, 0.85f, 0.35f, 1f));
                    }
                }
            }
        }
    }
}
