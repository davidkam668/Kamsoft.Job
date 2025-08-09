using System;

namespace Kamsoft.Job
{
    public static class JobFactory
    {
        /// <summary>
        /// 使用配置和日志器创建 Job
        /// </summary>
        public static T Create<T>(JobConfig config, IJobLogger logger)
            where T : JobBase, new()
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            //if (logger == null) throw new ArgumentNullException(nameof(logger));

            var job = new T();
            job.Initialize(config, logger);
            return job;
        }

        /// <summary>
        /// 简化构造配置（仅名称+间隔），必须传入日志器
        /// </summary>
        public static T Create<T>(string name, TimeSpan interval, bool executeAtStart, IJobLogger logger)
            where T : JobBase, new()
        {
            var config = new JobConfig.Builder()
                .WithName(name)
                .WithInterval(interval)
                .WithExecuteAtStart(executeAtStart)
                .Build();

            return Create<T>(config, logger);
        }
    }
}
