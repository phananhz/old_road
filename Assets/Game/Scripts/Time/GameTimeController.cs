using UnityEngine;

namespace TheOldRoad.Time
{
    /// <summary>Prototype in-game clock for the HUD. This is presentation/simulation time, not save time.</summary>
    public sealed class GameTimeController : MonoBehaviour
    {
        private const int MinutesPerDay = 24 * 60;

        [SerializeField, Min(1)] private int startDay = 1;
        [SerializeField, Range(0, 23)] private int startHour = 6;
        [SerializeField, Range(0, 59)] private int startMinute = 0;
        [SerializeField, Min(0f)] private float gameMinutesPerRealSecond = 1.2f;

        private float elapsedGameMinutes;

        public int Day => startDay + Mathf.FloorToInt(TotalMinutes / MinutesPerDay);
        public int Hour => Mathf.FloorToInt(TotalMinutes % MinutesPerDay) / 60;
        public int Minute => Mathf.FloorToInt(TotalMinutes % 60);
        public string ClockText => "Day " + Day + "  " + Hour.ToString("00") + ":" + Minute.ToString("00");
        public float DayFraction => Mathf.Repeat(TotalMinutes, MinutesPerDay) / MinutesPerDay;
        public float SunlightIntensity => EvaluateSunlight(DayFraction);
        public int AbsoluteMinute => Mathf.FloorToInt(TotalMinutes);

        private float TotalMinutes => startHour * 60f + startMinute + elapsedGameMinutes;

        private void Update()
        {
            elapsedGameMinutes += UnityEngine.Time.deltaTime * gameMinutesPerRealSecond;
        }

        public void AdvanceHours(float hours)
        {
            elapsedGameMinutes += Mathf.Max(0f, hours) * 60f;
        }

        public void LoadAbsoluteMinute(int absoluteMinute)
        {
            elapsedGameMinutes = Mathf.Max(0f, absoluteMinute - (startHour * 60f + startMinute));
        }

        private static float EvaluateSunlight(float dayFraction)
        {
            float hour = dayFraction * 24f;
            if (hour < 5f) return 0.12f;
            if (hour < 7f) return Mathf.Lerp(0.12f, 0.78f, (hour - 5f) / 2f);
            if (hour < 11f) return Mathf.Lerp(0.78f, 1f, (hour - 7f) / 4f);
            if (hour < 15f) return 1f;
            if (hour < 19f) return Mathf.Lerp(1f, 0.18f, (hour - 15f) / 4f);
            if (hour < 21f) return Mathf.Lerp(0.18f, 0.12f, (hour - 19f) / 2f);
            return 0.12f;
        }
    }
}
