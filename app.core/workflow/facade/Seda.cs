using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;

namespace app.core.workflow.facade
{
    public class Seda
    {
        public ConcurrentQueue<Exchange> SedaQueue = new ConcurrentQueue<Exchange>();
    }
}
