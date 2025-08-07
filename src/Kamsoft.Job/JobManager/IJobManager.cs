using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsoft.Job
{
    public interface IJobManager
    {
        void Register(IJob job);
        Task StartAllAsync();
        Task StopAllAsync();
    }

}
