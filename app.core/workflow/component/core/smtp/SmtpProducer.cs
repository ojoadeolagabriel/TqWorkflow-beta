using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.smtp
{
    public class SmtpProducer : DefaultProducer
    {
        public SmtpProducer(UriDescriptor uriInformation, Route route) : base(uriInformation, route)
        {
        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            return base.Process(exchange, endPointDescriptor);
        }
    }
}
