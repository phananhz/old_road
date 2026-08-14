namespace TheOldRoad.Time
{
    public interface IClock
    {
        long NowUnixSeconds { get; }
    }
}
