using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.normalizer
{
    /// <summary>
    /// Normalizer EndPoint.
    /// </summary>
    public class NormalizerEndPoint : DefaultEndpoint
    {
        public NormalizerEndPoint(string uri, Route route) : base(uri, route)
        {

        }

        public override void Send(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var producer = CreateProducer();
            producer.Process(exchange, endPointDescriptor);
        }

        public DefaultProducer CreateProducer()
        {
            return new NormalizerProducer(UriInformation, Route);
        }
    }
}
