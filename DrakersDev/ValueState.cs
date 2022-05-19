namespace DrakersDev
{
    public abstract class ValueState
    {
        private readonly EventSet eventSet = new();
        private static readonly EventKey valueChangedKey = new();

        public event EventHandler<EventArgs> ValueChanged
        {
            add { this.eventSet.Add(valueChangedKey, value); }
            remove { this.eventSet.Remove(valueChangedKey, value); }
        }

        public String Name { get; private set; }

        public Boolean IsChanged { get; protected set; }

        public Boolean AllowRaiseEvent { get; set; }

        public ValueState(String name)
        {
            this.Name = name;
        }

        protected void RaiseValueChanged()
        {
            this.eventSet.Raise(valueChangedKey, this, EventArgs.Empty);
        }
    }

    public class ValueState<T> : ValueState
    {
        private T curValue;

        public T Value
        {
            get { return this.curValue; }
            set
            {
                this.PreviousValue = this.curValue;
                this.curValue = value;
                this.IsChanged = !Equals(curValue, this.PreviousValue);
                if (this.AllowRaiseEvent && this.IsChanged)
                {
                    RaiseValueChanged();
                }
            }
        }

        public T PreviousValue { get; private set; }

        public ValueState(T initialValue)
            : this(String.Empty, initialValue)
        {

        }

        public ValueState(String name, T initialValue)
            : base(name)
        {
            this.curValue = this.PreviousValue = initialValue;
        }

        public void ForceToRaiseValueChangedEvent()
        {
            RaiseValueChanged();
        }

        public Boolean IsPulseDetected(T value)
        {
            return this.IsChanged && Equals(this.Value, value);
        }

        public override Int32 GetHashCode()
        {
            return this.Value == null ? base.GetHashCode() : this.Value.GetHashCode();
        }

        public override Boolean Equals(Object? obj)
        {
            if (obj is ValueState<T> comp)
            {
                if (this.Name.Equals(comp.Name, StringComparison.Ordinal))
                {
                    return Equals(this.Value, comp.Value);
                }
            }
            return false;
        }
    }
}
