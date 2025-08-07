using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kamsoft.Job
{
    #region 作业状态 JobStatus
    public enum JobStatus
    {
        NotStarted,
        Running,
        Stopped
    }
    #endregion
}