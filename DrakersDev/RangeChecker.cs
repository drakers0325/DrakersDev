namespace DrakersDev
{
    public class RangeChecker<T> where T : IComparable<T>
    {
        public T Low { get; set; }
        public T High { get; set; }
        public Boolean IncludeLowBorder { get; set; }

        public Boolean IncludeHighBorder { get; set; }

        public RangeChecker(T low, T high)
        {
            this.Low = low;
            this.High = high;
            this.IncludeLowBorder = false;
            this.IncludeHighBorder = false;
        }


        public Boolean IsInRange(T value)
        {
            Int32 highResult = this.High.CompareTo(value);
            Int32 lowResult = this.Low.CompareTo(value);

            if (this.IncludeHighBorder)
            {
                return this.IncludeLowBorder ?
                    lowResult <= 0 && 0 <= highResult :
                    lowResult < 0 && 0 <= highResult;
            }
            else
            {
                return this.IncludeLowBorder ?
                    lowResult <= 0 && 0 < highResult :
                    lowResult < 0 && 0 < highResult;
            }
        }

        public override string ToString()
        {
            String? high = this.High == null ? String.Empty : this.High.ToString();
            String? low = this.Low == null ? String.Empty : this.Low.ToString();

            if (this.IncludeHighBorder)
            {
                return this.IncludeLowBorder ?
                    $"{low} <= Value <= {high}" :
                    $"{low} < Value <= {high}";
            }
            else
            {
                return this.IncludeLowBorder ?
                    $"{low} <= Value < {high}" :
                    $"{low} < Value < {high}";
            }
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}