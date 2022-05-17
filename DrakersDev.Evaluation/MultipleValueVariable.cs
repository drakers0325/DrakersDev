namespace DrakersDev.Evaluation
{
    public class MultipleValueVariable<T> : VariableToken
    {
        private T[] values = Array.Empty<T>();
        public IEnumerable<T> Values => this.values;

        public MultipleValueVariable(String variableName) : base(variableName)
        {
        }

        public void SetVariableValues(T[] values)
        {
            this.values = values;
        }

        protected override Type GetEvalType()
        {
            return typeof(T[]);
        }

        public override Double[] Eval()
        {
            return this.values.Length == 0
                ? Array.Empty<Double>()
                : TypeChecker.IsNumber(GetType().GetGenericArguments()[0])
                    ? this.Values.Select(v => Convert.ToDouble(v)).ToArray()
                    : Array.Empty<Double>();
        }

        internal override Object? ConvertToFunctionParameter()
        {
            return this.values;
        }

        public override Boolean Equals(Object? obj)
        {
            if (obj is MultipleValueVariable<T> comp)
            {
                return Enumerable.SequenceEqual(this.Values, comp.Values);
            }
            return false;
        }
        public override Int32 GetHashCode()
        {
            return HashCode.Combine(this.VariableName, this.Values);
        }
    }
}
