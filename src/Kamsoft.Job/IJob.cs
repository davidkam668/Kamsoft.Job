using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kamsoft.Job
{
    #region 作业接口 IJob
    public interface IJob
    {
        string Name { get; }
        string StatusDesc { get; }
        void Start();
        Task StartAsync();
        void Stop();
        Task StopAsync();
    }
    #endregion
}