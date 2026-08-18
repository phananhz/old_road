using UnityEngine;
using TheOldRoad.Audio;
using TheOldRoad.Time;

namespace TheOldRoad.World
{
    public enum WeatherType
    {
        Clear,
        LightRain,
        Thunderstorm,
        ForestFog
    }

    /// <summary>
    /// Atmospheric Weather Controller managing Rain, Thunderstorms, Lightning flashes, and Fog.
    /// </summary>
    public sealed class WeatherController : MonoBehaviour
    {
        [SerializeField] private WeatherType currentWeather = WeatherType.Clear;
        [SerializeField] private float weatherCycleMinutes = 4f;

        private GameTimeController gameTime;
        private Camera mainCamera;
        private float weatherTimer;
        private float rainIntensity;
        private float lightningTimer;
        private bool isLightningFlash;
        private float flashDuration;
        private float fogOffset;

        private Texture2D rainStreakTexture;
        private Texture2D fogTexture;

        public WeatherType CurrentWeather => currentWeather;
        public float RainIntensity => rainIntensity;

        public void SetWeather(WeatherType weather)
        {
            currentWeather = weather;
        }

        private void Awake()
        {
            gameTime = FindAnyObjectByType<GameTimeController>();
            mainCamera = Camera.main;
            EnsureTextures();
        }

        private void Update()
        {
            if (gameTime == null) gameTime = FindAnyObjectByType<GameTimeController>();
            if (mainCamera == null) mainCamera = Camera.main;

            weatherTimer += UnityEngine.Time.deltaTime;
            if (weatherTimer >= weatherCycleMinutes * 60f)
            {
                weatherTimer = 0f;
                CycleNextWeather();
            }

            float targetRain = (currentWeather == WeatherType.LightRain) ? 0.65f :
                               (currentWeather == WeatherType.Thunderstorm) ? 1.0f : 0f;
            rainIntensity = Mathf.MoveTowards(rainIntensity, targetRain, UnityEngine.Time.deltaTime * 0.35f);
            AudioManager.SetRainIntensity(rainIntensity);

            // Thunderstorm Lightning logic
            if (currentWeather == WeatherType.Thunderstorm)
            {
                lightningTimer -= UnityEngine.Time.deltaTime;
                if (lightningTimer <= 0f)
                {
                    TriggerLightning();
                    lightningTimer = Random.Range(7f, 16f);
                }

                if (isLightningFlash)
                {
                    flashDuration -= UnityEngine.Time.deltaTime;
                    if (flashDuration <= 0f)
                    {
                        isLightningFlash = false;
                    }
                }
            }
            else
            {
                isLightningFlash = false;
            }

            fogOffset += UnityEngine.Time.deltaTime * 0.08f;
        }

        private void CycleNextWeather()
        {
            // Cycle naturally: Clear (50%), LightRain (25%), Thunderstorm (15%), ForestFog (10%)
            int roll = Random.Range(0, 100);
            if (roll < 45) currentWeather = WeatherType.Clear;
            else if (roll < 75) currentWeather = WeatherType.LightRain;
            else if (roll < 90) currentWeather = WeatherType.Thunderstorm;
            else currentWeather = WeatherType.ForestFog;
        }

        private void TriggerLightning()
        {
            isLightningFlash = true;
            flashDuration = Random.Range(0.08f, 0.16f);
            AudioManager.PlayThunder();
        }

        private void EnsureTextures()
        {
            if (rainStreakTexture == null)
            {
                rainStreakTexture = new Texture2D(2, 8, TextureFormat.RGBA32, false);
                rainStreakTexture.filterMode = FilterMode.Point;
                for (int y = 0; y < 8; y++)
                {
                    float alpha = y / 8f;
                    rainStreakTexture.SetPixel(0, y, new Color(0.75f, 0.88f, 1f, alpha * 0.75f));
                    rainStreakTexture.SetPixel(1, y, new Color(0.6f, 0.8f, 0.95f, alpha * 0.5f));
                }
                rainStreakTexture.Apply();
            }

            if (fogTexture == null)
            {
                fogTexture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
                fogTexture.filterMode = FilterMode.Bilinear;
                for (int y = 0; y < 32; y++)
                {
                    for (int x = 0; x < 32; x++)
                    {
                        float n = Mathf.PerlinNoise(x * 0.15f, y * 0.15f);
                        fogTexture.SetPixel(x, y, new Color(0.85f, 0.92f, 0.96f, n * 0.35f));
                    }
                }
                fogTexture.Apply();
            }
        }

        private void OnGUI()
        {
            // Lightning full-screen flash
            if (isLightningFlash)
            {
                Color prevColor = GUI.color;
                GUI.color = new Color(0.92f, 0.96f, 1f, 0.65f);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
                GUI.color = prevColor;
            }

            // Rain streaks rendering
            if (rainIntensity > 0.05f)
            {
                EnsureTextures();
                Color prev = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, rainIntensity * 0.7f);

                float t = UnityEngine.Time.unscaledTime * 700f;
                int dropCount = Mathf.RoundToInt(rainIntensity * 55f);

                for (int i = 0; i < dropCount; i++)
                {
                    float seedX = (i * 137.5f) % Screen.width;
                    float seedY = (i * 211.3f + t) % (Screen.height + 60f) - 30f;
                    Rect dropRect = new Rect(seedX - (seedY * 0.2f), seedY, 3f, 22f);
                    GUI.DrawTexture(dropRect, rainStreakTexture);
                }

                GUI.color = prev;
            }

            // Forest Fog rendering
            if (currentWeather == WeatherType.ForestFog)
            {
                EnsureTextures();
                Color prev = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, 0.22f);

                float scrollX = (fogOffset * 100f) % Screen.width;
                GUI.DrawTexture(new Rect(scrollX - Screen.width, 0, Screen.width * 2f, Screen.height), fogTexture, ScaleMode.StretchToFill);
                GUI.color = prev;
            }
        }
    }
}
