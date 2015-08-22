using System;
using System.Threading;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.handlers.tag
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
