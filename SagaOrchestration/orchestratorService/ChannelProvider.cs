using System.Threading.Channels;

namespace orchestratorService
{
    public class ChannelProvider
    {
        private readonly Channel<OrderSagaRequest> _channel;

        public ChannelReader<OrderSagaRequest> ChannelReader { get; }

        public ChannelWriter<OrderSagaRequest> ChannelWriter { get; }

        public ChannelProvider()
        {
            _channel = Channel.CreateUnbounded<OrderSagaRequest>();
            ChannelReader = _channel.Reader;
            ChannelWriter = _channel.Writer;
        }
    }
}