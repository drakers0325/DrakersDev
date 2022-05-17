using DrakersDev.Evaluation.Operator;

namespace DrakersDev.Evaluation
{
    public abstract class OperatorToken : FormulaToken
    {
        public static OperatorToken CreateOperatorToken(String operatorExpression)
        {
            switch (operatorExpression)
            {
                case "+":
                    return new AdditionToken();
                case "-":
                    return new SubtractionToken();
                case "*":
                    return new MultiplicationToken();
                case "/":
                    return new DivisionToken();
                default:
                    throw new ApplicationException($"'{operatorExpression}'은 지원되지 않는 연산자 입니다");
            }
        }

        public static Boolean IsOperator(String operatorExpression)
        {
            try
            {
                CreateOperatorToken(operatorExpression);
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected static (Double[], Double[]) GetCalculateableValues(Double[] left, Double[] right, EvaluationAlignment align)
        {
            Int32 min = Math.Min(left.Length, right.Length);
            var calLeft = new Double[min];
            var calRight = new Double[min];
            if (align == EvaluationAlignment.Left)
            {
                Array.Copy(left, 0, calLeft, 0, min);
                Array.Copy(right, 0, calRight, 0, min);
            }
            else
            {
                Array.Copy(left, left.Length - min, calLeft, 0, min);
                Array.Copy(right, right.Length - min, calRight, 0, min);
            }
            return (calLeft, calRight);
        }


        protected override void ValidateTokenToLinkable(FormulaToken? token, String propertyName)
        {
            if (token != null && token.TokenType == TokenType.Operator)
            {
                throw new ArgumentException($"{propertyName}으로 OperatorToken을 사용 할 수 없습니다");
            }
        }

        public String Operator { get; private set; }
        public EvaluationAlignment EvaluationAlignment { get; set; } = EvaluationAlignment.Left;

        protected OperatorToken(String operatorExpression) : base(TokenType.Operator)
        {
            this.Operator = operatorExpression;
        }

        protected override Type GetEvalType()
        {
            return typeof(Double[]);
        }

        public override Double[] Eval()
        {
            if (this.LeftToken == null)
            {
                if (this.RightToken == null)
                {
                    throw new ApplicationException("양쪽항 모두 없으면 계산 할 수 없습니다");
                }
                else
                {
                    return CalculateRightToken(this.RightToken.Eval());
                }
            }
            else
            {
                if (this.RightToken == null)
                {
                    throw new ApplicationException("왼쪽 항만 있으면 계산 할 수 없습니다");
                }
                else
                {
                    var left = this.LeftToken.Eval();
                    var right = this.RightToken.Eval();
                    return Calculate(left, right);
                }
            }
        }

        private Double[] Calculate(Double[] left, Double[] right)
        {
            if (left.Length == 1 || right.Length == 1)
            {
                var result = new Double[Math.Max(left.Length, right.Length)];
                Int32 index = 0;
                foreach (var eachLeft in left)
                {
                    foreach (var eachRight in right)
                    {
                        result[index] = Calculate(eachLeft, eachRight);
                        index++;
                    }
                }
                return result;
            }
            else
            {
                var (calLeft, calRight) = GetCalculateableValues(left, right, this.EvaluationAlignment);
                return Enumerable.Range(0, calLeft.Length)
                    .Select(index => Calculate(calLeft[index], calRight[index])).ToArray();
            }
        }

        protected abstract Double Calculate(Double left, Double right);

        protected abstract Double[] CalculateRightToken(Double[] right);

        public override String ToString()
        {
            return this.Operator;
        }

        public override Boolean Equals(Object? obj)
        {
            if (obj is OperatorToken comp)
            {
                return this.Operator.Equals(comp.Operator);
            }
            return false;
        }

        public override Int32 GetHashCode()
        {
            return HashCode.Combine(this.Operator);
        }
    }
}