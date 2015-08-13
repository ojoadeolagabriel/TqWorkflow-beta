using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.amq
{
    public class AmqEndPoint : DefaultEndpoint
    {
        private AmqConsumer _consumer;

        public AmqEndPoint(string uri, Route route)
            : base(uri, route)
        {
        }

        public override void Send(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var producer = CreateProducer();
            producer.Process(exchange, endPointDescriptor);
        }

        public override void Start()
        {
            _consumer = (AmqConsumer)CreateConsumer();

            if (_consumer.GetType().IsSubclassOf(typeof(PollingConsumer)))
            {
                _consumer.Poll();
            }
            else
            {
                _consumer.Execute();
            }
        }

        public DefaultConsumer CreateConsumer()
        {
            return new AmqConsumer(new AmqProcessor(UriInformation, Route));
        }
        public DefaultProducer CreateProducer()
        {
            return new AmqProducer(UriInformation, Route);
        }
    }
}
