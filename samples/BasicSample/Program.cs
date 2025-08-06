using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSample
{
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
