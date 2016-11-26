using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentCollections
{
    /* Thread Syncronization Techniques
     * 1. Locks
     * 2. Memory Barriers
     * 3. Special Assembly instructions (H/W does the locking)
     * 4. Syncronization Primitives (Semaphore, mutex, etc)
     * 5. Algorthms
     * 
     * Concurrent Collections doesnt uses locks so much to provide thread safety
     * Optimistic Concurrency technique
     * This technique is used sometimes by concurrent colelctions to provide thread safety.
     * 1. Calculate all changes that are required
     * 2. At the last moment, atomically checks if any other thread has made those chnages.
     * 3. If no, then atomically apply the changes.
     * 4. Else re-calculate the changes again.
     * This technique scales well because other threads do not need to wait longer to get unblocked by the running thread. So it is scalable and protects against internal data corruption.
     * 
     * Race Condition (Results are sensitive to precise timing of threads)
     * In the order example, race condition doesnt matter because the order in which the orders were placed doesnt matter, what matters is that all the orders gets placed. But in a different application it may happen that the order is which the requests are placed matters, so in such an application usage of concurrent collections will raise a bug. Concurrent collections doesnt protect the application from race condition between method calls. (Protection is available only aginst internat data corruption and internal race conditions).
     * 
     * ConcurrentCollections
     * A. General Purpose
     *  1. ConcurrentDictionary
     * B. Producer-Consumer
     *  1. ConcurrentQueue
     *  2. ConcurrentStack
     *  3. ConcurrentBag
     * C. Partitioners (Partition iterations between threads)
     *  1. Partitioner
     *  2. OrderbalePartitioner
     *  3. EnumerablePartitionerOptions
     */ 
    class Program
    {
        static void Main(string[] args)
        {
            //WithLocks.main(); //Achieving muli-threading using locks
            var orders = new ConcurrentQueue<string>();
            //ConcurrentQueue is thread safe
            Task task1 = Task.Run(() => PlaceOrders("Pratik B", orders));
            Task task2 = Task.Run(() => PlaceOrders("Pradeep B", orders));
            Task.WaitAll(task1, task2);

            foreach (var order in orders)
                Console.WriteLine("ORDER: " + order);

            Console.ReadKey();
        }

        static void PlaceOrders(string customerName, ConcurrentQueue<string> orders)
        {
            for (int i = 0; i < 5; i++)
            {
                Thread.Sleep(1);
                var orderName = string.Format("{0} wants t-shirt {1}", customerName, i + 1);
                //Concurrent.Enqueue is atomic.
                orders.Enqueue(orderName);
            }
        }
    }
}
