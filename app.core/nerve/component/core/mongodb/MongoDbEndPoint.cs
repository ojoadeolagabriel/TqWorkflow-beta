using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.mongodb
{
    public class MongoDbEndPoint : DefaultEndpoint
    {
        private MongoDbConsumer _consumer;
        public MongoDbEndPoint(string uri, Route route) : base(uri, route)
        {
        }

        public override void Send(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var producer = CreateProducer();
            producer.Process(exchange, endPointDescriptor);
        }

        public override void Start()
        {
            _consumer = (MongoDbConsumer)CreateConsumer();

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
            return new MongoDbConsumer(new MongoDbProcessor(UriInformation, base.Route));
        }

        private DefaultProducer CreateProducer()
        {
            return new MongoDbProducer(UriInformation,Route);
        }
    }
}
