namespace DrakersDev
{
    public class BorderIntervalCounter : IntervalCounterBase
    {
        public BorderIntervalDirection CompareDirection { get; private set; }
        public Double Value { get; private set; }
        public Boolean IncludeBorder { get; private set; }

        public BorderIntervalCounter(String name, Double value, Boolean includeBorder, BorderIntervalDirection compareDirection)
            : base(name)
        {
            this.Value = value;
            this.IncludeBorder = includeBorder;
            this.CompareDirection = compareDirection;
        }

        protected override Boolean VerifyValue(Double value)
        {
            if (this.CompareDirection == BorderIntervalDirection.Upper)
            {
                return this.IncludeBorder ?
                    value >= this.Value :
                    value > this.Value;
            }
            else
            {
                return this.IncludeBorder ?
                    value <= this.Value :
                    value < this.Value;
            }
        }

        public override Boolean Equals(Object? obj)
        {
            if (obj is BorderIntervalCounter comp)
            {
                return this.Name.Equals(comp.Name) &&
                    this.Count == comp.Count &&
                    this.Value == comp.Value &&
                    this.IncludeBorder == comp.IncludeBorder &&
                    this.CompareDirection == comp.CompareDirection;
            }
            return false;
        }

        public override Int32 GetHashCode()
        {
            return HashCode.Combine(this.Name, this.Count, this.Value, this.IncludeBorder, this.CompareDirection);
        }
    }
}
