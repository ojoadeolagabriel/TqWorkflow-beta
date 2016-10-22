using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace camelcontext.server.facade.network
{
    public class BasicTcpProcessor : ProcessorBase
    {
        public override void Process(Exchange exchange)
        {
            var body = exchange.InMessage.Body.ToString();

            if (body.EndsWith("\\r\\n"))
                body = body.Replace("\\r\\n", "");

            var msg = $"http://{body}";
            var xml = XElement.Parse(body);
            xml.Add(new XElement("response", "00"));
            exchange.InMessage.Body = xml.ToString();
        }
    }
}
