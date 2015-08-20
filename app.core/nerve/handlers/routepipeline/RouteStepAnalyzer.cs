using System.Xml.Linq;
using app.core.application.error;
using app.core.nerve.dto;
using app.core.nerve.facade;
using app.core.workflow.dto;

namespace app.core.nerve.handlers.routepipeline
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
        /// <param name="logProvider"></param>
        public static void ProcessRouteInformation(XElement route, bool autoExec = false, ISystemLogProvider logProvider = null)
        {
            if (route == null)
                throw new AppCoreException("error loading route-config: route cannot be null");

            var steps = route.Elements();
            var newRoute = new Route {LogProvider = logProvider};
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
