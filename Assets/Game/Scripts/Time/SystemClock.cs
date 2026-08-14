using System;

namespace TheOldRoad.Time
{
    public sealed class SystemClock : IClock
    {
        public long NowUnixSeconds => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}
