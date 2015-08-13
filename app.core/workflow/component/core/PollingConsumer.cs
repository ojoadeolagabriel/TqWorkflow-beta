using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;

namespace app.core.workflow.component.core
{
    public class PollingConsumer : DefaultConsumer
    {


        public virtual Exchange Poll()
        {
            return null;
        }
    }
}
