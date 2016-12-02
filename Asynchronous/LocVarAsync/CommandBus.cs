using System;
using System.Threading;

namespace LocVarAsync
{
    public class CommandBus
    {
        private readonly CommandHandler _handler;
        public CommandBus()
        {
            var serviceProvider = new ServiceProvider();
            _handler = new CommandHandler(serviceProvider);
        }

        public string SendCommandAndCreateData(string trackingData)
        {
            if (string.IsNullOrEmpty(trackingData))
                trackingData = Guid.NewGuid().ToString();

            var data = _handler.Handle(trackingData);
            return data;
        }
    }
}