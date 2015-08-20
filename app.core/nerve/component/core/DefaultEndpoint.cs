using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core
{
    public class DefaultEndpoint
    {
        public Route Route;
        public string ComponentTitle { get; set; }

        public virtual void Start()
        {

        }

        public string Uri { get; set; }
        public UriDescriptor UriInformation { get; set; }

        public DefaultEndpoint(string uri, Route route)
        {
            Route = route;
            Uri = uri;
            UriInformation = UriDescriptor.Parse(uri);
        }

        public virtual void Send(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            return;
        }
    }
}
