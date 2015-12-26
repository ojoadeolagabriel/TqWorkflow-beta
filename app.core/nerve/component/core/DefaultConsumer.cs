using app.core.nerve.dto;

namespace app.core.nerve.component.core
{
    public class DefaultConsumer
    {
        public virtual Exchange Execute()
        {
            return null;
        }

        public virtual bool PauseConsumer()
        {
            return false;
        }

        public virtual bool ResumeConsumer()
        {
            return false;
        }

        public virtual Exchange Execute(Exchange exchange)
        {
            return null;
        }

        public virtual bool UnLoad()
        {
            return true;
        }
    }
}
