using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApplication1
{
    /* DELGATES
     * Delegates are the link between Events and the Event Handler
     * Handler method mimick the delegate
     * Behind the screens a Delegate class (base class) is created which has 2 properties - Method and Target
     * **Method - Defines the method which will actually be invoked.
     * **Target - Object instance where the Method actually lives.
     * All delegates that get created inherits the MulticastDelegate. Multicast deleate is a way to hold multiple delegates.
     * MulticastDelegate cannot be directly inherited from, when we use the delegate keyword the inheritance happens.
     * **MulticastDelegate track multiple delegate references using an invocation list. Delegates in the list are invoked sequentially.
    */

    /* EVENTS
     * public event <Delegate_Name> <Event_Name>
     * So events are actually a wrapper around delegates
     */

    /* LAMDAS
     *  Lamda expression can be assigned to any delegate
     *   <event> = <lamda inline parameter> <lamda operator> <lamda body>
     *   worker.click = (s,e) => MessageBox.Show(e.data);
     *   
     *   delegate int AddDelegate(int a, int b)
     *   
     *   AddDelegate simpleAdd = (a, b) => a + b;
     *   int result = simpleAdd(1, 2);
     */

    /* BUILD-IN DELEGATES
     *  Action<T> - Accepts a single parameter and returns no value
     *  Action<T, T ...> - Accepts multiple parameter and returns no value
     *  Func<T, TResult> - Accepts a single parameter and return a value of type TResult
     *  Func<T, Y,... Z> - Accepts a multiple parameters and return a value of type Z
     */


    public delegate void WorkPerformedHandler(int hours, WorkType worktype);
    public delegate int BizRule(int hours, int rate);
    class Program
    {
        static void Main(string[] args)
        {
            //DelgateSample();
            //EventSample();
            //LamdaSample();
            QuerySample();
            Console.ReadLine();
        }

        private static void QuerySample()
        {
            var customers = new List<Customer>()
            {
                new Customer {Id=1, Name ="Pratik Bhattacharya", Country = "India", State = "Telengana" },
                new Customer {Id=2, Name ="Chris Martin", Country = "United States of America", State = "Wasington DC" },
                new Customer {Id=3, Name ="Anjay Gupta", Country = "India", State = "Telengana" },
            };

            var indians = 
                customers.Where(c => c.Country.Equals("India"));
            var americans = customers.Where(SearchAmerican);

        }

        private static bool SearchAmerican(Customer arg)
        {
            return arg.Country.Equals("United States of America");
        }

        static void DelgateSample()
        {
            WorkPerformedHandler logger = new WorkPerformedHandler(LogEvent);
            WorkPerformedHandler notify = new WorkPerformedHandler(NotifyUser);
            WorkPerformedHandler downstream = new WorkPerformedHandler(InformDownstream);

            Console.WriteLine("Individual Delegates");
            logger(9, WorkType.Heavy);
            notify(9, WorkType.Heavy);
            Console.Read();

            Console.WriteLine("Method Calls");
            DoWork(logger, 2, WorkType.Light);
            DoWork(notify, 2, WorkType.Light);
            Console.ReadLine();

            Console.WriteLine("Multicast");
            WorkPerformedHandler handler = logger + notify + downstream;
            DoWork(handler, 3, WorkType.Light);
        }

        static void EventSample()
        {
            var worker = new Worker();
            worker.WorkPerformed += new EventHandler<WorkPerformedEventArgs>(LogEvent);

            //worker.WorkCompleted += new EventHandler(Worker_WorkCompleted);
            //A.K.A
            worker.WorkCompleted += Worker_WorkCompleted;

            //Anonymous Method
            worker.WorkPerformed += delegate (object sender, WorkPerformedEventArgs e)
            {
                Console.WriteLine("Anonymous Event: Work Performed");
            };

            //Lamda Expression
            worker.WorkPerformed += (s, e) => Console.WriteLine("Lamda Expression: " + e.Hours);

            worker.Process(12, WorkType.Heavy);
        }

        static void LamdaSample()
        {
            BizRule simpleRule = (h, r) => h * r;
            BizRule fineRule = (h, r) => h * r - 100;

            var processor = new ProcessData();
            processor.Process(4, 150, simpleRule);
            processor.Process(4, 150, fineRule);

            Action<int, int> bizAction = ShowHoursAndRate;
            Action<int, int> bizAction2 = (x, y) => Console.WriteLine(x + " " + y);

            processor.ProcessAction(12, 3, bizAction);
            processor.ProcessAction(12, 3, bizAction2);

            Func<int, int, bool> fileLogger = (a, b) =>
            {
                Console.WriteLine("Logger: " + a + " " + b);
                return true;
            };

            processor.LogMessage(12, 3, fileLogger);
        }

        private static void ShowHoursAndRate(int hours, int rate)
        {
            Console.WriteLine("Hours:  " + hours + "  Rate: " + rate);
        }

        private static void Worker_WorkCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("Work Completed");
        }

        private static void LogEvent(object sender, WorkPerformedEventArgs e)
        {
            Console.WriteLine("Logging Data");
            Console.WriteLine(string.Format("Hours - {0}; Type - {1}", e.Hours, e.Type.ToString()));
        }

        static void DoWork(WorkPerformedHandler del, int hours, WorkType type)
        {
            del(hours, type);
        }

        static void LogEvent(int hours, WorkType type)
        {
            Console.WriteLine("Logging Data");
            Console.WriteLine(string.Format("Hours - {0}; Type - {1}", hours, type.ToString()));
        }

        static void NotifyUser(int hours, WorkType type)
        {
            Console.WriteLine("Notifying User");
            Console.WriteLine(string.Format("Hours - {0}; Type - {1}", hours, type.ToString()));
        }

        static void NotifyUser(object sender, WorkPerformedEventArgs e)
        {

        }

        static void InformDownstream(int hours, WorkType type)
        {
            Console.WriteLine("Informing Downstream");
            Console.WriteLine(string.Format("Hours - {0}; Type - {1}", hours, type.ToString()));
        }
    }

    public enum WorkType
    {
        Light = 0,
        Heavy
    }
    /*
    * Delegate
     *  public delegate void WorkPerformedHandler(int hours, string result)
     * Handler
     *  static void LoggingPerformed(int hours, string result)
     * Delegate Instance
     *  WorkPerformedHandler logger = new WorkPerformedHandler(LoggingPerformed)
     * Invoking Delegate
     *  logger(1, "Success")
     *  Multiple Delegates
     *   WorkPerformedHandler notifier = new WorkPerformedHandler(NotificationHandler)
     *   notifier += loggger
     *   notifier(1, "Success") - Both logger and notifier will be invoked with (1, "Success")
     *   
     *  In multicast delgates if there are multiple return types then only the last delegates return type will be considered.
     *  If del1 return x and del2 return y. del1 += del2. del1() will return y
     */
}
