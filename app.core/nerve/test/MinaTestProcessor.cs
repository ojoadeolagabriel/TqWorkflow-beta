using System;
using System.Xml.Linq;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.test
{
    public class MinaTestProcessor : ProcessorBase
    {
        public void Start()
        {
            int a = 0;
        }

        public override void Process(Exchange exchange)
        {
            var xE = XElement.Parse(exchange.InMessage.Body.ToString().Replace("\\r\\n", ""));

            var xElement = xE.Element("claim-type");
            if (xElement != null)
            {
                exchange.InMessage.SetHeader("response-message", "ola .. in tcp/ip test processor");

                var ans = DateTime.Now.Second % 3;
                var xml = new XElement("payments",
                    new XElement("code", ans != 0 ? "00" : "91"),
                    new XElement("information", new XElement("payment", new XAttribute("ref", "12345")),
                    new XElement("payment", new XAttribute("ref", "34343")),
                    new XElement("payment", new XAttribute("ref", "33411")),
                    new XElement("payment", new XAttribute("ref", "41445")),
                    new XElement("payment", new XAttribute("ref", "55553")))
                    ).ToString(SaveOptions.DisableFormatting);

                exchange.InMessage.Body = xml;
                exchange.InMessage.SetHeader("email_address", "aojo@Interswitchng.com");
            }

            exchange.Exception.Push(new Exception("Error processing transaction request"));
        }
    }
}
