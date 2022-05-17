namespace DrakersDev.Evaluation
{
    public abstract class FormulaToken
    {
        protected static void SetVariableValue<T>(IEnumerable<FormulaToken> tokenSource, String variableName, T value)
        {
            foreach (var eachToken in tokenSource)
            {
                if (eachToken is IVariableSettable varSettable)
                {
                    varSettable.SetVariableValue(variableName, value);
                }
                else if (eachToken is SingleValueVariable<T> varToken)
                {
                    if (varToken.VariableName.Equals(variableName))
                    {
                        varToken.SetVariableValue(value);
                    }
                }
            }
        }

        protected static void SetVariableValues<T>(IEnumerable<FormulaToken> tokenSource, String variableName, T[] values)
        {
            foreach (var eachToken in tokenSource)
            {
                if (eachToken is IVariableSettable varSettable)
                {
                    varSettable.SetVariableValues(variableName, values);
                }
                else if (eachToken is MultipleValueVariable<T> varToken)
                {
                    if (varToken.VariableName.Equals(variableName))
                    {
                        varToken.SetVariableValues(values);
                    }
                }
            }
        }

        protected static IEnumerable<String> GetVariableNames(IEnumerable<FormulaToken> tokenSource)
        {
            foreach (var eachToken in tokenSource)
            {
                if (eachToken is VariableToken varToken)
                {
                    yield return varToken.VariableName;
                }
                else if (eachToken is IVariableSettable varSettable)
                {
                    foreach (var eachName in varSettable.GetVariableNames())
                    {
                        yield return eachName;
                    }
                }
            }
        }

        protected static void SetFunction(IEnumerable<FormulaToken> tokenSource, FunctionManager funcManager)
        {
            foreach (var eachToken in tokenSource)
            {
                if (eachToken is IFunctionSettable funcSet)
                {
                    funcSet.SetFunction(funcManager);
                }
            }
        }

        public TokenType TokenType { get; private set; }

        private FormulaToken? leftToken = null;
        private FormulaToken? rightToken = null;
        public FormulaToken? LeftToken
        {
            get { return this.leftToken; }
            set
            {
                ValidateTokenToLinkable(value, nameof(this.LeftToken));
                if (this.leftToken != null)
                {
                    this.leftToken.rightToken = null;
                }
                if (value != null)
                {
                    if (value.RightToken != null)
                    {
                        value.RightToken.LeftToken = null;
                    }
                    value.rightToken = this;
                }
                this.leftToken = value;
            }
        }

        public FormulaToken? RightToken
        {
            get { return this.rightToken; }
            set
            {
                ValidateTokenToLinkable(value, nameof(this.RightToken));
                if (this.rightToken != null)
                {
                    this.rightToken.leftToken = null;
                }
                if (value != null)
                {
                    if (value.LeftToken != null)
                    {
                        value.LeftToken.RightToken = null;
                    }
                    value.leftToken = this;
                }
                this.rightToken = value;
            }
        }
        protected abstract void ValidateTokenToLinkable(FormulaToken? token, String propertyName);

        public Type EvalType { get { return GetEvalType(); } }
        protected abstract Type GetEvalType();

        public FormulaToken(TokenType tokenType)
        {
            this.TokenType = tokenType;
        }
        public abstract Double[] Eval();

        public IEnumerable<FormulaToken> GetLinkedTokens()
        {
            yield return this;
            if (this.rightToken != null)
            {
                foreach (var eachToken in this.rightToken.GetLinkedTokens())
                {
                    yield return eachToken;
                }
            }
        }

        internal virtual Object? ConvertToFunctionParameter()
        {
            return Eval();
        }
    }
}