using FluentAssertions;
using System;
using Xunit;

namespace DrakersDev.Evaluation.Tests
{
    public class NumericTokenTests
    {
        [Fact]
        public void Eval_Returns_Constructor_Parameter_Value()
        {
            Double value = 123.123;
            var numToken = new NumericToken<Double>(value);
            var values = numToken.Eval();
            values.Should().HaveCount(1);
            values.Should().Equal(value);
            numToken.Value.Should().Be(value);
        }

        [Theory]
        [InlineData(12345, "N0", "12,345")]
        [InlineData(12.432, "F2", "12.43")]
        public void ToString_Returns_Applied_Format(Double num, String format, String toString)
        {
            var numToken = new NumericToken<Double>(num)
            {
                DisplayFormat = format
            };
            numToken.ToString().Should().Be(toString);
        }

        [Fact]
        public void Equals_If_Each_Has_Same_Value()
        {
            var num1 = new NumericToken<Double>(12.12);
            var num2 = new NumericToken<Double>(12.12);
            num1.Should().Be(num2);
        }
    }
}
