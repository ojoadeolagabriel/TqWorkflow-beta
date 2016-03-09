using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace isofield.transformer.bundle.codebase
{
    public class IsoFieldProcessor : ProcessorBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exchange"></param>
        public override void Process(Exchange exchange)
        {
            try
            {
                var isoXmlMsg = exchange.InMessage.Body as string;
                if (isoXmlMsg != null)
                {
                    var isoXml = XElement.Parse(isoXmlMsg);
                    var isoFieldXmlTag = exchange.InMessage.GetHeader<string>("isoFieldXmlTag");
                    var isoFieldXmlTagValue = exchange.InMessage.GetHeader<string>("isoFieldXmlTagValue");

                    var xElement = isoXml.Element(isoFieldXmlTag);
                    if (xElement != null)
                        xElement.SetValue(isoFieldXmlTagValue);

                    if (xElement != null)
                    {
                        exchange.InMessage.Body = isoXml.ToString();
                        Console.WriteLine(exchange.InMessage.Body);
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}
