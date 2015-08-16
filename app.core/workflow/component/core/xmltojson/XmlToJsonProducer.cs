using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using app.core.workflow.dto;
using app.core.workflow.facade;
using Newtonsoft.Json;

namespace app.core.workflow.component.core.xmltojson
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
                var body = exchange.InMessage.Body.ToString();

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
                    exchange.InMessage.Body = doc;
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
