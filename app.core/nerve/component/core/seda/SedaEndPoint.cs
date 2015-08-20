using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.seda
{
    public class SedaEndPoint : DefaultEndpoint
    {
        private SedaConsumer _consumer;

        public SedaEndPoint(string uri, Route route)
            : base(uri, route)
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

        public SedaConsumer CreateConsumer()
        {
            return new SedaConsumer(new SedaProcessor(UriInformation, Route));
        }

        public SedaProducer CreateProducer()
        {
            return new SedaProducer(UriInformation, Route);
        }
    }
}
