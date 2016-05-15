using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve.dto;
using app.core.nerve.facade;
using app.core.nerve.language;
using Newtonsoft.Json.Linq;
using paydirect.rabbitmq.processor.codebase.facade;

namespace paydirect.rabbitmq.processor.codebase
{
    /// <summary>
    /// PaymentNotificationServiceJsonProcessor Class.
    /// </summary>
    public class PaymentNotificationServiceJsonProcessor : ProcessorBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exchange"></param>
        public override void Process(Exchange exchange)
        {
            try
            {
                var jsonMsg = exchange.InMessage.Body as string;
                if (string.IsNullOrEmpty(jsonMsg))
                    return;

                var jsonObj = JObject.Parse(jsonMsg);

                var smsXml = NotificationPartReader.ReadSms(jsonObj);
                var serviceXml = NotificationPartReader.ReadServiceRequest(jsonObj);
                var ftpXml = NotificationPartReader.ReadFtp(jsonObj);

                RouteBuilder.Build((exg) =>
                {
                    
                });

                new LanguageUtil(exchange)
                    .To("")
                    .Choice()
                        .When(new Header("").IsEqual(""))
                            .To("");
            }
            catch (Exception exception)
            {

            }         
        }
    }
}
