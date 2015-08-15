using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using app.core.workflow.dto;
using app.core.workflow.facade;
using app.core.workflow.test;
using RedBranch.Hammock;

namespace app.core.workflow.component.core.couchdb
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
