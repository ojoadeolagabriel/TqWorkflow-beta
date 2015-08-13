using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using app.core.workflow.component.core;
using app.core.workflow.dto;
using app.core.workflow.facade;
using app.core.workflow.registry;

namespace app.core.workflow
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

        /// <summary>
        /// 
        /// </summary>
        public static readonly ConcurrentDictionary<string, DefaultEndpoint> EnPointCollection = new ConcurrentDictionary<string, DefaultEndpoint>();

        /// <summary>
        ///     Bean registry.
        /// </summary>
        public static BeanRegistry Registry = new BeanRegistry();

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
            RouteCollection.ToList().ForEach(c=>c.Value.RouteProcess.Execute());
        }
    }
}