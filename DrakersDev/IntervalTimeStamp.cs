namespace DrakersDev
{
    public class IntervalTimeStamp
    {
        public DateTime TimeStamp { get; private set; }
        public Double Milliseconds { get; private set; }

        public IntervalTimeStamp(DateTime timeStamp, Double ms)
        {
            this.TimeStamp = timeStamp;
            this.Milliseconds = ms;
        }

        public override String ToString()
        {
            return $"{this.Milliseconds:N0}ms - {this.TimeStamp:yyyy/MM/dd HH:mm:ss}";
        }
    }
}
