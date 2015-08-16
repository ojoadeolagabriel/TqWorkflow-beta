using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;

namespace app.core.workflow.component.core.seda
{
    public class SedaConsumer : DefaultConsumer
    {
        private readonly SedaProcessor _sedaProcessor;

        public SedaConsumer(SedaProcessor sedaProcessor)
        {
            _sedaProcessor = sedaProcessor;
        }

        public override Exchange Execute()
        {
            var exchange = new Exchange(_sedaProcessor.Route);

            _sedaProcessor.Process(exchange);
            return exchange;
        }
    }
}
