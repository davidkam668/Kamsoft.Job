using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsoft.Job
{
    public class NullLogger : IJobLogger
    {
        public void Info(string message) { }
        public void Error(string message) { }
    }
}
