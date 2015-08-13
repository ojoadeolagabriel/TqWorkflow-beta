using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.mina
{
    public class MinaProcessor : DefaultProcessor
    {
        public MinaProcessor(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {
            
        }
    }
}
