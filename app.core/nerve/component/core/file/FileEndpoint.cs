using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.file
{
    /// <summary>
    /// File Endpoint
    /// </summary>
    public class FileEndpoint : DefaultEndpoint
    {
        /// <summary>
        /// File Consumer
        /// </summary>
        private FileConsumer _consumer;

        public override void Send(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var producer = CreateProducer();
            producer.Process(exchange, endPointDescriptor);
        }

        /// <summary>
        /// Start work
        /// </summary>
        public override void Start()
        {
            _consumer = (FileConsumer)CreateConsumer();

            if (_consumer.GetType().IsSubclassOf(typeof(PollingConsumer)))
            {
                _consumer.Poll();
            }
            else
            {
                _consumer.Execute();
            }
        }

        /// <summary>
        /// Init endpoint
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="route"></param>
        public FileEndpoint(string uri, Route route)
            : base(uri, route)
        {

        }

        public DefaultConsumer CreateConsumer()
        {
            return new FileConsumer(new FileProcessor(UriInformation, base.Route));
        }

        public DefaultProducer CreateProducer()
        {
            return new FileProducer(UriInformation, Route);
        }
    }
}
