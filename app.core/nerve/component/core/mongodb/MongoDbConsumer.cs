using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using app.core.nerve.dto;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace app.core.nerve.component.core.mongodb
{
    public class MongoDbConsumer : PollingConsumer
    {
        private MongoDbProcessor _processor;
        public MongoDbConsumer(MongoDbProcessor processor)
        {
            _processor = processor;
        }

        public override Exchange Poll()
        {
            try
            {
                Task.Factory.StartNew(PollHandler);
                return null;
            }
            catch (AggregateException exception)
            {
                
            }
            catch (Exception exception)
            {
                
            }

            return null;
        }

        private void PollHandler()
        {
            var exchange = new Exchange(_processor.Route);
            var noResultDelayInterval = _processor.UriInformation.GetUriProperty("noResultDelayInterval", 500);
            var initialDelay = _processor.UriInformation.GetUriProperty("initialDelay", 3000);
            var host = _processor.UriInformation.GetUriProperty("host");
            var database = _processor.UriInformation.GetUriProperty("database");
            var collection = _processor.UriInformation.GetUriProperty("collection");
            var readQuery = _processor.UriInformation.GetUriProperty("readQuery");
            var inparrallel = _processor.UriInformation.GetUriProperty<bool>("inparrallel");
            var createCollection = _processor.UriInformation.GetUriProperty<bool>("createCollection");

            if (initialDelay > 0)
                Thread.Sleep(initialDelay);

            var client = new MongoClient("mongodb://" + host);
            var db = client.GetServer().GetDatabase(database);

            if (!db.CollectionExists(collection) && createCollection)
                db.CreateCollection(collection);

            do
            {
                try
                {
                    var data = db.GetCollection(collection).FindOne();
                    if (data != null && CanRun(_processor))
                    {
                        if (inparrallel)
                            Task.Factory.StartNew(() => HandleResult(data, exchange));
                        else
                            HandleResult(data, exchange);
                    }
                    else
                    {
                        Thread.Sleep(noResultDelayInterval);
                    }
                }
                catch (AggregateException aggregateException)
                {
                    
                }
                catch (Exception exception)
                {
                    
                }

            } while (true);
        }

        private void HandleResult(BsonDocument data, Exchange exchange)
        {
            var bsonString = data.ToString();
            exchange.InMessage.Body = bsonString;
            _processor.Process(exchange);
        }
    }
}
