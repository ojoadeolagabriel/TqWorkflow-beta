using app.core.workflow.dto;

namespace app.core.workflow.test
{
    /// <summary>
    /// Log Provider
    /// </summary>
    public interface IDbLogProvider
    {
        void Log(Exchange exchange, string processorType);
    }
}