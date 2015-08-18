﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using app.core.workflow.component.core.http;
using app.core.workflow.dto;

namespace app.core.workflow.component.core.mina
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
            var exchange = new Exchange(_processor.Route);
            var initialDelay = _processor.UriInformation.GetUriProperty("initialDelay", 1000);
            var poll = _processor.UriInformation.GetUriProperty("poll", 1000);
            var parallel = _processor.UriInformation.GetUriProperty("parallel", true);
            var timeout = _processor.UriInformation.GetUriProperty("timeout", 10000, exchange);
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

            Console.WriteLine("listening for incomming connections @ {0}:{1}", ip, port);
            TcpListener.BeginAcceptTcpClient(ProcessIncommingClientAsync, new PassData
            {
                Exchange = exchange,
                TcpListener = TcpListener
            });
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

                using (var ns = client.GetStream())
                {
                    int totalBytes;
                    while ((totalBytes = ns.Read(buffer, 0, 1)) > 0)
                    {
                        var data = Encoding.ASCII.GetString(buffer, 0, totalBytes);
                        messageBuilder.Append(data);

                        if (messageBuilder.ToString().EndsWith(Environment.NewLine) || messageBuilder.ToString().EndsWith("\\r\\n"))
                            break;
                    }

                    exchange.InMessage.Body = messageBuilder;
                    _processor.Process(exchange);

                    exchange.OutMessage.Body = exchange.InMessage.Body;

                    Camel.TryLog(exchange, "consumer", _processor.UriInformation.ComponentName);
                    
                    var outputMessage = Encoding.ASCII.GetBytes(exchange.OutMessage.Body.ToString());
                    ns.Write(outputMessage, 0, outputMessage.Length);
                    ns.Flush();
                    ns.Close();

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
