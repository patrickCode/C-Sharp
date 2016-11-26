using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod_4_Lesson_2_Concurrent_Queue
{
    /*
     * For Dequeue and Peek to work there must be atleat 1 item in the Queue. But in a multithreaded system, its difficult to know that so Dequeue and Peek are not supported in Concurrent Queue.
     * Similar to ConcurrentDictionary, we have TryQueue and TryPeek for ConcurrentQueue, which will return a boolean statement based on whether they were successfull or not. The item is returned as a out parameter.
     */
    class Program
    {
        static void Main(string[] args)
        {
            var shirts = new ConcurrentQueue<string>();
            shirts.Enqueue("Lee");
            shirts.Enqueue("GAP");
            shirts.Enqueue("Turtle");

            Console.WriteLine("After Enqueue. Count = " + shirts.Count);

            string shirt1;
            if (shirts.TryDequeue(out shirt1))
                Console.WriteLine("\r\nRemoving - " + shirt1);
            else
                Console.WriteLine("Queue was empty");
            

            string shirt2;
            if (shirts.TryPeek(out shirt2))
                Console.WriteLine("Peeking - " + shirt2);
            else
                Console.WriteLine("Queue was empty");

            Console.WriteLine("\r\nEnumerating");
            foreach (var shirt in shirts)
                Console.WriteLine(shirt);

            Console.WriteLine("\r\nAfter Enumerating, count = " + shirts.Count);

            Console.ReadLine();
        }
    }
}
