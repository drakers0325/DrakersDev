namespace DrakersDev
{
    public class DataEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 데이터
        /// </summary>
        public T? Data { get; private set; }
        public DataEventArgs(T? data)
        {
            this.Data = data;
        }
    }
}