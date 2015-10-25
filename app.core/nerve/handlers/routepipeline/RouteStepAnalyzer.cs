using System.Xml.Linq;
using app.core.application.error;
using app.core.nerve.dto;
using app.core.nerve.facade;

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
        /// <param name="bundleDescriptorObject"></param>
        public static void ProcessRouteInformation(XElement route, bool autoExec = false, ISystemLogProvider logProvider = null, BundleDescriptorObject bundleDescriptorObject = null)
        {
            if (route == null)
                throw new AppCoreException("error loading route-config: route cannot be null");

            var xmlRouteDesc = route.Attribute("description");
            var routeDesc = xmlRouteDesc != null ? xmlRouteDesc.Value : "";

            var steps = route.Elements();
            var newRoute = new Route { LogProvider = logProvider, Description = routeDesc, BundleInfo = bundleDescriptorObject };
            RouteStep nextRouteStepProcessorToLink = null;

            foreach (var step in steps)
            {
                if (newRoute.CurrentRouteStep == null)
                {
                    newRoute.CurrentRouteStep = new RouteStep(step, newRoute);
                    nextRouteStepProcessorToLink = newRoute.CurrentRouteStep;
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
                newRoute.CurrentRouteStep.ProcessChannel();
        }
    }
}
