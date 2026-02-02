namespace SwitchService.Lib
{
    public sealed class ServiceStatus
    {
        public ServiceStatusCode Code { get; set; }

        public static ServiceStatus Default = new()
        {
            Code = ServiceStatusCode.Open
        };        
    }
}
