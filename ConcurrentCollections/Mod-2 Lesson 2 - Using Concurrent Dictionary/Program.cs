using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod_2_Lesson_2___Using_Concurrent_Dictionary
{
    class Program
    {
        static void Main(string[] args)
        {
            //Add and Remove method in Concurrent Dictionary has been used explicitly. So unless the IDictionary interface is used, Add() and Remove() method cannot be used on ConcurrentDictionary.
            //Initializers dont work in ConcurrentDictionary, becasue under the hood, initializer calls the constructor for ConcurrentDictionary, which in turn calls the Add method. Since the Add method of ConcurrentDictionary cannot be called without a cast (explicitly), the initializer wont work.
            //Add value works only when the key that is getting added must not have been added earlier. Keys must be unique in dictionary. So for adding a key in dictionary using add() method the user needs to know the state of the Dictioanry (i.e. it must know/ assume that the key that he is trying to add has not been already added. Same thing as Remove, the use must know that the key has already been added before.
            //In a sinngle threaded mode it might be possible to know this information, but in multithreaded it is often impossible to know whether a key is already present.
            IDictionary<string, int> stock = new ConcurrentDictionary<string, int>();
            stock.Add("ECO t-shirts", 4);
            stock.Add("ISRM t-shirts", 5);

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
