using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Apache.NMS.ActiveMQ.Threads;
using app.core.workflow.component.core;
using MTasks = System.Threading.Tasks;
using app.core.workflow.dto;
using app.core.workflow.utility;

namespace app.core.workflow.facade
{
    /// <summary>
    /// 
    /// </summary>
    public class Seda
    {
        public static ConcurrentDictionary<UriDescriptor, Exchange> SedaQueue = new ConcurrentDictionary<UriDescriptor, Exchange>();

        public void ProcessSedaMessageQueue()
        {
            MTasks.Task.Factory.StartNew(HandleQueue);
        }

        /// <summary>
        /// 
        /// </summary>
        private void HandleQueue()
        {
            while (true)
            {
                var data = SedaQueue.FirstOrDefault();
                if (data.IsNull()) continue;

                Exchange removedData;
                SedaQueue.TryRemove(data.Key, out removedData);

                var concurrentConsumers = data.Key.GetUriProperty("concurrentConsumers", false);
                var timeOut = data.Key.GetUriProperty("timeOut", 0);

                //if concurrent provided.
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
            xchangeInfo.Value.Route.RouteProcess.NextTag.Execute(xchangeInfo.Value);
        }
    }
}
