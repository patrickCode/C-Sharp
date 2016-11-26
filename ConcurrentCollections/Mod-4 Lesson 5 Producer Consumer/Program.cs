using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod_4_Lesson_5_Producer_Consumer
{
    class Program
    {
        /*
         * ConcurrentDictionary is known as General purpose collection whereas the others (Concurrent Stack, Queue, Bag) are known as Producer-Consumer collections. They don't give direct access to the elements. Unlike in Dictionary, the key/index of an item cannot be specifed to retrieve that item.
         * Task/Process requests scenario are fitted for the Producer-Consumer collections. Someone is producing tasks and someone else consumes it.
         * IProducerCollection interface encapsulates the 3 producer-consumer collections
         * Exposed APIs - TryAdd(), TryTake(), Count (TryPeek is not present)
         * IProducerConsumerCollection<string> col = new ConcurrentBag<string>();
         * IProducerConsumerCollection<string> col = new ConcurrentStack<string>();
         * IProducerConsumerCollection<string> col = new ConcurrentQueue<string>();
         */
        static void Main(string[] args)
        {
            IProducerConsumerCollection<string> shirts = new ConcurrentBag<string>();
            //IProducerConsumerCollection<string> shirts = new ConcurrentStack<string>();
            //IProducerConsumerCollection<string> shirts = new ConcurrentQueue<string>();
            shirts.TryAdd("Lee");
            shirts.TryAdd("GAP");
            shirts.TryAdd("Turtle");

            Console.WriteLine("After Push. Count = " + shirts.Count);

            string shirt1;
            if (shirts.TryTake(out shirt1))
                Console.WriteLine("\r\nRemoving - " + shirt1);
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
