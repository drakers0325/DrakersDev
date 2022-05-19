namespace DrakersDev
{
    public class ExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// 예외
        /// </summary>
        public Exception Exception { get; private set; }

        public ExceptionEventArgs(Exception ex)
        {
            this.Exception = ex;
        }
    }

    public class ExceptionEventArgs<TData> : DataEventArgs<TData>
    {
        /// <summary>
        /// 예외
        /// </summary>
        public Exception Exception { get; private set; }

        public ExceptionEventArgs(TData data, Exception ex) : base(data)
        {
            this.Exception = ex;
        }
    }
}