using System;
using System.Collections.Generic;
using UnityEngine;
using TheOldRoad.Inventory;
using TheOldRoad.Audio;
using TheOldRoad.UI;
using TheOldRoad.Time;
using TheOldRoad.Combat;

namespace TheOldRoad.World
{
    /// <summary>
    /// Wandering Mysterious Merchant who appears periodically at secret locations,
    /// rings a caravan bell, broadcasts world notifications, marks map POI, offers dynamic dialogues,
    /// and sells exclusive exotic seeds, saplings, sprinklers, and rare artifacts.
    /// </summary>
    public sealed class WanderingMerchantController : MonoBehaviour
    {
        [Serializable]
        public struct MerchantLocation
        {
            public string locationId;
            public string displayNameEn;
            public string displayNameVi;
            public Vector2 position;
        }

        private static readonly MerchantLocation[] Locations =
        {
            new MerchantLocation { locationId = "lake", displayNameEn = "Valen Lake Shore", displayNameVi = "Bờ Hồ Valen", position = new Vector2(18.0f, -4.5f) },
            new MerchantLocation { locationId = "crossroads", displayNameEn = "Old Crossroads", displayNameVi = "Ngã Ba Con Đường Cổ", position = new Vector2(0.0f, 6.0f) },
            new MerchantLocation { locationId = "bell_shrine", displayNameEn = "Ancient Bell Shrine", displayNameVi = "Di Tích Tháp Chuông Cổ", position = new Vector2(-16.0f, -8.0f) },
            new MerchantLocation { locationId = "forest_gate", displayNameEn = "Pine Forest Gate", displayNameVi = "Cổng Rừng Thông Cổ Thụ", position = new Vector2(-8.0f, 14.0f) }
        };

        [SerializeField] private bool isPresent = true;
        [SerializeField] private int currentLocationIndex = 0;
        [SerializeField] private float remainingStayMinutes = 720f; // 12 in-game hours
        [SerializeField] private float nextArrivalTimerMinutes = 0f;

        private GameTimeController gameTime;
        private WeatherController weather;
        private SpriteRenderer wagonRenderer;
        private DiscoverableLandmark landmarkComponent;
        private float lastMinuteCheck = -1f;
        private bool isPlayerNearby = false;

        public bool IsPresent => isPresent;
        public string CurrentLocationName => LocalizationRuntime.IsVietnamese ? Locations[currentLocationIndex].displayNameVi : Locations[currentLocationIndex].displayNameEn;
        public Vector2 CurrentPosition => Locations[currentLocationIndex].position;
        public float RemainingStayMinutes => remainingStayMinutes;

        public string RemainingTimeFormatted
        {
            get
            {
                if (!isPresent) return string.Empty;
                int hours = Mathf.FloorToInt(remainingStayMinutes / 60f);
                int mins = Mathf.FloorToInt(remainingStayMinutes % 60f);
                return LocalizationRuntime.IsVietnamese ? $"Còn ở lại: {hours}h {mins}p" : $"Departing in: {hours}h {mins}m";
            }
        }

        private void Awake()
        {
            EnsureComponents();
        }

        private void Start()
        {
            gameTime = FindAnyObjectByType<GameTimeController>();
            weather = FindAnyObjectByType<WeatherController>();
            TeleportToLocation(0);
        }

        private void Update()
        {
            if (gameTime == null) gameTime = FindAnyObjectByType<GameTimeController>();
            if (weather == null) weather = FindAnyObjectByType<WeatherController>();

            float currentMinute = gameTime != null ? gameTime.AbsoluteMinute : (UnityEngine.Time.time / 60f);
            if (lastMinuteCheck < 0f) lastMinuteCheck = currentMinute;
            float deltaMinutes = Mathf.Max(0f, currentMinute - lastMinuteCheck);
            lastMinuteCheck = currentMinute;

            if (isPresent)
            {
                remainingStayMinutes -= deltaMinutes;
                if (remainingStayMinutes <= 0f)
                {
                    Depart();
                }
            }
            else
            {
                nextArrivalTimerMinutes -= deltaMinutes;
                if (nextArrivalTimerMinutes <= 0f)
                {
                    ArriveAtRandomLocation();
                }
            }

            // Check player distance for interaction hint
            Camera cam = Camera.main;
            if (cam != null && isPresent)
            {
                float dist = Vector2.Distance(transform.position, cam.transform.position);
                isPlayerNearby = dist <= 3.5f;
            }
        }

        public void ArriveAtRandomLocation()
        {
            int nextLoc = UnityEngine.Random.Range(0, Locations.Length);
            TeleportToLocation(nextLoc);
            isPresent = true;
            remainingStayMinutes = UnityEngine.Random.Range(720f, 1440f); // 12 to 24 hours

            AudioManager.PlayMerchantBell();
            string locName = CurrentLocationName;
            string msg = LocalizationRuntime.IsVietnamese
                ? $"🔔 THƯƠNG NHÂN DU MỤC ĐÃ ĐẾN! Xe hàng đang dừng chân tại {locName}!"
                : $"🔔 THE WANDERING MERCHANT HAS ARRIVED! Caravan parked at {locName}!";

            FloatingTextController.Spawn(msg, transform.position + Vector3.up * 1.5f, new Color(0.95f, 0.75f, 0.25f), 4.0f);
            PlayerSpeechBubble.Say(msg);

            InventoryDebugHud hud = FindAnyObjectByType<InventoryDebugHud>();
            if (hud != null) hud.ShowMessage(msg);
        }

        public void Depart()
        {
            isPresent = false;
            nextArrivalTimerMinutes = UnityEngine.Random.Range(600f, 1200f); // 10 to 20 hours until next visit
            SetVisualsActive(false);

            if (landmarkComponent != null)
            {
                landmarkComponent.gameObject.SetActive(false);
            }

            string msg = LocalizationRuntime.IsVietnamese
                ? "🛒 Thương nhân du mục đã thu dọn xe hàng và lên đường tiếp tục hành trình..."
                : "🛒 The Wandering Merchant has packed up and moved on...";

            InventoryDebugHud hud = FindAnyObjectByType<InventoryDebugHud>();
            if (hud != null) hud.ShowMessage(msg);
        }

        public void TeleportToLocation(int locIndex)
        {
            currentLocationIndex = Mathf.Clamp(locIndex, 0, Locations.Length - 1);
            transform.position = new Vector3(Locations[currentLocationIndex].position.x, Locations[currentLocationIndex].position.y, 0f);
            SetVisualsActive(true);

            if (landmarkComponent != null)
            {
                landmarkComponent.gameObject.SetActive(true);
                landmarkComponent.transform.position = transform.position;
            }
        }

        public string GetContextualDialogue()
        {
            bool isRain = weather != null && (weather.CurrentWeather == WeatherType.LightRain || weather.CurrentWeather == WeatherType.Thunderstorm);
            bool isNight = gameTime != null && (gameTime.Hour >= 20 || gameTime.Hour < 5);

            if (isRain)
            {
                return LocalizationRuntime.IsVietnamese
                    ? "Thời tiết mưa gió thế này đi xe ngựa vất vả thật... Nhưng ta vừa nhập được hạt giống quý từ phương Nam đây, ngươi xem qua chứ?"
                    : "Traveling through this heavy rain is harsh... But I carry rare southern seeds that thrive in moisture, care to browse?";
            }

            if (isNight)
            {
                return LocalizationRuntime.IsVietnamese
                    ? "Đêm khuya thanh vắng thế này mà ngươi vẫn miệt mài ngoài đường sao? Cẩn thận bầy sói đêm đấy, hãy xem qua bùa hộ mệnh của ta!"
                    : "Out wandering so late in the shadows? Beware the wolves, friend. Perhaps my glowing charms might keep you safe!";
            }

            string[] greetingsVi =
            {
                "Chào người lữ khách! Xe hàng du mục của ta luôn mang đến những báu vật độc nhất vô nhị vùng đất Valen!",
                "Ta ngửi thấy mùi hương nông sản tươi tốt từ trang trại của ngươi! Ta sẵn sàng mua lại với giá cực kỳ hào phóng đấy!",
                "Hàng hóa của ta chỉ dừng lại trong hôm nay thôi, ngày mai ta lại lên đường băng qua thung lũng rồi!"
            };

            string[] greetingsEn =
            {
                "Greetings traveler! My nomadic caravan carries rare treasures you won't find in ordinary village stalls!",
                "The sweet aroma of fresh harvest lingers around you! I pay top coin for quality goods today!",
                "Browse freely! My caravan only stays for today before we head across the mountain pass!"
            };

            int pick = UnityEngine.Random.Range(0, greetingsVi.Length);
            return LocalizationRuntime.IsVietnamese ? greetingsVi[pick] : greetingsEn[pick];
        }

        private void EnsureComponents()
        {
            if (wagonRenderer == null)
            {
                wagonRenderer = GetComponent<SpriteRenderer>();
                if (wagonRenderer == null) wagonRenderer = gameObject.AddComponent<SpriteRenderer>();
                wagonRenderer.sprite = PrototypePixelArtFactory.TravelCartIcon();
                wagonRenderer.sortingOrder = 10;
            }

            if (landmarkComponent == null)
            {
                landmarkComponent = GetComponent<DiscoverableLandmark>();
                if (landmarkComponent == null) landmarkComponent = gameObject.AddComponent<DiscoverableLandmark>();
                landmarkComponent.Configure(
                    "landmark.wandering_merchant",
                    "🛒✨ Thương Nhân Du Mục (Wandering Merchant)",
                    "Xe hàng du mục bí ẩn mang đến nhiều hạt giống quý hiếm ngoại lai và cổ vật độc quyền.",
                    true,
                    "🛒",
                    new Color(0.85f, 0.45f, 0.95f, 1f));
                landmarkComponent.SetDiscovered(true);
            }
        }

        private void SetVisualsActive(bool active)
        {
            EnsureComponents();
            wagonRenderer.enabled = active;
            if (landmarkComponent != null) landmarkComponent.enabled = active;
        }

        private void OnGUI()
        {
            if (!isPresent || !isPlayerNearby) return;

            Camera cam = Camera.main;
            if (cam == null) return;

            Vector3 screenPos = cam.WorldToScreenPoint(transform.position + Vector3.up * 1.6f);
            if (screenPos.z < 0.1f) return;

            float guiY = Screen.height - screenPos.y;
            Rect box = new Rect(screenPos.x - 130f, guiY - 32f, 260f, 36f);

            DrawGuiRect(box, new Color(0.18f, 0.08f, 0.24f, 0.94f));
            DrawGuiBorder(box, new Color(0.95f, 0.75f, 0.25f, 1f), 1.5f);

            UiFontHelper.EnsureGlobalSkinFont();
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                font = UiFontHelper.CleanFont,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            titleStyle.normal.textColor = new Color(1f, 0.90f, 0.35f);

            GUIStyle timerStyle = new GUIStyle(GUI.skin.label)
            {
                font = UiFontHelper.CleanFont,
                fontSize = 11,
                alignment = TextAnchor.MiddleCenter
            };
            timerStyle.normal.textColor = new Color(0.85f, 0.85f, 0.85f);

            GUI.Label(new Rect(box.x + 4f, box.y + 2f, box.width - 8f, 16f), "🛒 " + (LocalizationRuntime.IsVietnamese ? "Thương Nhân Du Mục" : "Wandering Merchant"), titleStyle);
            GUI.Label(new Rect(box.x + 4f, box.y + 18f, box.width - 8f, 14f), RemainingTimeFormatted, timerStyle);
        }

        private static Texture2D pixelTexture;
        private static void DrawGuiRect(Rect r, Color c)
        {
            if (pixelTexture == null)
            {
                pixelTexture = new Texture2D(1, 1);
                pixelTexture.SetPixel(0, 0, Color.white);
                pixelTexture.Apply();
            }
            Color prev = GUI.color;
            GUI.color = c;
            GUI.DrawTexture(r, pixelTexture);
            GUI.color = prev;
        }

        private static void DrawGuiBorder(Rect r, Color c, float thickness)
        {
            DrawGuiRect(new Rect(r.x, r.y, r.width, thickness), c);
            DrawGuiRect(new Rect(r.x, r.yMax - thickness, r.width, thickness), c);
            DrawGuiRect(new Rect(r.x, r.y, thickness, r.height), c);
            DrawGuiRect(new Rect(r.xMax - thickness, r.y, thickness, r.height), c);
        }
    }
}
