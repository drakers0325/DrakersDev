namespace DrakersDev.Evaluation.Operator
{
    public class AdditionToken : OperatorToken
    {
        public AdditionToken() : base("+")
        {
        }
        protected override Double Calculate(Double left, Double right)
        {
            return left + right;
        }

        protected override Double[] CalculateRightToken(Double[] right)
        {
            return right;
        }

    }
}