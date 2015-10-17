using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve.utility;

namespace paymentnotification.generic.bundle.codebase
{
    /// <summary>
    /// Paydirect Configuration
    /// </summary>
    public class PaydirectConfiguration : ConfigBase<PaydirectConfiguration>
    {
        public bool IsGenericConfiguration { get; set; }
        public int IntegrationPort { get; set; }
        public string RestIntegrationPath { get; set; }

        public PaydirectConfiguration()
        {
            Load();
        }
    }
}
