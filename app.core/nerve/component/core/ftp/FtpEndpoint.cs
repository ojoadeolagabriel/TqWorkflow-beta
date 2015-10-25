using app.core.nerve.dto;

namespace app.core.nerve.component.core.ftp
{
    class FtpEndpoint : DefaultEndpoint
    {
        private FtpConsumer _consumer;

        public FtpEndpoint(string uri, Route route)
            : base(uri, route)
        {

        }

        public override void Start()
        {
            _consumer = (FtpConsumer)CreateConsumer();

            if (_consumer.GetType().IsSubclassOf(typeof(PollingConsumer)))
            {
                _consumer.Poll();
            }
            else
            {
                _consumer.Execute();
            }
        }

        public DefaultConsumer CreateConsumer()
        {
            return new FtpConsumer(new FtpProcessor(UriInformation, base.Route));
        }
    }
}
