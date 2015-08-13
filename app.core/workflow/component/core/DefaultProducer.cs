using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core
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
