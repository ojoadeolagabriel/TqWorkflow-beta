using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow
{
    public class RouteBuilder
    {
        private Route route = new Route();
        RouteStep nextRouteStepProcessorToLink = null;

        public void Build(Action<RouteBuilder> builder)
        {
            
        }

        public RouteBuilder From(string uri)
        {
            var node = new XElement("from", new XAttribute("uri", uri));
            var step = new RouteStep(node, route);

            route.RouteProcess = step;
            nextRouteStepProcessorToLink = route.RouteProcess;
            return this;
        }
        public RouteBuilder To(string uri)
        {
            var node = new XElement("to", new XAttribute("uri", uri));

            if (route.RouteProcess == null)
            {
                route.RouteProcess = new RouteStep(node, route);
                nextRouteStepProcessorToLink = route.RouteProcess;
            }
            else
            {
                var nextStep = new RouteStep(node, route);
                if (nextRouteStepProcessorToLink == null) 
                    return this;

                nextRouteStepProcessorToLink.NextTag = nextStep;
                nextRouteStepProcessorToLink = nextRouteStepProcessorToLink.NextTag;
            }

            return this;
        }
    }
}
