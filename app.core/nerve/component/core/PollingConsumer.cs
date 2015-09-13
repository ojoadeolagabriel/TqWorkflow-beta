using app.core.nerve.dto;

namespace app.core.nerve.component.core
{
    public class PollingConsumer : DefaultConsumer
    {
        public virtual Exchange Poll()
        {
            return null;
        }
    }
}
