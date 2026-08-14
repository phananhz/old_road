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
            Rect panel = CenterRect(620f, 440f);
            DrawPanel(panel);

            GUI.Label(new Rect(panel.x + 32f, panel.y + 32f, panel.width - 64f, 62f), "THE OLD ROAD", titleStyle);
            GUI.Label(
                new Rect(panel.x + 42f, panel.y + 98f, panel.width - 84f, 76f),
                LocalizationRuntime.T("title.subtitle"),
                subtitleStyle);

            DrawDivider(new Rect(panel.x + 54f, panel.y + 188f, panel.width - 108f, 2f));

            if (GUI.Button(new Rect(panel.x + 180f, panel.y + 220f, 260f, 48f), LocalizationRuntime.T("start"), buttonStyle))
            {
                hasStarted = true;
                settingsOpen = false;
                ResumeGameplay();
            }

            if (GUI.Button(new Rect(panel.x + 180f, panel.y + 282f, 260f, 44f), LocalizationRuntime.T("settings"), buttonStyle))
            {
                settingsOpen = true;
            }

            if (GUI.Button(new Rect(panel.x + 180f, panel.y + 340f, 260f, 44f), LocalizationRuntime.T("quit"), buttonStyle))
            {
                Application.Quit();
            }

            GUI.Label(new Rect(panel.x + 44f, panel.yMax - 40f, panel.width - 88f, 22f), LocalizationRuntime.T("settings.tip"), smallStyle);
        }

        private void DrawSettingsPanel()
        {
            Rect panel = CenterRect(700f, 560f);
            DrawPanel(panel);

            GUI.Label(new Rect(panel.x + 34f, panel.y + 28f, panel.width - 68f, 44f), LocalizationRuntime.T("settings").ToUpperInvariant(), titleStyle);
            GUI.Label(new Rect(panel.x + 44f, panel.y + 78f, panel.width - 88f, 44f), LocalizationRuntime.T("settings.subtitle"), subtitleStyle);

            float y = panel.y + 140f;
            GUI.Label(new Rect(panel.x + 54f, y, 210f, 28f), LocalizationRuntime.T("graphics_quality"), labelStyle);
            DrawQualityButtons(new Rect(panel.x + 54f, y + 34f, panel.width - 108f, 44f));

            y += 104f;
            GUI.Label(new Rect(panel.x + 54f, y, 210f, 28f), LocalizationRuntime.T("frame_rate"), labelStyle);
            DrawFrameRateButtons(new Rect(panel.x + 54f, y + 34f, panel.width - 108f, 44f));

            y += 104f;
            GUI.Label(new Rect(panel.x + 54f, y, 210f, 28f), LocalizationRuntime.T("language"), labelStyle);
            DrawLanguageButtons(new Rect(panel.x + 280f, y - 6f, 270f, 40f));

            y += 62f;
            GUI.Label(new Rect(panel.x + 54f, y, 210f, 28f), LocalizationRuntime.T("sound"), labelStyle);
            bool newSound = GUI.Toggle(new Rect(panel.x + 280f, y, 180f, 28f), GameSettingsRuntime.SoundEnabled, GameSettingsRuntime.SoundEnabled ? LocalizationRuntime.T("on") : LocalizationRuntime.T("off"), labelStyle);
            if (newSound != GameSettingsRuntime.SoundEnabled) GameSettingsRuntime.SetSoundEnabled(newSound);

            y += 62f;
            GUI.Label(new Rect(panel.x + 54f, y, 210f, 28f), LocalizationRuntime.T("master_volume"), labelStyle);
            float newVolume = GUI.HorizontalSlider(new Rect(panel.x + 280f, y + 8f, 230f, 24f), GameSettingsRuntime.MasterVolume, 0f, 1f);
            if (Mathf.Abs(newVolume - GameSettingsRuntime.MasterVolume) > 0.001f) GameSettingsRuntime.SetMasterVolume(newVolume);
            GUI.Label(new Rect(panel.x + 524f, y, 70f, 28f), Mathf.RoundToInt(GameSettingsRuntime.MasterVolume * 100f) + "%", labelStyle);

            GUI.Label(
                new Rect(panel.x + 54f, panel.yMax - 128f, panel.width - 108f, 52f),
                LocalizationRuntime.T("current") + ": " + GameSettingsRuntime.QualityName + " | " + GameSettingsRuntime.FrameRateLabel + " | " + LocalizationRuntime.T("language") + " " + LocalizationRuntime.LanguageName + " | " + LocalizationRuntime.T("sound") + " " + (GameSettingsRuntime.SoundEnabled ? LocalizationRuntime.T("on") : LocalizationRuntime.T("off")) + "\n" + GameSettingsRuntime.Diagnostics,
                smallStyle);

            string backText = hasStarted ? LocalizationRuntime.T("back_to_game") : LocalizationRuntime.T("back");
            if (GUI.Button(new Rect(panel.x + 200f, panel.yMax - 66f, 260f, 42f), backText, buttonStyle))
            {
                settingsOpen = false;
                if (hasStarted) ResumeGameplay();
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
            DrawRect(new Rect(0f, 0f, Screen.width, Screen.height), new Color(0.005f, 0.008f, 0.010f, 1f));
            DrawRect(new Rect(0f, 0f, Screen.width, Screen.height * 0.28f), new Color(0.10f, 0.07f, 0.04f, 0.55f));
            DrawRect(new Rect(0f, Screen.height * 0.62f, Screen.width, Screen.height * 0.38f), new Color(0f, 0f, 0f, 0.42f));
        }

        private void DrawPanel(Rect rect)
        {
            DrawRect(new Rect(rect.x + 8f, rect.y + 10f, rect.width, rect.height), new Color(0f, 0f, 0f, 0.45f));
            DrawRect(rect, ink);
            DrawBorder(rect, gold, 3f);
            DrawBorder(new Rect(rect.x + 8f, rect.y + 8f, rect.width - 16f, rect.height - 16f), new Color(0.26f, 0.17f, 0.08f, 1f), 1f);
            DrawRect(new Rect(rect.x, rect.y, rect.width, 8f), red);
        }

        private void DrawDivider(Rect rect)
        {
            DrawRect(rect, new Color(0.42f, 0.29f, 0.12f, 1f));
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
                fontSize = 34,
                fontStyle = FontStyle.Bold,
                normal = { textColor = gold }
            };

            subtitleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 15,
                wordWrap = true,
                normal = { textColor = parchment }
            };

            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                normal = { textColor = parchment }
            };

            smallStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                wordWrap = true,
                normal = { textColor = new Color(0.70f, 0.66f, 0.56f, 1f) }
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 16,
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
