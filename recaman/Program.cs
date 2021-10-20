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

        public static void Main()
        {
            //for (int i = 0; i < 1000000; i += 50)
            
            {
                RecamanComputer.Computation res = new RecamanComputer().PerformCompute(10000);
                Console.WriteLine(res.ToString());
            }

            Console.WriteLine("----------------------------------------------");

            //for (int i = 0; i < 1000000; i += 50)
            {
                RecamanComputer.Computation res = new RecamanComputer().PerformCompute(10000, false);
                Console.WriteLine(res.ToString());
            }
        }
        


    }
}
