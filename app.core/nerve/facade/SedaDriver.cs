﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using app.core.nerve.component.core;
using app.core.nerve.dto;
using app.core.nerve.handlers.tag;
using app.core.nerve.utility;

namespace app.core.nerve.facade
{
    /// <summary>
    /// 
    /// </summary>
    public class SedaDriver
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
                if (data.IsNull())
                {
                    Thread.Sleep(1000);
                    continue;
                }

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

            Route sedaRoute;
            Camel.RouteCollection.TryGetValue(endPoint.UriInformation.ComponentPath, out sedaRoute);
            if (sedaRoute != null)
            {
                EndPointBuilder.HandleFrom(endPoint.UriInformation, xchangeInfo.Value.Route, xchangeInfo.Value);
            }

            xchangeInfo.Value.Route.CurrentRouteStep.NextTag.ProcessChannel(xchangeInfo.Value);
        }
    }
}
