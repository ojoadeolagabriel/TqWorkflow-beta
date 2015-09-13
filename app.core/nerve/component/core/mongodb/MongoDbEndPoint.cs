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
        public MongoDbEndPoint(string uri, Route route) : base(uri, route)
        {
        }

        public override void Send(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var producer = CreateProducer();
            producer.Process(exchange, endPointDescriptor);
        }

        private DefaultProducer CreateProducer()
        {
            return new MongoDbProducer(UriInformation,Route);
        }
    }
}
