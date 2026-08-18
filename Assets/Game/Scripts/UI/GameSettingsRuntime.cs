using UnityEngine;
using System.Reflection;

namespace TheOldRoad.UI
{
    /// <summary>PlayerPrefs-backed settings for graphics quality, language, frame rate, and audio volumes.</summary>
    public static class GameSettingsRuntime
    {
        private const string QualityPrefKey = "the_old_road.settings.quality";
        private const string SoundEnabledPrefKey = "the_old_road.settings.sound_enabled";
        private const string VolumePrefKey = "the_old_road.settings.master_volume";
        private const string MusicVolumePrefKey = "the_old_road.settings.music_volume";
        private const string SfxVolumePrefKey = "the_old_road.settings.sfx_volume";
        private const string FrameRatePrefKey = "the_old_road.settings.frame_rate_option";
        private static readonly int[] FrameRateOptions = { 30, 60, 90, 120, -1 };

        public static int QualityIndex { get; private set; }
        public static bool SoundEnabled { get; private set; } = true;
        public static float MasterVolume { get; private set; } = 0.85f;
        public static float MusicVolume { get; private set; } = 0.80f;
        public static float SfxVolume { get; private set; } = 0.90f;
        public static int FrameRateOptionIndex { get; private set; } = 1;
        public static float MeasuredFps { get; private set; }

        private static float fpsTimer;
        private static int fpsFrames;
        private static float enforceTimer;

        public static string QualityName
        {
            get
            {
                string[] names = QualitySettings.names;
                if (names == null || names.Length == 0) return "Default";
                return names[Mathf.Clamp(QualityIndex, 0, names.Length - 1)];
            }
        }

        public static int FrameRateOptionCount => FrameRateOptions.Length;
        public static int CurrentFrameRate => FrameRateOptions[Mathf.Clamp(FrameRateOptionIndex, 0, FrameRateOptions.Length - 1)];
        public static string FrameRateLabel => CurrentFrameRate < 0 ? LocalizationRuntime.T("unlimited") : CurrentFrameRate + " FPS";
        public static string Diagnostics => LocalizationRuntime.T("actual") + " " + Mathf.RoundToInt(MeasuredFps) + " FPS | " + LocalizationRuntime.T("target") + " " + FrameRateLabel + " | vSync " + QualitySettings.vSyncCount + " | AA x" + QualitySettings.antiAliasing;

        public static string GetFrameRateLabel(int optionIndex)
        {
            int frameRate = FrameRateOptions[Mathf.Clamp(optionIndex, 0, FrameRateOptions.Length - 1)];
            return frameRate < 0 ? LocalizationRuntime.T("unlimited") : frameRate + " FPS";
        }

        public static void Tick()
        {
            fpsFrames++;
            fpsTimer += UnityEngine.Time.unscaledDeltaTime;
            if (fpsTimer >= 0.5f)
            {
                MeasuredFps = fpsFrames / fpsTimer;
                fpsFrames = 0;
                fpsTimer = 0f;
            }

            enforceTimer += UnityEngine.Time.unscaledDeltaTime;
            if (enforceTimer >= 0.25f)
            {
                enforceTimer = 0f;
                EnforceAppliedSettings();
            }
        }

        public static void EnforceAppliedSettings()
        {
            QualitySettings.vSyncCount = 0;
            if (Application.targetFrameRate != CurrentFrameRate)
            {
                Application.targetFrameRate = CurrentFrameRate;
            }

            ApplyAudioByReflection();
        }

        public static void LoadAndApply()
        {
            LocalizationRuntime.Load();
            int currentQuality = QualitySettings.GetQualityLevel();
            string[] names = QualitySettings.names;
            int maxQuality = names == null || names.Length == 0 ? 0 : names.Length - 1;

            QualityIndex = Mathf.Clamp(PlayerPrefs.GetInt(QualityPrefKey, currentQuality), 0, maxQuality);
            SoundEnabled = PlayerPrefs.GetInt(SoundEnabledPrefKey, 1) != 0;
            MasterVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(VolumePrefKey, 0.85f));
            MusicVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(MusicVolumePrefKey, 0.80f));
            SfxVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(SfxVolumePrefKey, 0.90f));
            FrameRateOptionIndex = Mathf.Clamp(PlayerPrefs.GetInt(FrameRatePrefKey, 1), 0, FrameRateOptions.Length - 1);
            Apply();
        }

        public static void SetQuality(int index)
        {
            string[] names = QualitySettings.names;
            int maxQuality = names == null || names.Length == 0 ? 0 : names.Length - 1;
            QualityIndex = Mathf.Clamp(index, 0, maxQuality);
            ApplyAndSave();
        }

        public static void SetSoundEnabled(bool enabled)
        {
            SoundEnabled = enabled;
            ApplyAndSave();
        }

        public static void SetMasterVolume(float volume)
        {
            MasterVolume = Mathf.Clamp01(volume);
            ApplyAndSave();
        }

        public static void SetMusicVolume(float volume)
        {
            MusicVolume = Mathf.Clamp01(volume);
            ApplyAndSave();
        }

        public static void SetSfxVolume(float volume)
        {
            SfxVolume = Mathf.Clamp01(volume);
            ApplyAndSave();
        }

        public static void SetFrameRateOption(int optionIndex)
        {
            FrameRateOptionIndex = Mathf.Clamp(optionIndex, 0, FrameRateOptions.Length - 1);
            ApplyAndSave();
        }

        public static void ApplyAndSave()
        {
            Apply();
            Save();
        }

        private static void Apply()
        {
            string[] names = QualitySettings.names;
            if (names != null && names.Length > 0)
            {
                QualitySettings.SetQualityLevel(Mathf.Clamp(QualityIndex, 0, names.Length - 1), true);
            }

            ApplyPrototypeGraphicsPreset();
            EnforceAppliedSettings();
        }

        private static void ApplyPrototypeGraphicsPreset()
        {
            string qualityName = QualityName.ToLowerInvariant();
            bool veryLow = qualityName.Contains("very") || qualityName.Contains("low");
            bool medium = qualityName.Contains("medium");
            bool high = qualityName.Contains("high") || qualityName.Contains("ultra");

            QualitySettings.vSyncCount = 0;
            QualitySettings.antiAliasing = veryLow ? 0 : medium ? 2 : high ? 4 : 2;
            QualitySettings.anisotropicFiltering = veryLow
                ? AnisotropicFiltering.Disable
                : high
                    ? AnisotropicFiltering.ForceEnable
                    : AnisotropicFiltering.Enable;
            QualitySettings.shadows = veryLow ? ShadowQuality.Disable : high ? ShadowQuality.All : ShadowQuality.HardOnly;
            QualitySettings.shadowResolution = high ? ShadowResolution.High : ShadowResolution.Low;
            QualitySettings.shadowDistance = veryLow ? 0f : high ? 55f : 25f;
            QualitySettings.lodBias = veryLow ? 0.5f : high ? 1.5f : 1f;
            QualitySettings.realtimeReflectionProbes = high;
            QualitySettings.softParticles = high;
            ApplyAudioByReflection();
        }

        private static void ApplyAudioByReflection()
        {
            TheOldRoad.Audio.AudioManager.MasterVolume = MasterVolume;
            TheOldRoad.Audio.AudioManager.MusicVolume = MusicVolume;
            TheOldRoad.Audio.AudioManager.SfxVolume = SfxVolume;
            TheOldRoad.Audio.AudioManager.IsMuted = !SoundEnabled;

            System.Type audioListenerType = System.Type.GetType("UnityEngine.AudioListener, UnityEngine.AudioModule");
            if (audioListenerType == null) return;

            PropertyInfo volumeProperty = audioListenerType.GetProperty("volume", BindingFlags.Public | BindingFlags.Static);
            if (volumeProperty != null) volumeProperty.SetValue(null, SoundEnabled ? MasterVolume : 0f);

            PropertyInfo pauseProperty = audioListenerType.GetProperty("pause", BindingFlags.Public | BindingFlags.Static);
            if (pauseProperty != null) pauseProperty.SetValue(null, !SoundEnabled);
        }

        private static void Save()
        {
            PlayerPrefs.SetInt(QualityPrefKey, QualityIndex);
            PlayerPrefs.SetInt(SoundEnabledPrefKey, SoundEnabled ? 1 : 0);
            PlayerPrefs.SetFloat(VolumePrefKey, MasterVolume);
            PlayerPrefs.SetFloat(MusicVolumePrefKey, MusicVolume);
            PlayerPrefs.SetFloat(SfxVolumePrefKey, SfxVolume);
            PlayerPrefs.SetInt(FrameRatePrefKey, FrameRateOptionIndex);
            PlayerPrefs.Save();
        }
    }
}
