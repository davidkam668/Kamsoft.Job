using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kamsoft.Job
{
    //public abstract class JobBase<T> : IDisposable, IJob where T : JobBase<T>, new()
    public abstract class JobBase : IJob
    {
        #region Instance

        //private static readonly Lazy<T> instance = new Lazy<T>(() => new T());
        //public static T Instance => instance.Value;

        #endregion

        #region 构造函数 JobBase
        protected JobBase()
        {
            //// 子类需要通过Initialize方法初始化配置
            //Name = GetType().Name;
            //logger = LogManager.GetLogger(Name);

            Name = GetType().Name;
        }
        #endregion

        private CancellationTokenSource _cts;
        private Task _runningTask;
        private readonly object _startStopLock = new object();
        private DateTime[] _sortedSpecificTimes; // 预排序的指定时间
        private TimeSpan[] _sortedDailyTimes;   // 预排序的每天时间
        protected IJobLogger Logger { get; private set; }

        protected JobConfig Config { get; private set; }
        public string Name { get; set; }

        #region 作业状态 Status

        //public JobStatus Status { get; private set; } = JobStatus.NotStarted;

        private readonly object _statusLock = new object();
        private JobStatus _status = JobStatus.NotStarted;
        public JobStatus Status
        {
            get
            {
                lock (_statusLock)
                    return _status;
            }
            private set
            {
                lock (_statusLock)
                    _status = value;
            }
        }
        public string StatusDesc
        {
            get
            {
                switch (Status)
                {
                    case JobStatus.NotStarted:
                        return "未启动";
                    case JobStatus.Running:
                        return "运行中";
                    case JobStatus.Stopped:
                        return "已停止";
                    default:
                        return "未知状态";
                }
            }
        }
        #endregion

        #region 初始化 Initialize
        public void Initialize(JobConfig config, IJobLogger logger = null)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            Config = config;
            Name = config.Name ?? GetType().Name;
            Logger = logger ?? new ConsoleJobLogger(Name); // 默认使用 ConsoleLogger
            //Logger = logger ?? new NullLogger(); // 没传就用空日志器
        }

        #endregion

        #region 启动 Start
        public void Start()
        {
            StartAsync().GetAwaiter().GetResult();
        }
        #endregion

        #region 启动 StartAsync
        public async Task StartAsync()
        {
            lock (_startStopLock)
            {
                if (Status == JobStatus.Running)
                {
                    Logger.Info($"作业当前状态为【{StatusDesc}】不允许启动");
                    return;
                }

                if (Config == null)
                {
                    throw new InvalidOperationException($"【{Name}】作业配置未初始化，请先调用 Initialize 方法");
                }

                // 初始化时间数据
                InitializeTimes();

                Logger.Info($"正在启动作业...");
                _cts = new CancellationTokenSource();

                //必须放在这里,否则会有并发问题
                Status = JobStatus.Running;
                Logger.Info($"作业已启动");

                // 异步启动作业
                _runningTask = Task.Run(RunJob, _cts.Token);
            }
            await Task.Yield();
        }
        #endregion

        #region 停止 Stop
        public void Stop()
        {
            StopAsync().GetAwaiter().GetResult();
        }
        #endregion

        #region 异步停止 StopAsync
        public async Task StopAsync()
        {
            lock (_startStopLock)
            {
                if (Status != JobStatus.Running)
                {
                    Logger.Info($"作业当前状态为【{StatusDesc}】不允许停止");
                    return;
                }

                Logger.Info($"正在停止作业...");
                _cts?.Cancel();
            }

            if (_runningTask != null)
            {
                // 异步等待作业完成
                await _runningTask;
            }

            // 状态标记为已停止
            Status = JobStatus.Stopped;

            // 释放资源
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            _runningTask = null;
        }
        #endregion

        #region 运行作业 RunJob
        private async Task RunJob()
        {
            #region 启动后立即执行一次

            if (Config.ExecuteAtStart && !_cts.Token.IsCancellationRequested)
            {
                try
                {
                    Logger.Info($"启动后立即执行一次");
                    await ExecuteAsync(_cts.Token);
                }
                catch (OperationCanceledException)
                {
                    Logger.Info("任务已取消,作业将停止");
                }
                catch (Exception ex)
                {
                    Logger.Info($"启动后立即执行一次失败\r\n{ex.ToString()}");
                }
            } 

            #endregion

            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    DateTime? nextTime = GetNextTime();
                    if (!nextTime.HasValue)
                    {
                        Logger.Info("没有匹配到下一次作业执行时间,作业将停止");
                        break;
                    }

                    Logger.Info($"作业下次执行时间: {nextTime}");

                    var delay = nextTime.Value - DateTime.Now;
                    if (delay > TimeSpan.Zero)
                    {
                        var maxDelay = TimeSpan.FromMilliseconds(int.MaxValue);

                        // 若delay超出最大允许值，就分段等待
                        while (delay > maxDelay)
                        {
                            await Task.Delay(maxDelay, _cts.Token);
                            delay = nextTime.Value - DateTime.Now;
                        }

                        await Task.Delay(delay, _cts.Token);
                    }

                    if (!_cts.Token.IsCancellationRequested)
                    {
                        await ExecuteAsync(_cts.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    Logger.Info("任务已取消");
                }
                catch (Exception ex)
                {
                    Logger.Info($"作业执行失败\r\n{ex.ToString()}");
                    //延时一会,避免高频错误日志
                    await Task.Delay(2000, _cts.Token);
                }
            }

            Status = JobStatus.Stopped;
            Logger.Info("作业已停止");
        }
        #endregion

        #region 执行 ExecuteAsync
        protected abstract Task ExecuteAsync(CancellationToken token);
        #endregion

        #region 初始化时间数据 InitializeTimes
        private void InitializeTimes()
        {
            _sortedSpecificTimes = Config.SpecificTimes.Select(t => DateTime.Parse(t)).OrderBy(t => t).ToArray();
            _sortedDailyTimes = Config.DailyTimes.Select(t => TimeSpan.Parse(t)).OrderBy(t => t).ToArray();
        }
        #endregion

        #region 获取下一次执行时间 GetNextTime
        private DateTime? GetNextTime()
        {
            DateTime? nextTime = null;

            // 计算下次间隔执行时间
            if (Config.Interval.HasValue)
            {
                var intervalTime = DateTime.Now.Add(Config.Interval.Value);
                if (!nextTime.HasValue || intervalTime < nextTime)
                {
                    nextTime = intervalTime;
                }
            }

            // 计算下次每天指定时间的执行时间
            if (_sortedDailyTimes.Length > 0)
            {
                var now = DateTime.Now;
                var today = DateTime.Today;

                var nextDailyTimeToday = _sortedDailyTimes.FirstOrDefault(t => t > now.TimeOfDay);
                if (nextDailyTimeToday != TimeSpan.Zero)
                {
                    var nextTimeToday = today.Add(nextDailyTimeToday);
                    if (!nextTime.HasValue || nextTimeToday < nextTime)
                    {
                        nextTime = nextTimeToday;
                    }
                }
                else
                {
                    var nextDailyTimeTomorrow = _sortedDailyTimes.FirstOrDefault();
                    if (nextDailyTimeTomorrow != TimeSpan.Zero)
                    {
                        var nextTimeTomorrow = today.AddDays(1).Add(nextDailyTimeTomorrow);
                        if (!nextTime.HasValue || nextTimeTomorrow < nextTime)
                        {
                            nextTime = nextTimeTomorrow;
                        }
                    }
                }
            }

            // 计算下次指定时间的执行时间
            if (_sortedSpecificTimes.Length > 0)
            {
                var nextSpecificTime = _sortedSpecificTimes.FirstOrDefault(t => t > DateTime.Now);
                if (nextSpecificTime != DateTime.MinValue)
                {
                    if (!nextTime.HasValue || nextSpecificTime < nextTime)
                    {
                        nextTime = nextSpecificTime;
                    }
                }
            }

            return nextTime;
        }
        #endregion

        #region 释放资源 Dispose

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;
                _runningTask = null;
            }

            disposed = true;
        }

        public void Dispose()
        {
            Stop(); // 确保作业停止
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}