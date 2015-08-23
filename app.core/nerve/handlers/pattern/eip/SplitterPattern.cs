﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using app.core.nerve.dto;
using app.core.nerve.expression;
using app.core.nerve.facade;

namespace app.core.nerve.handlers.pattern.eip
{
    /// <summary>
    /// Splitter Pattern.
    /// </summary>
    public class SplitterPattern
    {
        /// <summary>
        /// ProcessChannel
        /// </summary>
        /// <param name="splitElement"></param>
        /// <param name="exchange"></param>
        public static void Execute(XElement splitElement, Exchange exchange)
        {
            try
            {
                if (splitElement == null)
                    return;

                var spliterXml = splitElement.Elements().FirstOrDefault();
                if (spliterXml == null)
                    return;

                var splitterName = spliterXml.Name.ToString();

                var result = new List<String>();
                switch (splitterName)
                {
                    case "xpath":
                        result = SplitByXPath(spliterXml.Value, exchange);
                        break;
                    case "simple":
                        result = SplitSimple(spliterXml.Value, exchange);
                        break;
                }

                //get next steps.
                var nextSteps = splitElement.Elements().Skip(1);

                //process each
                foreach (var nextStep in nextSteps)
                {
                    try
                    {
                        //hold original message
                        var originalMessage = exchange.InMessage.Body;
                        foreach (var item in result)
                        {
                            var exchangeMsg = item;

                            var splitterExchange = exchange.CloneExchange(new Message
                                {
                                    Body = exchangeMsg,
                                    HeaderCollection = exchange.InMessage.HeaderCollection
                                },
                                new Message(),
                                exchange.Route);

                            RouteStep.ProcessStep(nextStep, exchange.Route, splitterExchange);
                        }
                        //restore original message
                        exchange.InMessage.Body = originalMessage;
                    }
                    catch (Exception exception)
                    {

                    }
                }
            }
            catch (Exception exception)
            {

            }
        }

        private static List<string> SplitSimple(string value, Exchange exchange)
        {
            var listOfParts = new List<String>();

            var simpleData = SimpleExpression.ResolveSpecifiedUriPart(value, exchange);

            return listOfParts;
        }

        private static List<String> SplitByXPath(string xpath, Exchange exchange)
        {
            var message = exchange.InMessage.Body.ToString();

            var xml = XElement.Parse(message);
            var navigator = xml.CreateNavigator();

            var expr = navigator.Compile(xpath);
            var iterator = navigator.Select(expr);
            var listOfParts = new List<String>();

            while (iterator.MoveNext())
            {
                var curr = iterator.Current.OuterXml;
                listOfParts.Add(curr);
            }

            return listOfParts;
        }
    }
}
