namespace orchestratorService
{
    public class OrderSaga
    {
        private readonly HttpClient paymentHttpClient;
        private readonly HttpClient orderHttpClient;

        public OrderSaga(IHttpClientFactory httpClientFactory)
        {
            paymentHttpClient = httpClientFactory.CreateClient();
            orderHttpClient = httpClientFactory.CreateClient();

            paymentHttpClient.BaseAddress = new Uri("http://localhost:5016");
            orderHttpClient.BaseAddress = new Uri("http://localhost:5009");
        }

        public async Task ExecuteAsync(OrderSagaRequest orderSagaRequest)
        {
            HttpResponseMessage response;

            try
            {
                Console.WriteLine($"{DateTime.UtcNow:s} => saga started {orderSagaRequest.SagaId}");

                // Business logic e.g. try calling payment service
                Console.WriteLine($"{DateTime.UtcNow:s} => Attempting payment {orderSagaRequest.SagaId}");
                response = await paymentHttpClient.GetAsync($"/pay/{orderSagaRequest.OrderId}");

                response.EnsureSuccessStatusCode();

                // Simulate long running business process
                await Task.Delay(5_000);

                // Call Order service for 2Phase commit
                Console.WriteLine($"{DateTime.UtcNow:s} => Calling order complete {orderSagaRequest.SagaId}");
                response = await orderHttpClient.GetAsync($"/complete/{orderSagaRequest.OrderId}");

                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                Console.WriteLine($"{DateTime.UtcNow:s} => Payment failed {orderSagaRequest.SagaId}");
                Console.WriteLine($"{DateTime.UtcNow:s} => Calling order cancel {orderSagaRequest.SagaId}");
                _ = await orderHttpClient.GetAsync($"/cancel/{orderSagaRequest.OrderId}");
            }
        }
    }
}