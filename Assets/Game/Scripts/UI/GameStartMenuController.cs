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
        private bool pausedByMenu;
        private float previousTimeScale = 1f;

        public static bool IsOpen => instance != null && (!instance.hasStarted || instance.settingsOpen);

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
            instance.PauseForMenu();
        }

        private void Update()
        {
            GameSettingsRuntime.Tick();
            if (hasStarted && !settingsOpen && SafeEscapeDown())
            {
                OpenSettingsFromGame();
            }
        }

        private void OnGUI()
        {
            EnsureStyles();
            if (!IsOpen) return;

            DrawBackdrop();
            if (settingsOpen) DrawSettingsPanel();
            else DrawTitlePanel();
        }

        private void DrawTitlePanel()
        {
            // Positioned elegantly in center-right to showcase the knight on left and castle on right
            float panelWidth = Mathf.Min(560f, Screen.width - 40f);
            float panelHeight = Mathf.Min(430f, Screen.height - 40f);
            float panelX = (Screen.width - panelWidth) * 0.5f;
            float panelY = (Screen.height - panelHeight) * 0.5f;
            Rect panel = new Rect(panelX, panelY, panelWidth, panelHeight);

            DrawPanel(panel);

            // Title Banner
            GUI.Label(new Rect(panel.x + 20f, panel.y + 24f, panel.width - 40f, 54f), "THE OLD ROAD", titleStyle);
            
            // Subtitle
            GUI.Label(
                new Rect(panel.x + 36f, panel.y + 80f, panel.width - 72f, 68f),
                LocalizationRuntime.T("title.subtitle"),
                subtitleStyle);

            DrawDivider(new Rect(panel.x + 48f, panel.y + 160f, panel.width - 96f, 2f));

            // Ornate Action Buttons
            float btnW = Mathf.Min(280f, panel.width - 80f);
            float btnX = panel.x + (panel.width - btnW) * 0.5f;

            if (GUI.Button(new Rect(btnX, panel.y + 185f, btnW, 50f), "⚔  " + LocalizationRuntime.T("start") + "  ⚔", buttonStyle))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                hasStarted = true;
                settingsOpen = false;
                ResumeGameplay();
            }

            if (GUI.Button(new Rect(btnX, panel.y + 248f, btnW, 46f), "⚙  " + LocalizationRuntime.T("settings"), buttonStyle))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                settingsOpen = true;
            }

            if (GUI.Button(new Rect(btnX, panel.y + 306f, btnW, 46f), "✕  " + LocalizationRuntime.T("quit"), buttonStyle))
            {
                TheOldRoad.Audio.AudioManager.PlayUiClick();
                Application.Quit();
            }

            GUI.Label(new Rect(panel.x + 30f, panel.yMax - 36f, panel.width - 60f, 22f), LocalizationRuntime.T("settings.tip"), smallStyle);
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

            if (titleStyle != null) return;

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 32,
                fontStyle = FontStyle.Bold,
                normal = { textColor = gold }
            };

            subtitleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 14,
                wordWrap = true,
                normal = { textColor = parchment }
            };

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = parchment }
            };

            smallStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                wordWrap = true,
                normal = { textColor = new Color(0.78f, 0.72f, 0.60f, 1f) }
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = parchment },
                hover = { textColor = gold },
                active = { textColor = Color.white }
            };

            selectedButtonStyle = new GUIStyle(buttonStyle)
            {
                normal = { textColor = gold }
            };
        }
    }
}
