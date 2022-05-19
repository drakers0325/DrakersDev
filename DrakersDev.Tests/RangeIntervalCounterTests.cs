using FluentAssertions;
using System;
using Xunit;

namespace DrakersDev.Tests
{
    public class RangeIntervalCounterTests
    {
        [Theory]
        [InlineData(1, 2, true, true, 1)]
        [InlineData(1, 2, true, true, 2)]
        [InlineData(1, 2, true, false, 1)]
        [InlineData(1, 2, false, true, 2)]
        [InlineData(1, 2, false, false, 1.5)]
        public void Increase_Count_If_Value_Verified(Double lowLimitValue, Double highLimitValue, Boolean includeLowBorder, Boolean includeHighBorder, Double value)
        {
            var counter = new RangeIntervalCounter("테스트", lowLimitValue, highLimitValue, includeLowBorder, includeHighBorder);
            counter.Count.Should().Be(0);
            counter.Verify(value);
            counter.Count.Should().Be(1);
        }
    }
}
