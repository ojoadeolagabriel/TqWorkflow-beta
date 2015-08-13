using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.mina
{
    public class MinaProducer : DefaultProducer
    {
        public MinaProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {

        }
    }
}
