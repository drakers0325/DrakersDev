using FluentAssertions;
using System;
using System.ComponentModel;
using Xunit;

namespace DrakersDev.Tests
{
    public class EnumHelperTests
    {
        private enum TestEnum
        {
            [Description("테스트1")]
            Test1,
            Test2,
            [Description("테스트3")]
            Test3
        }

        [Fact]
        public void Can_Get_EnumValue_Description_Attribute_Value()
        {
            EnumHelper.GetEnumDescription(TestEnum.Test1).Should().Be("테스트1");
        }

        [Fact]
        public void Get_Self_ToString_Value_If_Do_Not_Have_Description_Attribute()
        {
            EnumHelper.GetEnumDescription(TestEnum.Test2).Should().Be("Test2");
        }

        [Fact]
        public void Can_Get_Description_Values_In_Specific_Enum_Type()
        {
            EnumHelper.GetEnumDescriptions<TestEnum>().Should().Equal(new String[] { "테스트1", "Test2", "테스트3" });
        }

        [Fact]
        public void Can_Get_Description_Values_That_Excluded_Specific_Values()
        {
            EnumHelper.GetEnumDescriptions(TestEnum.Test3).Should().Equal(new String[] { "테스트1", "Test2" });
        }

        [Fact]
        public void Can_Get_Enum_Value_By_Description_Value()
        {
            EnumHelper.GetEnumValue<TestEnum>("테스트1").Should().Be(TestEnum.Test1);
        }

        [Fact]
        public void Get_By_Self_ToString_Value_If_Do_Not_Have_Description_Attribute()
        {
            EnumHelper.GetEnumValue<TestEnum>("Test2").Should().Be(TestEnum.Test2);
        }

        [Fact]
        public void Throws_Exception_If_Do_Not_Have_Matched_Description()
        {
            var exAction = () => EnumHelper.GetEnumValue<TestEnum>("테스트");
            exAction.Should().Throw<ApplicationException>().WithMessage("'테스트'에 맞는 'DrakersDev.Tests.EnumHelperTests+TestEnum'열거형의 값이 없습니다");
        }
    }
}
