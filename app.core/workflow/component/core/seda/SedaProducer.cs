using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.seda
{
    public class SedaProducer : DefaultProducer
    {
        public SedaProducer(UriDescriptor uriInformation, Route route) : base(uriInformation, route)
        {
        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            Route route;
            if (!Camel.RouteCollection.TryGetValue(endPointDescriptor.ComponentPath, out route)) return exchange;

            exchange.Route = route;
            Seda.SedaQueue.TryAdd(endPointDescriptor, exchange);
            return exchange;
        }
    }
}
