using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using app.core.workflow.dto;
using app.core.workflow.expression;

namespace app.core.workflow.handlers.pattern.eip
{
    public class Transform
    {
        public static void HandleTransform(XElement step, Route routeObj, Exchange exchange)
        {
            try
            {
                var transformXml = step.Elements().First();
                var opName = transformXml.Name.ToString();

                switch (opName)
                {
                    case "simple":
                        ProcessSimple(transformXml, routeObj, exchange);
                        break;
                    case "xpath":
                        break;
                }
            }
            catch (AggregateException aggregateException)
            {
                
            }
            catch (Exception exception)
            {

            }
        }

        private static void ProcessSimple(XElement transformXml, Route routeObj, Exchange exchange)
        {
            var newBody = SimpleExpression.ResolveExpression(transformXml.Value, exchange);
            exchange.InMessage.Body = newBody;
        }
    }
}
