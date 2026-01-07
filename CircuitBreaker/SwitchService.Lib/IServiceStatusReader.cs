namespace SwitchService.Lib
{
    public interface IServiceStatusReader
    {
        Task<ServiceStatus> ReadServiceStatusAsync();
    }
}
