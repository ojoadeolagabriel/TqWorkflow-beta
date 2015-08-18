using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using app.core.application.error;
using app.core.workflow.dto;
using app.core.workflow.error;
using app.core.workflow.facade;
using app.core.workflow.handlers.pattern;

namespace app.core.workflow.handlers.routepipeline
{
    /// <summary>
    /// RoutePipeline Default Processor.
    /// </summary>
    public class RouteStepAnalyzer
    {
        /// <summary>
        /// ProcessRouteInformation Now.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="autoExec"></param>
        public static void ProcessRouteInformation(XElement route, bool autoExec = false)
        {
            if (route == null)
                throw new AppCoreException("error loading route-config: route cannot be null");

            var steps = route.Elements();
            var newRoute = new Route();
            RouteStep nextRouteStepProcessorToLink = null;

            foreach (var step in steps)
            {
                if (newRoute.RouteProcess == null)
                {
                    newRoute.RouteProcess = new RouteStep(step, newRoute);
                    nextRouteStepProcessorToLink = newRoute.RouteProcess;
                }
                else
                {
                    var nextStep = new RouteStep(step, newRoute);
                    if (nextRouteStepProcessorToLink == null) continue;

                    nextRouteStepProcessorToLink.NextTag = nextStep;
                    nextRouteStepProcessorToLink = nextRouteStepProcessorToLink.NextTag;
                }
            }

            Camel.SetRoute(newRoute);
            if (autoExec)
                newRoute.RouteProcess.ProcessChannel();
        }
    }
}
