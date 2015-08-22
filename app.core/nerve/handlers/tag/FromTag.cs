using app.core.nerve.dto;
using app.core.nerve.facade;

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
