using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using app.core.workflow.dto;
using app.core.workflow.facade;
using Newtonsoft.Json;

namespace app.core.workflow.component.core.amq
{
    public class AmqProducer : DefaultProducer
    {
        public AmqProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {

        }

        private static IConnection _connection;

        public IConnection CreateConnection(UriDescriptor endPointDescriptor)
        {
            if (_connection != null)
                return _connection;

            var path = endPointDescriptor.ComponentPath;
            var factory = new ConnectionFactory(string.Format("tcp://{0}/", path));
            var connection = factory.CreateConnection();
            return _connection = connection;
        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {           
            try
            {                
                var connection = CreateConnection(endPointDescriptor);
                var queue = endPointDescriptor.GetUriProperty("queue", "test_queue");

                using (var session = connection.CreateSession())
                {
                    IDestination temporaryDestination = session.CreateTemporaryQueue();

                    var topic = session.GetQueue(queue);
                    var producer = session.CreateProducer(topic);
                    producer.RequestTimeout = new TimeSpan(0, 10, 0);

                    //create message
                    var messageSer = JsonConvert.SerializeObject(exchange);
                    var message = session.CreateTextMessage(messageSer);

                    message.NMSCorrelationID = exchange.ExchangeId.ToString();
                    message.NMSReplyTo = temporaryDestination;
                    producer.Send(message);
                }
            }
            catch (AggregateException aggregateException)
            {

            }
            catch(Exception exception)
            {
                exchange.SetException(new Exception("error connecting to amq", exception));
            }

            return exchange;
        }
    }
}
