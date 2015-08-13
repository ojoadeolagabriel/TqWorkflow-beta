using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core
{
    public class DefaultEndpoint
    {
        public Route Route;
        public string ComponentTitle { get; set; }

        public virtual void Start()
        {

        }

        public string Uri { get; set; }
        public UriDescriptor UriInformation { get; set; }

        public DefaultEndpoint(string uri, Route route)
        {
            Route = route;
            Uri = uri;
            UriInformation = UriDescriptor.Parse(uri);
        }

        public virtual void Send(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            return;
        }
    }
}
