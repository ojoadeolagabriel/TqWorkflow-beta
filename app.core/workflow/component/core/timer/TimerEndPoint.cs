using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;

namespace app.core.workflow.component.core.timer
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
