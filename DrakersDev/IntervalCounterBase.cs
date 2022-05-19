namespace DrakersDev
{
    public abstract class IntervalCounterBase : IIntervalCounter
    {
        public Int64 Count { get; private set; }

        public string Name { get; private set; }

        public IntervalCounterBase(String name)
        {
            this.Name = name;
            this.Count = 0;
        }

        public Boolean Verify(Double value)
        {
            if (VerifyValue(value))
            {
                IncreaseCount(1);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected abstract Boolean VerifyValue(Double value);

        public virtual void Reset()
        {
            this.Count = 0;
        }

        protected void IncreaseCount(Int64 count)
        {
            this.Count += count;
            if (this.Count < 0)
            {
                throw new OverflowException();
            }
        }

        public override String ToString()
        {
            return this.Name;
        }
    }
}
