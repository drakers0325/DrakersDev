namespace DrakersDev
{
    public interface IIntervalCalculator
    {
        event EventHandler<DataEventArgs<IntervalTimeStamp>> AbnormalIntervalDetected;
        Double AbnormalIntervalRatio { get; set; }
        Double AverageMillisecondInterval { get; }
        Double ElapsedMilliseconds { get; }
        Double LatestMillisecondInterval { get; }
        RepeatIntervalCalculateType IntervalCalculateType { get; set; }
        IntervalTimeStamp? LongestInterval { get; }
        IntervalTimeStamp? ShortestInterval { get; }
        Int32 LatestSleepTime { get; }
        Int32 SamplingCount { get; }
        Int32 CurrentSampleCount { get; }
        void AddInterval(IIntervalCounter interval);
        IEnumerable<IIntervalCounter> IntervalCounters { get; }
        void Reset();
    }
}
