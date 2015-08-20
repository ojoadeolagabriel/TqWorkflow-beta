using app.core.nerve.dto;
using app.core.nerve.expression;
using app.core.nerve.facade;
using app.core.workflow.dto;

namespace app.core.nerve.handlers.tag
{
    public class ToTag
    {
        public static void Execute(string uri, Exchange exchange, Route route)
        {
            var leafNodeParts = UriDescriptor.Parse(uri);

            leafNodeParts.ComponentQueryPath = SimpleExpression.ResolveExpression(leafNodeParts.ComponentQueryPath, exchange);
            leafNodeParts.ComponentPath = SimpleExpression.ResolveExpression(leafNodeParts.ComponentPath, exchange);

            EndPointBuilder.HandleTo(leafNodeParts, exchange, route);
        }
    }
}
