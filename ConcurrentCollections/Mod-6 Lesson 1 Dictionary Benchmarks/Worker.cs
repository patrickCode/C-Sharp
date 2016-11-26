using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod_6_Lesson_1_Dictionary_Benchmarks
{
    class Worker
    {
        public static void DoSomething()
        {
            var total = 0;
            for (int i = 0; i < 1000; i++)
            {
                total += i;
            }
        }
    }
}
