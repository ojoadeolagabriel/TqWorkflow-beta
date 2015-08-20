using System;
using System.Xml.Linq;
using app.core.nerve.dto;
using app.core.nerve.facade;
using app.core.workflow.dto;

namespace app.core.nerve.test
{
    public class HttpRequestBean : ProcessorBase
    {
        public override void Process(Exchange exchange)
        {
            var elem = XElement.Parse(exchange.InMessage.Body.ToString());

            elem.Add(new XElement("responsecode","00"));
            elem.Add(new XElement("responsemessage", "Successful"));
            elem.Add(new XElement("pin",Guid.NewGuid().ToString()));

            exchange.OutMessage.Body = elem.ToString();
            exchange.InMessage.SetHeader("htt-request-bean", DateTime.Now.ToString());
        }
    }
}
