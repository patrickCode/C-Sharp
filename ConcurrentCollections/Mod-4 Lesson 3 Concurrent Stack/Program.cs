using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod_4_Lesson_3_Concurrent_Stack
{
    class Program
    {
        //Methods are same as Queue. Enqueue is replaced by push and TryDequeue is replaced by TryPop
        static void Main(string[] args)
        {
            var shirts = new ConcurrentStack<string>();
            shirts.Push("Lee");
            shirts.Push("GAP");
            shirts.Push("Turtle");

            Console.WriteLine("After Push. Count = " + shirts.Count);

            string shirt1;
            if (shirts.TryPop(out shirt1))
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
