using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app.core.workflow.builder
{
    public class DeadLetterChannelBuilder
    {
        public string DeadLetterUri { get; set; }
        public int MaxDeliveryAttempts { get; set; }
    }
}
