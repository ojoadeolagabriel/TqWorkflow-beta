using System;
using System.Xml.Linq;
using app.core.nerve.dto;
using app.core.nerve.facade;
using app.core.nerve.strategy;

namespace app.core.nerve.pattern.eip
{
    /// <summary>
    /// 
    /// </summary>
    public class EnricherPattern
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="step"></param>
        /// <param name="exchange"></param>
        /// <param name="routeObj"></param>
        public static void Enrich(XElement step, Exchange exchange, Route routeObj)
        {
            try
            {
                var uri = step.Attribute("uri");
                var strategyref = step.Attribute("strategyref");

                if (uri == null || strategyref == null)
                    return;

                var uriInfo = UriDescriptor.Parse(uri.Value, exchange);
                var route = Camel.GetRouteBy(uriInfo.ComponentPath);

                if (route == null) return;

                var clonedExchange = exchange.CloneExchange();
                route.CurrentRouteStep.ProcessChannel(clonedExchange);

                //fetch strategy
                var stragegyObj = Camel.Registry[strategyref.Value] as AggregationStrategy;
                if (stragegyObj != null)
                {
                    stragegyObj.Aggregate(exchange, clonedExchange);
                }
            }
            catch (Exception exc)
            {

            }
        }
    }
}
