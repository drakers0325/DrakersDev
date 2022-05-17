namespace DrakersDev.Evaluation
{
    public abstract class NumericToken : SourceToken
    {
        public String? DisplayFormat { get; set; }

        public NumericToken() : base(TokenType.Numeric)
        {
        }
    }

    public class NumericToken<T> : NumericToken where T :
        struct,
        IComparable,
        IComparable<T>,
        IConvertible,
        IEquatable<T>,
        IFormattable
    {
        public T Value { get; private set; }

        public NumericToken(T value)
        {
            this.Value = value;
        }

        protected override Type GetEvalType()
        {
            return typeof(T);
        }

        public override Double[] Eval()
        {
            return new Double[] { Convert.ToDouble(this.Value) };
        }

        internal override Object? ConvertToFunctionParameter()
        {
            return this.Value;
        }

        public override String ToString()
        {
            return this.Value.ToString(this.DisplayFormat, null);
        }

        public override Boolean Equals(Object? obj)
        {
            if (obj is NumericToken<T> comp)
            {
                return this.Value.Equals(comp.Value);
            }
            return false;
        }

        public override Int32 GetHashCode()
        {
            return HashCode.Combine(this.Value);
        }
    }
}