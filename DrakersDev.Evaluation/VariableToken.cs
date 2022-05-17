namespace DrakersDev.Evaluation
{
    public abstract class VariableToken : SourceToken
    {
        public String VariableName { get; private set; }

        public VariableToken(String variableName) : base(TokenType.Variable)
        {
            this.VariableName = variableName;
        }

        public override String ToString()
        {
            return this.VariableName;
        }
    }
}
