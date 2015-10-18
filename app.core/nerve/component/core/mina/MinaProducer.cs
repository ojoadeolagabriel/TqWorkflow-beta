using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.mina
{
    public class MinaProducer : DefaultProducer
    {
        public MinaProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {

        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            try
            {
                var port = endPointDescriptor.GetUriProperty("port", 7000, exchange);
                var receivetimeout = endPointDescriptor.GetUriProperty("receivetimeout", 10000, exchange);
                var sendtimeout = endPointDescriptor.GetUriProperty("sendtimeout", 5000, exchange);
                var ip = endPointDescriptor.ComponentPath;

                var client = new TcpClient();
                client.Connect(ip, port);

                if (client.Connected)
                {
                    var bodyMsg = exchange.InMessage.Body.ToString();
                    var buffer = Encoding.ASCII.GetBytes(bodyMsg);

                    Camel.TryLog(exchange, "producer", endPointDescriptor.ComponentName);

                    client.Client.ReceiveTimeout = receivetimeout;
                    client.Client.SendTimeout = sendtimeout;
                    var totalBytesSent = client.Client.Send(buffer);

                    using (var networkStream = client.GetStream())
                    {
                        if (networkStream.CanRead)
                        {
                            // Buffer to store the response bytes.
                            var readBuffer = new byte[client.ReceiveBufferSize];
                            using (var writer = new MemoryStream())
                            {
                                do
                                {
                                    var numberOfBytesRead = networkStream.Read(readBuffer, 0, readBuffer.Length);
                                    if (numberOfBytesRead <= 0)
                                    {
                                        break;
                                    }
                                    writer.Write(readBuffer, 0, numberOfBytesRead);
                                } while (networkStream.DataAvailable);
                                exchange.InMessage.Body = Encoding.UTF8.GetString(writer.ToArray());
                            }
                        }
                    }
                }
            }
            catch (System.Exception exception)
            {
                Console.WriteLine(exception.StackTrace);
            }
            return exchange;
        }
    }
}
