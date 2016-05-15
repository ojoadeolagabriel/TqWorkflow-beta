using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace paydirect.rabbitmq.processor.codebase.facade
{
    public class NotificationPartReader
    {
        public static string ReadSms(JObject parentJson)
        {
            try
            {
                var sms = parentJson["smsrequest"];
                if (sms != null)
                {
                    var xmlBody = JsonConvert.DeserializeXmlNode(sms.Value<string>()).ToString();
                    return xmlBody;
                }
            }
            catch (Exception exception)
            {
                
            }

            return null;
        }

        public static string ReadServiceRequest(JObject parentJson)
        {
            try
            {
                var sms = parentJson["servicerequest"];
                if (sms != null)
                {
                    var xmlBody = JsonConvert.DeserializeXmlNode(sms.Value<string>()).ToString();
                    return xmlBody;
                }
            }
            catch (Exception exception)
            {

            }

            return null;
        }

        public static string ReadFtp(JObject parentJson)
        {
            try
            {
                var sms = parentJson["ftprequest"];
                if (sms != null)
                {
                    var xmlBody = JsonConvert.DeserializeXmlNode(sms.Value<string>()).ToString();
                    return xmlBody;
                }
            }
            catch (Exception exception)
            {

            }

            return null;
        }
    }
}
