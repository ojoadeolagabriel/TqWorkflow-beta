using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.http
{
    public class HttpProcessor : DefaultProcessor
    {
        public HttpProcessor(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {
            
        }
    }
}
