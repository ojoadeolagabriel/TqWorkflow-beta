using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.jwt
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
