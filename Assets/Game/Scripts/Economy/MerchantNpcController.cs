using UnityEngine;
using TheOldRoad.World;
using TheOldRoad.UI;
using TheOldRoad.Input;

namespace TheOldRoad.Economy
{
    /// <summary>
    /// Travelling Merchant Eldon that sells seeds, tools, maps, and buys farm/crafting goods.
    /// </summary>
    public sealed class MerchantNpcController : MonoBehaviour
    {
        [SerializeField] private string merchantName = "Eldon";
        [SerializeField] private Vector3 cartPosition = new Vector3(4.5f, 2.5f, 0f);

        private GameObject cartObj;
        public string MerchantName => merchantName;
        public string InteractionHint { get; private set; } = string.Empty;

        private void Start()
        {
            // Spawn Merchant Wagon Cart
            cartObj = new GameObject("MerchantCart");
            cartObj.transform.SetParent(transform, false);
            cartObj.transform.position = cartPosition;
            SpriteRenderer cr = cartObj.AddComponent<SpriteRenderer>();
            cr.sprite = PrototypePixelArtFactory.MerchantCart();
            cr.sortingOrder = 40;
        }

        private void Update()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) return;

            float distance = Vector2.Distance(player.transform.position, cartPosition);
            if (distance <= 2.2f)
            {
                InteractionHint = LocalizationRuntime.IsVietnamese ? "[F] Giao dịch với Thương nhân" : "[F] Trade with Merchant";
                if (PrototypeInput.GetKeyDown(KeyCode.F) || PrototypeInput.GetKeyDown(KeyCode.E))
                {
                    InventoryDebugHud hud = FindAnyObjectByType<InventoryDebugHud>();
                    if (hud != null)
                    {
                        hud.ToggleMerchantOverlay();
                    }
                }
            }
            else
            {
                InteractionHint = string.Empty;
            }
        }
    }
}
