using System;

namespace ConsoleApplication1
{
    public class ProcessData
    {
        public void Process(int hours, int rate, BizRule ruleDelegate)
        {
            var result = ruleDelegate(hours, rate);
            Console.WriteLine("Data Processed - Result: " + result);
        }

        public void ProcessAction(int hours, int rate, Action<int, int> rule)
        {
            //No return type
            rule(hours, rate);
        }

        public bool LogMessage(int hours, int rate, Func<int, int, bool> loggerFunc)
        {
            return loggerFunc(hours, rate);
        }
    }
}