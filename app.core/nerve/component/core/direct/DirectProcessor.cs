using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.direct
{
    public class DirectProcessor : DefaultProcessor
    {
        public DirectProcessor(UriDescriptor uriInformation, Route route) :
            base(uriInformation, route)
        {

        }

        public override Exchange Process(Exchange exchange)
        {
            exchange.Route.CurrentRouteStep.NextTag.ProcessChannel(exchange);
            return exchange;
        }
    }
}
