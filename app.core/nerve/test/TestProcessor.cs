using System;
using System.Globalization;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.test
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
