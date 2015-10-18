using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using app.core.nerve.dto;

namespace app.core.nerve.component.core.mina
{
    public class MinaConsumer : PollingConsumer
    {
        class PassData
        {
            public TcpListener TcpListener { get; set; }
            public Exchange Exchange { get; set; }
        }

        private MinaProcessor _processor;

        public MinaConsumer(MinaProcessor processor)
        {
            _processor = processor;
        }

        public override Exchange Poll()
        {
            //PollHandler();
            Task.Factory.StartNew(PollHandler);
            return null;
        }

        public TcpListener TcpListener;

        private void PollHandler()
        {
            try
            {
                var exchange = new Exchange(_processor.Route);
                var initialDelay = _processor.UriInformation.GetUriProperty("initialDelay", 1000);
                var poll = _processor.UriInformation.GetUriProperty("poll", 1000);
                var parallel = _processor.UriInformation.GetUriProperty("parallel", true);
                var timeout = _processor.UriInformation.GetUriProperty("timeout", 1000000, exchange);
                var maxThreadCount = _processor.UriInformation.GetUriProperty("threadCount", 2, exchange);
                var sync = _processor.UriInformation.GetUriProperty("sync", false);
                var textline = _processor.UriInformation.GetUriProperty("textline", true);
                var encoding = _processor.UriInformation.GetUriProperty("encoding", "");
                var port = _processor.UriInformation.GetUriProperty("port", 7000, exchange);
                var ip = _processor.UriInformation.ComponentPath;

                //new
                if (TcpListener == null)
                    TcpListener = new TcpListener(IPAddress.Parse(ip), port);

                TcpListener.Server.SendTimeout = timeout;
                TcpListener.Server.ReceiveTimeout = timeout;
                TcpListener.Start(1000);

                Console.WriteLine("Activiating TCP Endpoint {0}:{1}", ip, port);
                TcpListener.BeginAcceptTcpClient(ProcessIncommingClientAsync, new PassData
                {
                    Exchange = exchange,
                    TcpListener = TcpListener
                });
            }
            catch (Exception exception)
            {
                Console.WriteLine("{0}-{1}", exception.Message, exception.StackTrace);
            }
        }


        private void ProcessIncommingClientAsync(IAsyncResult res)
        {
            var passData = (PassData)res.AsyncState;
            var listener = passData.TcpListener;

            TcpListener.BeginAcceptTcpClient(ProcessIncommingClientAsync, new PassData { Exchange = new Exchange(_processor.Route), TcpListener = TcpListener });
            var exchange = passData.Exchange;
            try
            {

                var client = listener.EndAcceptTcpClient(res);

                var messageBuilder = new StringBuilder();
                var buffer = new byte[1];

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
                        _processor.Process(exchange);
                        exchange.OutMessage.Body = exchange.InMessage.Body;
                        Camel.TryLog(exchange, "consumer", _processor.UriInformation.ComponentName);

                        var outputMessage = Encoding.ASCII.GetBytes(exchange.OutMessage.Body.ToString());
                        networkStream.Write(outputMessage, 0, outputMessage.Length);
                        networkStream.Flush();
                        networkStream.Close();
                    }
                    throw new Exception();
                }
            }
            catch (Exception escException)
            {
                exchange.CurrentException = escException;
                Camel.TryLog(exchange, "consumer", _processor.UriInformation.ComponentName);
            }
        }
    }
}
