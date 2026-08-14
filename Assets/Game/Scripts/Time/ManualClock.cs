namespace TheOldRoad.Time
{
    public sealed class ManualClock : IClock
    {
        public ManualClock(long nowUnixSeconds)
        {
            NowUnixSeconds = nowUnixSeconds;
        }

        public long NowUnixSeconds { get; private set; }

        public void AdvanceSeconds(long seconds)
        {
            NowUnixSeconds += seconds;
        }
    }
}
