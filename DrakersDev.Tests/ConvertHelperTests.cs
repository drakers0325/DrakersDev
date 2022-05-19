using FluentAssertions;
using System;
using Xunit;

namespace DrakersDev.Tests
{
    public class ConvertHelperTests
    {
        [Theory]
        [InlineData("0A113CFF", new Byte[] { 0x0A, 0x11, 0x3C, 0xFF })]
        public void Convertible_HexString_To_Byte_Array(String hexString, Byte[] expact)
        {
            ConvertHelper.ConvertStringToByteArray(hexString).Should().Equal(expact);
        }

        [Theory]
        [InlineData(new Byte[] { 0x0a, 0x11, 0x3C, 0xFF }, "0A113CFF")]
        public void Convertible_Byte_Array_To_HexString(Byte[] bytes, String expact)
        {
            ConvertHelper.ConvertByteArrayToHexString(bytes).Should().Be(expact);
        }
    }
}
