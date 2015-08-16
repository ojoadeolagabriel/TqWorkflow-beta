using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using app.core.application.error;
using app.core.workflow.component;
using app.core.workflow.component.core;
using app.core.workflow.dto;

namespace app.core.workflow.facade
{
    /// <summary>
    /// Endpoint Manager Class
    /// </summary>
    public class EndPointBuilder
    {
        public static ConcurrentQueue<string> PermissibleNamespaces = new ConcurrentQueue<string>();

        public static void BuildNamespaces(List<string> namespaces)
        {
            namespaces.ForEach(c => PermissibleNamespaces.Enqueue(c));
        }


        /// <summary>
        /// ProcessRouteInformation From Method
        /// </summary>
        /// <param name="leafDescriptor"></param>
        /// <param name="route"></param>
        public static void HandleFrom(UriDescriptor leafDescriptor, Route route)
        {
            try
            {
                //init endpoint
                DefaultEndpoint endPoint;
                if (Camel.EnPointCollection.TryGetValue(leafDescriptor.FullUri, out endPoint))
                {

                }
                else
                {
                    var execAssembly = Assembly.GetExecutingAssembly();
                    var types = execAssembly.GetTypes();

                    foreach (var namespaceToCheck in PermissibleNamespaces)
                    {
                        var typeData = types.FirstOrDefault(c => c.FullName.Equals(string.Format("{0}.{1}.{2}", namespaceToCheck, leafDescriptor.ComponentName, leafDescriptor.ComponentName),
                           StringComparison.InvariantCultureIgnoreCase) ||
                           c.FullName.Equals(string.Format("{0}.{1}.{2}EndPoint", namespaceToCheck, leafDescriptor.ComponentName, leafDescriptor.ComponentName),
                           StringComparison.InvariantCultureIgnoreCase));

                        if (typeData == null)
                            continue;

                        endPoint = (DefaultEndpoint)Activator.CreateInstance(typeData, leafDescriptor.FullUri, route);
                        Camel.EnPointCollection.TryAdd(leafDescriptor.FullUri, endPoint);
                        break;
                    }
                }

                if (endPoint != null) endPoint.Start();
                else
                    throw new AppCoreException("end-point not found: " + leafDescriptor.ComponentName);
            }
            catch (Exception exception)
            {
                throw new AppCoreException("error handling [from-tag] :" + exception.Message, exception);
            }
        }

        public static void HandleTo(UriDescriptor leafDescriptor, Exchange exchange, Route route)
        {
            try
            {
                DefaultEndpoint endPoint;
                if (Camel.EnPointCollection.TryGetValue(leafDescriptor.FullUri, out endPoint))
                {

                }
                else
                {
                    var execAssembly = Assembly.GetExecutingAssembly();
                    var types = execAssembly.GetTypes();

                    foreach (var namespaceToCheck in PermissibleNamespaces)
                    {
                        var typeData = types.FirstOrDefault(c => c.FullName.Equals(string.Format("{0}.{1}.{2}", namespaceToCheck, leafDescriptor.ComponentName, leafDescriptor.ComponentName),
                           StringComparison.InvariantCultureIgnoreCase) ||
                           c.FullName.Equals(string.Format("{0}.{1}.{2}EndPoint", namespaceToCheck, leafDescriptor.ComponentName, leafDescriptor.ComponentName),
                           StringComparison.InvariantCultureIgnoreCase));

                        if (typeData == null)
                            continue;

                        endPoint = (DefaultEndpoint)Activator.CreateInstance(typeData, leafDescriptor.FullUri, route);
                        Camel.EnPointCollection.TryAdd(leafDescriptor.FullUri, endPoint);
                        break;
                    }
                }

                if (endPoint != null) 
                    endPoint.Send(exchange, leafDescriptor);
                else
                    throw new AppCoreException("end-point not found: " + leafDescriptor.ComponentName);
            }
            catch (Exception exception)
            {
                throw new AppCoreException("error handling from:", exception);
            }
        }
    }
}
