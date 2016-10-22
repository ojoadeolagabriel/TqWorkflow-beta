using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core
{
    public class DefaultProducer
    {
        private UriDescriptor _uriInformation;
        private Route _route;

        public DefaultProducer(UriDescriptor uriInformation, Route route)
        {
            _uriInformation = uriInformation;
            _route = route;
        }
        public virtual Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            return null;
        }
    }
}
