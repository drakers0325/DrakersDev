namespace DrakersDev.Evaluation.Operator
{
    public class MultiplicationToken : OperatorToken
    {
        public MultiplicationToken() : base("*")
        {
        }

        protected override Double Calculate(Double left, Double right)
        {
            return left * right;
        }

        protected override Double[] CalculateRightToken(Double[] right)
        {
            throw new ApplicationException("오른쪽 항만 있으면 계산 할 수 없습니다");
        }
    }
}
