using System.Text;

namespace DrakersDev.Evaluation
{
    public class Formula : SourceToken, IVariableSettable, IFunctionSettable
    {
        private class CalculateToken : SourceToken
        {
            private readonly FormulaToken calToken;
            public CalculateToken(FormulaToken tokken) : base(TokenType.Variable)
            {
                this.calToken = tokken;
                if (this.calToken.LeftToken != null)
                {
                    this.LeftToken = this.calToken.LeftToken.LeftToken;
                }
                if (this.calToken.RightToken != null)
                {
                    this.RightToken = this.calToken.RightToken.RightToken;
                }
            }

            public override Double[] Eval()
            {
                return this.calToken.Eval();
            }

            protected override Type GetEvalType()
            {
                return typeof(Double[]);
            }
        }

        private static readonly Char[] operators = new char[] { '+', '-', '*', '/' };

        #region 공식 파싱
        private static IEnumerable<FormulaToken> Parse(String expression, IVariableTokenFactory variableFactory)
        {
            var sb = new StringBuilder();
            for (Int32 index = 0; index < expression.Length; index++)
            {
                var eachChar = expression[index];
                if (eachChar == ' ')
                {
                    continue;
                }
                if (operators.Contains(eachChar))
                {
                    if (sb.Length > 0)
                    {
                        yield return CreateNumerOrVariableToken(sb.ToString(), variableFactory);
                        sb.Clear();
                    }
                    yield return OperatorToken.CreateOperatorToken(eachChar.ToString());
                }
                else if (eachChar.Equals('('))
                {
                    CollectParenthesisSection(expression, index, sb, out Int32 readCount);
                    index += readCount - 1;
                    if (sb[0].Equals('('))
                    {
                        sb.Remove(0, 1);
                        sb.Remove(sb.Length - 1, 1);
                        yield return new Formula(sb.ToString(), variableFactory);
                    }
                    else
                    {
                        yield return new FunctionToken(sb.ToString(), variableFactory);
                    }
                    sb.Clear();
                }
                else
                {
                    sb.Append(eachChar);
                }
            }

            if (sb.Length != 0)
            {
                yield return CreateNumerOrVariableToken(sb.ToString(), variableFactory);
            }
        }

        private static void CollectParenthesisSection(String expression, Int32 startIndex, StringBuilder sb, out Int32 readCount)
        {
            Int32 count = 0;
            Int32 parenthesisCount = 0;
            for (Int32 index = startIndex; index < expression.Length; index++)
            {
                var eachChar = expression[index];
                if (eachChar.Equals('('))
                {
                    parenthesisCount++;
                }
                if (eachChar.Equals(')'))
                {
                    parenthesisCount--;
                }
                sb.Append(eachChar);
                count++;
                if (parenthesisCount == 0)
                {
                    break;
                }
            }
            if (parenthesisCount > 0)
            {
                throw new ApplicationException("공식의 괄호에 문제가 있습니다");
            }
            readCount = count;
        }

        private static FormulaToken CreateNumerOrVariableToken(String expression, IVariableTokenFactory variableFactory)
        {
            if (expression[0] == ')')
            {
                throw new ApplicationException("공식의 괄호에 문제가 있습니다");
            }
            var numToken = CreateNumericToken(expression);
            return numToken != null ?
                numToken : variableFactory.CreateVariableToken(expression);
        }

        private static NumericToken? CreateNumericToken(String expression)
        {
            if (Int32.TryParse(expression, out var int32Value))
            {
                return new NumericToken<Int32>(int32Value);
            }
            if (Int64.TryParse(expression, out var int64Value))
            {
                return new NumericToken<Int64>(int64Value);
            }
            if (Double.TryParse(expression, out var doubleValue))
            {
                return new NumericToken<Double>(doubleValue);
            }
            return null;
        }

        #endregion

        private readonly FormulaToken[] tokens;
        public FormulaToken[] Tokens => this.tokens;

        private EvaluationAlignment evalAlign = EvaluationAlignment.Left;
        public EvaluationAlignment EvaluationAlignment
        {
            get { return this.evalAlign; }
            set
            {
                this.evalAlign = value;
                foreach (var eachOp in this.tokens)
                {
                    if (eachOp is OperatorToken opToken)
                    {
                        opToken.EvaluationAlignment = this.evalAlign;
                    }
                }
            }
        }

        public Formula(String expression) : this(expression, new MultipleDoubleVariableFactory())
        {
        }

        public Formula(String expression, IVariableTokenFactory variableFactory) : base(TokenType.Formula)
        {
            this.tokens = Parse(RemoveParentheses(expression), variableFactory).ToArray();
        }

        private static String RemoveParentheses(String expression)
        {
            expression = expression.Trim();
            Int32 openCount = 0;
            foreach (var eachChar in expression)
            {
                if (eachChar == '(')
                {
                    openCount++;
                }
                else
                {
                    break;
                }
            }
            if (openCount > 0)
            {
                Int32 closeIndex = expression.IndexOf(')');
                if (closeIndex + openCount == expression.Length)
                {
                    String checkSection = expression.Substring(closeIndex, openCount);
                    String openStr = String.Concat(Enumerable.Repeat(")", openCount));
                    if (checkSection.Equals(openStr))
                    {
                        expression = expression[..closeIndex];
                        expression = expression[openCount..];
                    }
                }
            }
            return expression;
        }

        private void SetTokenLink()
        {
            if (this.tokens.Length > 0)
            {
                FormulaToken? prevToken = null;
                for (Int32 index = 0; index < this.tokens.Length - 1; index++)
                {
                    var curToken = this.tokens[index];
                    curToken.LeftToken = prevToken;
                    curToken.RightToken = this.tokens[index + 1];
                    prevToken = curToken;
                }
            }
        }

        protected override Type GetEvalType()
        {
            return typeof(Double[]);
        }

        public override Double[] Eval()
        {
            if (this.tokens.Length == 0)
            {
                throw new ApplicationException("계산 가능한 토큰이 없습니다");
            }

            SetTokenLink();
            var tokenList = new List<FormulaToken>(this.tokens[0].GetLinkedTokens());
            var calToken = CreateCalculateToken(tokenList, 1);
            var values = calToken.Eval();
            SetTokenLink();
            return values;
        }

        #region 계산
        private CalculateToken CreateCalculateToken(IList<FormulaToken> tokenList, Int32 calculateSequnce)
        {
            Boolean isHandled = false;
            String[] operators = calculateSequnce == 1 ?
                new String[] { "*", "/" } : new String[] { "+", "-" };
            for (Int32 index = 0; index < tokenList.Count; index++)
            {
                var curToken = tokenList[index];
                if (curToken is OperatorToken opToken)
                {
                    if (operators.Contains(opToken.Operator))
                    {
                        ReplaceOperatorTokenToCalculateToken(tokenList, index);
                        isHandled = true;
                        break;
                    }
                }
            }

            if (tokenList.Count == 1)
            {
                var returnToken = tokenList[0];
                return returnToken is CalculateToken ? (CalculateToken)tokenList[0] : new CalculateToken(returnToken);
            }
            else
            {
                if (!isHandled)
                {
                    calculateSequnce++;
                }
                return CreateCalculateToken(tokenList, calculateSequnce);
            }
        }

        private static void ReplaceOperatorTokenToCalculateToken(IList<FormulaToken> tokenList, Int32 index)
        {
            var token = tokenList[index];
            if (token.RightToken != null)
            {
                tokenList.RemoveAt(index + 1);
            }
            tokenList.RemoveAt(index);
            if (token.LeftToken != null)
            {
                tokenList.RemoveAt(index - 1);
            }
            var calToken = new CalculateToken(token);
            if (tokenList.Count <= 0)
            {
                tokenList.Add(calToken);
            }
            else
            {
                tokenList.Insert(index - 1, calToken);
            }
        }
        #endregion

        public void SetVariableValue<T>(String variableName, T value)
        {
            SetVariableValue<T>(this.tokens, variableName, value);
        }

        public void SetVariableValues<T>(String variableName, T[] values)
        {
            SetVariableValues(this.tokens, variableName, values);
        }

        public String[] GetVariableNames()
        {
            return GetVariableNames(this.tokens).Distinct().ToArray();
        }
        public void SetFunction(FunctionManager funcManager)
        {
            SetFunction(this.tokens, funcManager);
        }

        public override String ToString()
        {
            SetTokenLink();
            var sb = new StringBuilder();
            foreach (var eachToken in this.tokens)
            {
                sb.Append(eachToken.ToString());
            }
            if (this.LeftToken != null || this.RightToken != null)
            {
                sb.Insert(0, '(');
                sb.Append(')');
            }
            return sb.ToString();
        }

        public override Boolean Equals(Object? obj)
        {
            if (obj is Formula comp)
            {
                return Enumerable.SequenceEqual(this.tokens, comp.tokens);
            }
            return false;
        }

        public override Int32 GetHashCode()
        {
            Int32 hash = 1;
            foreach (var eachToken in this.tokens)
            {
                hash = HashCode.Combine(hash, eachToken.GetHashCode());
            }
            return hash;
        }
    }
}
