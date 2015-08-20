using app.core.nerve.dto;
using app.core.nerve.facade;
using app.core.workflow.dto;

namespace app.core.nerve.handlers.tag
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
