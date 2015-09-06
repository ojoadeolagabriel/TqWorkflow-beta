using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core
{
    public abstract class DefaultProcessor
    {
        public UriDescriptor UriInformation;

        public Route Route;

        protected DefaultProcessor(UriDescriptor uriInformation, Route route)
        {
            UriInformation = uriInformation;
            Route = route;
        }

        public virtual void PrepareOut(Exchange exchange)
        {

        }

        public virtual Exchange Process(Exchange exchange)
        {
            if (Route.CurrentRouteStep.NextTag != null)
                Route.CurrentRouteStep.NextTag.ProcessChannel(exchange);
            return exchange;
        }
    }
}
