using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.jwt
{
    public class JwtEndPoint : DefaultEndpoint
    {

        public JwtEndPoint(string uri, Route route) : base(uri, route)
        {

        }

        /// <summary>
        /// Send
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="endPointDescriptor"></param>
        public override void Send(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var producer = CreateProducer();
            producer.Process(exchange, endPointDescriptor);
        }

        public DefaultProducer CreateProducer()
        {
            return new JwtProducer(UriInformation, Route);
        }
    }
}
