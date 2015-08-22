using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using app.core.nerve.component.core;
using app.core.nerve.dto;
using app.core.nerve.facade;
using app.core.nerve.registry;

namespace app.core.nerve
{
    /// <summary>
    ///     Camel
    /// </summary>
    public class Camel
    {
        /// <summary>
        /// Route Collection.
        /// </summary>
        public static readonly ConcurrentDictionary<string, Route> RouteCollection = new ConcurrentDictionary<string, Route>();

        public static void TryLog(Exchange exchange, string processorType = "consumer", string componentName = "--")
        {
            if (exchange.Route.LogProvider == null)
                return;

            exchange.Route.LogProvider.Log(exchange, processorType, componentName);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly ConcurrentDictionary<string, DefaultEndpoint> EnPointCollection = new ConcurrentDictionary<string, DefaultEndpoint>();

        /// <summary>
        ///     Bean registry.
        /// </summary>
        public static BeanRegistry Registry = new BeanRegistry();

        public static void StartSedaProcessor()
        {
            new Seda().ProcessSedaMessageQueue();
        }

        public static void SetRoute(Route route)
        {
            try
            {
                RouteCollection.TryAdd(route.RouteId, route);
            }
            catch
            {

            }
        }

        public static Route GetRouteBy(string direct)
        {
            var data = RouteCollection.FirstOrDefault(c => c.Value.RouteId == direct);
            return data.Value;
        }

        public static Route GetRoute(string routeId)
        {
            try
            {
                return RouteCollection[routeId];
            }
            catch
            {
                return null;
            }
        }

        public static void InitDependencyLibs(List<string> namespaces)
        {
            EndPointBuilder.BuildNamespaces(namespaces);
        }

        public static void StartAllRoutes()
        {
            RouteCollection.ToList().ForEach(c=>c.Value.RouteProcess.ProcessChannel());
        }
    }
}