using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsoft.Job
{
    public interface IJobLogger
    {
        void Info(string message);
        void Error(string message);
    }
}
