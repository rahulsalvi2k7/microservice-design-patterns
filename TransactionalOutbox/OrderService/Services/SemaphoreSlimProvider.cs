namespace OrderService.Services
{
    public class SemaphoreSlimProvider(int initialCount = 1)
    {
        private readonly SemaphoreSlim semaphore = new(initialCount);

        public async Task WaitAsync(CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);
        }

        public void Release()
        {
            semaphore.Release();
        }
    }
}
