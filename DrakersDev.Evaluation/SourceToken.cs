namespace DrakersDev.Evaluation
{
    public abstract class SourceToken : FormulaToken
    {
        protected override void ValidateTokenToLinkable(FormulaToken? token, String propertyName)
        {
            if (token != null && token.TokenType != TokenType.Operator)
            {
                throw new ArgumentException($"{propertyName}으로 OperatorToken만 사용 할 수 있습니다");
            }
        }

        protected SourceToken(TokenType tokenType) : base(tokenType)
        {
        }
    }
}