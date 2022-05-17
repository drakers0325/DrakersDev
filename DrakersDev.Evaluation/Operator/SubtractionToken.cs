namespace DrakersDev.Evaluation.Operator
{
    public class SubtractionToken : OperatorToken
    {
        public SubtractionToken() : base("-")
        {

        }
        protected override Double Calculate(Double left, Double right)
        {
            return left - right;
        }

        protected override Double[] CalculateRightToken(Double[] right)
        {
            return right.Select(r => -r).ToArray();
        }
    }
}
