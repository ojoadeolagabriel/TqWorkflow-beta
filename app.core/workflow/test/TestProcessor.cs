using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.test
{
    public class TestProcessor : ProcessorBase
    {
        public override void Process(Exchange exchange)
        {
            try
            {
                exchange.InMessage.Body = "processor.data";
                exchange.InMessage.SetHeader("paydirect-httpmethod", "POST");
                exchange.OutMessage.SetHeader("date", DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture));
            }
            catch
            {

            }
        }
    }
}
