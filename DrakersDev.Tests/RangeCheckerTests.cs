using FluentAssertions;
using System;
using Xunit;

namespace DrakersDev.Tests
{
    public class RangeCheckerTests
    {
        [Theory]
        [InlineData(1, 0, 2)]
        [InlineData(2.3, 2, 3)]
        [InlineData(1.1, 1, 4)]
        public void IsInRange_ValueBetweenUpperAndLower_ReturnsTrue(Double value, Double low, Double high)
        {
            var checker = new RangeChecker<Double>(low, high);
            checker.IsInRange(value).Should().BeTrue();
        }

        [Theory]
        [InlineData(1, 0, 1)]
        [InlineData(2.1, 1, 2.1)]
        public void IsInRange_IncludeBorderAndValueEqualsUpper_ReturnsTrue(Double value, Double low, Double high)
        {
            var checker = new RangeChecker<Double>(low, high)
            {
                IncludeHighBorder = true,
                IncludeLowBorder = true,
            };
            checker.IsInRange(value).Should().BeTrue();
        }

        [Theory]
        [InlineData(1, 0, 1)]
        [InlineData(2.1, 1, 2.1)]
        public void IsInRange_NotIncludeBorderAndValueEqualsUpper_ReturnsFalse(Double value, Double low, Double high)
        {
            var checker = new RangeChecker<Double>(low, high)
            {
                IncludeHighBorder = false,
                IncludeLowBorder = false,
            };
            checker.IsInRange(value).Should().BeFalse();
        }

        [Theory]
        [InlineData(0, 0, 1)]
        [InlineData(1, 1, 2.1)]
        public void IsInRange_IncludeBorderAndValueEqualsLower_ReturnsTrue(Double value, Double low, Double high)
        {
            var checker = new RangeChecker<Double>(low, high)
            {
                IncludeHighBorder = true,
                IncludeLowBorder = true,
            };
            checker.IsInRange(value).Should().BeTrue();
        }

        [Theory]
        [InlineData(0, 0, 1)]
        [InlineData(1, 1, 2.1)]
        public void IsInRange_NotIncludeBorderAndValueEqualsLower_ReturnsFalse(Double value, Double low, Double high)
        {
            var checker = new RangeChecker<Double>(low, high)
            {
                IncludeHighBorder = false,
                IncludeLowBorder = false,
            };
            checker.IsInRange(value).Should().BeFalse();
        }

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(3, 4, 5)]
        public void IsInRange_ValueNotBetweenUpperAndLower_ReturnsFalse(Double value, Double low, Double high)
        {
            var checker = new RangeChecker<Double>(low, high)
            {
                IncludeHighBorder = false,
                IncludeLowBorder = false,
            };
            checker.IsInRange(value).Should().BeFalse();
        }
    }
}
