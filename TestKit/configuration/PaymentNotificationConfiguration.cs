using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using app.core.nerve.utility;

namespace TestKit.configuration
{
    public class PaymentNotificationConfiguration : ConfigBase<PaymentNotificationConfiguration>
    {
        public string EnterpriseDbConnectionString { get; set; }
    }
}
