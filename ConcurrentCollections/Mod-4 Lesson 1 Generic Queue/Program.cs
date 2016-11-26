using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod_4_Lesson_1_Generic_Queue
{
    class Program
    {
        static void Main(string[] args)
        {
            var shirts = new Queue<string>();
            shirts.Enqueue("Lee");
            shirts.Enqueue("GAP");
            shirts.Enqueue("Turtle");

            Console.WriteLine("After Enqueue. Count = " + shirts.Count);

            string shirt1 = shirts.Dequeue();
            Console.WriteLine("\r\nRemoving - " + shirt1);

            var shirt2 = shirts.Peek();
            Console.WriteLine("Peeking - " + shirt2);

            Console.WriteLine("\r\nEnumerating");
            foreach(var shirt in shirts)
                Console.WriteLine(shirt);

            Console.WriteLine("\r\nAfter Enumerating, count = " + shirts.Count);

            Console.ReadLine();
        }
    }
}
