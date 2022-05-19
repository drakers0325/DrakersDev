namespace DrakersDev
{
    public abstract class RepeatOperationBase
    {
        private Task? task;
        private Thread? thread;
        private CancellationTokenSource? cts;
        private readonly IntervalCalculator intervalCalc = new(10);
        private RepeatOperationStage stage = RepeatOperationStage.Stop;

        protected readonly EventSet eventSet = new();
        private static readonly EventKey startKey = new(nameof(RepeatStart));
        private static readonly EventKey eachRepeatStartKey = new();
        private static readonly EventKey eachRepeatEndKey = new();
        private static readonly EventKey endKey = new(nameof(RepeatEnd));

        public event EventHandler<EventArgs> RepeatStart
        {
            add => this.eventSet.Add(startKey, value);
            remove => this.eventSet.Remove(startKey, value);
        }

        public event EventHandler<EventArgs> RepeatEnd
        {
            add => this.eventSet.Add(endKey, value);
            remove => this.eventSet.Remove(endKey, value);
        }

        public event EventHandler<EventArgs> EachRepeatStart
        {
            add => this.eventSet.Add(eachRepeatStartKey, value);
            remove => this.eventSet.Remove(eachRepeatStartKey, value);
        }

        public event EventHandler<EventArgs> EachRepeatEnd
        {
            add => this.eventSet.Add(eachRepeatEndKey, value);
            remove => this.eventSet.Remove(eachRepeatEndKey, value);
        }

        /// <summary>
        /// 동작중인지 여부
        /// </summary>
        public Boolean IsRunning => this.stage != RepeatOperationStage.Stop;

        /// <summary>
        /// 작업간 시간 간격(ms)
        /// </summary>
        public Int32 Interval { get; set; } = 100;

        /// <summary>
        /// 작업간 간격 계산 타입
        /// </summary>
        public RepeatIntervalCalculateType IntervalCalculateType
        {
            get => this.intervalCalc.IntervalCalculateType;
            set => this.intervalCalc.IntervalCalculateType = value;
        }

        /// <summary>
        /// 작업에 걸린 평균 시간
        /// </summary>
        public Double AverageOperationTime => this.intervalCalc.AverageMillisecondInterval;

        /// <summary>
        /// 최근 Sleep 시간
        /// </summary>
        public Double LatestSleepTime => this.intervalCalc.LatestSleepTime;

        /// <summary>
        /// 반복 작업 타입
        /// </summary>
        public RepeatOperationType OperationType { get; private set; } = RepeatOperationType.Task;

        /// <summary>
        /// 작업 스테이지
        /// </summary>
        public RepeatOperationStage OperationStage => this.IsRunning ? this.stage : RepeatOperationStage.Stop;

        protected abstract void Operation();
        protected abstract void PrepareTask();
        protected abstract void FinishTask();

        protected RepeatOperationBase()
        {
        }

        protected RepeatOperationBase(RepeatOperationType operationType)
        {
            this.OperationType = operationType;
        }

        /// <summary>
        /// 작업 시작
        /// </summary>
        public void Start()
        {
            if (this.OperationType == RepeatOperationType.Task)
            {
                if (this.task == null)
                {
                    this.cts = new CancellationTokenSource();
                    this.task = Task.Factory.StartNew(RepeatStub, this.cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }
            }
            else if (this.OperationType == RepeatOperationType.Thread)
            {
                if (this.thread == null)
                {
                    this.cts = new CancellationTokenSource();
                    this.thread = new Thread(RepeatStub)
                    {
                        IsBackground = true
                    };
                    this.thread.Start();
                }
            }
        }

        private void RepeatStub()
        {
            if (this.cts == null)
            {
                throw new ApplicationException("CancellationTokenSource가 만들어지지 않았습니다");
            }
            this.intervalCalc.ClearElapsedIntervals();
            this.stage = RepeatOperationStage.Start;
            this.eventSet.Raise(startKey, this, EventArgs.Empty);
            PrepareTask();
            this.stage = RepeatOperationStage.Repeat;
            while (!this.cts.IsCancellationRequested)
            {
                this.eventSet.Raise(eachRepeatStartKey, this, EventArgs.Empty);
                this.intervalCalc.Start();
                Operation();
                this.intervalCalc.Stop();
                this.eventSet.Raise(eachRepeatEndKey, this, EventArgs.Empty);
                this.intervalCalc.SleepRemain(this.Interval);
            }
            FinishTask();
            this.task = null;
            this.thread = null;
            this.cts = null;
            this.stage = RepeatOperationStage.Stop;
            this.eventSet.Raise(endKey, this, EventArgs.Empty);
        }

        /// <summary>
        /// 작업 중지
        /// </summary>
        public void Stop()
        {
            if (this.cts != null)
            {
                this.cts.Cancel();
            }
        }
    }
}
