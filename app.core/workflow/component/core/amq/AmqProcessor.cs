using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.amq
{
    public class AmqProcessor : DefaultProcessor
    {
        public AmqProcessor(UriDescriptor uriInformation, Route route) : base(uriInformation, route)
        {

        }

        public override void PrepareOut(Exchange exchange)
        {
            
            base.PrepareOut(exchange);
        }
    }
}
