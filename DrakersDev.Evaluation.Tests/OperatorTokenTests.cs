using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace DrakersDev.Evaluation.Tests
{
    public class OperatorTokenTests
    {
        private static readonly String[] arithmeticOps = new String[] { "+", "-", "*", "/" };

        [Fact]
        public void Allow_Only_Arithmetic_Operator()
        {
            for (Int32 value = 0; value <= 255; value++)
            {
                String opExp = ((Char)(Byte)value).ToString();
                var createAction = new Action(() =>
                {
                    var opToken = OperatorToken.CreateOperatorToken(opExp);
                });
                if (arithmeticOps.Contains(opExp))
                {
                    createAction.Should().NotThrow();
                }
                else
                {
                    createAction.Should().Throw<ApplicationException>()
                        .WithMessage($"'{opExp}'은 지원되지 않는 연산자 입니다");
                }
            }
        }

        #region LeftToken
        [Fact]
        public void If_LeftToken_Is_Set_They_Link_To_Each_Other()
        {
            foreach (var eachOp in arithmeticOps)
            {
                var opToken = OperatorToken.CreateOperatorToken(eachOp);
                var leftToken = new NumericToken<Double>(1);
                opToken.LeftToken = leftToken;
                opToken.LeftToken.Should().Be(leftToken);
                leftToken.RightToken.Should().Be(opToken);
            }
        }

        [Fact]
        public void When_LeftToken_Is_Setting_If_Prexist_LeftToken_Unlink_Preexist_LeftToken_And_Self()
        {
            foreach (var eachOp in arithmeticOps)
            {
                var opToken = OperatorToken.CreateOperatorToken(eachOp);
                var oldLeftToken = new NumericToken<Double>(1);
                opToken.LeftToken = oldLeftToken;
                opToken.LeftToken.Should().Be(oldLeftToken);
                oldLeftToken.RightToken.Should().Be(opToken);

                var newLeftToken = new NumericToken<Double>(2);
                opToken.LeftToken = newLeftToken;
                opToken.LeftToken.Should().Be(newLeftToken);
                newLeftToken.RightToken.Should().Be(opToken);
                oldLeftToken.RightToken.Should().BeNull();
            }
        }


        [Fact]
        public void LeftToken_Not_Allow_OperatorToken()
        {
            foreach (var eachOp in arithmeticOps)
            {
                var opToken = OperatorToken.CreateOperatorToken(eachOp);
                var leftToken = OperatorToken.CreateOperatorToken(eachOp);
                var setAct = () => opToken.LeftToken = leftToken;
                String paramName = nameof(opToken.LeftToken);
                setAct.Should()
                    .Throw<ArgumentException>()
                    .WithMessage($"{paramName}으로 OperatorToken을 사용 할 수 없습니다");
            }
        }

        [Fact]
        public void LeftToken_Allow_NumericToken()
        {
            foreach (var eachOp in arithmeticOps)
            {
                var opToken = OperatorToken.CreateOperatorToken(eachOp);
                var leftToken = new NumericToken<Double>(1);
                var setAct = () => opToken.LeftToken = leftToken;
                setAct.Should().NotThrow();
            }
        }

        [Fact]
        public void LeftToken_Allow_VariableToken()
        {
            foreach (var eachOp in arithmeticOps)
            {
                var opToken = OperatorToken.CreateOperatorToken(eachOp);
                var leftToken = new MultipleValueVariable<Double>("변수");
                var setAct = () => opToken.LeftToken = leftToken;
                setAct.Should().NotThrow();
            }
        }

        [Fact]
        public void LeftToken_Allow_Formula()
        {
            foreach (var eachOp in arithmeticOps)
            {
                var opToken = OperatorToken.CreateOperatorToken(eachOp);
                var leftToken = new Formula("1+2");
                var setAct = () => opToken.LeftToken = leftToken;
                setAct.Should().NotThrow();
            }
        }
        #endregion

        #region RightToken
        [Fact]
        public void RightToken_Not_Allow_OperatorToken()
        {
            foreach (var eachOp in arithmeticOps)
            {
                var opToken = OperatorToken.CreateOperatorToken(eachOp);
                var rightToken = OperatorToken.CreateOperatorToken(eachOp);
                var setAct = () => opToken.RightToken = rightToken;
                String paramName = nameof(opToken.RightToken);
                setAct.Should()
                    .Throw<ArgumentException>()
                    .WithMessage($"{paramName}으로 OperatorToken을 사용 할 수 없습니다");
            }
        }

        [Fact]
        public void RightToken_Allow_NumericToken()
        {
            foreach (var eachOp in arithmeticOps)
            {
                var opToken = OperatorToken.CreateOperatorToken(eachOp);
                var rightToken = new NumericToken<Double>(1);
                var setAct = () => opToken.RightToken = rightToken;
                setAct.Should().NotThrow();
            }
        }

        [Fact]
        public void RightToken_Allow_VariableToken()
        {
            foreach (var eachOp in arithmeticOps)
            {
                var opToken = OperatorToken.CreateOperatorToken(eachOp);
                var rightToken = new MultipleValueVariable<Double>("변수");
                var setAct = () => opToken.RightToken = rightToken;
                setAct.Should().NotThrow();
            }
        }

        [Fact]
        public void RightToken_Allow_Formula()
        {
            foreach (var eachOp in arithmeticOps)
            {
                var opToken = OperatorToken.CreateOperatorToken(eachOp);
                var rightToken = new Formula("1+2");
                var setAct = () => opToken.RightToken = rightToken;
                setAct.Should().NotThrow();
            }
        }
        #endregion

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("*")]
        [InlineData("/")]
        public void ToString_Returns_OperatorSymbol(String op)
        {
            var opToken = OperatorToken.CreateOperatorToken(op);
            opToken.ToString().Should().Be(op);
        }

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("*")]
        [InlineData("/")]
        public void Equal_If_Each_Variable_Has_Same_Values(String op)
        {
            var op1 = OperatorToken.CreateOperatorToken(op);
            var op2 = OperatorToken.CreateOperatorToken(op);
            op1.Should().Be(op2);
        }
    }
}