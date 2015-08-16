using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.component.core.direct;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.seda
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
