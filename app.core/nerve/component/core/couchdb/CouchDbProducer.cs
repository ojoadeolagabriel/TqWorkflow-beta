using System;
using System.Linq;
using app.core.nerve.dto;
using app.core.nerve.facade;
using RedBranch.Hammock;

namespace app.core.nerve.component.core.couchdb
{
    public class CouchDbProducer : DefaultProducer
    {
        public CouchDbProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {
        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            try
            {
                Camel.TryLog(exchange, "provider", endPointDescriptor.ComponentName);

                var dbHost = endPointDescriptor.ComponentPath;
                var port = endPointDescriptor.GetUriProperty<int>("port");
                var couchDbDatabase = exchange.InMessage.GetHeader("CouchDbDatabase");
                var createDb = endPointDescriptor.GetUriProperty<bool>("createDb");
                var document = exchange.InMessage.Body;

                var dbUrl = string.Format("http://{0}:{1}", dbHost, port);
                var conn = new Connection(new Uri(dbUrl));

                if (createDb && !conn.ListDatabases().Contains(couchDbDatabase))
                {
                    conn.CreateDatabase(couchDbDatabase.ToString());
                }
            }
            catch (Exception exception)
            {
            }

            return base.Process(exchange, endPointDescriptor);
        }
    }
}
