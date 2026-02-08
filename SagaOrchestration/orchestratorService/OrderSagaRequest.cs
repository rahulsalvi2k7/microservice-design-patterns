namespace orchestratorService
{
    public sealed record OrderSagaRequest
    {
        public OrderSagaRequest(Guid sagaId, string orderId)
        {
            SagaId = sagaId;
            OrderId = orderId;
        }

        public Guid SagaId { get; set; }

        public string OrderId { get; }
    }
}