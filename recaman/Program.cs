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
            try
            {
                RecamanComputer.Computation[] results = Enumerable.Range(0, loops).ToArray().Select(x => new RecamanComputer().PerformCompute(n, method)).ToArray();
                //TODO: make sure results are proper

                var mean_compute_time = results.Select(res => res.time).Sum() / results.Length;
                Console.WriteLine($"Evaluated method {method}; computed recaman({n}) {loops} times with average compute time: {mean_compute_time}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occured while evaluation of method {method}: " + e.ToString());
            }

        }

        //TODO: ThreadPool

        public static void Main()
        {
            Console.WriteLine(Environment.ProcessorCount);

            //evaluate(10, 10000, RecamanComputer.ComputationMethod.RECURSIVE_ASCENDANT);

            //evaluate(10, 10000, RecamanComputer.ComputationMethod.MULTITHREADED_RECURSIVE);
            //evaluate(10, 10000, RecamanComputer.ComputationMethod.MULTITHREADED_RECURSIVE_ASCENDANT);
            evaluate(10, 10000, RecamanComputer.ComputationMethod.THREADPOOL_RECURSIVE_ASCENDANT);
            //evaluate(10, 10000, RecamanComputer.ComputationMethod.ITERATIVE);

            //evaluate(3, 100000000, RecamanComputer.ComputationMethod.MULTITHREADED_RECURSIVE_ASCENDANT);
            //evaluate(3, 100000000, RecamanComputer.ComputationMethod.ITERATIVE);
        }
    }
}
