using System;
using System.Threading;
using app.core.workflow.dto;
using app.core.workflow.error;
using app.core.workflow.facade;

namespace app.core.workflow.handlers.tag
{
    public class DelayTag
    {
        public static void Execute(string delay, Exchange exchange, UriDescriptor uriDescriptor)
        {
            var ndxDelay = 1000;
            try
            {
                ndxDelay = Convert.ToInt32(delay);
            }
            catch (Exception exception)
            {

            }

            Thread.Sleep(ndxDelay);
        }
    }
}
