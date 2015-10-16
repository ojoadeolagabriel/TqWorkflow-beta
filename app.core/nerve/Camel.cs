using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
        public static ConcurrentQueue<string> AssemblyCollection = new ConcurrentQueue<string>();

        public static object LoadBean(string type)
        {
            foreach (var assemblyCollection in AssemblyCollection)
            {
                try
                {
                    var instance = Activator.CreateInstance(assemblyCollection, type).Unwrap();
                    if (instance != null)
                        return instance;
                }
                catch
                {
                }
            }
            return null;
        }

        /// <summary>
        /// Load Camel Context
        /// </summary>
        /// <param name="path"></param>
        /// <param name="nameSpaces"></param>
        public static void LoadCamelContext(List<string> path, List<string> nameSpaces = null, List<string> assemblies = null)
        {
            InitDependencyLibs(nameSpaces ?? new List<string> { "app.core.nerve.component.core" });
            if (assemblies != null) assemblies.ForEach(c => AssemblyCollection.Enqueue(c));

            foreach (var filePath in path)
            {
                CameContextConfigFileInitializer.Initialize(filePath);
            }
        }

        public static void LoadBundle(List<string> bundleDllPaths)
        {
            foreach (var filePath in bundleDllPaths)
            {
                try
                {
                    
                    var assemName = Path.GetFileNameWithoutExtension(filePath);
                    AssemblyCollection.Enqueue(assemName);
                    CameContextConfigFileInitializer.Initialize(filePath, isBundle: true);
                }
                catch (Exception exception)
                {

                }
            }
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
            Console.WriteLine("Starting all components");
            RouteCollection.ToList().ForEach(c => c.Value.CurrentRouteStep.ProcessChannel());
        }
    }
}