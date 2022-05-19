namespace DrakersDev
{
    public sealed class EventSet
    {
        private readonly Dictionary<EventKey, Delegate> eventDic = new Dictionary<EventKey, Delegate>();

        public void Add(EventKey eventKey, Delegate handler)
        {
            Boolean lockTaken = false;
            Monitor.Enter(this.eventDic, ref lockTaken);
            try
            {
                this.eventDic.TryGetValue(eventKey, out var d);
                this.eventDic[eventKey] = Delegate.Combine(d, handler);
            }
            finally
            {
                Monitor.Exit(this.eventDic);
            }
        }

        public void Remove(EventKey eventKey, Delegate handler)
        {
            Boolean lockTaken = false;
            Monitor.Enter(this.eventDic, ref lockTaken);
            try
            {
                if (this.eventDic.TryGetValue(eventKey, out var d))
                {
                    d = Delegate.Remove(d, handler);
                    if (d != null)
                    {
                        this.eventDic[eventKey] = d;
                    }
                    else
                    {
                        this.eventDic.Remove(eventKey);
                    }
                }
            }
            finally
            {
                Monitor.Exit(this.eventDic);
            }
        }

        public void Raise(EventKey eventKey, Object sender, EventArgs e)
        {
            Boolean lockTaken = false;
            Delegate? d;
            Monitor.Enter(this.eventDic, ref lockTaken);
            try
            {
                this.eventDic.TryGetValue(eventKey, out d);
            }
            finally
            {
                Monitor.Exit(this.eventDic);
            }
            if (d != null)
            {
                d.DynamicInvoke(new Object[] { sender, e });
            }
        }
    }
}
