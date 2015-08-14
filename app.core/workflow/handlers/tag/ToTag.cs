using app.core.workflow.dto;
using app.core.workflow.expression;
using app.core.workflow.facade;

namespace app.core.workflow.handlers.tag
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
