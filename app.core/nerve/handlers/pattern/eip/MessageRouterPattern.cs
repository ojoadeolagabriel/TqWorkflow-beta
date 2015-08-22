using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using app.core.nerve.dto;
using app.core.nerve.expression;
using app.core.nerve.facade;

namespace app.core.nerve.handlers.pattern.eip
{
    public class MessageRouterPattern
    {
        public static void Execute(XElement choicElement, Exchange exchange)
        {
            var whenElements = choicElement.Elements("when");

            foreach (var whenElement in whenElements)
            {
                var passed = CheckRequiremnt(whenElement, exchange);
                if (!passed) continue;

                var functions = whenElement.Elements().Skip(1);
                foreach (var xmlStep in functions)
                {
                    RouteStep.ProcessStep(xmlStep, exchange.Route, exchange);
                }
                break;
            }

            //handle otherwise
            var otherwiseXml = choicElement.Element("otherwise");
            if (otherwiseXml == null) return;

            var otherwiseFunctions = otherwiseXml.Elements();

            foreach (var xmlStep in otherwiseFunctions)
            {
                RouteStep.ProcessStep(xmlStep, exchange.Route, exchange);
            }
        }

        private static bool CheckRequiremnt(XElement whenElement, Exchange exchange)
        {
            var conditionXml = whenElement.Elements().FirstOrDefault();
            if (conditionXml == null)
                return false;

            var conditionType = conditionXml.Name.ToString();
            switch (conditionType)
            {
                case "simple":
                    return ProcessSimple(conditionXml, exchange);
                    break;
                case "xpath":
                    return ProcessXPath(conditionXml, exchange);
                    break;
                case "method":
                    return ProcessBean(conditionXml, exchange);
                    break;
                default:
                    return false;
            }

            return false;

        }

        private static bool ProcessBean(XElement conditionXml, Exchange exchange)
        {
            if (conditionXml == null)
                return false;   

            var bean = conditionXml.Attribute("bean");
            var method = conditionXml.Attribute("method");
            if (bean == null || method == null)
                return false;

            var beanObj = Camel.Registry[bean.Value];
            var methodInst = beanObj.GetType().GetMethod(method.Value, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (methodInst == null)
                return false;

            var result  = methodInst.Invoke(beanObj, null);
            return Convert.ToBoolean(result);
        }

        private static bool ProcessXPath(XElement conditionXml, Exchange exchange)
        {
            var rule = conditionXml.Value;
            var message = exchange.InMessage.Body.ToString();

            var xml = XElement.Parse(message);
            var isMatch = (bool)xml.XPathEvaluate(rule);

            return isMatch;
        }

        private static bool ProcessSimple(XElement conditionXml, Exchange exchange)
        {
            return SimpleExpression.Evaluate(exchange, conditionXml.Value);
        }
    }
}
