using DrakersDev.Evaluation.Operator;
using FluentAssertions;
using System;
using Xunit;

namespace DrakersDev.Evaluation.Tests.Operator
{
    public class AdditionTokenTests
    {
        [Fact]
        public void Eval_Returns_Empty_Result_When_Left_And_RightToken_Is_Null()
        {
            var addToken = new AdditionToken();
            addToken.LeftToken = addToken.RightToken = null;
            var exAction = () => addToken.Eval();
            exAction.Should().Throw<ApplicationException>()
                .WithMessage("양쪽항 모두 없으면 계산 할 수 없습니다");
        }

        [Fact]
        public void Eval_Throws_Exception_When_LeftToken_Is_Not_Null_And_RightToken_Is_Null()
        {
            var addToken = new AdditionToken();
            addToken.LeftToken = new NumericToken<Double>(1);
            addToken.RightToken = null;
            var exAction = () => addToken.Eval();
            exAction.Should().Throw<ApplicationException>()
                .WithMessage("왼쪽 항만 있으면 계산 할 수 없습니다");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2.234)]
        public void Can_Calculate_With_Only_RightNumericToken(Double value)
        {
            var opToken = new AdditionToken();
            var nToken = new NumericToken<Double>(value);
            opToken.RightToken = nToken;
            var values = opToken.Eval();
            values.Should().HaveCount(1);
            values[0].Should().Be(value);
        }

        [Theory]
        [InlineData(new Double[] { 1 }, new Double[] { 2 }, new Double[] { 3 })]
        [InlineData(new Double[] { 1.23 }, new Double[] { 2.67 }, new Double[] { 3.9 })]
        [InlineData(new Double[] { 1 }, new Double[] { 2, 3 }, new Double[] { 3, 4 })]
        [InlineData(new Double[] { 1, 2, 3 }, new Double[] { 2, 3 }, new Double[] { 3, 5 }, EvaluationAlignment.Left)]
        [InlineData(new Double[] { 1, 2, 3 }, new Double[] { 2, 3 }, new Double[] { 4, 6 }, EvaluationAlignment.Right)]
        public void Can_Add_Left_And_RightToken(Double[] left, Double[] right, Double[] result, EvaluationAlignment align = EvaluationAlignment.Left)
        {
            var addToken = new AdditionToken()
            {
                EvaluationAlignment = align
            };
            var leftToken = new MultipleValueVariable<Double>("왼쪽");
            leftToken.SetVariableValues(left);
            var rightToken = new MultipleValueVariable<Double>("오른쪽");
            rightToken.SetVariableValues(right);
            addToken.LeftToken = leftToken;
            addToken.RightToken = rightToken;
            var values = addToken.Eval();
            values.Should().Equal(result);
        }
    }
}