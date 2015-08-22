using app.core.nerve.dto;

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