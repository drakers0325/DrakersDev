using System.Diagnostics;

namespace DrakersDev
{
    public sealed class IntervalCalculator : IIntervalCalculator
    {
        public static void SetIntervalSection(IIntervalCalculator checker, Int32 lowLimit, Int32 interval, Int32 intervalCount, Boolean addHighBorderInterval = true, Boolean addLowBorderInterval = true)
        {
            if (addLowBorderInterval)
            {
                checker.AddInterval(new BorderIntervalCounter($"<{lowLimit}ms", lowLimit, false, BorderIntervalDirection.Lower));
            }
            Int32 low = lowLimit;
            Int32 high = low + interval;

            for (Int32 index = 0; index < intervalCount; index++)
            {
                checker.AddInterval(new RangeIntervalCounter($"{low}-{high}ms", low, high));
                if (index < intervalCount - 1)
                {
                    low += interval;
                    high += interval;
                }
            }
            if (addHighBorderInterval)
            {
                checker.AddInterval(new BorderIntervalCounter($">{high}ms", high, false, BorderIntervalDirection.Upper));
            }
        }

        private readonly Stopwatch watch = new();
        private Double sampleIntervalSum = 0;
        private Double sampleIntervalCount = 0;
        private readonly Queue<Double> samplingQueue;
        private readonly List<IIntervalCounter> counterList = new();

        public IEnumerable<IIntervalCounter> IntervalCounters { get { return this.counterList; } }

        /// <summary>
        /// 흐른 시간(ms)
        /// </summary>
        public Double ElapsedMilliseconds { get { return this.watch.Elapsed.TotalMilliseconds; } }

        /// <summary>
        /// 작업간 간격 계산 타입
        /// </summary>
        public RepeatIntervalCalculateType IntervalCalculateType { get; set; }

        public Int32 SamplingCount { get; private set; }

        public Int32 CurrentSampleCount => this.samplingQueue.Count;

        /// <summary>
        /// 평균 간격(ms)
        /// </summary>
        public Double AverageMillisecondInterval { get; private set; }
        public Double LatestMillisecondInterval { get; private set; }

        public IntervalTimeStamp? LongestInterval { get; private set; }
        public IntervalTimeStamp? ShortestInterval { get; private set; }
        public Double AbnormalIntervalRatio { get; set; }
        public Int32 LatestSleepTime { get; private set; }

        private readonly EventSet eventSet = new();
        private static readonly EventKey abnormalDetectKey = new();
        public event EventHandler<DataEventArgs<IntervalTimeStamp>> AbnormalIntervalDetected
        {
            add { this.eventSet.Add(abnormalDetectKey, value); }
            remove { this.eventSet.Remove(abnormalDetectKey, value); }
        }

        public IntervalCalculator(Int32 samplingCount = 1000)
        {
            this.IntervalCalculateType = RepeatIntervalCalculateType.TotalPeriod;
            this.AbnormalIntervalRatio = 2;
            if (samplingCount < 1)
            {
                throw new ArgumentException("SamplingCount는 1보다 작을수 없습니다", nameof(samplingCount));
            }
            this.SamplingCount = samplingCount;
            this.samplingQueue = new Queue<Double>(this.SamplingCount);
        }

        public void AddInterval(IIntervalCounter interval)
        {
            this.counterList.Add(interval);
        }

        public void Reset()
        {
            Stop();
            this.watch.Reset();
            this.samplingQueue.Clear();
            this.LatestMillisecondInterval = 0;
            this.sampleIntervalSum = 0;
            this.sampleIntervalCount = 0;
            this.ShortestInterval = null;
            this.LongestInterval = null;
            this.AverageMillisecondInterval = 0;
            foreach (var eachInterval in this.counterList)
            {
                eachInterval.Reset();
            }
        }

        /// <summary>
        /// 측정 시작
        /// </summary>
        public void Start()
        {
            this.watch.Restart();
        }

        /// <summary>
        /// 측정 완료
        /// </summary>
        public void Stop()
        {
            if (this.watch.IsRunning)
            {
                this.watch.Stop();
                Double elapsedMs = this.ElapsedMilliseconds;
                this.samplingQueue.Enqueue(elapsedMs);
                this.LatestMillisecondInterval = elapsedMs;
                this.sampleIntervalSum += elapsedMs;
                this.sampleIntervalCount++;
                if (this.sampleIntervalCount > this.SamplingCount)
                {
                    Double removeValue = this.samplingQueue.Dequeue();
                    this.sampleIntervalSum -= removeValue;
                    this.sampleIntervalCount--;
                }
                this.AverageMillisecondInterval = (Double)(this.sampleIntervalSum == 0 ? 0 : this.sampleIntervalSum / this.sampleIntervalCount);
                CheckShortestInterval(elapsedMs);
                CheckLongestInterval(elapsedMs);
                CheckAbnormalInterval(elapsedMs);
                VerifyInterval(elapsedMs);
            }
        }

        private void CheckLongestInterval(Double ms)
        {
            if (this.LongestInterval == null || this.LongestInterval.Milliseconds < ms)
            {
                this.LongestInterval = new IntervalTimeStamp(DateTime.Now, ms);
            }
        }

        private void CheckShortestInterval(Double ms)
        {
            if (this.ShortestInterval == null || this.ShortestInterval.Milliseconds > ms)
            {
                this.ShortestInterval = new IntervalTimeStamp(DateTime.Now, ms);
            }
        }

        private void CheckAbnormalInterval(Double ms)
        {
            if (ms >= this.AverageMillisecondInterval * AbnormalIntervalRatio)
            {
                var value = new IntervalTimeStamp(DateTime.Now, ms);
                this.eventSet.Raise(abnormalDetectKey, this, new DataEventArgs<IntervalTimeStamp>(value));
            }
        }

        private void VerifyInterval(Double ms)
        {
            foreach (var eachInterval in this.counterList)
            {
                eachInterval.Verify(ms);
            }
        }

        /// <summary>
        /// 초기화
        /// </summary>
        public void ClearElapsedIntervals()
        {
            this.LongestInterval = null;
            this.ShortestInterval = null;
        }

        /// <summary>
        /// 해당 interval 사이에 남은 시간만큼 sleep
        /// </summary>
        /// <param name="interval">interval</param>
        public void SleepRemain(Int32 interval)
        {
            if (this.IntervalCalculateType == RepeatIntervalCalculateType.TotalPeriod)
            {
                Int32 sleep = interval - (Int32)this.ElapsedMilliseconds;
                if (sleep > 0)
                {
                    Thread.Sleep(sleep);
                }
                this.LatestSleepTime = sleep;
            }
            else if (this.IntervalCalculateType == RepeatIntervalCalculateType.AfterOperation)
            {
                Thread.Sleep(interval);
                this.LatestSleepTime = interval;
            }
        }
    }
}