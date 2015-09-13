using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve.dto;
using app.core.nerve.facade;
using MongoDB.Driver;

namespace app.core.nerve.component.core.mongodb
{
    public class MongoDbProducer : DefaultProducer
    {
        public MongoDbProducer(UriDescriptor uriInformation, Route route) : base(uriInformation, route)
        {
        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var host = endPointDescriptor.GetUriProperty("host");
            var client = new MongoClient("mongodb://" + host);
            var db = client.GetServer().GetDatabase("local");
            var coll = db.CreateCollection("user_info");
            
            //var data = coll.FindAll();

            return exchange;
        }
    }
}
