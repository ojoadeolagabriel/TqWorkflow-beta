﻿using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.log2
{
    public class Log2Producer : DefaultProducer
    {

        private static readonly object locker = new object();

        public static void Log(string path, string message)
        {
            lock (locker)
            {
                using (var file = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    try
                    {
                        var writer = new StreamWriter(file);
                        writer.WriteLine(message);
                        writer.Flush();
                        file.Close();
                    }
                    catch
                    {

                    }
                }
            }
        }

        public Log2Producer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {

        }

        public static ConcurrentQueue<UriExchange> LogStack = new ConcurrentQueue<UriExchange>();
        public static System.Threading.Tasks.Task LogTask;

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            LogStack.Enqueue(new UriExchange { Exchange = exchange, UriDescriptor = endPointDescriptor });

            if (LogTask == null)
                LogTask = System.Threading.Tasks.Task.Factory.StartNew(ProcessLogQueue);

            return exchange;
        }

        public  void ProcessLogQueue()
        {
            while (true)
            {
                UriExchange data;
                if (LogStack.TryDequeue(out data))
                {
                    LogToFile(data);
                }
                else
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private static void LogToFile(UriExchange data)
        {
            var exchange = data.Exchange;
            var uriDescriptor = data.UriDescriptor;
            var msg = string.Empty;

            //in data
            var inHeaderBuilder = new StringBuilder();

            if (exchange.InMessage != null)
            {
                try
                {
                    exchange.InMessage.HeaderCollection.ToList()
                                .ForEach(c => inHeaderBuilder.AppendFormat("{0}='{1}', ", c.Key, c.Value.ToString()));

                    var propertyBuilder = new StringBuilder();
                    exchange.PropertyCollection.ToList()
                        .ForEach(c => propertyBuilder.AppendFormat("{0}='{1}',", c.Key, c.Value.ToString(CultureInfo.InvariantCulture)));
                    var inBody = exchange.InMessage.Body.ToString();

                    msg = string.Format(
                            "{6}: Exchange-Id: {0}, MEP={5}. >> Route: ID({1}), Properties: [ {3} ], In Message: [ Headers: {2} InBody: {4} ]",
                            exchange.ExchangeId, exchange.Route.RouteId, inHeaderBuilder, propertyBuilder, inBody,
                            exchange.MepPattern, DateTime.Now.ToUniversalTime());
                }
                catch
                {
       
                }
            }

            //out data
            if (exchange.MepPattern == Exchange.Mep.InOut)
            {
                var outHeaderBuilder = new StringBuilder();
                exchange.OutMessage.HeaderCollection.ToList().ForEach(c => outHeaderBuilder.AppendFormat("{0}='{1}', ", c.Key, c.Value.ToString()));
                var outBody = exchange.OutMessage.Body.ToString();
                msg = string.Format("{0}, Out Message: [ Headers: {1} OutBody: {2} ]", msg, outHeaderBuilder, outBody);
            }

            var errors = exchange.DequeueAllErrors;
            if (errors.Count > 0)
            {
                var errorListing = new StringBuilder();
                errors.ForEach(c => errorListing.AppendFormat("[ error-message?: {0}, any-stack-trace?: {1} ]", c.Message, c.StackTrace));
                msg = string.Format("{0}, {1}", msg, errorListing);
            }

            try
            {
                Log(uriDescriptor.ComponentPath, msg);
            }
            catch (Exception exception)
            {
                
            }
        }
    }
}
