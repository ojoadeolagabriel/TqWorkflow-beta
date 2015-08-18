using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace app.core.workflow.dto
{
    public interface ISystemLogProvider
    {
        void Log(Exchange exchange, string processorType, string componentName, Exception exception = null);
    }
}
