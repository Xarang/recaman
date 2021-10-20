using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace recaman
{
    class Program
    {

        static void evaluate(int loops, int n, RecamanComputer.ComputationMethod method)
        {
            RecamanComputer.Computation[] results = Enumerable.Range(0, loops).ToArray().Select(x => new RecamanComputer().PerformCompute(n, method)).ToArray();

            //TODO: make sure results are proper

            var mean_compute_time = results.Select(res => res.time).Sum() / results.Length;

            Console.WriteLine($"Evaluated method {method}; computed recaman({n}) {loops} times with average compute time: {mean_compute_time}");
        }

        public static void Main()
        {
            //evaluate(20, 1000, RecamanComputer.ComputationMethod.MULTITHREADED);
            //evaluate(20, 1000, RecamanComputer.ComputationMethod.MULTITHREADED_BIS);

            for (int i = 0; i < 10; i++)
            {
                RecamanComputer.Computation res = new RecamanComputer().PerformCompute(i, RecamanComputer.ComputationMethod.MULTITHREADED_BIS);
                Console.WriteLine(res.ToString());
            }

            //for (int i = 0; i < 1000000; i += 50)

            /*
            {
                RecamanComputer.Computation res = new RecamanComputer().PerformCompute(1000, RecamanComputer.ComputationMethod.MULTITHREADED);
                Console.WriteLine(res.ToString());
            }

            //}

            Console.WriteLine("----------------------------------------------");

            //for (int i = 0; i < 1000000; i += 50)
            {
                RecamanComputer.Computation res = new RecamanComputer().PerformCompute(1000, RecamanComputer.ComputationMethod.MULTITHREADED_BIS);
                Console.WriteLine(res.ToString());
            }
            */
        }
    }
}
