using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;

namespace app.core.workflow.component.core.direct
{
    public class DirectConsumer : DefaultConsumer
    {
        private readonly DirectProcessor _directProcessor;

        public DirectConsumer(DirectProcessor directProcessor)
        {
            _directProcessor = directProcessor;
        }

        public override Exchange Execute()
        {
            var exchange = new Exchange(_directProcessor.Route);
            _directProcessor.Process(exchange);
            return exchange;
        }
    }
}
