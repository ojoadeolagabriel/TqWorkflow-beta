using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;

namespace app.core.workflow.component.core.mina
{
    public class MinaEndPoint : DefaultEndpoint
    {
        private MinaConsumer _consumer;

        public MinaEndPoint(string uri, Route route)
            : base(uri, route)
        {

        }

        public override void Start()
        {
            _consumer = (MinaConsumer)CreateConsumer();

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
            return new MinaConsumer(new MinaProcessor(UriInformation, Route));
        }

        public DefaultProducer CreateProducer()
        {
            return new MinaProducer(UriInformation, Route);
        }
    }
}
