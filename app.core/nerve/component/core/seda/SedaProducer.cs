using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.seda
{
    public class SedaProducer : DefaultProducer
    {
        public SedaProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {
        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            Route route;
            if (!Camel.RouteCollection.TryGetValue(endPointDescriptor.ComponentPath, out route))
                return exchange;

            Camel.TryLog(exchange, "producer", endPointDescriptor.ComponentName);

            exchange.Route = route;
            Seda.SedaQueue.TryAdd(endPointDescriptor, exchange);
            return exchange;
        }
    }
}
