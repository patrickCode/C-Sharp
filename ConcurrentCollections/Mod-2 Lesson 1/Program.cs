using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod_2_Lesson_1
{
    class Program
    {
        static void Main(string[] args)
        {
            var stock = new Dictionary<string, int>()
            {
                {"ECO t-Shirts", 4},
                {"ISRM t-shirts", 5}
            };

            Console.WriteLine(string.Format("No. of T-Shirts in stock = {0}", stock.Count));

            stock.Add("MPSIT t-shirts", 6);
            stock["ESSIT t-shirts"] = 7;

            stock["MPSIT t-shirts"] = 8; //Update if exists
            Console.WriteLine(string.Format("\r\nstock[MPSIT t-shirts] = {0}", stock["MPSIT t-shirts"]));

            stock.Remove("ISRM t-shirts");

            Console.WriteLine("\r\nEnumerating");
            foreach (var keyValPair in stock)
            {
                Console.WriteLine("{0} : {1}", keyValPair.Key, keyValPair.Value);
            }
            Console.ReadLine();
        }
    }
}
