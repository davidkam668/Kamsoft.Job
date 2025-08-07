using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsoft.Job
{
    public class JobManager : IJobManager
    {
        private readonly List<IJob> _jobs = new List<IJob>();

        public void Register(IJob job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            _jobs.Add(job);
        }

        public async Task StartAllAsync()
        {
            foreach (var job in _jobs)
            {
                await job.StartAsync();
            }
        }

        public async Task StopAllAsync()
        {
            foreach (var job in _jobs)
            {
                await job.StopAsync();
            }
        }
    }

}
