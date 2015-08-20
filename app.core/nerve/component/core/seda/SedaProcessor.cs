using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.seda
{
    public class SedaProcessor : DefaultProcessor
    {
        public SedaProcessor(UriDescriptor uriInformation, Route route) : base(uriInformation, route)
        {
        }

        public override Exchange Process(Exchange exchange)
        {
            exchange.Route.RouteProcess.NextTag.ProcessChannel(exchange);
            return base.Process(exchange);
        }
    }
}
