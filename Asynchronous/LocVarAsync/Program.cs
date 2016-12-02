using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LocVarAsync
{
    public class Program
    {
        public static int ThreadCount = 10;
        public static void Main()
        {
            var commandBus = new CommandBus();
            var tasks = new List<Task<string>>();

            for (var itr = 0; itr <= ThreadCount; itr++)
            {
                tasks.Add(
                    Task.Run(() =>
                    {
                        var guid = Guid.NewGuid().ToString();
                        Console.WriteLine($"Thread - {Thread.CurrentThread.ManagedThreadId} | Tracker - {guid}");
                        var data = commandBus.SendCommandAndCreateData(guid);
                        //Console.WriteLine($"Data Received - {data}. Thread ID - {Thread.CurrentThread.ManagedThreadId}");
                        return data;
                    })
                );
            }


            foreach(var task in tasks)
            {
                var res = task.Result;
                Console.WriteLine($"After thread completion. Task ID - {task.Id}. Result - {res}");
            }

            Console.ReadLine();
        }
    }
}