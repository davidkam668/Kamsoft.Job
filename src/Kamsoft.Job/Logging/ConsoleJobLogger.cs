using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsoft.Job
{
    public class ConsoleJobLogger : IJobLogger
    {
        private readonly string _name;

        public ConsoleJobLogger(string name) => _name = name;

        public void Info(string message) => Write("INFO", message);
        public void Warn(string message) => Write("WARN", message);
        public void Error(string message) => Write("ERROR", message);
        public void Error(Exception ex, string message = null)
        {
            Write("ERROR", $"{message}\n{ex}");
        }

        private void Write(string level, string message)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff}] [{level}] [{_name}] {message}");
        }
    }
}
