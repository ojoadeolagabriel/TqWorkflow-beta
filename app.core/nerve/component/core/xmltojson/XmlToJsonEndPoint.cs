using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.xmltojson
{
    public class XmlToJsonEndPoint : DefaultEndpoint
    {
        public XmlToJsonEndPoint(string uri, Route route) : base(uri, route)
        {
        }

        public override void Send(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var producer = CreateProducer();
            producer.Process(exchange, endPointDescriptor);
        }

        public DefaultProducer CreateProducer()
        {
            return new XmlToJsonProducer(UriInformation, Route);
        }
    }
}
