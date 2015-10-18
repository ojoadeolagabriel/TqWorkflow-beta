using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using app.core.nerve.dto;
using app.core.nerve.facade;
using Newtonsoft.Json;

namespace mina.nibss.consumer.bundle.codebase
{
    public class GetDateTimeProcessor : ProcessorBase
    {
        public override void Process(Exchange exchange)
        {
            var body = exchange.InMessage.Body;
            var result = new XElement("date-time", new XElement("datetime", DateTime.Now));
            exchange.InMessage.Body = result.ToString();
        }
    }
}
