using UnityEngine;

namespace TheOldRoad.Time
{
    /// <summary>Prototype in-game clock for the HUD. This is presentation/simulation time, not save time.</summary>
    public sealed class GameTimeController : MonoBehaviour
    {
        private const int MinutesPerDay = 24 * 60;

        [SerializeField, Min(1)] private int startDay = 1;
        [SerializeField, Range(0, 23)] private int startHour = 6;
        [SerializeField, Range(0, 59)] private int startMinute;
        [SerializeField, Min(0f)] private float gameMinutesPerRealSecond = 1.2f;

        private float elapsedGameMinutes;

        public int Day => startDay + Mathf.FloorToInt(TotalMinutes / MinutesPerDay);
        public int Hour => Mathf.FloorToInt(TotalMinutes % MinutesPerDay) / 60;
        public int Minute => Mathf.FloorToInt(TotalMinutes % 60);
        public string ClockText => "Day " + Day + "  " + Hour.ToString("00") + ":" + Minute.ToString("00");

        private float TotalMinutes => startHour * 60f + startMinute + elapsedGameMinutes;

        private void Update()
        {
            elapsedGameMinutes += UnityEngine.Time.deltaTime * gameMinutesPerRealSecond;
        }
    }
}
