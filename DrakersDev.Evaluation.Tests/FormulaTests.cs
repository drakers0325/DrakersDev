using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace DrakersDev.Evaluation.Tests
{
    public class FormulaTests
    {
        private struct Variable
        {
            public String Name { get; private set; }
            public Double[] Values { get; private set; }

            public Variable(String name, Double[] values)
            {
                this.Name = name;
                this.Values = values;
            }
        }

        private static void CheckFormulaTokenType(FormulaToken token, TokenType type)
        {
            token.TokenType.Should().Be(type);
        }

        private static void CheckFlotingPointNumericToken(FormulaToken token, Double value)
        {
            CheckFormulaTokenType(token, TokenType.Numeric);
            token.Should().BeOfType<NumericToken<Double>>();
            NumericToken<Double> nToken = (NumericToken<Double>)token;
            var evalValues = nToken.Eval();
            evalValues.Length.Should().Be(1);
            var tokenValue = evalValues[0];
            tokenValue.Should().Be(value);
            nToken.Value.Should().Be(value);
        }

        private static void CheckIntegerNumericToken(FormulaToken token, Int64 value)
        {
            CheckFormulaTokenType(token, TokenType.Numeric);
            NumericToken nToken;
            if (value > Int32.MaxValue)
            {
                token.Should().BeOfType<NumericToken<Int64>>();
                nToken = new NumericToken<Int64>(value);
                ((NumericToken<Int64>)nToken).Value.Should().Be(value);
            }
            else
            {
                token.Should().BeOfType<NumericToken<Int32>>();
                nToken = new NumericToken<Int32>((Int32)value);
                ((NumericToken<Int32>)nToken).Value.Should().Be((Int32)value);
            }
            var evalValues = nToken.Eval();
            evalValues.Length.Should().Be(1);
            var tokenValue = evalValues[0];
            tokenValue.Should().Be(value);

        }

        private static void CheckVariableToken(FormulaToken token, String variableName)
        {
            CheckFormulaTokenType(token, TokenType.Variable);
            VariableToken vToken = (VariableToken)token;
            vToken.VariableName.Should().Be(variableName);
        }

        [Fact]
        public void Eval_Throws_Exception_When_Formula_Is_Empty()
        {
            var formula = new Formula(String.Empty);
            var exAction = () => formula.Eval();
            exAction.Should().Throw<ApplicationException>()
                .WithMessage("계산 가능한 토큰이 없습니다");
        }

        [Theory]
        [InlineData("123.45", 123.45)]
        [InlineData("12345654321.789", 12345654321.789)]
        public void Can_Parse_FlotingPoint_Numeric_Expression(String numericExpression, Double value)
        {
            var formula = new Formula(numericExpression);
            var tokens = formula.Tokens;
            tokens.Length.Should().Be(1);
            CheckFlotingPointNumericToken(tokens[0], value);
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("123", 123)]
        [InlineData("5000000000", 5000000000)]
        public void Can_Parse_Integer_Numeric_Expression(String numericExpression, Int64 value)
        {
            var formula = new Formula(numericExpression);
            var tokens = formula.Tokens;
            tokens.Length.Should().Be(1);
            CheckIntegerNumericToken(tokens[0], value);
        }

        [Theory]
        [InlineData("변수")]
        [InlineData("변수1")]
        [InlineData("1변수")]
        [InlineData("variable")]
        [InlineData("variable1")]
        [InlineData("1variable")]
        public void Can_Parse_Variable_Expression(String variableName)
        {
            var formula = new Formula(variableName);
            var tokens = formula.Tokens;
            tokens.Length.Should().Be(1);
            CheckVariableToken(tokens[0], variableName);
        }


        private static void CheckCalculateResult(String expression, Double[] results)
        {
            var formula = new Formula(expression);
            var values = formula.Eval();
            values.Should().Equal(results);
        }

        private static void CheckCalculateResult(String expression, Double[] results, EvaluationAlignment evalAlign, params Variable[] vars)
        {
            var formula = new Formula(expression);
            formula.EvaluationAlignment = evalAlign;
            foreach (var eachVar in vars)
            {
                formula.SetVariableValues(eachVar.Name, eachVar.Values);
            }
            var values = formula.Eval();
            values.Should().Equal(results);
        }

        [Theory]
        [InlineData("1+2", 3)]
        [InlineData("1.2 + 2.5", 3.7)]
        [InlineData("1-2", -1)]
        [InlineData("1.2 - 2.5", -1.3)]
        [InlineData("1*2", 2)]
        [InlineData("1.2 * 2.5", 3)]
        [InlineData("1/2", 0.5)]
        [InlineData("1.2 / 2.5", 0.48)]
        public void Can_Calculate_Simple_Numeric_Formula(String expression, Double result)
        {
            CheckCalculateResult(expression, new Double[] { result });
        }

        [Theory]
        [InlineData("1+3*2.3-56/5", -3.3)]
        [InlineData("12.5 * 44 / 11  -3+ 21", 68)]
        [InlineData("(1+2)*1.2/ (10-4)", 0.6)]
        public void Can_Calculate_Complex_Numeric_Formula(String expression, Double result)
        {
            CheckCalculateResult(expression, new Double[] { result });
        }

        [Theory]
        [InlineData("변수+2.3", "변수", new Double[] { 1, 2 }, new Double[] { 3.3, 4.3 })]
        [InlineData("변수+변수", "변수", new Double[] { 1, 2 }, new Double[] { 2, 4 })]
        public void Can_Calculate_With_OneVariable(String expression, String variableName, Double[] variableValues, Double[] results)
        {
            CheckCalculateResult(expression, results, EvaluationAlignment.Left, new Variable(variableName, variableValues));
        }

        [Theory]
        [InlineData("변수1+변수2", "변수1", new Double[] { 1, 2 }, "변수2", new Double[] { 1, 2, 3 }, new Double[] { 2, 4 }, EvaluationAlignment.Left)]
        [InlineData("변수1+변수2", "변수1", new Double[] { 1, 2 }, "변수2", new Double[] { 1, 2, 3 }, new Double[] { 3, 5 }, EvaluationAlignment.Right)]
        public void Can_Calculate_With_EvaluationAlignment(String expression, String variable1Name, Double[] variable1Values, String variable2Name, Double[] variable2Values, Double[] results, EvaluationAlignment evalAlign)
        {
            CheckCalculateResult(expression, results, evalAlign,
                new Variable(variable1Name, variable1Values),
                new Variable(variable2Name, variable2Values));
        }

        [Fact]
        public void If_Doesnt_Exist_Left_Or_Right_Token_Remove_Parentheses()
        {
            var formula = new Formula("(((1+2)))");
            formula.ToString().Should().Be("1+2");
        }

        [Theory]
        [InlineData("3 *  (1+2)", "3*(1+2)")]
        [InlineData("(1 + 2) *3", "(1+2)*3")]
        public void If_Exist_LeftToken_Enclosed_In_Parentheses(String expression, String createdExpression)
        {
            var formula = new Formula(expression);
            formula.ToString().Should().Be(createdExpression);
        }

        [Fact]
        public void Equal_If_Each_Variable_Has_Same_Value()
        {
            var f1 = new Formula("1+변수1+arg3+Test(변수1, arg3)");
            var f2 = new Formula("1+a+b+Test(a,b)");
            f1.SetVariableValues("변수1", new Double[] { 1, 2 });
            f1.SetVariableValues("arg3", new Double[] { 1.3, 2.1, 4 });
            f2.SetVariableValues("a", new Double[] { 1, 2 });
            f2.SetVariableValues("b", new Double[] { 1.3, 2.1, 4 });
            f1.Should().Be(f2);
        }

        [Fact]
        public void Can_Get_VariableName()
        {
            var formula = new Formula("변수1+arg2/(Test(변수1, arg5) - 테스트)");
            var names = formula.GetVariableNames();
            names.Should().Equal("변수1", "arg2", "arg5", "테스트");
        }

        private class TestVariableFactory : IVariableTokenFactory
        {
            public VariableToken CreateVariableToken(String variableName)
            {
                return variableName.Equals("테스트변수", StringComparison.Ordinal)
                    ? new MultipleValueVariable<String>(variableName)
                    : new MultipleValueVariable<Double>(variableName);
            }
        }

        [Fact]
        public void Can_Set_Object_Variable()
        {
            var values = new String[] { "1", "테스트", "test" };
            var formula = new Formula("테스트변수 + Test(1, 테스트변수)", new TestVariableFactory());
            formula.SetVariableValues("테스트변수", values);
            var var1Token = (MultipleValueVariable<String>)formula.Tokens
                .Where(v => v.TokenType == TokenType.Variable && ((VariableToken)v).VariableName.Equals("테스트변수"))
                .First();
            var1Token.Values.Should().Equal(values);

            var var2Token = (MultipleValueVariable<String>)((FunctionToken)formula.Tokens
                .Where(v => v.TokenType == TokenType.Function)
                .First()).Arguments
                .Where(v => v.TokenType == TokenType.Variable && ((VariableToken)v).VariableName.Equals("테스트변수"))
                .First();
            var2Token.Values.Should().Equal(values);
        }
    }
}