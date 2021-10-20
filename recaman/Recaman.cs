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
        private Dictionary<int, decimal> values;

        private decimal reverse_return_value = 0;

        public RecamanComputer()
        {

            this.safe_values = new ConcurrentDictionary<int, decimal>(concurrencyLevel, initialCapacity);
            this.values = new Dictionary<int, decimal>();
            this.values.Add(0, 0);
        }


        private decimal multi_recaman(int n)
        {
            //Console.WriteLine($"entering: {n}");
            if (n == 0)
            {
                return 0;
            }
            Task<decimal> computePrev = Task<decimal>.Run(() => multi_recaman(n - 1));
            decimal prev = computePrev.Result;
            //PrintCollection();
            decimal res = (prev >= n && !values.Values.Contains(prev - n)) ? prev - n : prev + n;

            values.TryAdd(n, res);
            return res;
        }

        private void multi_recaman_reverse(int n, decimal prev, int max)
        {
            //Console.WriteLine($"entering: {n}");
            if (n == max)
            {
                reverse_return_value = (prev >= n && !values.Values.Contains(prev - n)) ? prev - n : prev + n;
            }
            else
            {
                decimal res = (prev >= n && !values.Values.Contains(prev - n)) ? prev - n : prev + n;
                values.TryAdd(n, res);
                Task computeNext = Task.Run(() => multi_recaman_reverse(n + 1, res, max));
            }
        }

        private decimal recursive_recaman(int n)
        {
            //Console.WriteLine($"entering: {n}");
            if (n == 0)
            {
                return 0;
            }
            var prev = recursive_recaman(n - 1);
            decimal res = (prev >= n && !values.Values.Contains(prev - n)) ? prev - n : prev + n;

            values.TryAdd(n, res);
            return res;
        }

        private decimal iterative_recaman(int n)
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
            MULTITHREADED,
            MULTITHREADED_BIS
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
                        return iterative_recaman(n);
                    case ComputationMethod.MULTITHREADED:
                        return multi_recaman(n);
                    case ComputationMethod.RECURSIVE:
                        return recursive_recaman(n);
                    case ComputationMethod.MULTITHREADED_BIS:
                        multi_recaman_reverse(1, 0, n);
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
