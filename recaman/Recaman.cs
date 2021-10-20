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

        static int numProcs = Environment.ProcessorCount;
        static int concurrencyLevel = numProcs * 2;
        static int initialCapacity = 256;

        private ConcurrentDictionary<int, decimal> values;

        public RecamanComputer()
        {

            this.values = new ConcurrentDictionary<int, decimal>(concurrencyLevel, initialCapacity);
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

        private decimal basic_recaman(int n)
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
            int input;
            long time;
            decimal result;

            public Computation(int input, long time, decimal result)
            {
                this.input = input;
                this.time = time;
                this.result = result;
            }

            public override string ToString()
            {
                return $"{input}:  {result}   (computed in {time})";
            }
        }

        public Computation PerformCompute(int n, bool useMultithreading = true)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Decimal value = useMultithreading ? multi_recaman(n) : basic_recaman(n);
            stopwatch.Stop();
            return new Computation(n, stopwatch.ElapsedMilliseconds, value);
        }

    }
}
