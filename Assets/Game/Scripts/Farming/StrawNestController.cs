using UnityEngine;
using TheOldRoad.Audio;
using TheOldRoad.Combat;
using TheOldRoad.Inventory;
using TheOldRoad.UI;
using TheOldRoad.World;

namespace TheOldRoad.Farming
{
    /// <summary>
    /// Interactive Straw Nest that produces farm fresh eggs on the grass.
    /// </summary>
    public sealed class StrawNestController : MonoBehaviour
    {
        [SerializeField] private InventorySession inventorySession;
        [SerializeField] private bool hasEgg = true;
        [SerializeField] private float eggRespawnSeconds = 35f;

        private float nextEggTime;
        private SpriteRenderer nestRenderer;
        private Transform playerTransform;

        public void Configure(InventorySession session)
        {
            this.inventorySession = session;
        }

        private void Start()
        {
            nestRenderer = GetComponent<SpriteRenderer>();
            if (nestRenderer == null)
            {
                nestRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
            nestRenderer.sortingOrder = 8;
            UpdateVisual();
        }

        private void Update()
        {
            if (!hasEgg && UnityEngine.Time.time >= nextEggTime)
            {
                hasEgg = true;
                UpdateVisual();
            }

            if (playerTransform == null)
            {
                var player = GameObject.FindWithTag("Player");
                if (player != null) playerTransform = player.transform;
                return;
            }

            if (hasEgg && Vector2.Distance(playerTransform.position, transform.position) <= 1.8f)
            {
                if (TheOldRoad.Input.PrototypeInput.GetKeyDown(KeyCode.F))
                {
                    CollectEgg();
                }
            }
        }

        private void CollectEgg()
        {
            hasEgg = false;
            nextEggTime = UnityEngine.Time.time + eggRespawnSeconds;
            UpdateVisual();

            AudioManager.PlayChestOpen();

            if (inventorySession != null && inventorySession.Runtime != null)
            {
                inventorySession.Runtime.Add("item.egg", 1);
            }

            FloatingTextController.Spawn("+1 " + (LocalizationRuntime.IsVietnamese ? "Trứng gà tươi" : "Fresh Egg"), transform.position + Vector3.up * 0.8f, new Color(1f, 0.92f, 0.45f, 1f));
        }

        private void UpdateVisual()
        {
            if (nestRenderer != null)
            {
                nestRenderer.sprite = PrototypePixelArtFactory.StrawNest(hasEgg);
            }
        }
    }
}
