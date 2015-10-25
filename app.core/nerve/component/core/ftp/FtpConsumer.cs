using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using app.core.nerve.dto;
using Renci.SshNet;

namespace app.core.nerve.component.core.ftp
{
    public class FtpConsumer : PollingConsumer
    {
        private readonly FtpProcessor _processor;

        public FtpConsumer(FtpProcessor processor)
        {
            _processor = processor;
        }

        public override Exchange Poll()
        {
            Task.Factory.StartNew(PollHandler);
            return base.Poll();
        }

        private void PollHandler()
        {
            var exchange = new Exchange(_processor.Route);
            var ipaddress = _processor.UriInformation.GetUriProperty("host");
            var username = _processor.UriInformation.GetUriProperty("username");
            var password = _processor.UriInformation.GetUriProperty("password");
            var destination = _processor.UriInformation.GetUriProperty("destination");
            var port = _processor.UriInformation.GetUriProperty<int>("port");
            var interval = _processor.UriInformation.GetUriProperty<int>("interval", 100);
            var secure = _processor.UriInformation.GetUriProperty<bool>("secure");

            do
            {
                Thread.Sleep(interval);
                try
                {
                    if (secure)
                    {
                        SftpClient sftp = null;
                        sftp = new SftpClient(ipaddress, port, username, password);
                        sftp.Connect();

                        foreach (var ftpfile in sftp.ListDirectory("."))
                        {
                            var destinationFile = Path.Combine(destination, ftpfile.Name);
                            using (var fs = new FileStream(destinationFile, FileMode.Create))
                            {
                                var data = sftp.ReadAllText(ftpfile.FullName);
                                exchange.InMessage.Body = data;

                                if (!string.IsNullOrEmpty(data))
                                {
                                    sftp.DownloadFile(ftpfile.FullName, fs);
                                }
                            }
                        }
                    }
                    else
                    {
                        ReadFtp(exchange);
                    }
                }
                catch (Exception exception)
                {
                    var msg = exception.Message;
                }
            } while (true);
        }

        private void ReadFtp(Exchange exchange)
        {
            
        }
    }
}
