using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.direct
{
    public class DirectProducer : DefaultProducer
    {
        public DirectProducer(UriDescriptor uriInformation, Route route) :
            base(uriInformation, route)
        {

        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var path = endPointDescriptor.ComponentPath;
            var route = Camel.GetRouteBy(path);

            if (route != null)
                route.CurrentRouteStep.ProcessChannel(exchange);
  
            return exchange;
       }
    }
}
