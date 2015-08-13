using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.normalizer
{
    /// <summary>
    /// Normalizer EndPoint.
    /// </summary>
    public class NormalizerEndPoint : DefaultEndpoint
    {
        public NormalizerEndPoint(string uri, Route route) : base(uri, route)
        {

        }

        public override void Send(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var producer = CreateProducer();
            producer.Process(exchange, endPointDescriptor);
        }

        public DefaultProducer CreateProducer()
        {
            return new NormalizerProducer(UriInformation, Route);
        }
    }
}
