using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.amq
{
    public class AmqProcessor : DefaultProcessor
    {
        public AmqProcessor(UriDescriptor uriInformation, Route route) : base(uriInformation, route)
        {

        }

        public override void PrepareOut(Exchange exchange)
        {
            
            base.PrepareOut(exchange);
        }
    }
}
