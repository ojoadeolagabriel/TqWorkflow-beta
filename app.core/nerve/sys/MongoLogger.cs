using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve.dto;

namespace app.core.nerve.sys
{
    public class MongoLogger : ISystemLogProvider
    {
        public void Log(Exchange exchange, string processorType, string componentName, Exception exception = null)
        {
            
        }
    }
}
