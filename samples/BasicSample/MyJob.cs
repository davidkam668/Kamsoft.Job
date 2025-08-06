using Kamsoft.Job;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BasicSample
{
    /// <summary>
    /// 测试作业1
    /// </summary>
    public class MyJob : JobBase
    {
        public MyJob()
        {
            // 创建配置
            var config = new JobConfig.Builder()
            .WithName("测试作业1")                                    //作业名称
            .WithExecuteAtStart(true)                                  //启动后执行一次
            .WithInterval(TimeSpan.FromSeconds(5))          //间隔时间
            //.WithDailyTimes("00:10", "12:10")                   //每天指定时间
            //.WithSpecificTimes("2025/12/31 01:00")         //指定时间
            .Build();

            Initialize(config);
        }

        #region 作业执行 Execute
        protected override async Task ExecuteAsync(CancellationToken token)
        {
            Logger.Info("执行作业中...");

            await Task.Delay(2000, token);

            Logger.Info("作业完成");
        }
        #endregion
    }
}
