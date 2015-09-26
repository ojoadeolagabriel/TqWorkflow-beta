using System;
using System.Xml;
using app.core.nerve.dto;
using app.core.nerve.facade;
using Newtonsoft.Json;

namespace app.core.nerve.component.core.xmltojson
{
    public class XmlToJsonProducer : DefaultProducer
    {
        public XmlToJsonProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {
        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            try
            {
                var inReverse = endPointDescriptor.GetUriProperty("reverse", false);
                var useOut = endPointDescriptor.GetUriProperty("useout", false);

                var body = useOut ? exchange.OutMessage.Body.ToString() : exchange.InMessage.Body.ToString();

                if (!inReverse)
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(body);
                    var jsonText = JsonConvert.SerializeXmlNode(doc);
                    exchange.InMessage.Body = jsonText;
                }
                else
                {
                    var doc = JsonConvert.DeserializeXmlNode(body);
                    exchange.InMessage.Body = doc.InnerXml;
                }
                Camel.TryLog(exchange, "processor", endPointDescriptor.ComponentName);
            }
            catch (Exception exception)
            {

            }
            return base.Process(exchange, endPointDescriptor);
        }
    }
}
