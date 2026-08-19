using UnityEngine;
using TheOldRoad.Core;

namespace TheOldRoad.UI
{
    /// <summary>Runtime title screen and settings overlay for the current prototype.</summary>
    public sealed class GameStartMenuController : MonoBehaviour
    {
        private static GameStartMenuController instance;

        private readonly Color ink = new Color(0.035f, 0.028f, 0.022f, 0.96f);
        private readonly Color gold = new Color(0.95f, 0.72f, 0.32f, 1f);
        private readonly Color parchment = new Color(0.86f, 0.78f, 0.60f, 1f);
        private readonly Color red = new Color(0.36f, 0.08f, 0.07f, 1f);

        private Texture2D pixel;
        private GUIStyle titleStyle;
        private GUIStyle subtitleStyle;
        private GUIStyle labelStyle;
        private GUIStyle smallStyle;
        private GUIStyle buttonStyle;
        private GUIStyle selectedButtonStyle;
        private bool hasStarted;
        private bool settingsOpen;
        private bool guideOpen;
        private int guideTab;
        private bool pausedByMenu;
        private float previousTimeScale = 1f;

        public static bool IsOpen => instance != null && (!instance.hasStarted || instance.settingsOpen || instance.guideOpen);

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            GameSettingsRuntime.LoadAndApply();
            PauseForMenu();
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
                PrototypeGameState.GameplayInputBlocked = false;
                if (Mathf.Approximately(UnityEngine.Time.timeScale, 0f)) UnityEngine.Time.timeScale = 1f;
            }
        }

        public static void OpenSettingsFromGame()
        {
            if (instance == null) return;
            instance.settingsOpen = true;
            instance.guideOpen = false;
            instance.PauseForMenu();
        }

        public static void OpenGuideFromGame(int tab = 0)
        {
            if (instance == null) return;
            instance.guideOpen = true;
            instance.guideTab = Mathf.Clamp(tab, 0, 4);
            instance.settingsOpen = false;
            instance.PauseForMenu();
        }

        private void Update()
        {
            GameSettingsRuntime.Tick();
            if (hasStarted && !settingsOpen && !guideOpen && SafeEscapeDown())
            {
                OpenSettingsFromGame();
            }
        }

        private void OnGUI()
        {
            EnsureStyles();
            if (!IsOpen) return;

            DrawBackdrop();
            if (guideOpen) DrawGuidePanel();
            else if (settingsOpen) DrawSettingsPanel();
            else DrawTitlePanel();
        }

        private void DrawTitlePanel()
        {
            // Positioned elegantly in center-right to showcase the knight on left and castle on right
            float panelWidth = Mathf.Min(560f, Screen.width - 40f);
            float panelHeight = Mathf.Min(470f, Screen.height - 40f);
            float panelX = (Screen.width - panelWidth) * 0.5f;
            float panelY = (Screen.height - panelHeight) * 0.5f;
            Rect panel = new Rect(panelX, panelY, panelWidth, panelHeight);

            DrawPanel(panel);

            // Title Banner
            GUI.Label(new Rect(panel.x + 20f, panel.y + 20f, panel.width - 40f, 54f), "THE OLD ROAD", titleStyle);
            
            // Subtitle
            GUI.Label(
                new Rect(panel.x + 36f, panel.y + 74f, panel.width - 72f, 60f),
                LocalizationRuntime.T("title.subtitle"),
                subtitleStyle);

            DrawDivider(new Rect(panel.x + 48f, panel.y + 144f, panel.width - 96f, 2f));

            // Ornate Action Buttons
            float btnW = Mathf.Min(280f, panel.width - 80f);
            float btnX = panel.x + (panel.width - btnW) * 0.5f;

            if (GUI.Button(new Rect(btnX, panel.y + 162f, btnW, 46f), "⚔  " + LocalizationRuntime.T("start") + "  ⚔", buttonStyle))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                hasStarted = true;
                settingsOpen = false;
                guideOpen = false;
                ResumeGameplay();
            }

            if (GUI.Button(new Rect(btnX, panel.y + 218f, btnW, 42f), "📖  " + LocalizationRuntime.T("guide_short"), buttonStyle))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                guideOpen = true;
            }

            if (GUI.Button(new Rect(btnX, panel.y + 270f, btnW, 42f), "⚙  " + LocalizationRuntime.T("settings"), buttonStyle))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                settingsOpen = true;
            }

            if (GUI.Button(new Rect(btnX, panel.y + 322f, btnW, 42f), "✕  " + LocalizationRuntime.T("quit"), buttonStyle))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                Application.Quit();
            }

            GUI.Label(new Rect(panel.x + 30f, panel.yMax - 32f, panel.width - 60f, 22f), LocalizationRuntime.T("settings.tip"), smallStyle);
        }

        private void DrawSettingsPanel()
        {
            Rect panel = CenterRect(720f, 660f);
            DrawPanel(panel);

            GUI.Label(new Rect(panel.x + 34f, panel.y + 24f, panel.width - 68f, 40f), LocalizationRuntime.T("settings").ToUpperInvariant(), titleStyle);
            GUI.Label(new Rect(panel.x + 44f, panel.y + 68f, panel.width - 88f, 36f), LocalizationRuntime.T("settings.subtitle"), subtitleStyle);

            float y = panel.y + 116f;
            GUI.Label(new Rect(panel.x + 54f, y, 210f, 26f), LocalizationRuntime.T("graphics_quality"), labelStyle);
            DrawQualityButtons(new Rect(panel.x + 54f, y + 28f, panel.width - 108f, 38f));

            y += 76f;
            GUI.Label(new Rect(panel.x + 54f, y, 210f, 26f), LocalizationRuntime.T("frame_rate"), labelStyle);
            DrawFrameRateButtons(new Rect(panel.x + 54f, y + 28f, panel.width - 108f, 38f));

            y += 76f;
            GUI.Label(new Rect(panel.x + 54f, y, 210f, 26f), LocalizationRuntime.T("language"), labelStyle);
            DrawLanguageButtons(new Rect(panel.x + 280f, y - 4f, 270f, 36f));

            y += 48f;
            GUI.Label(new Rect(panel.x + 54f, y, 210f, 26f), LocalizationRuntime.T("sound"), labelStyle);
            bool newSound = GUI.Toggle(new Rect(panel.x + 280f, y, 180f, 26f), GameSettingsRuntime.SoundEnabled, GameSettingsRuntime.SoundEnabled ? LocalizationRuntime.T("on") : LocalizationRuntime.T("off"), labelStyle);
            if (newSound != GameSettingsRuntime.SoundEnabled) GameSettingsRuntime.SetSoundEnabled(newSound);

            y += 44f;
            GUI.Label(new Rect(panel.x + 54f, y, 210f, 26f), LocalizationRuntime.T("master_volume"), labelStyle);
            float newMaster = GUI.HorizontalSlider(new Rect(panel.x + 280f, y + 6f, 230f, 22f), GameSettingsRuntime.MasterVolume, 0f, 1f);
            if (Mathf.Abs(newMaster - GameSettingsRuntime.MasterVolume) > 0.001f) GameSettingsRuntime.SetMasterVolume(newMaster);
            GUI.Label(new Rect(panel.x + 524f, y, 70f, 26f), Mathf.RoundToInt(GameSettingsRuntime.MasterVolume * 100f) + "%", labelStyle);

            y += 40f;
            GUI.Label(new Rect(panel.x + 54f, y, 210f, 26f), LocalizationRuntime.T("music_volume"), labelStyle);
            float newMusic = GUI.HorizontalSlider(new Rect(panel.x + 280f, y + 6f, 230f, 22f), GameSettingsRuntime.MusicVolume, 0f, 1f);
            if (Mathf.Abs(newMusic - GameSettingsRuntime.MusicVolume) > 0.001f) GameSettingsRuntime.SetMusicVolume(newMusic);
            GUI.Label(new Rect(panel.x + 524f, y, 70f, 26f), Mathf.RoundToInt(GameSettingsRuntime.MusicVolume * 100f) + "%", labelStyle);

            y += 40f;
            GUI.Label(new Rect(panel.x + 54f, y, 210f, 26f), LocalizationRuntime.T("sfx_volume"), labelStyle);
            float newSfx = GUI.HorizontalSlider(new Rect(panel.x + 280f, y + 6f, 230f, 22f), GameSettingsRuntime.SfxVolume, 0f, 1f);
            if (Mathf.Abs(newSfx - GameSettingsRuntime.SfxVolume) > 0.001f) GameSettingsRuntime.SetSfxVolume(newSfx);
            GUI.Label(new Rect(panel.x + 524f, y, 70f, 26f), Mathf.RoundToInt(GameSettingsRuntime.SfxVolume * 100f) + "%", labelStyle);

            GUI.Label(
                new Rect(panel.x + 54f, panel.yMax - 110f, panel.width - 108f, 44f),
                LocalizationRuntime.T("current") + ": " + GameSettingsRuntime.QualityName + " | " + GameSettingsRuntime.FrameRateLabel + " | " + LocalizationRuntime.T("language") + " " + LocalizationRuntime.LanguageName + " | " + LocalizationRuntime.T("sound") + " " + (GameSettingsRuntime.SoundEnabled ? LocalizationRuntime.T("on") : LocalizationRuntime.T("off")) + "\n" + GameSettingsRuntime.Diagnostics,
                smallStyle);

            string backText = hasStarted ? LocalizationRuntime.T("back_to_game") : LocalizationRuntime.T("back");
            float btnW = 230f;
            float btnH = 42f;
            float totalButtonsW = btnW * 2f + 20f;
            float startBtnX = panel.x + (panel.width - totalButtonsW) * 0.5f;

            if (GUI.Button(new Rect(startBtnX, panel.yMax - 60f, btnW, btnH), "↩  " + backText, buttonStyle))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                settingsOpen = false;
                if (hasStarted) ResumeGameplay();
            }

            if (GUI.Button(new Rect(startBtnX + btnW + 20f, panel.yMax - 60f, btnW, btnH), "✕  " + LocalizationRuntime.T("quit"), buttonStyle))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                Application.Quit();
            }
        }

        private void DrawQualityButtons(Rect area)
        {
            string[] names = QualitySettings.names;
            if (names == null || names.Length == 0)
            {
                GUI.Label(area, LocalizationRuntime.T("no_quality_levels"), labelStyle);
                return;
            }

            float gap = 8f;
            int count = Mathf.Min(names.Length, 6);
            float width = (area.width - gap * (count - 1)) / count;
            int start = names.Length > 6 ? names.Length - 6 : 0;
            for (int i = 0; i < count; i++)
            {
                int qualityIndex = start + i;
                Rect button = new Rect(area.x + i * (width + gap), area.y, width, area.height);
                GUIStyle style = qualityIndex == GameSettingsRuntime.QualityIndex ? selectedButtonStyle : buttonStyle;
                if (GUI.Button(button, names[qualityIndex], style)) GameSettingsRuntime.SetQuality(qualityIndex);
            }
        }

        private void DrawFrameRateButtons(Rect area)
        {
            float gap = 8f;
            int count = GameSettingsRuntime.FrameRateOptionCount;
            float width = (area.width - gap * (count - 1)) / count;
            for (int i = 0; i < count; i++)
            {
                Rect button = new Rect(area.x + i * (width + gap), area.y, width, area.height);
                GUIStyle style = i == GameSettingsRuntime.FrameRateOptionIndex ? selectedButtonStyle : buttonStyle;
                if (GUI.Button(button, GameSettingsRuntime.GetFrameRateLabel(i), style)) GameSettingsRuntime.SetFrameRateOption(i);
            }
        }

        private void DrawLanguageButtons(Rect area)
        {
            const float gap = 8f;
            float width = (area.width - gap) * 0.5f;
            Rect english = new Rect(area.x, area.y, width, area.height);
            Rect vietnamese = new Rect(area.x + width + gap, area.y, width, area.height);

            if (GUI.Button(english, "English", LocalizationRuntime.LanguageIndex == 0 ? selectedButtonStyle : buttonStyle))
            {
                LocalizationRuntime.SetLanguage(0);
            }

            if (GUI.Button(vietnamese, "Tiếng Việt", LocalizationRuntime.LanguageIndex == 1 ? selectedButtonStyle : buttonStyle))
            {
                LocalizationRuntime.SetLanguage(1);
            }
        }

        private void DrawGuidePanel()
        {
            Rect panel = CenterRect(880f, 620f);
            DrawPanel(panel);

            GUI.Label(new Rect(panel.x + 34f, panel.y + 20f, panel.width - 68f, 36f), LocalizationRuntime.T("guide"), titleStyle);

            string[] tabs = new string[]
            {
                LocalizationRuntime.T("guide_tab_basics"),
                LocalizationRuntime.T("guide_tab_farming"),
                LocalizationRuntime.T("guide_tab_fishing"),
                LocalizationRuntime.T("guide_tab_combat"),
                LocalizationRuntime.T("guide_tab_expansion")
            };

            float tabW = (panel.width - 68f) / tabs.Length;
            for (int i = 0; i < tabs.Length; i++)
            {
                Rect tabRect = new Rect(panel.x + 34f + i * tabW, panel.y + 60f, tabW - 4f, 34f);
                bool isActive = guideTab == i;
                GUIStyle style = isActive ? selectedButtonStyle : buttonStyle;
                if (GUI.Button(tabRect, tabs[i], style))
                {
                    guideTab = i;
                    TheOldRoad.Audio.AudioManager.PlayUiClick();
                }
            }

            Rect contentArea = new Rect(panel.x + 34f, panel.y + 104f, panel.width - 68f, panel.height - 176f);
            DrawRect(contentArea, new Color(0.02f, 0.016f, 0.012f, 0.75f));
            DrawBorder(contentArea, gold, 1.5f);

            DrawGuideContent(contentArea, guideTab);

            // Bottom Buttons
            float btnW = 160f;
            float btnH = 44f;
            float navY = panel.yMax - 58f;

            if (guideTab > 0)
            {
                Rect prevRect = new Rect(panel.x + 34f, navY, btnW, btnH);
                if (GUI.Button(prevRect, LocalizationRuntime.IsVietnamese ? "◀ Trang Trước" : "◀ Previous", buttonStyle))
                {
                    guideTab--;
                    TheOldRoad.Audio.AudioManager.PlayUiClick();
                }
            }

            if (guideTab < tabs.Length - 1)
            {
                Rect nextRect = new Rect(panel.xMax - 34f - btnW, navY, btnW, btnH);
                if (GUI.Button(nextRect, LocalizationRuntime.IsVietnamese ? "Trang Tiếp ▶" : "Next ▶", buttonStyle))
                {
                    guideTab++;
                    TheOldRoad.Audio.AudioManager.PlayUiClick();
                }
            }

            Rect closeBtn = new Rect(panel.x + (panel.width - btnW) * 0.5f, navY, btnW, btnH);
            if (GUI.Button(closeBtn, "↩ " + LocalizationRuntime.T("back"), buttonStyle))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                guideOpen = false;
            }
        }

        private void DrawGuideContent(Rect area, int tab)
        {
            float padding = 16f;
            float curY = area.y + padding;

            if (tab == 0) // Điều khiển & Cơ bản
            {
                GUI.Label(new Rect(area.x + padding, curY, area.width - padding * 2, 22f),
                    LocalizationRuntime.IsVietnamese ? "🔰 HƯỚNG DẪN TÂN THỦ & ĐIỀU KHIỂN CƠ BẢN" : "🔰 BEGINNER BASICS & CONTROLS", labelStyle);
                curY += 28f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 44f),
                    "[W][A][S][D] / Cần Gạt Ảo",
                    LocalizationRuntime.IsVietnamese ? "Di chuyển Hiệp sĩ khám phá thế giới, đi qua các vùng đất, khu rừng và ven sông." : "Move your Knight across the road, forests, and riverbanks.");
                curY += 48f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 44f),
                    "[SPACE] / Nút Chém ⚔",
                    LocalizationRuntime.IsVietnamese ? "Tấn công quái vật bằng Kiếm / Bắn Cung tên / Thu thập tài nguyên cây cối, quặng đá." : "Attack enemies with Sword / Fire Bow / Gather nearby trees and rock nodes.");
                curY += 48f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 44f),
                    "[F] / Nút Tương Tác",
                    LocalizationRuntime.IsVietnamese ? "Cuốc đất, gieo hạt, tưới nước, thu hoạch cây, câu cá, vắt sữa bò, nhặt đồ, mở rương." : "Till soil, plant seed, water, harvest crop, fish, milk cow, talk NPC, open chest.");
                curY += 48f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 44f),
                    "[V] / Chuột Phải",
                    LocalizationRuntime.IsVietnamese ? "Giơ Khiên gỗ tròn để phòng thủ, chặn đứng và giảm 75% sát thương nhận vào." : "Raise Round Shield to block incoming monster attacks (reduces damage by 75%).");
                curY += 48f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 44f),
                    "[Q] / [TAB] / [M] / [J] / [B]",
                    LocalizationRuntime.IsVietnamese ? "[Q] Ăn thực phẩm hồi máu | [TAB/I] Túi đồ | [M] Bản đồ toàn cảnh | [J] Nhiệm vụ | [B] Xây dựng." : "[Q] Eat food | [TAB/I] Inventory | [M] Full Map | [J] Quest Log | [B] Build menu.");
            }
            else if (tab == 1) // Nông trại & Trồng trọt
            {
                GUI.Label(new Rect(area.x + padding, curY, area.width - padding * 2, 22f),
                    LocalizationRuntime.IsVietnamese ? "🌾 HỆ THỐNG NÔNG TRẠI & TRỒNG TRỌT ĐỒNG QUÊ" : "🌾 CROP FARMING & HARVESTING SYSTEM", labelStyle);
                curY += 28f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "1. Cuốc Xới Đất" : "1. Till Soil",
                    LocalizationRuntime.IsVietnamese ? "Trang bị Cuốc làm vườn (Worn Hoe) và nhấn [F] vào ô đất hoang để xới luống đất màu mỡ." : "Equip Worn Hoe and press [F] on wild soil plot to till fertile farm land.");
                curY += 54f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "2. Gieo Hạt Giống" : "2. Plant Seeds",
                    LocalizationRuntime.IsVietnamese ? "Chọn hạt giống (Lúa mì, Bắp, Cà rốt, Khoai tây, Cà chua, Dứa) trên thanh công cụ và nhấn [F] để gieo." : "Select seeds (Wheat, Corn, Carrot, Potato, Tomato, Pineapple) on hotbar and press [F] to plant.");
                curY += 54f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "3. Tưới Nước & Mưa" : "3. Watering & Rain",
                    LocalizationRuntime.IsVietnamese ? "Dùng Bình tưới nước [F] để đất ẩm giúp cây lớn nhanh gấp 2 lần! Mưa sẽ tự động tưới đất." : "Use Watering Can [F] so soil is watered (grows 2x faster). Rain will automatically water farm plots!");
                curY += 54f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "4. 5 Giai Đoạn & Offline" : "4. 5 Stages & Offline",
                    LocalizationRuntime.IsVietnamese ? "Cây lớn qua 5 giai đoạn hình ảnh sinh động. Cây vẫn tiếp tục lớn ngay cả khi bạn tắt game." : "Crops grow across 5 visual stages. Crops continue to grow even when you quit the game.");
                curY += 54f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "5. Thu Hoạch Dễ Dàng" : "5. Easy Harvest",
                    LocalizationRuntime.IsVietnamese ? "Nhấn [F] khi cây trĩu hạt để thu hoạch. Ô đất vẫn giữ nguyên trạng thái đã xới để gieo đợt mới!" : "Press [F] when crops mature to harvest. Soil remains tilled, ready for your next seed!");
            }
            else if (tab == 2) // Câu cá & Nấu ăn
            {
                GUI.Label(new Rect(area.x + padding, curY, area.width - padding * 2, 22f),
                    LocalizationRuntime.IsVietnamese ? "🎣 CÂU CÁ SÔNG VALEN & ẨM THỰC HỒI MÁU" : "🎣 RIVER FISHING & CAMPFIRE COOKING", labelStyle);
                curY += 28f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 54f),
                    LocalizationRuntime.IsVietnamese ? "1. Thả Phao Câu Ven Sông" : "1. Cast River Line",
                    LocalizationRuntime.IsVietnamese ? "Tiến sát bờ sông Valen. Trang bị Cần câu tre [item.fishing-rod] và bấm [F] để quăng phao nổi dập dềnh." : "Walk to the southern Valen river. Equip Bamboo Rod and press [F] to cast bobber into the stream.");
                curY += 58f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 54f),
                    LocalizationRuntime.IsVietnamese ? "2. Dùng Mồi Trùn Đất" : "2. Earthworm Bait",
                    LocalizationRuntime.IsVietnamese ? "Có Mồi trùn đất trong túi đồ sẽ giúp cá cắn câu nhanh hơn nhiều lần và tăng tỉ lệ bắt cá quý hiếm." : "Carrying Earthworm Bait makes fish bite significantly faster and increases rare fish chance.");
                curY += 58f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 54f),
                    LocalizationRuntime.IsVietnamese ? "3. Giật Cần Khi Phao Rung [!]" : "3. Reel On Bite [!]",
                    LocalizationRuntime.IsVietnamese ? "Khi phao rung lắc và hiện biểu tượng [!], lập tức bấm [F] trong vòng 1.8 giây để kéo cá lên bờ!" : "When the bobber splashes and shows [!], quickly press [F] within 1.8s to reel in your catch!");
                curY += 58f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 54f),
                    LocalizationRuntime.IsVietnamese ? "4. Nướng Cá & Bữa Ăn Nóng" : "4. Grilled Fish & Meals",
                    LocalizationRuntime.IsVietnamese ? "Đem cá và nông sản đến Lửa trại hoặc Bếp lò để nấu Cá nướng (+18 HP) và Bữa ăn nóng (+12 HP)." : "Bring fish and crops to campfire or stove to prepare Herb Grilled Fish (+18 HP) and Cooked Meals (+12 HP).");
            }
            else if (tab == 3) // Chiến đấu & Trang bị
            {
                GUI.Label(new Rect(area.x + padding, curY, area.width - padding * 2, 22f),
                    LocalizationRuntime.IsVietnamese ? "⚔️ CHIẾN ĐẤU, VŨ KHÍ, CUNG TÊN & PHÒNG THỦ" : "⚔️ COMBAT, WEAPONS, BOW & DEFENSE", labelStyle);
                curY += 28f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "Kiếm Sắt Dài (+7 DMG)" : "Iron Longsword (+7 DMG)",
                    LocalizationRuntime.IsVietnamese ? "Sát thương chém cận chiến mạnh mẽ, tầm quét rộng để tiêu diệt bầy thú dữ và đạo tặc." : "High melee slash damage with wide sweep arc to defeat wild beasts and bandits.");
                curY += 54f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "Cung Săn Bắn & Mũi Tên" : "Hunter's Bow & Flint Arrows",
                    LocalizationRuntime.IsVietnamese ? "Tấn công từ xa an toàn. Nhấn phím Space để bắn mũi tên bay tiêu diệt mục tiêu từ xa." : "Safe ranged attack. Press Space to fire flint arrows directly at distant monsters.");
                curY += 54f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "Khiên Gỗ Tròn (-75% DMG)" : "Round Shield (-75% DMG)",
                    LocalizationRuntime.IsVietnamese ? "Giữ phím [V] hoặc Chuột phải để giơ khiên đỡ đòn, giảm 75% sát thương nhận vào." : "Hold [V] or Right-click to raise shield and reduce 75% incoming damage.");
                curY += 54f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 50f),
                    LocalizationRuntime.IsVietnamese ? "Giáp Ngực Hiệp Sĩ (+10 HP)" : "Knight Cuirass (+10 Max HP)",
                    LocalizationRuntime.IsVietnamese ? "Gia tăng lượng máu tối đa của nhân vật, giúp bạn sống sót qua những cuộc săn đêm nguy hiểm." : "Increases maximum player health, essential for surviving dangerous night hunts.");
            }
            else if (tab == 4) // Chăn nuôi & Mở rộng đất
            {
                GUI.Label(new Rect(area.x + padding, curY, area.width - padding * 2, 22f),
                    LocalizationRuntime.IsVietnamese ? "🐄 CHĂN NUÔI GIA SÚC & MỞ RỘNG ĐẤT ĐAI" : "🐄 ANIMAL HUSBANDRY & FARM EXPANSION", labelStyle);
                curY += 28f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 54f),
                    LocalizationRuntime.IsVietnamese ? "Bò Sữa & Cho Ăn Cỏ Khô" : "Dairy Cow & Hay Feeding",
                    LocalizationRuntime.IsVietnamese ? "Bò cho Sữa tươi. Đem Bó cỏ khô [item.hay] hoặc Lúa mì cho bò ăn để reset thời gian vắt sữa ngay lập tức!" : "Cows yield fresh Milk. Feeding Dry Hay or Wheat immediately resets milk cooldown and shows hearts!");
                curY += 58f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 54f),
                    LocalizationRuntime.IsVietnamese ? "Gà & Rải Thóc Ổ Rơm" : "Hens & Straw Nests",
                    LocalizationRuntime.IsVietnamese ? "Gà đẻ trứng tại ổ rơm. Rải hạt giống lúa mì vào ổ rơm để đàn gà đẻ trứng nhanh hơn." : "Hens lay fresh Eggs in straw nests. Scattering wheat seeds into nests speeds up egg laying.");
                curY += 58f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 54f),
                    LocalizationRuntime.IsVietnamese ? "Thư Khai Hoang Đất Đai" : "Farm Land Deed Expansion",
                    LocalizationRuntime.IsVietnamese ? "Sở hữu Thư khai hoang [item.farm-deed] sẽ mở thêm 12 ô đất Grid B (tổng cộng 24 ô đất nông trại)!" : "Owning a Farm Land Deed unlocks 12 additional farm plots (doubling your field to 24 total plots)!");
                curY += 58f;

                DrawMenuGuideRow(new Rect(area.x + padding, curY, area.width - padding * 2, 54f),
                    LocalizationRuntime.IsVietnamese ? "Thương Nhân Eldon" : "Merchant Eldon Trade",
                    LocalizationRuntime.IsVietnamese ? "Bán nông sản, cá sông, quặng sắt để lấy Đồng bạc và mua sắm hạt giống cùng trang bị xịn." : "Sell harvest crops, river fish, iron ore for Silver Coins to buy rare seeds and high tier equipment.");
            }
        }

        private void DrawMenuGuideRow(Rect rect, string title, string description)
        {
            DrawRect(rect, new Color(0.08f, 0.07f, 0.05f, 0.90f));
            DrawBorder(rect, gold, 1f);

            GUI.Label(new Rect(rect.x + 12f, rect.y + 4f, rect.width - 24f, 20f), title, labelStyle);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 22f, rect.width - 24f, rect.height - 24f), description, smallStyle);
        }

        private void PauseForMenu()
        {
            if (!pausedByMenu)
            {
                previousTimeScale = Mathf.Approximately(UnityEngine.Time.timeScale, 0f) ? 1f : UnityEngine.Time.timeScale;
                pausedByMenu = true;
            }

            PrototypeGameState.GameplayInputBlocked = true;
            UnityEngine.Time.timeScale = 0f;
        }

        private void ResumeGameplay()
        {
            pausedByMenu = false;
            PrototypeGameState.GameplayInputBlocked = false;
            UnityEngine.Time.timeScale = previousTimeScale <= 0f ? 1f : previousTimeScale;
        }

        private static bool SafeEscapeDown()
        {
            try
            {
                return UnityEngine.Input.GetKeyDown(KeyCode.Escape);
            }
            catch (System.InvalidOperationException)
            {
                return false;
            }
        }

        private void DrawBackdrop()
        {
            Texture2D panorama = TheOldRoad.World.PrototypePixelArtFactory.TitleKnightSunsetTexture();
            if (panorama != null)
            {
                GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), panorama, ScaleMode.ScaleAndCrop);
            }
            else
            {
                DrawRect(new Rect(0f, 0f, Screen.width, Screen.height), new Color(0.005f, 0.008f, 0.010f, 1f));
            }

            // Floating sunset ember particles
            float time = UnityEngine.Time.unscaledTime;
            for (int i = 0; i < 36; i++)
            {
                float seedX = (i * 127.3f) % Screen.width;
                float speed = 18f + (i % 7) * 4f;
                float seedY = Screen.height - ((time * speed + i * 47.9f) % (Screen.height + 40f));
                float sway = Mathf.Sin(time * 1.5f + i) * 12f;
                float size = 2f + (i % 3);
                float alpha = Mathf.Sin((seedY / Screen.height) * Mathf.PI) * 0.75f;

                Color emberCol = (i % 2 == 0)
                    ? new Color(1f, 0.78f, 0.28f, alpha)
                    : new Color(1f, 0.45f, 0.18f, alpha * 0.85f);

                DrawRect(new Rect(seedX + sway, seedY, size, size), emberCol);
            }

            // Top & bottom cinematic vignette gradients
            DrawRect(new Rect(0f, 0f, Screen.width, Screen.height * 0.22f), new Color(0.04f, 0.02f, 0.06f, 0.45f));
            DrawRect(new Rect(0f, Screen.height * 0.78f, Screen.width, Screen.height * 0.22f), new Color(0.02f, 0.01f, 0.03f, 0.55f));
        }

        private void DrawPanel(Rect rect)
        {
            // Drop shadow
            DrawRect(new Rect(rect.x + 6f, rect.y + 8f, rect.width, rect.height), new Color(0f, 0f, 0f, 0.65f));
            // Dark forged iron plate
            DrawRect(rect, new Color(0.06f, 0.045f, 0.04f, 0.94f));
            // Gold filigree borders
            DrawBorder(rect, gold, 3f);
            DrawBorder(new Rect(rect.x + 5f, rect.y + 5f, rect.width - 10f, rect.height - 10f), new Color(0.38f, 0.26f, 0.12f, 1f), 1f);
            DrawBorder(new Rect(rect.x + 8f, rect.y + 8f, rect.width - 16f, rect.height - 16f), new Color(0.18f, 0.12f, 0.08f, 0.8f), 1f);
            // Scarlet header ribbon
            DrawRect(new Rect(rect.x + 6f, rect.y + 4f, rect.width - 12f, 6f), red);
        }

        private void DrawDivider(Rect rect)
        {
            DrawRect(rect, new Color(0.55f, 0.38f, 0.16f, 1f));
        }

        private static Rect CenterRect(float width, float height)
        {
            width = Mathf.Min(width, Screen.width - 42f);
            height = Mathf.Min(height, Screen.height - 42f);
            return new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
        }

        private void DrawRect(Rect rect, Color color)
        {
            Color previous = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, pixel);
            GUI.color = previous;
        }

        private void DrawBorder(Rect rect, Color color, float thickness)
        {
            DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
            DrawRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color);
            DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
            DrawRect(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color);
        }

        private void EnsureStyles()
        {
            if (pixel == null)
            {
                pixel = new Texture2D(1, 1, TextureFormat.RGBA32, false)
                {
                    filterMode = FilterMode.Point,
                    hideFlags = HideFlags.DontSave
                };
                pixel.SetPixel(0, 0, Color.white);
                pixel.Apply(false, true);
            }

            UiFontHelper.EnsureGlobalSkinFont();
            Font clean = UiFontHelper.CleanFont;

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                font = clean,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 32,
                fontStyle = FontStyle.Bold,
                normal = { textColor = gold }
            };

            subtitleStyle = new GUIStyle(GUI.skin.label)
            {
                font = clean,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                wordWrap = true,
                normal = { textColor = parchment }
            };

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                font = clean,
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = parchment }
            };

            smallStyle = new GUIStyle(GUI.skin.label)
            {
                font = clean,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                wordWrap = true,
                normal = { textColor = new Color(0.78f, 0.72f, 0.60f, 1f) }
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                font = clean,
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = parchment },
                hover = { textColor = gold },
                active = { textColor = Color.white }
            };

            selectedButtonStyle = new GUIStyle(buttonStyle)
            {
                font = clean,
                normal = { textColor = gold }
            };
        }
    }
}
