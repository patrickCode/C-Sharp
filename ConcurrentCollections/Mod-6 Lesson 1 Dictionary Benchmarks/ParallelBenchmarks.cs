using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mod_6_Lesson_1_Dictionary_Benchmarks
{
    class ParallelBenchmarks
    {
        static void PopulateDictionary(ConcurrentDictionary<int, int> dictionary, int dictSize)
        {
            Parallel.For(0, dictSize, (i) => dictionary.TryAdd(i, 0));
            Parallel.For(0, dictSize, (i) =>
            {
                var done = dictionary.TryUpdate(i, 1, 0);   //OldValue will always be 0, so we can use TryUpdate
                if (!done)
                    throw new Exception("Error Updating. Old Value was " + dictSize);
            });
        }

        static void PopulateDictionaryWithWork(ConcurrentDictionary<int, int> dictionary, int dictSize)
        {
            Parallel.For(0, dictSize, (i) => dictionary.TryAdd(i, 0));
            Parallel.For(0, dictSize, (i) =>
            {
                var done = dictionary.TryUpdate(i, 1, 0);   //OldValue will always be 0, so we can use TryUpdate
                Worker.DoSomething();
                if (!done)
                    throw new Exception("Error Updating. Old Value was " + dictSize);
            });
        }

        static int GetTotalValue(ConcurrentDictionary<int, int> dictionary)
        {
            var expectedTotal = dictionary.Count;

            var total = 0;
            Parallel.ForEach(dictionary, keyValPair =>
            {
                //This is a bad pattern, becasue all the threads are working on shared data, and that will make all the threads spend too much time on thread syncronization.
                //
                Interlocked.Add(ref total, keyValPair.Value);   //Interlocked class is used becasue multiple threads will try to update the total
            });
            return total;
        }

        static int GetTotalValueWithWork(ConcurrentDictionary<int, int> dictionary)
        {
            var expectedTotal = dictionary.Count;

            var total = 0;
            Parallel.ForEach(dictionary, keyValPair =>
            {   
                Interlocked.Add(ref total, keyValPair.Value);
                Worker.DoSomething();
            });
            return total;
        }


        public static long TimeDictionaryBuilding(ConcurrentDictionary<int, int> dictionary, int dictSize, bool withWork = false)
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

        public static long TimeDictionaryEnumeration(ConcurrentDictionary<int, int> dictionary, int dictSize, bool withWork = false)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var total = withWork ? GetTotalValueWithWork(dictionary) : GetTotalValue(dictionary);
            stopWatch.Stop();

            if (total != dictSize)
                Console.WriteLine("ERROR: Wrong Total!");
            return stopWatch.ElapsedMilliseconds;
        }

        public static void TimeDictionary(ConcurrentDictionary<int, int> dictionary, int dictSize)
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

            Console.WriteLine("Total is: " + total);
            if (total != dictSize)
                Console.WriteLine("ERROR: Wrong Total!");
        }

    }
}
