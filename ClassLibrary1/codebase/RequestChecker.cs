using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace paymentnotification.generic.bundle.codebase
{
    public class HttpGenericChecker
    {
        public bool IsHttpGenericRequest(Exchange exchange)
        {
            var request = XElement.Parse(exchange.InMessage.Body.ToString());
            var xElement = request.Element("routeid");
            if (xElement == null) return false;

            var routeId = xElement.Value;
            return routeId == "HTTPGENERIC";
        }
    }

    public class HttpGenericUtil : ProcessorBase
    {
        public override void Process(Exchange exchange)
        {
            var request = XElement.Parse(exchange.InMessage.Body.ToString());
            request.Add(new XElement("time-stamp",DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt")));
            exchange.InMessage.Body = request.ToString();
        }
    }
}
