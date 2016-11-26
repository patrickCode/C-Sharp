using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod_6_Lesson_1_Dictionary_Benchmarks
{
    class SingleThreadBenchmark
    {
        static void PopulateDictionary(IDictionary<int, int> dict, int dictSize)
        {
            for (var i = 0; i < dictSize; i++)
            {
                dict.Add(i, 0);
            }

            for (var i = 0; i < dictSize; i++)
            {   
                dict[i] += 1;
            }
        }

        static void PopulateDictionaryWithWork(IDictionary<int, int> dict, int dictSize)
        {
            for (var i = 0; i < dictSize; i++)
            {
                dict.Add(i, 0);
            }

            for (var i = 0; i < dictSize; i++)
            {
                Worker.DoSomething();
                dict[i] += 1;
            }
        }

        static int GetTotalValue(IDictionary<int, int> dict)
        {
            return dict.Sum(item => item.Value);
        }

        static int GetTotalValueWithWork(IDictionary<int, int> dict)
        {
            var total = 0;
            foreach (var item in dict)
            {
                total += item.Value;
                Worker.DoSomething();
            }
            return total;
        }

        public static long TimeDictionaryBuilding(IDictionary<int, int> dictionary, int dictSize, bool withWork=false)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            if (withWork)
                PopulateDictionaryWithWork(dictionary, dictSize);
            else 
                PopulateDictionary(dictionary, dictSize);
            stopWatch.Stop();
            return stopWatch.ElapsedMilliseconds;
        }

        public static long TimeDictionaryEnumeration(IDictionary<int, int> dictionary, int dictSize, bool withWork = false)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var total = withWork ? GetTotalValueWithWork(dictionary) : GetTotalValue(dictionary);
            stopWatch.Stop();

            if (total != dictSize)
                Console.WriteLine("ERROR: Wrong Total!");
            return stopWatch.ElapsedMilliseconds;
        }



        public static void TimeDictionary(IDictionary<int, int> dictionary, int dictSize)
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            PopulateDictionary(dictionary, dictSize);
            stopWatch.Stop();
            Console.WriteLine(string.Format("Time taken to build dictionary (ms):   {0}", stopWatch.ElapsedMilliseconds));

            stopWatch.Restart();
            var total = GetTotalValue(dictionary);
            stopWatch.Stop();
            Console.WriteLine(string.Format("Time taken to enumerate dictionaer (ms):   {0}",
                stopWatch.ElapsedMilliseconds));

            Console.WriteLine("Total is: "+ total);
            if (total != dictSize)
                Console.WriteLine("ERROR: Wrong Total!");
        }

    }
}
