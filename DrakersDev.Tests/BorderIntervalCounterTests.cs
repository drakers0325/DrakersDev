using FluentAssertions;
using System;
using Xunit;

namespace DrakersDev.Tests
{
    public class BorderIntervalCounterTests
    {
        [Theory]
        [InlineData(1, true, BorderIntervalDirection.Upper, 1)]
        [InlineData(1, true, BorderIntervalDirection.Lower, 1)]
        [InlineData(1, false, BorderIntervalDirection.Upper, 1.1)]
        [InlineData(1, false, BorderIntervalDirection.Lower, 0.9)]
        public void Increase_Count_If_Value_Verified(Double limitValue, Boolean includeBorder, BorderIntervalDirection dir, Double value)
        {
            var counter = new BorderIntervalCounter(limitValue.ToString(), limitValue, includeBorder, dir);
            counter.Count.Should().Be(0);
            counter.Verify(value);
            counter.Count.Should().Be(1);
        }
    }
}
