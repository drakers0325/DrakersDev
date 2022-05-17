using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace DrakersDev.Evaluation.Tests
{
    public class FunctionTokenTests
    {
        [Theory]
        [InlineData("Test()", "Test", new String[] { })]
        [InlineData("Test1(arg1, arg2)", "Test1", new String[] { "arg1", "arg2" })]
        [InlineData("Test2(arg1,arg2,123)", "Test2", new String[] { "arg1", "arg2", "123" })]
        public void Can_Parse_Function(String funcExp, String funcName, String[] argNames)
        {
            var func = new FunctionToken(funcExp);
            func.FunctionName.Should().Be(funcName);
            func.Arguments.Select(a => a.ToString()).Should().Equal(argNames);
        }

        [Theory]
        [InlineData("Test(  )", "Test()")]
        [InlineData("Test1(1,2)", "Test1(1,2)")]
        [InlineData("Test2(1,변수1  )", "Test2(1,변수1)")]
        public void ToString_Tests(String expression, String toString)
        {
            var funcToken = new FunctionToken(expression);
            funcToken.ToString().Should().Be(toString);
        }

        [Fact]
        public void Equal_If_Each_Variable_Has_Same_Value()
        {
            var f1 = new FunctionToken("Test(변수1, 변수2)");
            var f2 = new FunctionToken("Test(arg1, arg2)");
            f1.SetVariableValues("변수1", new Double[] { 1, 2 });
            f1.SetVariableValues("변수2", new Double[] { 1.1, 2.231 });
            f2.SetVariableValues("arg1", new Double[] { 1, 2 });
            f2.SetVariableValues("arg2", new Double[] { 1.1, 2.231 });
            f1.Should().Be(f2);
        }

        [Fact]
        public void Can_Get_VariableName()
        {
            var funcToken = new FunctionToken("Test(arg1, 1+arg2 / (변수3+수2))");
            var names = funcToken.GetVariableNames();
            names.Should().Equal("arg1", "arg2", "변수3", "수2");
        }

        [Theory]
        [InlineData("ReturnDoubleArray(1.2, 2.3)", new Double[] { 1.2, 2.3 })]
        [InlineData("SameName(1, 3)", new Double[] { 4 })]
        [InlineData("SameName(1.0, 3)", new Double[] { 3 })]
        public void Can_Calculate_Numer_Parameter_Function(String func, Double[] result)
        {
            var funcManager = new FunctionManager();
            funcManager.SetupFunctionList(typeof(TestFunctionClass));
            var funcToken = new FunctionToken(func);
            funcToken.SetFunction(funcManager);
            var expacted = funcToken.Eval();
            expacted.Should().Equal(result);
        }
    }
}
