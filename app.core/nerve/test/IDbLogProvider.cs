using app.core.nerve.dto;
using app.core.workflow.dto;

namespace app.core.nerve.test
{
    /// <summary>
    /// Log Provider
    /// </summary>
    public interface IDbLogProvider
    {
        void Log(Exchange exchange, string processorType, string componentName);
    }
}