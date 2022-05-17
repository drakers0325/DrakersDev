using FluentAssertions;
using System;
using Xunit;

namespace DrakersDev.Evaluation.Tests
{
    public class MultipleDoubleValueVariableTests
    {
        [Theory]
        [InlineData(new Double[] { 1, 2, 3 })]
        [InlineData(new Double[] { 1.23, 2.34, 3.45 })]
        public void Can_Set_Variable_Value(Double[] values)
        {
            var varToken = new MultipleValueVariable<Double>("변수");
            varToken.SetVariableValues(values);
            varToken.Eval().Should().Equal(values);
        }

        [Theory]
        [InlineData("변수1")]
        [InlineData("arg1")]
        public void ToString_Returns_VariableName(String variableName)
        {
            var varToken = new MultipleValueVariable<Double>(variableName);
            varToken.ToString().Should().Be(variableName);
        }

        [Fact]
        public void Equal_If_Each_Variable_Has_Same_Values()
        {
            var var1 = new MultipleValueVariable<Double>("변수1");
            var var2 = new MultipleValueVariable<Double>("변수2");
            var1.SetVariableValues(new Double[] { 1, 1.2, 3.214 });
            var2.SetVariableValues(new Double[] { 1, 1.2, 3.214 });
            var1.Should().Be(var2);
        }
    }
}
