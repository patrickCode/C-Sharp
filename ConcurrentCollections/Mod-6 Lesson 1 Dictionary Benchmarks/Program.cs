using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod_6_Lesson_1_Dictionary_Benchmarks
{
    /*
     * FINE-GRAIN LOCKING
     * Concurrent Dictionary and bags use fine-grained locking. Under the hood ConcurrentDictionary uses Hash Table. The Hash Table is divided into several buckets, each bucket stores a portion of the dictionary. Internally ConcurrentDictionary maintains locks for each of the bucket to protect the data. Since each bucket has its own lock, 2 threads accessing can simultaneously access the dictionary if they are accessing different buckets. However 2 threads cannot access the same bucket simultaneously because of the lock.
     * Most of the time we dont need to worry about the scenario because rarely do multiple threads need to access data from the same dictionary. However if we want to query the dictionary about its aggregate (overall) state then fine-grained locking mechanism cause performances issues.
     * IsEmpty - Checks if the dictionary is empty. To do this, all the buckets need to be checked, hence all the buckets are locked at the same time. The locks remain until the operation is complete. Depending upon the size of the dictionary this can cause performance and scalability issues.
     * Similar to IsEmpty the following methods need to lock all the buckets: IsEmpty(), Count, ClearAll(), ToArray(), CopyTo(), Values, Keys.
     * Similar to ConcurrentDictionary the following methods also need to lock all the buckets in ConcurrentBag - IsEmpty(), Count, ToArray(), CopyTo(), GetEnumerator(). If a ForEach loop is used then GetEnumerator() is always called.
     * GOOD PRACTICE: Avoid querying the state of the concurrent collections too often. When the aggregate state of the collection is queried all the buckets need to be locked, hence it is a good practice not query the aggregate state too often. All the above mentioned methods query the aggregate state of the collection. Another reason why its a good practice that the answer to the aggregate queries may change as soon as the answer is obtained (some other thread may modify the collection)
     * WHEN TO USE CONCURRENT COLLECTIONS
     * - Mutiple Threads
     *  a. If the threads will modify the collections then concurrent dictionary is the best case scenario.
     *  b. If the threads will only read the collection values then we can continue with Standard collection.
     *      RISK: However a risk is involved. If the collecion can modify its contents under the hood while reading, there sing standard collection may not be thread safe. For e.g. Lazy-initialized properties (these properties are set on the first time someone tries to read them). To mitigate such risks there are 2 options
     *          i. Use ConcurrentCollections (thread safe).
     *          ii. Immutable Collections - These are readonly collections and are thread safe. Need to install them from Nu-Get.
     */
    class Program
    {
        private const int IterationLimit = 3;
        private const int DictionarySize = 1000000;
        static void Main(string[] args)
        {   
            //On a single Thread, concurrent dictionary is massively slower than Dictionary
            //Don't use concurrent Dictionary unless Thread Safety is needed.
            //For using methods like AddOrUpdate, extend the methods in normal Dictionary.
            //Console.WriteLine("Statictics when Threads are not doing any Time Consuming Work");
            //SingleThreadedStatistics();

            //ParallelThreadedStatistics();
            //Console.ReadLine();

            //Unless the individual threads are not doing any time consuming work, concurrent dictionary should not be used.
            Console.WriteLine("Statictics when Threads are doing some Time Consuming Work");
            SingleThreadedStatistics(true);

            ParallelThreadedStatistics(true);
            Console.ReadLine();
        }

        static void SingleThreadedStatistics(bool withWork = false)
        {
            long totalBuildTime = 0;
            long totalEnumerationTime = 0;

            long conTotalBuildTime = 0;
            long conTotalEnumerationTime = 0;


            Console.WriteLine("Dictionary - Single Threaded");
            for (var i = 0; i < IterationLimit; i++)
            {
                Console.WriteLine("IERATION - " + i);
                var dict = new Dictionary<int, int>();
                var buildTime = SingleThreadBenchmark.TimeDictionaryBuilding(dict, DictionarySize, withWork);
                totalBuildTime += buildTime;
                var enumerationTime = SingleThreadBenchmark.TimeDictionaryEnumeration(dict, DictionarySize, withWork);
                totalEnumerationTime += enumerationTime;
                Console.WriteLine("Time taken to build dictionary (ms):     {0}", buildTime);
                Console.WriteLine("Time taken to enumerate dictionary (ms):     {0}", enumerationTime);
            }

            var averageBuildTime = totalBuildTime / IterationLimit;
            var averageEnumerationTime = totalEnumerationTime / IterationLimit;


            Console.WriteLine("\r\nConcurrent Dictionary - Single Threaded");
            for (var i = 0; i < IterationLimit; i++)
            {
                Console.WriteLine("IERATION - " + i);
                var dict = new ConcurrentDictionary<int, int>();
                var buildTime = SingleThreadBenchmark.TimeDictionaryBuilding(dict, DictionarySize, withWork);
                conTotalBuildTime += buildTime;
                var enumerationTime = SingleThreadBenchmark.TimeDictionaryEnumeration(dict, DictionarySize, withWork);
                conTotalEnumerationTime += enumerationTime;
                Console.WriteLine("Time taken to build dictionary (ms):     {0}", buildTime);
                Console.WriteLine("Time taken to enumerate dictionary (ms):     {0}", enumerationTime);
            }

            var conAverageBuildTime = conTotalBuildTime / IterationLimit;
            var conAverageEnumerationTime = conTotalEnumerationTime / IterationLimit;

            Console.WriteLine("DICTIONARY STATISTICS");
            Console.WriteLine("Average Build Time (ms):     {0}", averageBuildTime);
            Console.WriteLine("Average Enumeration Time (ms):     {0}", averageEnumerationTime);

            Console.WriteLine("CONCURRENT DICTIONARY STATISTICS");
            Console.WriteLine("Average Build Time (ms):     {0}", conAverageBuildTime);
            Console.WriteLine("Average Enumeration Time (ms):     {0}", conAverageEnumerationTime);
        }

        static void ParallelThreadedStatistics(bool withWork=false)
        {
            long totalBuildTime = 0;
            long totalEnumerationTime = 0;

            Console.WriteLine("Dictionary - Single Threaded");
            for (var i = 0; i < IterationLimit; i++)
            {
                Console.WriteLine("IERATION - " + i);
                var dict = new ConcurrentDictionary<int, int>();
                var buildTime = ParallelBenchmarks.TimeDictionaryBuilding(dict, DictionarySize, withWork);
                totalBuildTime += buildTime;
                var enumerationTime = ParallelBenchmarks.TimeDictionaryEnumeration(dict, DictionarySize, withWork);
                totalEnumerationTime += enumerationTime;
                Console.WriteLine("Time taken to build dictionary (ms):     {0}", buildTime);
                Console.WriteLine("Time taken to enumerate dictionary (ms):     {0}", enumerationTime);
            }

            var averageBuildTime = totalBuildTime / IterationLimit;
            var averageEnumerationTime = totalEnumerationTime / IterationLimit;

            Console.WriteLine("CONCURRENT DICTIONARY STATISTICS");
            Console.WriteLine("Average Build Time (ms):     {0}", averageBuildTime);
            Console.WriteLine("Average Enumeration Time (ms):     {0}", averageEnumerationTime);
        }
    }
}
