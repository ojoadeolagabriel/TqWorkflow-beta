using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.mina
{
    public class MinaEndPoint : DefaultEndpoint
    {
        private MinaConsumer _consumer;

        public MinaEndPoint(string uri, Route route)
            : base(uri, route)
        {

        }

        public override void Start()
        {
            _consumer = (MinaConsumer)CreateConsumer();

            if (_consumer.GetType().IsSubclassOf(typeof(PollingConsumer)))
            {
                _consumer.Poll();
            }
            else
            {
                _consumer.Execute();
            }
        }

        public override void Send(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var producer = CreateProducer();
            producer.Process(exchange, endPointDescriptor);
        }

        public DefaultConsumer CreateConsumer()
        {
            return new MinaConsumer(new MinaProcessor(UriInformation, Route));
        }

        public DefaultProducer CreateProducer()
        {
            return new MinaProducer(UriInformation, Route);
        }
    }
}
