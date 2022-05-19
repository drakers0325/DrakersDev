using FluentAssertions;
using System;
using Xunit;

namespace DrakersDev.Tests
{
    public class IntervalCounterBaseTests
    {
        private class TestCounter : IntervalCounterBase
        {
            public TestCounter(String name = "Test") : base(name)
            {
            }

            public void AddCount(Int64 value)
            {
                IncreaseCount(value);
            }

            protected override Boolean VerifyValue(Double value)
            {
                return true;
            }
        }

        [Fact]
        public void Throws_OverflowException_If_Count_Over_Int64_MaxValue()
        {
            var counter = new TestCounter();
            counter.AddCount(Int64.MaxValue);
            var exception = Assert.Throws<OverflowException>(() => counter.Verify(0));
            exception.Should().BeOfType<OverflowException>();
        }

        [Fact]
        public void ToString_Returns_Name()
        {
            String name = "테스트이름";
            var counter = new TestCounter(name);
            counter.ToString().Should().Be(name);
        }
    }
}
