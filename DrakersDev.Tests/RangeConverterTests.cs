using FluentAssertions;
using System;
using Xunit;

namespace DrakersDev.Tests
{
    public class RangeConverterTests
    {
        [Theory]
        [InlineData(65, 135, 0, 4000, 0, 65)]
        [InlineData(65, 135, 0, 4000, 4000, 135)]
        [InlineData(65, 135, 0, 4000, 2000, 100)]
        public void ConvertToStandardUnit_NotInvertedRange_Test(Decimal stMin, Decimal stMax, Decimal compMin, Decimal compMax, Decimal compValue, Decimal standardValue)
        {
            var converter = new RangeConverter(stMin, stMax, compMin, compMax);
            converter.ConvertToStandardUnit(compValue).Should().Be(standardValue);
        }

        [Theory]
        [InlineData(65, 135, 0, 4000, 0, 65)]
        [InlineData(65, 135, 0, 4000, 4000, 135)]
        [InlineData(65, 135, 0, 4000, 2000, 100)]
        public void ConvertToComparisonUnit_NotInvertedRange_Test(Decimal stMin, Decimal stMax, Decimal compMin, Decimal compMax, Decimal compValue, Decimal standardValue)
        {
            var converter = new RangeConverter(stMin, stMax, compMin, compMax);
            converter.ConvertToComparisionUnit(standardValue).Should().Be(compValue);
        }

        [Theory]
        [InlineData(65, 135, 4000, 0, 0, 135)]
        [InlineData(65, 135, 4000, 0, 4000, 65)]
        [InlineData(65, 135, 4000, 0, 2000, 100)]
        public void ConvertToStandardUnit_InvertedRange_Test(Decimal stMin, Decimal stMax, Decimal compMin, Decimal compMax, Decimal compValue, Decimal standardValue)
        {
            var converter = new RangeConverter(stMin, stMax, compMin, compMax);
            converter.ConvertToStandardUnit(compValue).Should().Be(standardValue);
        }

        [Theory]
        [InlineData(65, 135, 4000, 0, 0, 135)]
        [InlineData(65, 135, 4000, 0, 4000, 65)]
        [InlineData(65, 135, 4000, 0, 2000, 100)]
        public void ConvertToComparisonUnit_InvertedRange_Test(Decimal stMin, Decimal stMax, Decimal compMin, Decimal compMax, Decimal compValue, Decimal standardValue)
        {
            var converter = new RangeConverter(stMin, stMax, compMin, compMax);
            converter.ConvertToComparisionUnit(standardValue).Should().Be(compValue);
        }
    }
}
