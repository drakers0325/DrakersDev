using FluentAssertions;
using System;
using Xunit;

namespace DrakersDev.Tests
{
    public class DateTimeHelperTests
    {
        [Fact]
        public void Can_Get_Start_DateTime_Of_Date()
        {
            var dt = new DateTime(2022, 5, 18, 13, 12, 32, 0);
            DateTimeHelper.GetStartDateTime(dt).Should().Be(new DateTime(2022, 5, 18, 0, 0, 0, 0));
        }

        [Fact]
        public void Can_Get_End_DateTime_Of_Date()
        {
            var dt = new DateTime(2022, 5, 18, 13, 12, 32, 0);
            DateTimeHelper.GetEndDateTime(dt).Should().Be(new DateTime(2022, 5, 18, 23, 59, 59, 999));
        }
    }
}
