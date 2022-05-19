namespace DrakersDev
{
    public sealed class EventKey
    {
        public String EventName { get; private set; }

        public EventKey()
        {
            this.EventName = String.Empty;
        }

        public EventKey(String eventName)
        {
            this.EventName = eventName;
        }
    }
}
