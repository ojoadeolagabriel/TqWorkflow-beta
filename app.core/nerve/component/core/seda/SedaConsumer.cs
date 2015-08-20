using app.core.nerve.dto;

namespace app.core.nerve.component.core.seda
{
    public class SedaConsumer : DefaultConsumer
    {
        private readonly SedaProcessor _sedaProcessor;

        public SedaConsumer(SedaProcessor sedaProcessor)
        {
            _sedaProcessor = sedaProcessor;
        }

        public override Exchange Execute()
        {
            var exchange = new Exchange(_sedaProcessor.Route);
            Camel.TryLog(exchange, "consumer", _sedaProcessor.UriInformation.ComponentName);

            _sedaProcessor.Process(exchange);
            return exchange;
        }
    }
}
