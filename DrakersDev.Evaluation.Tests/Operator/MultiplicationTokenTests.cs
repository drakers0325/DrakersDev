using DrakersDev.Evaluation.Operator;
using FluentAssertions;
using System;
using Xunit;

namespace DrakersDev.Evaluation.Tests.Operator
{
    public class MultiplicationTokenTests
    {
        [Fact]
        public void Eval_Returns_Empty_Result_When_Left_And_RightToken_Is_Null()
        {
            var multiToken = new MultiplicationToken();
            multiToken.LeftToken = multiToken.RightToken = null;
            var exAction = () => multiToken.Eval();
            exAction.Should().Throw<ApplicationException>()
                .WithMessage("양쪽항 모두 없으면 계산 할 수 없습니다");
        }

        [Fact]
        public void Eval_Returns_Empty_Result_When_LeftToken_Is_Not_Null_And_RightToken_Is_Null()
        {
            var multiToken = new MultiplicationToken();
            multiToken.LeftToken = new NumericToken<Double>(1);
            multiToken.RightToken = null;
            var exAction = () => multiToken.Eval();
            exAction.Should().Throw<ApplicationException>()
                .WithMessage("왼쪽 항만 있으면 계산 할 수 없습니다");
        }

        [Fact]
        public void Eval_Returns_Empty_Result_When_LeftToken_Is_Null_And_RightToken_Is_Not_Null()
        {
            var multiToken = new MultiplicationToken();
            multiToken.LeftToken = null;
            multiToken.RightToken = new NumericToken<Double>(1);
            var exAction = () => multiToken.Eval();
            exAction.Should().Throw<ApplicationException>()
                .WithMessage("오른쪽 항만 있으면 계산 할 수 없습니다");
        }

        [Theory]
        [InlineData(new Double[] { 1 }, new Double[] { 2 }, new Double[] { 2 })]
        [InlineData(new Double[] { 1.23 }, new Double[] { 2.67 }, new Double[] { 3.2841 })]
        [InlineData(new Double[] { 1 }, new Double[] { 2, 3 }, new Double[] { 2, 3 })]
        [InlineData(new Double[] { 7, 8, 9 }, new Double[] { 2, 3 }, new Double[] { 14, 24 }, EvaluationAlignment.Left)]
        [InlineData(new Double[] { 7, 8, 9 }, new Double[] { 2, 3 }, new Double[] { 16, 27 }, EvaluationAlignment.Right)]
        public void Can_Multiplication_Left_And_RightToken(Double[] left, Double[] right, Double[] result, EvaluationAlignment align = EvaluationAlignment.Left)
        {
            var multiToken = new MultiplicationToken()
            {
                EvaluationAlignment = align
            };
            var leftToken = new MultipleValueVariable<Double>("왼쪽");
            leftToken.SetVariableValues(left);
            var rightToken = new MultipleValueVariable<Double>("오른쪽");
            rightToken.SetVariableValues(right);
            multiToken.LeftToken = leftToken;
            multiToken.RightToken = rightToken;
            var values = multiToken.Eval();
            values.Should().Equal(result);
        }
    }
}