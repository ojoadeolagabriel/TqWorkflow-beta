using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using app.core.nerve.dto;
using app.core.nerve.strategy;
using app.core.utility;

namespace paymentnotification.generic.bundle.codebase
{
    public class RemoveRouteAggregationStrategy: AggregationStrategy
    {
        public override Exchange Aggregate(Exchange oldExchange, Exchange newExchange)
        {
            var dateTime = XElement.Parse(newExchange.InMessage.Body.ToString());
            var result = XmlHelper.GetValue<DateTime>(dateTime, "datetime");

            var notifyRes = XElement.Parse(oldExchange.InMessage.Body.ToString());
            notifyRes.Add(new XElement("aggregator_date_time", result));
            oldExchange.InMessage.Body = notifyRes.ToString();
            return oldExchange;
        }
    }
}
