using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve.dto;
using app.core.nerve.strategy;

namespace app.core.nerve.test
{
    public class DummyAggreationStrategy : AggregationStrategy
    {
        public override Exchange Aggregate(Exchange oldExchange, Exchange newExchange)
        {
            oldExchange.InMessage.SetHeader("strat-info", newExchange.InMessage.Body.ToString());
            newExchange.InMessage.Body = null;
            return oldExchange;
        }
    }
}
