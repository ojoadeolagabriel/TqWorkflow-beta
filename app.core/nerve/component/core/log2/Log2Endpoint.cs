using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.log2
{
    public class Log2Endpoint : DefaultEndpoint
    {
        public Log2Endpoint(string uri, Route route):
            base(uri, route)
        {
             
        }

        public override void Send(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var producer = CreateProducer();
            producer.Process(exchange, endPointDescriptor);
        }

        public DefaultProducer CreateProducer()
        {
            return new Log2Producer(UriInformation, Route);
        }
    }
}
