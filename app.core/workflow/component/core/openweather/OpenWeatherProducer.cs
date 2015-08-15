using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using app.core.workflow.dto;
using app.core.workflow.facade;
using System.Threading;

namespace app.core.workflow.component.core.openweather
{
    public class OpenWeatherProducer : DefaultProducer
    {
        public OpenWeatherProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {
        }

        class PassData
        {
            public Exchange Exchange { get; set; }
            public UriDescriptor UriInfo { get; set; }
        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            Task.Factory.StartNew(() => PollHandler(exchange, endPointDescriptor));
            return exchange;
        }

        private Timer timer;
        private void PollHandler(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var poll = endPointDescriptor.GetUriProperty("poll", 1000);
            var dueTime = endPointDescriptor.GetUriProperty("dueTime", 1000);

            timer = new Timer(CallBack, new PassData { Exchange = exchange, UriInfo = endPointDescriptor }, dueTime, poll);
        }

        private void CallBack(object state)
        {
            var stateData = (PassData)state;
            var location = stateData.UriInfo.GetUriProperty("location", "");

            using (var client = new WebClient())
            {
                var queryKeyValue = UriDescriptor.BuildKeyValueListWithEquality("");
                queryKeyValue.ForEach(c => client.QueryString.Add(c.Key, c.Value));

                client.DownloadString(location);
            }
        }
    }
}
