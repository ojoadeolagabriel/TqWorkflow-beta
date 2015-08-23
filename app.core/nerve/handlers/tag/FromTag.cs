using app.core.nerve.dto;
using app.core.nerve.expression;
using app.core.nerve.facade;

namespace app.core.nerve.handlers.tag
{
    public class FromTag
    {
        public static void Execute(string uri, Exchange exchange, Route route)
        {
            uri = SimpleExpression.ResolveSpecifiedUriPart(uri, exchange);
            var leafNodeParts = UriDescriptor.Parse(uri,exchange);
            EndPointBuilder.HandleFrom(leafNodeParts, route);
        }
    }
}
