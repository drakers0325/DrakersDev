namespace DrakersDev.Evaluation
{
    public class SingleValueVariable<T> : VariableToken
    {
        public T? Value { get; private set; }

        public SingleValueVariable(String variableName) : base(variableName)
        {
        }
        public void SetVariableValue(T value)
        {
            this.Value = value;
        }

        protected override Type GetEvalType()
        {
            return typeof(T);
        }

        public override Double[] Eval()
        {
            return this.Value is Double convertValue ? new Double[] { convertValue } : Array.Empty<Double>();
        }

        internal override Object? ConvertToFunctionParameter()
        {
            return this.Value;
        }

        public override Boolean Equals(Object? obj)
        {
            if (obj is SingleValueVariable<T> comp)
            {
                return Equals(this.Value, comp.Value);
            }
            return false;
        }
        public override Int32 GetHashCode()
        {
            return HashCode.Combine(this.VariableName, this.Value);
        }
    }
}