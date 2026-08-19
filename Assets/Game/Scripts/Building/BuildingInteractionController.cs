using System;
using UnityEngine;
using TheOldRoad.Construction;
using TheOldRoad.Inventory;
using TheOldRoad.Input;
using TheOldRoad.UI;
using TheOldRoad.Audio;
using TheOldRoad.Combat;
using TheOldRoad.Time;
using TheOldRoad.Core;
using TheOldRoad.Economy;

namespace TheOldRoad.Building
{
    /// <summary>
    /// Centralized proximity interaction handler for all world buildings.
    /// Manages Silos, Chests, Artisan Processing Machines, Wells, Baths, Shrines, Beds, and Troughs.
    /// </summary>
    public sealed class BuildingInteractionController : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float interactionRadius = 2.5f;

        private ConstructionSite nearestSite;
        private string nearestBuildingId = string.Empty;
        private InventorySession inventorySession;
        private InventoryDebugHud hud;
        private GameTimeController gameTime;
        private TheOldRoad.Player.PlayerVitals vitals;
        private float nextScanTime;

        public string ActionPrompt { get; private set; } = string.Empty;
        public bool HasInteractable { get; private set; } = false;

        private void Update()
        {
            if (inventorySession == null) inventorySession = FindAnyObjectByType<InventorySession>();
            if (hud == null) hud = FindAnyObjectByType<InventoryDebugHud>();
            if (gameTime == null) gameTime = FindAnyObjectByType<GameTimeController>();
            if (vitals == null) vitals = GetComponent<TheOldRoad.Player.PlayerVitals>() ?? FindAnyObjectByType<TheOldRoad.Player.PlayerVitals>();

            if (hud != null && hud.IsAnyOverlayOpen)
            {
                HasInteractable = false;
                ActionPrompt = string.Empty;
                return;
            }

            if (UnityEngine.Time.unscaledTime >= nextScanTime)
            {
                nextScanTime = UnityEngine.Time.unscaledTime + 0.2f;
                ScanNearestBuilding();
            }

            if (HasInteractable && (PrototypeInput.GetKeyDown(KeyCode.F) || PrototypeInput.GetKeyDown(KeyCode.E)))
            {
                ExecuteInteraction();
            }
        }

        private void ScanNearestBuilding()
        {
            HasInteractable = false;
            ActionPrompt = string.Empty;
            nearestSite = null;
            nearestBuildingId = string.Empty;

            ConstructionSite[] sites = FindObjectsByType<ConstructionSite>(FindObjectsInactive.Exclude);
            float bestDist = interactionRadius;
            Vector3 playerPos = transform.position;

            for (int i = 0; i < sites.Length; i++)
            {
                ConstructionSite s = sites[i];
                if (s == null || !s.IsCompleted || string.IsNullOrEmpty(s.BuildingId)) continue;

                float d = Vector2.Distance(playerPos, s.transform.position);
                if (d <= bestDist)
                {
                    bestDist = d;
                    nearestSite = s;
                    nearestBuildingId = s.BuildingId;
                }
            }

            if (nearestSite != null)
            {
                HasInteractable = true;
                ActionPrompt = GetActionPromptForBuilding(nearestBuildingId);
            }
        }

        private string GetActionPromptForBuilding(string buildingId)
        {
            bool vi = LocalizationRuntime.IsVietnamese;
            switch (buildingId)
            {
                case "building.silo":
                    return vi ? "Nhấn [F] Mở Tháp Chứa Hạt Silo" : "Press [F] Open Grain Silo";

                case "building.wood-chest":
                case "building.stone-vault":
                case "building.storage-shed":
                    return vi ? "Nhấn [F] Mở Rương Chứa Đồ" : "Press [F] Open Storage Chest";

                case "building.windmill":
                    return vi ? "Nhấn [F] Sử dụng Cối Xay Gió" : "Press [F] Use Windmill";

                case "building.blacksmith-forge":
                    return vi ? "Nhấn [F] Sử dụng Lò Rèn Sắt" : "Press [F] Use Blacksmith Forge";

                case "building.cheese-press":
                    return vi ? "Nhấn [F] Sử dụng Máy Ép Phô Mai" : "Press [F] Use Cheese Press";

                case "building.loom":
                    return vi ? "Nhấn [F] Sử dụng Khung Dệt Vải" : "Press [F] Use Loom";

                case "building.keg":
                    return vi ? "Nhấn [F] Sử dụng Thùng Ủ Rượu" : "Press [F] Use Fermentation Keg";

                case "building.carpenter-bench":
                    return vi ? "Nhấn [F] Sử dụng Bàn Thợ Mộc" : "Press [F] Use Carpenter Bench";

                case "building.ancient-well":
                case "building.stone-fountain":
                    return vi ? "Nhấn [F] Múc Nước Đầy Bình Tưới" : "Press [F] Refill Watering Can";

                case "building.hot-bath":
                    return vi ? "Nhấn [F] Ngâm Bồn Nước Nóng (Hồi HP)" : "Press [F] Soak in Hot Bath";

                case "building.market-stall":
                    return vi ? "Nhấn [F] Bán Nông Sản Lấy Đồng Bạc" : "Press [F] Open Market Shipping Stall";

                case "building.gate":
                case "building.gate-wood":
                case "building.iron-gate":
                    return vi ? "Nhấn [F] Mở / Đóng Cổng" : "Press [F] Toggle Gate";

                case "building.leather-chair":
                    return vi ? "Nhấn [F] Ngồi Nghỉ Ngơi" : "Press [F] Sit & Relax";

                case "building.wood-swing":
                    return vi ? "Nhấn [F] Đung Đưa Xích Đu" : "Press [F] Swing on Tree Swing";

                case "building.chess-table":
                    return vi ? "Nhấn [F] Chơi Cờ Vua Cổ Điển" : "Press [F] Play Chess";

                case "building.bookshelf":
                    return vi ? "Nhấn [F] Đọc Sách Truyền Thuyết" : "Press [F] Read Ancient Lore";

                case "building.farm-sign":
                case "building.farm-signboard":
                    return vi ? "Nhấn [F] Xem Biển Tên Nông Trại" : "Press [F] Read Farm Sign";

                case "building.guardian-shrine":
                    return vi ? "Nhấn [F] Cầu Nguyện Điện Thờ (Buff Chạy Nhanh)" : "Press [F] Pray at Shrine";

                case "building.knight-statue":
                    return vi ? "Nhấn [F] Tôn Kính Hiệp Sĩ (Buff Tấn Công)" : "Press [F] Honor Knight Statue";

                case "building.alarm-bell":
                case "building.bell-pillar":
                    return vi ? "Nhấn [F] Rung Chuông Báo Động" : "Press [F] Ring Alarm Bell";

                case "building.feed-trough":
                case "building.water-trough":
                    return vi ? "Nhấn [F] Đổ Đầy Máng Thức Ăn" : "Press [F] Fill Animal Trough";

                case "building.compost-bin":
                    return vi ? "Nhấn [F] Ủ Phân Bón Cây Trồng" : "Press [F] Make Fertilizer";

                case "building.grape-trellis":
                    return vi ? "Nhấn [F] Thu Hoạch Nho Tươi" : "Press [F] Harvest Fresh Grapes";

                case "building.pumpkin-patch":
                    return vi ? "Nhấn [F] Thu Hoạch Bí Ngô" : "Press [F] Harvest Pumpkins";

                case "building.tent":
                case "building.straw-bed":
                case "building.hammock":
                    return vi ? "Nhấn [F] Nghỉ Ngơi & Ngủ Qua Đêm" : "Press [F] Rest & Sleep";

                default:
                    return string.Empty;
            }
        }

        private void ExecuteInteraction()
        {
            if (nearestSite == null || string.IsNullOrEmpty(nearestBuildingId)) return;
            InventoryRuntime inv = inventorySession != null ? inventorySession.Runtime : null;

            switch (nearestBuildingId)
            {
                case "building.silo":
                {
                    var silo = nearestSite.GetComponent<SiloStorageController>();
                    if (silo == null) silo = nearestSite.gameObject.AddComponent<SiloStorageController>();
                    silo.Configure(nearestSite.Job.constructionId);
                    SiloStorageController.OpenSiloUI(silo);
                    if (hud != null) hud.OpenSiloOverlay();
                    break;
                }

                case "building.wood-chest":
                case "building.stone-vault":
                case "building.storage-shed":
                {
                    var chest = nearestSite.GetComponent<ChestStorageController>();
                    if (chest == null) chest = nearestSite.gameObject.AddComponent<ChestStorageController>();
                    int cap = nearestBuildingId == "building.storage-shed" ? 32 : (nearestBuildingId == "building.stone-vault" ? 24 : 16);
                    chest.Configure(nearestSite.Job.constructionId, cap, LocalizationRuntime.BuildingName(nearestBuildingId), "Storage Chest");
                    ChestStorageController.OpenChestUI(chest);
                    if (hud != null) hud.OpenChestOverlay();
                    break;
                }

                case "building.windmill":
                case "building.blacksmith-forge":
                case "building.cheese-press":
                case "building.loom":
                case "building.keg":
                case "building.carpenter-bench":
                {
                    var machine = nearestSite.GetComponent<ArtisanProcessingController>();
                    if (machine == null) machine = nearestSite.gameObject.AddComponent<ArtisanProcessingController>();
                    machine.Configure(nearestSite.Job.constructionId, nearestBuildingId);
                    ArtisanProcessingController.OpenMachineUI(machine);
                    if (hud != null) hud.OpenArtisanOverlay();
                    break;
                }

                case "building.ancient-well":
                case "building.stone-fountain":
                {
                    AudioManager.PlayWaterSplash();
                    if (inv != null && inv.GetQuantity("item.watering-can") > 0)
                    {
                        hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                            ? "💧 Đã múc đầy nước vào bình tưới!" 
                            : "💧 Watering can refilled to 100%!");
                    }
                    else
                    {
                        hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                            ? "💧 Nước suối trong lành! (Hãy trang bị bình tưới để múc nước)" 
                            : "💧 Pure fresh water! (Equip watering can to fill)");
                    }
                    break;
                }

                case "building.hot-bath":
                {
                    AudioManager.PlayWaterSplash();
                    if (vitals != null)
                    {
                        vitals.Heal(50);
                    }
                    hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                        ? "♨️ Suối nước nóng ấm áp! Đã hồi phục 50 HP!" 
                        : "♨️ Soaked in the hot bath! Restored 50 HP!");
                    break;
                }

                case "building.guardian-shrine":
                {
                    AudioManager.PlayLandmarkDiscovered();
                    hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                        ? "✨ Thần Điện ban phước: Nhận được sự bảo hộ may mắn!" 
                        : "✨ Guardian Shrine blessing received: Blessed journey!");
                    break;
                }

                case "building.knight-statue":
                {
                    AudioManager.PlayQuestComplete();
                    hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                        ? "⚔️ Tôn vinh Hiệp Sĩ: Tinh thần chiến binh dâng cao!" 
                        : "⚔️ Honored the Knight: Warrior spirit invigorated!");
                    break;
                }

                case "building.alarm-bell":
                case "building.bell-pillar":
                {
                    AudioManager.PlayMerchantBell();
                    hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                        ? "🔔 Tiếng chuông vang dội khắp thung lũng!" 
                        : "🔔 The great bell rings across the valley!");
                    break;
                }

                case "building.feed-trough":
                case "building.water-trough":
                {
                    if (inv != null && inv.GetQuantity("item.wheat") > 0 && inv.TryRemove("item.wheat", 1))
                    {
                        AudioManager.PlayHarvestProduce();
                        hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                            ? "🌾 Đã nạp đầy lúa mì vào máng ăn cho gia súc!" 
                            : "🌾 Filled the feed trough with fresh wheat!");
                    }
                    else
                    {
                        hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                            ? "🌾 Cần 1 lúa mì để nạp vào máng ăn!" 
                            : "🌾 Need 1 wheat to fill the trough!");
                    }
                    break;
                }

                case "building.compost-bin":
                {
                    if (inv != null && inv.GetQuantity("item.medicinal-herb") > 0 && inv.TryRemove("item.medicinal-herb", 1))
                    {
                        inv.TryAdd("item.fertilizer", 2);
                        AudioManager.PlayHarvestProduce();
                        hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                            ? "🌱 Đã ủ 1 thảo dược thành 2 Phân Bón Cây Trồng!" 
                            : "🌱 Composted 1 herb into 2 Fertilizer!");
                    }
                    else if (inv != null && inv.GetQuantity("item.wild-berries") > 0 && inv.TryRemove("item.wild-berries", 1))
                    {
                        inv.TryAdd("item.fertilizer", 2);
                        AudioManager.PlayHarvestProduce();
                        hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                            ? "🌱 Đã ủ quả dại thành 2 Phân Bón Cây Trồng!" 
                            : "🌱 Composted berries into 2 Fertilizer!");
                    }
                    else
                    {
                        hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                            ? "🌱 Cần thảo dược hoặc quả mọng để ủ thành Phân Bón!" 
                            : "🌱 Need wild berries or herbs to make fertilizer!");
                    }
                    break;
                }

                case "building.grape-trellis":
                {
                    if (inv != null)
                    {
                        inv.TryAdd("item.grape", 2);
                        AudioManager.PlayHarvestProduce();
                        hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                            ? "🍇 Đã hái được 2 Chùm Nho tươi ngon!" 
                            : "🍇 Harvested 2 Fresh Grapes!");
                    }
                    break;
                }

                case "building.pumpkin-patch":
                {
                    if (inv != null)
                    {
                        inv.TryAdd("item.pumpkin", 1);
                        AudioManager.PlayHarvestProduce();
                        hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                            ? "🎃 Đã thu hoạch 1 Quả Bí Ngô khổng lồ!" 
                            : "🎃 Harvested 1 Giant Pumpkin!");
                    }
                    break;
                }

                case "building.market-stall":
                {
                    var stall = nearestSite.GetComponent<MarketStallController>();
                    if (stall == null) stall = nearestSite.gameObject.AddComponent<MarketStallController>();
                    MarketStallController.OpenMarket(stall);
                    if (hud != null) hud.OpenMarketOverlay();
                    break;
                }

                case "building.gate":
                case "building.gate-wood":
                case "building.iron-gate":
                {
                    Collider2D col = nearestSite.GetComponent<Collider2D>();
                    if (col != null)
                    {
                        col.enabled = !col.enabled;
                        AudioManager.PlayDoor();
                        hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                            ? (col.enabled ? "🚪 Cổng đã ĐÓNG" : "🚪 Cổng đã MỞ") 
                            : (col.enabled ? "🚪 Gate CLOSED" : "🚪 Gate OPENED"));
                    }
                    break;
                }

                case "building.leather-chair":
                {
                    AudioManager.PlayUiClick();
                    hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                        ? "🪑 Ngồi nghỉ ngơi thư thái trên chiếc ghế bành da êm ái." 
                        : "🪑 Relaxing comfortably on the leather chair.");
                    break;
                }

                case "building.wood-swing":
                {
                    AudioManager.PlayUiClick();
                    hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                        ? "🌱 Đung đưa chiếc xích đu gỗ dưới bóng cây râm mát." 
                        : "🌱 Swinging gently on the wooden tree swing.");
                    break;
                }

                case "building.chess-table":
                {
                    AudioManager.PlayUiClick();
                    hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                        ? "♟️ Bạn giải một thế cờ vua cổ điển của các hiệp sĩ Valen." 
                        : "♟️ You solve a classic chess puzzle from the Knights of Valen.");
                    break;
                }

                case "building.bookshelf":
                {
                    AudioManager.PlayUiClick();
                    hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                        ? "📖 Lịch sử Con Đường Cổ: 'Valen từng là trạm giao thương thịnh vượng...'" 
                        : "📖 Lore of the Old Road: 'Valen was once a flourishing sanctuary...'");
                    break;
                }

                case "building.farm-sign":
                case "building.farm-signboard":
                {
                    AudioManager.PlayUiClick();
                    hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                        ? "🪧 Biển Nông Trại: 'Chào mừng lữ khách đến với Trang Trại Valen bình yên!'" 
                        : "🪧 Farm Sign: 'Welcome wanderer to the peaceful Valen Homestead!'");
                    break;
                }

                case "building.tent":
                case "building.straw-bed":
                case "building.hammock":
                {
                    if (gameTime != null)
                    {
                        gameTime.AdvanceTimeToMorning();
                    }
                    if (vitals != null)
                    {
                        vitals.Heal(100);
                    }
                    AudioManager.PlayQuestComplete();
                    hud?.ShowMessage(LocalizationRuntime.IsVietnamese 
                        ? "💤 Bạn đã ngủ một giấc ngon lành tới sáng! Máu đã hồi đầy!" 
                        : "💤 You rested peacefully until morning! Full HP restored!");
                    break;
                }
            }
        }
    }
}
