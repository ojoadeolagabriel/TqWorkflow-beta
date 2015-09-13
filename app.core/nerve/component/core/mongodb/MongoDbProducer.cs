using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve.dto;
using app.core.nerve.facade;
using MongoDB.Bson;
using MongoDB.Driver;

namespace app.core.nerve.component.core.mongodb
{
    public class MongoDbProducer : DefaultProducer
    {
        public enum OperationType
        {
            FetchById, Insert, FetchAll
        }

        public MongoDbProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {
        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var host = endPointDescriptor.GetUriProperty("host");
            var database = endPointDescriptor.GetUriProperty("database");
            var collection = endPointDescriptor.GetUriProperty("collection");
            var createCollection = endPointDescriptor.GetUriProperty<bool>("createCollection");
            var operation = endPointDescriptor.GetUriProperty<OperationType>("operation");

            var client = new MongoClient("mongodb://" + host);
            var db = client.GetServer().GetDatabase(database);

            if (!db.CollectionExists(collection) && createCollection)
                db.CreateCollection(collection);

            var doc = BsonDocument.Parse(exchange.InMessage.Body.ToString());

            ServeOperation(db, exchange, endPointDescriptor, doc, operation, collection);

            return exchange;
        }

        private void ServeOperation(MongoDatabase db, Exchange exchange, UriDescriptor endPointDescriptor, 
            BsonDocument doc, OperationType operation, string collection)
        {
            switch (operation)
            {
                case OperationType.Insert:
                    db.GetCollection(collection).Insert(doc);
                    break;
                case OperationType.FetchById:
                    break;
            }
        }
    }
}
