using System.Reflection;
using System.Text;

namespace DrakersDev.Evaluation
{
    public class FunctionToken : SourceToken, IVariableSettable, IFunctionSettable
    {
        private MethodInfo? funcMethod;
        public String FunctionName { get; private set; }
        private readonly FormulaToken[] arguments;
        public IEnumerable<FormulaToken> Arguments => this.arguments;

        public FunctionToken(String functionExpression) : this(functionExpression, new MultipleDoubleVariableFactory())
        {
        }

        public FunctionToken(String functionExpression, IVariableTokenFactory variableFactory) : base(TokenType.Function)
        {
            this.FunctionName = GetFunctionName(functionExpression, out Int32 parenthesesIndex);
            this.arguments = GetArguments(functionExpression, parenthesesIndex + 1, variableFactory).ToArray();
        }

        private static String GetFunctionName(String funcExp, out Int32 startParenthesesIndex)
        {
            startParenthesesIndex = funcExp.IndexOf("(");
            return funcExp[..(startParenthesesIndex)];
        }

        private static IEnumerable<FormulaToken> GetArguments(String functionExp, Int32 startIndex, IVariableTokenFactory variableFactory)
        {
            if (functionExp.Contains(','))
            {
                Int32 endIndex = functionExp.LastIndexOf(")");
                var argueSection = functionExp[startIndex..endIndex];
                String[] comps = argueSection.Split(',', StringSplitOptions.TrimEntries);
                foreach (var eachComp in comps)
                {
                    if (String.IsNullOrEmpty(eachComp))
                    {
                        throw new ApplicationException($"'{functionExp}'에 잘못된 인수가 있습니다");
                    }
                    var formula = new Formula(eachComp, variableFactory);
                    yield return formula.Tokens.Length == 1 ?
                        formula.Tokens[0] :
                        formula;
                }
            }
        }

        protected override Type GetEvalType()
        {
            return typeof(Double[]);
        }

        public override Double[] Eval()
        {
            if (this.funcMethod == null)
            {
                throw new ApplicationException($"'{this}'를 계산할 함수가 설정되지 않았습니다");
            }
            var result = this.funcMethod.Invoke(null, CreateParameters());
            if (result == null)
            {
                throw new ApplicationException($"'{this}'의 계산값이 없습니다");
            }
            return (Double[])result;
        }

        private Object?[] CreateParameters()
        {
            var parameters = new Object?[this.arguments.Length];
            for (Int32 index = 0; index < parameters.Length; index++)
            {
                var arg = this.arguments[index];
                parameters[index] = arg.ConvertToFunctionParameter();
            }
            return parameters;
        }

        public void SetVariableValue<T>(String variableName, T value)
        {
            SetVariableValue(this.arguments, variableName, value);
        }

        public void SetVariableValues<T>(String variableName, T[] values)
        {
            SetVariableValues(this.arguments, variableName, values);
        }

        public String[] GetVariableNames()
        {
            return GetVariableNames(this.arguments).Distinct().ToArray();
        }

        public void SetFunction(FunctionManager funcManager)
        {
            this.funcMethod = funcManager.GetMethodInfo(this);
            SetFunction(this.arguments, funcManager);
        }

        public override String ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.FunctionName);
            sb.Append('(');
            sb.Append(String.Join(',', this.Arguments));
            sb.Append(')');
            return sb.ToString();
        }

        public override Boolean Equals(Object? obj)
        {
            if (obj is FunctionToken comp)
            {
                return this.FunctionName.Equals(comp.FunctionName) &&
                    Enumerable.SequenceEqual(this.arguments, comp.arguments);
            }
            return false;
        }

        public override Int32 GetHashCode()
        {
            Int32 hash = this.FunctionName.GetHashCode();
            foreach (var eachArg in this.arguments)
            {
                hash = HashCode.Combine(hash, eachArg.GetHashCode());
            }
            return hash;
        }
    }
}
