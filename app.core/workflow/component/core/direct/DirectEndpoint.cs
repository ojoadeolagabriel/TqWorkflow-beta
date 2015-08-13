using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.component.core.file;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.direct
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
