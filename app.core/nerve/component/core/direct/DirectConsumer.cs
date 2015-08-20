using app.core.nerve.dto;

namespace app.core.nerve.component.core.direct
{
    public class DirectConsumer : DefaultConsumer
    {
        private readonly DirectProcessor _directProcessor;

        public DirectConsumer(DirectProcessor directProcessor)
        {
            _directProcessor = directProcessor;
        }

        public override Exchange Execute()
        {
            var exchange = new Exchange(_directProcessor.Route);
            Camel.TryLog(exchange, "consumer", _directProcessor.UriInformation.ComponentName);

            _directProcessor.Process(exchange);
            return exchange;
        }
    }
}
