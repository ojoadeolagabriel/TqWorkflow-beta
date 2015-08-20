using System;
using System.Threading.Tasks;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.Util;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.amq
{
    public class AmqConsumer : PollingConsumer
    {
        private readonly AmqProcessor _processor;

        public AmqConsumer(AmqProcessor processor)
        {
            _processor = processor;
        }

        public override Exchange Poll()
        {
            Task.Factory.StartNew(PollHandler);
            return null;
        }

        private static IConnection _connection;
        private IMessageConsumer consumer;

        public IConnection CreateConnection(UriDescriptor endPointDescriptor)
        {
            if (_connection != null)
                return _connection;

            try
            {
                var path = endPointDescriptor.ComponentPath;
                var factory = new ConnectionFactory(string.Format("activemq:tcp://{0}/", path));
                var connection = factory.CreateConnection();
                connection.Start();
                return _connection = connection;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void PollHandler()
        {
            var uriInfo = _processor.UriInformation;
            var queue = uriInfo.GetUriProperty("queue");
            var conn = CreateConnection(_processor.UriInformation);

            if(conn == null)
                return;

            var session = conn.CreateSession();
            var destination = SessionUtil.GetDestination(session, string.Format("queue://{0}", queue));
            consumer = session.CreateConsumer(destination);
            consumer.Listener += OnMessage;
        }

        private void OnMessage(IMessage message)
        {
            try
            {
                var msg = message as ITextMessage;
                if (msg == null) return;

                var ex = new Exchange(_processor.Route) { InMessage = { Body = msg.Text } };
                Camel.TryLog(ex, "consumer", "amqcomponent");
                _processor.Process(ex);
            }
            catch (Exception)
            {

            }
        }
    }
}
