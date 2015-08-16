using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core
{
    public abstract class DefaultProcessor
    {
        public UriDescriptor UriInformation;

        public Route Route;

        protected DefaultProcessor(UriDescriptor uriInformation, Route route)
        {
            UriInformation = uriInformation;
            Route = route;
        }

        public virtual void PrepareOut(Exchange exchange)
        {

        }

        public virtual Exchange Process(Exchange exchange)
        {
            if (Route.RouteProcess.NextTag != null)
                Route.RouteProcess.NextTag.Execute(exchange);
            return exchange;
        }
    }
}
