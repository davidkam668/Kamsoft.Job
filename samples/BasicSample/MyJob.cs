using Kamsoft.Job;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BasicSample
{
    public class MyJob : JobBase
    {
        public MyJob()
        {
            // 创建配置
            var config = new JobConfig.Builder()
                .WithName("我的作业")// 作业名称
                .WithExecuteAtStart(true)// 启动后执行一次
                .WithInterval(TimeSpan.FromSeconds(5))// 间隔时间
                //.WithDailyTimes("04:15", "11:55")// 每天时间点
                //.WithSpecificTimes("2025/08/07 11:54")// 指定日期时间
                .Build();

            Initialize(config);
        }

        #region 作业执行 ExecuteAsync
        protected override async Task ExecuteAsync(CancellationToken token)
        {
            Console.WriteLine("执行作业中...");
            await Task.Delay(2000, token);
            Console.WriteLine("作业完成");
        }
        #endregion
    }
}
