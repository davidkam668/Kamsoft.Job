using Kamsoft.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            MyJob myJob = new MyJob();
            JobManager  jobManager = new JobManager();
            jobManager.Register(myJob);

            //myJob.StartAsync();
            await jobManager.StartAllAsync();
            Console.WriteLine("作业已启动，按任意键停止...");
            Console.ReadKey();

            //myJob.StopAsync();
            await jobManager.StopAllAsync();
            Console.WriteLine("作业已停止，按任意键退出...");
            Console.ReadKey();

        }
    }
}
