using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.handlers.pattern.eip
{
    public class PublishSubscribePattern
    {
        public static void Process(XElement multicastElement, Exchange exchange)
        {
            try
            {
                var toElements = multicastElement.Elements("to");
                if (!toElements.Any())
                    return;

                foreach (var toTag in toElements)
                {
                    RouteStep.ProcessStep(toTag, exchange.Route, exchange);
                }
            }
            catch (Exception exception)
            {
                
            }
        }
    }
}
