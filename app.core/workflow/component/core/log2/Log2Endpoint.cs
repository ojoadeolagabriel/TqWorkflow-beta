using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.log2
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
