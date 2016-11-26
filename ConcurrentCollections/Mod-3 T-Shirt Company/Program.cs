using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mod_3_T_Shirt_Company
{
    class Program
    {
        public static readonly List<string> AllShirtNames = new List<string> {"GAP", "Max", "Lee", "Amul"}; 
        static void Main(string[] args)
        {
            StockController controller = new StockController();
            TimeSpan workDay = new TimeSpan(0, 0, 2);

            Task t1 = Task.Run(() => new SalesPerson("Pratik").Work(controller, workDay));
            Task t2 = Task.Run(() => new SalesPerson("Varun").Work(controller, workDay));
            Task t3 = Task.Run(() => new SalesPerson("Pradeep").Work(controller, workDay));
            Task t4 = Task.Run(() => new SalesPerson("Himanshu").Work(controller, workDay));


            Task.WaitAll(t1, t2, t3, t4);
            controller.DisplayStatus();

            Console.ReadLine();
        }
    }
}
