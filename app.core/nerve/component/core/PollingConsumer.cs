using app.core.nerve.dto;

namespace app.core.nerve.component.core
{
    public class PollingConsumer : DefaultConsumer
    {
        public bool CanRun(DefaultProcessor processor)
        {
            return processor.Route.BundleInfo.BundleStatus == BundleDescriptorObject.Status.Active;
        }

        public void StopRun()
        {
            
        }

        public virtual Exchange Poll()
        {
            return null;
        }
    }
}
