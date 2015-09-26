using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using app.core.nerve.component.core;
using app.core.nerve.dto;
using app.core.nerve.facade;
using app.core.nerve.handlers.routepipeline;
using app.core.nerve.registry;

namespace app.core.nerve
{
    /// <summary>
    ///     Camel
    /// </summary>
    public class Camel
    {
        /// <summary>
        /// Load Camel Context
        /// </summary>
        /// <param name="path"></param>
        /// <param name="nameSpaces"></param>
        public static void LoadCamelContext(List<string> path,List<string> nameSpaces = null)
        {
            InitDependencyLibs(nameSpaces ?? new List<string> { "app.core.nerve.component.core" });

            foreach (var filePath in path)
            {
                CameContextConfigFileInitializer.Initialize(filePath);
            }

            StartAllRoutes();
            StartSedaProcessor();
        }

        /// <summary>
        /// Route Collection.
        /// </summary>
        public static readonly ConcurrentDictionary<string, Route> RouteCollection = new ConcurrentDictionary<string, Route>();

        /// <summary>
        /// Try Log
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="processorType"></param>
        /// <param name="componentName"></param>
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
            new SedaDriver().ProcessSedaMessageQueue();
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
            var data = RouteCollection.FirstOrDefault(c =>
            {
                try
                {
                    return c.Value.CurrentRouteStep.ComponentPathInfo == direct;
                }
                catch (System.Exception)
                {
                    return false;
                }
            });
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
            RouteCollection.ToList().ForEach(c => c.Value.CurrentRouteStep.ProcessChannel());
        }
    }
}