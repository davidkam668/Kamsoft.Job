using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kamsoft.Job
{
    #region 作业配置 JobConfig
    public class JobConfig
    {
        public string Name { get; }
        public bool ExecuteAtStart { get; }
        public TimeSpan? Interval { get; }
        public string[] DailyTimes { get; }
        public string[] SpecificTimes { get; }

        // 私有构造函数，只能通过Builder创建
        private JobConfig(
            string name,
            bool executeAtStart,
            TimeSpan? interval,
            string[] dailyTimes,
            string[] specificTimes)
        {
            Name = name;
            ExecuteAtStart = executeAtStart;
            Interval = interval;
            DailyTimes = dailyTimes ?? Array.Empty<string>();
            SpecificTimes = specificTimes ?? Array.Empty<string>();
        }

        #region 建造者类 Builder
        // 建造者类
        public class Builder
        {
            private string _name;
            private bool _executeAtStart;
            private TimeSpan? _interval;
            private string[] _dailyTimes = Array.Empty<string>();
            private string[] _specificTimes = Array.Empty<string>();

            public Builder WithName(string name)
            {
                _name = name;
                return this;
            }

            public Builder WithExecuteAtStart(bool executeAtStart)
            {
                _executeAtStart = executeAtStart;
                return this;
            }

            public Builder WithInterval(TimeSpan interval)
            {
                _interval = interval;
                return this;
            }

            public Builder WithDailyTimes(params string[] dailyTimes)
            {
                _dailyTimes = dailyTimes ?? Array.Empty<string>();
                return this;
            }

            public Builder WithSpecificTimes(params string[] specificTimes)
            {
                _specificTimes = specificTimes ?? Array.Empty<string>();
                return this;
            }

            public JobConfig Build()
            {
                // 检查 DailyTimes 时间格式
                for (int i = 0; i < _dailyTimes.Length; i++)
                {
                    TimeSpan parsedTime;
                    if (!TimeSpan.TryParse(_dailyTimes[i], out parsedTime))
                    {
                        throw new Exception($"【{_name}】无效的每天时间格式: {_dailyTimes[i]}");
                    }
                }
                // 检查 SpecificTimes 时间格式
                for (int i = 0; i < _specificTimes.Length; i++)
                {
                    DateTime parsedDate;
                    if (!DateTime.TryParse(_specificTimes[i], out parsedDate))
                    {
                        throw new Exception($"【{_name}】无效的指定日期时间格式: {_specificTimes[i]}");
                    }
                }

                return new JobConfig(
                    _name,
                    _executeAtStart,
                    _interval,
                    _dailyTimes,
                    _specificTimes);
            }
        }
        #endregion

    }
    #endregion
}