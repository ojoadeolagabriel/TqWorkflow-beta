using System;

namespace app.core.nerve.dto
{
    public interface ISystemLogProvider
    {
        void Log(Exchange exchange, string processorType, string componentName, Exception exception = null);
    }
}
