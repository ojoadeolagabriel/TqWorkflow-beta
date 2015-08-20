﻿using System;
using System.Linq;
using System.Xml.Linq;
using app.core.nerve.dto;
using app.core.nerve.expression;
using app.core.workflow.dto;

namespace app.core.nerve.handlers.pattern.eip
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