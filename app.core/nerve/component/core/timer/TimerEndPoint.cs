using app.core.nerve.dto;

namespace app.core.nerve.component.core.timer
{
    /// <summary>
    /// 
    /// </summary>
    public class TimerEndPoint : DefaultEndpoint
    {
        private TimerConsumer _consumer;

        public TimerEndPoint(string uri, Route route) : base(uri, route)
        {

        }

        public override void Start()
        {
            _consumer = (TimerConsumer)CreateConsumer();

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
            return new TimerConsumer(new TimerProcessor(UriInformation, base.Route));
        }
    }
}
