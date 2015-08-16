using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.direct
{
    public class DirectProducer : DefaultProducer
    {
        public DirectProducer(UriDescriptor uriInformation, Route route) :
            base(uriInformation, route)
        {

        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var path = endPointDescriptor.ComponentPath;
            var route = Camel.GetRouteBy(path);

            if (route != null)
                route.RouteProcess.Execute(exchange);

            return exchange;
       }
    }
}
