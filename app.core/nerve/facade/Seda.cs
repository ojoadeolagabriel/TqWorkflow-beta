using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using app.core.nerve.component.core;
using app.core.nerve.dto;
using app.core.nerve.utility;
using app.core.workflow.dto;

namespace app.core.nerve.facade
{
    /// <summary>
    /// 
    /// </summary>
    public class Seda
    {
        public static ConcurrentDictionary<UriDescriptor, Exchange> SedaQueue = new ConcurrentDictionary<UriDescriptor, Exchange>();

        public void ProcessSedaMessageQueue()
        {
            System.Threading.Tasks.Task.Factory.StartNew(HandleQueue);
        }

        /// <summary>
        /// 
        /// </summary>
        private static void HandleQueue()
        {
            while (true)
            {
                var data = SedaQueue.FirstOrDefault();
                if (data.IsNull()) continue;

                Exchange removedData;
                SedaQueue.TryRemove(data.Key, out removedData);

                var concurrentConsumers = data.Key.GetUriProperty("concurrentConsumers", false);
                var timeOut = data.Key.GetUriProperty("timeOut", 0);

                if (concurrentConsumers)
                {
                    System.Threading.Tasks.Task.Factory.StartNew(() => ProcessNextStep(data));
                }
                else
                    ProcessNextStep(data);
            }
        }

        /// <summary>
        /// Process Next Step
        /// </summary>
        /// <param name="xchangeInfo"></param>
        private static void ProcessNextStep(KeyValuePair<UriDescriptor, Exchange> xchangeInfo)
        {
            //handle
            DefaultEndpoint endPoint;
            if (!Camel.EnPointCollection.TryGetValue(xchangeInfo.Key.FullUri, out endPoint)) return;

            //build from step request.
            var uriInfo = endPoint.UriInformation;
            var step = new XElement("from",
                new XAttribute("uri", uriInfo.FullUri));

            //process step.
            RouteStep.ProcessStep(step, xchangeInfo.Value.Route, xchangeInfo.Value);
            xchangeInfo.Value.Route.RouteProcess.NextTag.ProcessChannel(xchangeInfo.Value);
        }
    }
}
