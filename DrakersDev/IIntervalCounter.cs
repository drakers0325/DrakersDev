namespace DrakersDev
{
    public interface IIntervalCounter
    {
        Int64 Count { get; }
        String Name { get; }
        Boolean Verify(Double value);
        void Reset();
    }
}
