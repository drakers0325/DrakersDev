using FluentAssertions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Xunit;

namespace DrakersDev.Tests
{
    public class IntervalCalculatorTests
    {
        [Fact]
        public void Throws_Exception_When_SamplingCount_Less_Than_1()
        {
            var exAction = () =>
            {
                var calc = new IntervalCalculator(0);
            };
            exAction.Should().Throw<ArgumentException>().WithMessage("SamplingCount는 1보다 작을수 없습니다 (Parameter 'samplingCount')");
        }

        [Fact]
        public void Can_Calculate_Interval_Time()
        {
            var calc = new IntervalCalculator();
            var totalWatch = new Stopwatch();
            totalWatch.Start();
            calc.Start();
            Thread.Sleep(100);
            calc.Stop();
            totalWatch.Stop();
            calc.AverageMillisecondInterval.Should().BeGreaterThanOrEqualTo(100);
            totalWatch.Elapsed.TotalMilliseconds.Should().BeLessThan(115);
        }

        [Fact]
        public void Remove_Oldest_Sample_When_The_Number_Of_Stored_Samples_Exceeds_The_Set_Limit()
        {
            var calc = new IntervalCalculator(3);
            for (Int32 index = 0; index < 5; index++)
            {
                calc.Start();
                Thread.Sleep(1);
                calc.Stop();
            }
            calc.Start();
            Thread.Sleep(100);
            calc.Stop();
            calc.CurrentSampleCount.Should().Be(3);
            calc.AverageMillisecondInterval.Should().BeGreaterThanOrEqualTo(30);
            calc.AverageMillisecondInterval.Should().BeLessThanOrEqualTo(50);
        }

        [Fact]
        public void Can_Reset_Intervals()
        {
            var calc = new IntervalCalculator(3);
            calc.AddInterval(new RangeIntervalCounter("테스트", 0, 100, true, true));
            for (Int32 index = 0; index < 5; index++)
            {
                calc.Start();
                Thread.Sleep(1);
                calc.Stop();
            }
            var interval = calc.IntervalCounters.First();
            interval.Count.Should().Be(5);
            calc.CurrentSampleCount.Should().Be(3);
            calc.LatestMillisecondInterval.Should().NotBe(0);
            calc.AverageMillisecondInterval.Should().NotBe(0);
            calc.ElapsedMilliseconds.Should().NotBe(0);
            calc.ShortestInterval.Should().NotBeNull();
            calc.LongestInterval.Should().NotBeNull();
            calc.Reset();
            calc.CurrentSampleCount.Should().Be(0);
            calc.LatestMillisecondInterval.Should().Be(0);
            calc.AverageMillisecondInterval.Should().Be(0);
            calc.ElapsedMilliseconds.Should().Be(0);
            calc.ShortestInterval.Should().BeNull();
            calc.LongestInterval.Should().BeNull();
            interval.Count.Should().Be(0);
        }

        [Fact]
        public void Can_Set_Interval_Section_Include_Border()
        {
            var calc = new IntervalCalculator();
            IntervalCalculator.SetIntervalSection(calc, 100, 100, 2);
            var intervals = calc.IntervalCounters.ToArray();
            intervals.Should().HaveCount(4);
            intervals[0].Should().Be(new BorderIntervalCounter("<100ms", 100, false, BorderIntervalDirection.Lower));
            intervals[1].Should().Be(new RangeIntervalCounter("100-200ms", 100, 200));
            intervals[2].Should().Be(new RangeIntervalCounter("200-300ms", 200, 300));
            intervals[^1].Should().Be(new BorderIntervalCounter(">300ms", 300, false, BorderIntervalDirection.Upper));
        }

        [Fact]
        public void Can_Set_Interval_Section_Not_Include_High_Border()
        {
            var calc = new IntervalCalculator();
            IntervalCalculator.SetIntervalSection(calc, 100, 100, 2, false, true);
            var intervals = calc.IntervalCounters.ToArray();
            intervals.Should().HaveCount(3);
            intervals[0].Should().Be(new BorderIntervalCounter("<100ms", 100, false, BorderIntervalDirection.Lower));
            intervals[1].Should().Be(new RangeIntervalCounter("100-200ms", 100, 200));
            intervals[2].Should().Be(new RangeIntervalCounter("200-300ms", 200, 300));
        }

        [Fact]
        public void Can_Set_Interval_Section_Not_Include_Low_Border()
        {
            var calc = new IntervalCalculator();
            IntervalCalculator.SetIntervalSection(calc, 100, 100, 2, true, false);
            var intervals = calc.IntervalCounters.ToArray();
            intervals.Should().HaveCount(3);
            intervals[0].Should().Be(new RangeIntervalCounter("100-200ms", 100, 200));
            intervals[1].Should().Be(new RangeIntervalCounter("200-300ms", 200, 300));
            intervals[^1].Should().Be(new BorderIntervalCounter(">300ms", 300, false, BorderIntervalDirection.Upper));
        }

        [Fact]
        public void Can_Set_Interval_Section_Not_Include_Border()
        {
            var calc = new IntervalCalculator();
            IntervalCalculator.SetIntervalSection(calc, 100, 100, 2, false, false);
            var intervals = calc.IntervalCounters.ToArray();
            intervals.Should().HaveCount(2);
            intervals[0].Should().Be(new RangeIntervalCounter("100-200ms", 100, 200));
            intervals[1].Should().Be(new RangeIntervalCounter("200-300ms", 200, 300));
        }

        [Fact]
        public void Can_Set_Added_IntervalCounter_Count()
        {
            var calc = new IntervalCalculator(3);
            IntervalCalculator.SetIntervalSection(calc, 100, 100, 1);
            for (Int32 index = 0; index < 2; index++)
            {
                calc.Start();
                Thread.Sleep(1);
                calc.Stop();
            }

            for (Int32 index = 0; index < 2; index++)
            {
                calc.Start();
                Thread.Sleep(100);
                calc.Stop();
            }

            calc.Start();
            Thread.Sleep(200);
            calc.Stop();

            var counters = calc.IntervalCounters.ToArray();
            counters[0].Count.Should().Be(2);
            counters[1].Count.Should().Be(2);
            counters[2].Count.Should().Be(1);
        }

        [Fact]
        public void Raise_AbnormalIntervalDetectedEvent_When_Interval_Exceed_The_AverageInterval_Multiplied_By_AbnormalIntervalRatio()
        {
            IntervalTimeStamp? abnormalStamp = null;
            var calc = new IntervalCalculator(3);
            calc.AbnormalIntervalDetected += (s, e) =>
            {
                abnormalStamp = e.Data;
            };
            for (Int32 index = 0; index < 2; index++)
            {
                calc.Start();
                Thread.Sleep(1);
                calc.Stop();
            }
            calc.Start();
            Thread.Sleep(200);
            calc.Stop();

            abnormalStamp.Should().NotBeNull();
            if (abnormalStamp != null)
            {
                abnormalStamp.Milliseconds.Should().BeGreaterThanOrEqualTo(200);
            }
        }
    }
}
