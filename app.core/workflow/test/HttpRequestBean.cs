using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.test
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
