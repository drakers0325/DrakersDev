using DrakersDev.Evaluation.Operator;
using FluentAssertions;
using System;
using Xunit;

namespace DrakersDev.Evaluation.Tests.Operator
{
    public class DivisionTokenTests
    {
        [Fact]
        public void Eval_Returns_Empty_Result_When_Left_And_RightToken_Is_Null()
        {
            var divToken = new DivisionToken();
            divToken.LeftToken = divToken.RightToken = null;
            var exAction = () => divToken.Eval();
            exAction.Should().Throw<ApplicationException>()
                .WithMessage("양쪽항 모두 없으면 계산 할 수 없습니다");
        }

        [Fact]
        public void Eval_Returns_Empty_Result_When_LeftToken_Is_Not_Null_And_RightToken_Is_Null()
        {
            var divToken = new DivisionToken();
            divToken.LeftToken = new NumericToken<Double>(1);
            divToken.RightToken = null;
            var exAction = () => divToken.Eval();
            exAction.Should().Throw<ApplicationException>()
                .WithMessage("왼쪽 항만 있으면 계산 할 수 없습니다");
        }

        [Fact]
        public void Eval_Returns_Empty_Result_When_LeftToken_Is_Null_And_RightToken_Is_Not_Null()
        {
            var divToken = new DivisionToken();
            divToken.LeftToken = null;
            divToken.RightToken = new NumericToken<Double>(1);
            var exAction = () => divToken.Eval();
            exAction.Should().Throw<ApplicationException>()
                .WithMessage("오른쪽 항만 있으면 계산 할 수 없습니다");
        }

        [Theory]
        [InlineData(new Double[] { 1 }, new Double[] { 2 }, new Double[] { 0.5 })]
        [InlineData(new Double[] { 2.64 }, new Double[] { 0.2 }, new Double[] { 13.2 })]
        [InlineData(new Double[] { 1 }, new Double[] { 2, 4 }, new Double[] { 0.5, 0.25 })]
        [InlineData(new Double[] { 7, 8, 9 }, new Double[] { 2, 4 }, new Double[] { 3.5, 2 }, EvaluationAlignment.Left)]
        [InlineData(new Double[] { 7, 8, 9 }, new Double[] { 2, 4 }, new Double[] { 4, 2.25 }, EvaluationAlignment.Right)]
        public void Can_Division_Left_And_RightToken(Double[] left, Double[] right, Double[] result, EvaluationAlignment align = EvaluationAlignment.Left)
        {
            var divToken = new DivisionToken()
            {
                EvaluationAlignment = align
            };
            var leftToken = new MultipleValueVariable<Double>("왼쪽");
            leftToken.SetVariableValues(left);
            var rightToken = new MultipleValueVariable<Double>("오른쪽");
            rightToken.SetVariableValues(right);
            divToken.LeftToken = leftToken;
            divToken.RightToken = rightToken;
            var values = divToken.Eval();
            values.Should().Equal(result);
        }
    }
}
