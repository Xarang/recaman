using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace recaman
{
    public class RecamanComputer
    {

        public static int numProcs = Environment.ProcessorCount;
        public static int concurrencyLevel = numProcs * 2;
        public static int initialCapacity = 256;

        private ConcurrentDictionary<int, decimal> safe_values;
        private HashSet<decimal> values;

        private decimal reverse_return_value = 0;

        public RecamanComputer()
        {

            this.safe_values = new ConcurrentDictionary<int, decimal>(concurrencyLevel, initialCapacity);
            this.values = new HashSet<decimal>();
            this.values.Add(0);
        }


        private decimal multithreaded_recursive(int n)
        {
            //Console.WriteLine($"entering: {n}");
            if (n == 0)
            {
                return 0;
            }
            Task<decimal> computePrev = Task<decimal>.Run(() => multithreaded_recursive(n - 1));

            decimal prev = computePrev.Result;
            //PrintCollection();
            decimal res = (prev >= n && !values.Contains(prev - n)) ? prev - n : prev + n;

            values.Add(res);
            return res;
        }

        private void multithreaded_recursive_ascendant(int n, decimal prev, int max)
        {
            //Console.WriteLine($"entering: {n}");
            if (n == max)
            {
                reverse_return_value = (prev >= n && !values.Contains(prev - n)) ? prev - n : prev + n;
            }
            else
            {
                decimal res = (prev >= n && !values.Contains(prev - n)) ? prev - n : prev + n;
                values.Add(res);
                Task computeNext = Task.Run(() => multithreaded_recursive_ascendant(n + 1, res, max));
            }
        }

        private void threadpool_recursive_ascendant(int n, decimal prev, int max)
        {
            //Console.WriteLine($"entering: {n}");
            if (n == max)
            {
                reverse_return_value = (prev >= n && !values.Contains(prev - n)) ? prev - n : prev + n;
            }
            else
            {
                decimal res = (prev >= n && !values.Contains(prev - n)) ? prev - n : prev + n;
                values.Add(res);
                Console.WriteLine(ThreadPool.ThreadCount);
                ThreadPool.QueueUserWorkItem((stateInfo) => multithreaded_recursive_ascendant(n + 1, res, max));
                //Task computeNext = Task.Run(() => multithreaded_recursive_ascendant(n + 1, res, max));
            }
        }

        private void recursive_ascendant(int n, decimal prev, int max)
        {
            if (n == max)
            {
                reverse_return_value = (prev >= n && !values.Contains(prev - n)) ? prev - n : prev + n;
            }
            else
            {
                decimal res = (prev >= n && !values.Contains(prev - n)) ? prev - n : prev + n;
                values.Add(res);
                recursive_ascendant(n + 1, res, max);
            }
        }

        private decimal recursive(int n)
        {
            //Console.WriteLine($"entering: {n}");
            if (n == 0)
            {
                return 0;
            }
            var prev = recursive(n - 1);
            decimal res = (prev >= n && !values.Contains(prev - n)) ? prev - n : prev + n;

            values.Add(res);
            return res;
        }

        private decimal iterative(int n)
        {
            if (n <= 0)
                return 0;

            // Print first term and store it in a hash
            HashSet<int> s = new HashSet<int>();
            s.Add(0);

            // Print remaining terms using
            // recursive formula.
            int prev = 0;
            for (int i = 1; i < n; i++)
            {
                int curr = prev - i;

                // If arr[i-1] - i is negative or
                // already exists.
                if (curr < 0 || s.Contains(curr))
                    curr = prev + i;

                s.Add(curr);

                prev = curr;
                if (i == n - 1)
                    return curr;
            }
            return 0;
        }

        public struct Computation
        {
            public int input;
            public long time;
            public decimal result;
            public ComputationMethod method;

            public Computation(int input, long time, decimal result, ComputationMethod method)
            {
                this.input = input;
                this.time = time;
                this.result = result;
                this.method = method;
            }

            public override string ToString()
            {
                return $"{method}  --> {input}:  {result}   (computed in {time})";
            }
        }

        public enum ComputationMethod
        {
            ITERATIVE,
            RECURSIVE,
            RECURSIVE_ASCENDANT,
            MULTITHREADED_RECURSIVE,
            MULTITHREADED_RECURSIVE_ASCENDANT,
            THREADPOOL_RECURSIVE_ASCENDANT
        }

        public Computation PerformCompute(int n, ComputationMethod method)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Func<ComputationMethod, decimal> get_res = (method) =>
            {
                if (n == 0)
                {
                    return 0;
                }
                switch (method)
                {
                    case ComputationMethod.ITERATIVE:
                        return iterative(n);
                    case ComputationMethod.MULTITHREADED_RECURSIVE:
                        return multithreaded_recursive(n);
                    case ComputationMethod.RECURSIVE:
                        return recursive(n);
                    case ComputationMethod.MULTITHREADED_RECURSIVE_ASCENDANT:
                        multithreaded_recursive_ascendant(1, 0, n);
                        while (reverse_return_value == 0)
                        {
                            //
                        }
                        return reverse_return_value;
                    case ComputationMethod.RECURSIVE_ASCENDANT:
                        recursive_ascendant(1, 0, n);
                        while (reverse_return_value == 0)
                        {
                            //
                        }
                        return reverse_return_value;
                    case ComputationMethod.THREADPOOL_RECURSIVE_ASCENDANT:
                        threadpool_recursive_ascendant(1, 0, n);
                        while (reverse_return_value == 0)
                        {
                            //
                        }
                        return reverse_return_value;
                }
                return 0;
            };
            decimal value = get_res(method);
            stopwatch.Stop();


            // collect garbage at the end to make sure benchmarking
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized, blocking: true);
            return new Computation(n, stopwatch.ElapsedMilliseconds, value, method);
        }

    }
}
