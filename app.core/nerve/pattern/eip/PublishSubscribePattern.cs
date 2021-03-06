﻿using System;
using System.Linq;
using System.Xml.Linq;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.pattern.eip
{
    public class PublishSubscribePattern
    {
        public static void Process(XElement multicastElement, Exchange exchange)
        {
            try
            {
                var toElements = multicastElement.Elements("to");
                var xElements = toElements as XElement[] ?? toElements.ToArray();
                if (!xElements.Any())
                    return;

                foreach (var toTag in xElements)
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
