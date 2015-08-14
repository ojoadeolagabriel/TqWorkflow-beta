﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.smtp
{
    public class SmtpProducer : DefaultProducer
    {
        public SmtpProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {
        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            try
            {
                var protocol = endPointDescriptor.GetUriProperty<string>("protocol");

                switch (protocol)
                {
                    case "smtp":
                        SendMail(endPointDescriptor, exchange);
                        break;
                    case "imap":
                        break;
                }               
            }
            catch
            {
                
            }

            return base.Process(exchange, endPointDescriptor);
        }

        private static void SendMail(UriDescriptor endPointDescriptor, Exchange exchange)
        {
            var toAddress = endPointDescriptor.GetUriProperty("to");
            var from = endPointDescriptor.GetUriProperty("from");
            var port = endPointDescriptor.GetUriProperty<int>("port");
            var host = endPointDescriptor.GetUriProperty<string>("host");
            var subject = endPointDescriptor.GetUriProperty<string>("subject");
            var body = endPointDescriptor.GetUriProperty<string>("body");
            var username = endPointDescriptor.GetUriProperty<string>("username");
            var password = endPointDescriptor.GetUriProperty<string>("password");
            

            var mail = new MailMessage();
            var client = new SmtpClient
            {
                Port = port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = host,
                Credentials = new NetworkCredential(username, password)
            };

            mail.To.Add(toAddress);
            mail.From = new MailAddress(@from);
            mail.Body = exchange.InMessage.Body.ToString();

            mail.Subject = subject;
            client.Send(mail);
        }
    }
}
