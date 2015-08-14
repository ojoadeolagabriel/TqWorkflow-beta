using app.core.workflow.dto;
using app.core.workflow.expression;
using app.core.workflow.facade;

namespace app.core.workflow.handlers.tag
{
    public class FromTag
    {
        public static void Execute(string uri, Exchange exchange, Route route)
        {
            var leafNodeParts = UriDescriptor.Parse(uri);
            EndPointBuilder.HandleFrom(leafNodeParts, route);
        }
    }
}
