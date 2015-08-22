using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve.dto;

namespace app.core.nerve.strategy
{
    public abstract class AggregationStrategy
    {
        public abstract Exchange Aggregate(Exchange oldExchange, Exchange newExchange);
    }
}
