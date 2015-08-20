using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.direct
{
    public class DirectEndpoint : DefaultEndpoint
    {
        private DirectConsumer _consumer;

        public DirectEndpoint(string uri, Route route) : base(uri, route)
        {

        }

        public override void Start()
        {
            _consumer = CreateConsumer();
        }

        public override void Send(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var producer = CreateProducer();
            producer.Process(exchange, endPointDescriptor);
        }

        public DirectConsumer CreateConsumer()
        {
            return new DirectConsumer(new DirectProcessor(UriInformation, Route));
        }

        public DefaultProducer CreateProducer()
        {
            return new DirectProducer(UriInformation, Route);
        }
    }
}
