using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using app.core.nerve.dto;
using app.core.nerve.facade;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace app.core.nerve.component.core.mongodb
{
    public class MongoDbProducer : DefaultProducer
    {
        public enum OperationType
        {
            FetchById, Insert, FetchAll, Remove
        }

        public MongoDbProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {
        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var host = endPointDescriptor.GetUriProperty("host");
            var isBodyXml = endPointDescriptor.GetUriProperty<bool>("isBodyXml");
            var database = endPointDescriptor.GetUriProperty("database");
            var collection = endPointDescriptor.GetUriProperty("collection");
            var createCollection = endPointDescriptor.GetUriProperty<bool>("createCollection");
            var operation = endPointDescriptor.GetUriProperty<OperationType>("operation");

            var client = new MongoClient("mongodb://" + host);
            var db = client.GetServer().GetDatabase(database);

            if (!db.CollectionExists(collection) && createCollection)
                db.CreateCollection(collection);

            var bson = exchange.InMessage.Body.ToString();

            if (isBodyXml)
            {
                var doc = new XmlDocument();
                doc.LoadXml(exchange.InMessage.Body.ToString());
                bson = JsonConvert.SerializeXmlNode(doc);
            }
            var bsonDoc = BsonDocument.Parse(bson);

            ServeOperation(db, exchange, endPointDescriptor, bsonDoc, operation, collection);
            return exchange;
        }

        private static void ServeOperation(MongoDatabase db, Exchange exchange,
            UriDescriptor endPointDescriptor,
            BsonDocument doc, OperationType operation, string collection)
        {
            switch (operation)
            {
                case OperationType.Insert:
                    db.GetCollection(collection).Insert(doc);
                    break;
                case OperationType.FetchById:
                    db.GetCollection(collection).FindOneById("");
                    break;
                case OperationType.FetchAll:
                    db.GetCollection(collection).FindAll();
                    break;
                case OperationType.Remove:
                    db.GetCollection(collection).Remove(null);
                    break;
            }
        }
    }
}
