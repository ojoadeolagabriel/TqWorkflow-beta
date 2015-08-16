using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.NMS.ActiveMQ.Threads;
using MTasks = System.Threading.Tasks;
using app.core.workflow.dto;

namespace app.core.workflow.facade
{
    /// <summary>
    /// 
    /// </summary>
    public class Seda
    {
        public ConcurrentQueue<Exchange> SedaQueue = new ConcurrentQueue<Exchange>();

        public void ProcessSedaMessageQueue()
        {
            MTasks.Task.Factory.StartNew(HandleQueue);
        }

        /// <summary>
        /// 
        /// </summary>
        private void HandleQueue()
        {
            while (true)
            {
                Exchange xchange;
                if (SedaQueue.TryDequeue(out xchange))
                {
                    //trigger next step.
                    ProcessNextStep(xchange);
                }
            }
        }

        /// <summary>
        /// Process Next Step
        /// </summary>
        /// <param name="xchange"></param>
        private void ProcessNextStep(Exchange xchange)
        {
            
        }
    }
}
