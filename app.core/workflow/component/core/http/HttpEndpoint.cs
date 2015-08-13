using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;

namespace app.core.workflow.component.core.http
{
    public class HttpEndpoint : DefaultEndpoint
    {
        private HttpConsumer _consumer;

        public HttpEndpoint(string uri, Route route) : base(uri, route)
        {

        }

        public override void Start()
        {
            _consumer = (HttpConsumer)CreateConsumer();

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
            return new HttpConsumer(new HttpProcessor(UriInformation, base.Route));
        }
    }
}
