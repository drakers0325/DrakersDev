namespace DrakersDev
{
    public class RangeIntervalCounter : IntervalCounterBase
    {
        private readonly RangeChecker<Double> checker = new(0, 0);

        public Double Low
        {
            get => this.checker.Low;
            set => this.checker.Low = value;
        }

        public Double High
        {
            get => this.checker.High;
            set => this.checker.High = value;
        }

        public Boolean IncludeLowBorder
        {
            get => this.checker.IncludeLowBorder;
            set => this.checker.IncludeLowBorder = value;
        }

        public Boolean IncludeHighBorder
        {
            get => this.checker.IncludeHighBorder;
            set => this.checker.IncludeHighBorder = value;
        }

        public RangeIntervalCounter(String name, Double low, Double high, Boolean includeLowBorder = true, Boolean includeHighBorder = true)
            : base(name)
        {
            this.Low = low;
            this.High = high;
            this.IncludeLowBorder = includeLowBorder;
            this.IncludeHighBorder = includeHighBorder;
        }

        protected override Boolean VerifyValue(Double value)
        {
            return this.checker.IsInRange(value);
        }

        public override Boolean Equals(Object? obj)
        {
            if (obj is RangeIntervalCounter comp)
            {
                return this.Name.Equals(comp.Name, StringComparison.Ordinal) &&
                    this.Count == comp.Count &&
                    this.High == comp.High &&
                    this.Low == comp.Low &&
                    this.IncludeHighBorder == comp.IncludeHighBorder &&
                    this.IncludeLowBorder == comp.IncludeLowBorder;
            }
            return false;
        }

        public override Int32 GetHashCode()
        {
            return HashCode.Combine(this.Name, this.Count, this.High, this.Low, this.IncludeHighBorder, this.IncludeLowBorder);
        }
    }
}
