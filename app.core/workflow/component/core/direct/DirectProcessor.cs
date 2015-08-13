using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.direct
{
    public class DirectProcessor : DefaultProcessor
    {
        public DirectProcessor(UriDescriptor uriInformation, Route route) :
            base(uriInformation, route)
        {

        }

        public override Exchange Process(Exchange exchange)
        {
            exchange.Route.RouteProcess.NextTag.Execute(exchange);
            return exchange;
        }
    }
}
