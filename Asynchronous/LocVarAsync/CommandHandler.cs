namespace LocVarAsync
{
    public class CommandHandler
    {
        private readonly ServiceProvider _serviceProvider;
        public CommandHandler(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Handle(string trackingGuid)
        {
            return _serviceProvider.CreateData(trackingGuid);
        }
    }
}