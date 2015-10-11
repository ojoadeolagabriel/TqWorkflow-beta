using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace tcp.consumer.bundle
{
    public class JsonValidator : ProcessorBase
    {
        public override void Process(Exchange exchange)
        {
            var body = exchange.InMessage.Body;
            base.Process(exchange);
        }
    }
}
