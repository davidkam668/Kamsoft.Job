# Kamsoft.Job

轻量级作业调度框架，支持灵活的定时执行、任务注入、日志解耦等特性，适用于服务作业场景。

## ✨ 特性

- 支持多种调度方式：固定间隔、每天特定时间、指定时间点
- 支持任务取消和异常捕获
- 解耦日志依赖（支持接入 NLog、Serilog 或自定义日志）
- 支持使用单例或多实例任务方式

## 🚀 快速开始

```csharp
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
                .WithName("测试作业1")// 作业名称
                .WithExecuteAtStart(true)// 启动后执行一次
                .WithInterval(TimeSpan.FromSeconds(600))// 间隔时间
                .WithDailyTimes("04:15", "12:10")// 每天指定时间
                //.WithSpecificTimes("2025/12/31 01:00")// 指定时间点
                .Build();

            Initialize(config);
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            Logger?.Info("执行作业中...");
            await Task.Delay(2000, token);
            Logger?.Info("作业完成");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            MyJob myJob = new MyJob();
            myJob.Start();

            Console.ReadKey();

            myJob.Stop();

            Console.ReadLine();

        }
    }
}
```

## 🔧 注入自定义日志

实现 `IJobLogger` 接口并传入 `Initialize` 方法：

```csharp
var job = new MyJob();
job.SetLogger(new NLogJobLogger());  // NLogJobLogger 实现 IJobLogger
```

## 🧩 Job 配置示例

```csharp
// 创建配置
var config = new JobConfig.Builder()
    .WithName("测试作业1")// 作业名称
    .WithExecuteAtStart(true)// 启动后执行一次
    .WithInterval(TimeSpan.FromSeconds(600))// 间隔时间
    .WithDailyTimes("04:15", "12:10")// 每天指定时间
    //.WithSpecificTimes("2025/12/31 01:00")// 指定时间点
    .Build();

```

## 📄 License

MIT
