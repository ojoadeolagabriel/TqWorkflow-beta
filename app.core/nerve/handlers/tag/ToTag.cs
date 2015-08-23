using app.core.nerve.dto;
using app.core.nerve.expression;
using app.core.nerve.facade;

namespace app.core.nerve.handlers.tag
{
    public class ToTag
    {
        public static void Execute(string uri, Exchange exchange, Route route)
        {
            var leafNodeParts = UriDescriptor.Parse(uri, exchange);
            EndPointBuilder.HandleTo(leafNodeParts, exchange, route);
        }
    }
}
