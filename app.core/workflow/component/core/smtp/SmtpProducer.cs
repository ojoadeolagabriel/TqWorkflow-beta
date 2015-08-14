using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.smtp
{
    public class SmtpProducer : DefaultProducer
    {
        public SmtpProducer(UriDescriptor uriInformation, Route route) : base(uriInformation, route)
        {
        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var toAddress = endPointDescriptor.GetUriProperty("to");
            var from = endPointDescriptor.GetUriProperty("from");
            var port = endPointDescriptor.GetUriProperty<int>("port");            
            var host = endPointDescriptor.GetUriProperty<string>("host");
            var subject = endPointDescriptor.GetUriProperty<string>("subject");            
            var body = endPointDescriptor.GetUriProperty<string>("body");            

            var mail = new MailMessage();
            var client = new SmtpClient
            {
                Port = port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = host
            };

            mail.To.Add(toAddress);
            mail.From = new MailAddress(from);
            mail.Subject = subject;
            mail.Body = body;
            client.Send(mail);

            return base.Process(exchange, endPointDescriptor);
        }
    }
}
