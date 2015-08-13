using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using app.core.workflow.dto;
using app.core.workflow.facade;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace app.core.workflow.component.core.log
{
    public class LogProducer : DefaultProducer
    {
        private static ILog log = null;
        private static FileAppender _appender = null;

        public LogProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {

        }

        public override Exchange Process(Exchange exchange, UriDescriptor uriDescriptor)
        {
            try
            {
                var showOut = uriDescriptor.GetUriProperty("show-out", false);
                var filePathToLogTo = uriDescriptor.GetUriProperty("file");
                var additionalMessage = uriDescriptor.GetUriProperty("message", "");

                //in data
                var inHeaderBuilder = new StringBuilder();
                exchange.InMessage.HeaderCollection.ToList().ForEach(c => inHeaderBuilder.AppendFormat("{0}='{1}', ", c.Key, c.Value.ToString()));

                var propertyBuilder = new StringBuilder();
                exchange.PropertyCollection.ToList().ForEach(c => propertyBuilder.AppendFormat("{0}='{1}',", c.Key, c.Value.ToString()));
                var inBody = exchange.InMessage.Body.ToString();

                var msg = string.Format("Exchange:  Id: {0}, MEP={5}. >> Route: ID({1}), Properties [{3}], In Message: [Headers: {2} Body: {4}]",
                    exchange.ExchangeId, exchange.Route.RouteId, inHeaderBuilder, propertyBuilder, inBody, exchange.MepPattern);

                //out data
                if (exchange.MepPattern == Exchange.Mep.InOut)
                {
                    var outHeaderBuilder = new StringBuilder();
                    exchange.OutMessage.HeaderCollection.ToList().ForEach(c => outHeaderBuilder.AppendFormat("{0}='{1}', ", c.Key, c.Value.ToString()));
                    var outBody = exchange.OutMessage.Body.ToString();
                    msg = string.Format("{0}, Out Message: [Headers: {1} Body: {2}]", msg, outHeaderBuilder, outBody);
                }

                var errors = exchange.DequeueAllErrors;
                if (errors.Count > 0)
                {
                    var errorListing = new StringBuilder();
                    errors.ForEach(c => errorListing.AppendFormat("[ error-message?: {0}, any-stack-trace?: {1} ]", c.Message, c.StackTrace));
                    msg = string.Format("{0}, {1}", msg, errorListing);
                }

                var starTime = DateTime.Now;
                if (!string.IsNullOrEmpty(filePathToLogTo))
                    Log(filePathToLogTo, msg, uriDescriptor);

                var interval = DateTime.Now - starTime;
                return exchange;
            }
            catch (AggregateException exception)
            {

            }
            catch (Exception exception)
            {

            }

            return exchange;
        }

        static readonly object _locker = new object();
        private void Log(string filePathToLogTo, string defaultStr, UriDescriptor descriptor)
        {

            lock (_locker)
            {
                var existingRepo = LogManager.GetAllRepositories().FirstOrDefault(c => c.Name == "core.app");
                if (existingRepo == null)
                {
                    var repository = LogManager.CreateRepository("core.app");

                    var fileAppender = new RollingFileAppender
                    {
                        MaxFileSize = 10000000,
                        MaxSizeRollBackups = 10,
                        RollingStyle = RollingFileAppender.RollingMode.Size,
                        AppendToFile = true,
                        File = filePathToLogTo,
                        Threshold = new Level(0, "DEBUG"),
                        LockingModel = new FileAppender.MinimalLock()
                    };

                    var pl = new PatternLayout { ConversionPattern = "%utcdate,%message%newline" };
                    pl.ActivateOptions();

                    fileAppender.Layout = pl;
                    fileAppender.ActivateOptions();

                    BasicConfigurator.Configure(repository, fileAppender);
                    log = LogManager.GetLogger(typeof(LogProcessor));

                    var hierarchy = (Hierarchy)LogManager.GetRepository();
                    hierarchy.Root.AddAppender(fileAppender);
                    hierarchy.Configured = true;
                }
                else
                {
                    var appender = (RollingFileAppender)LogManager.GetRepository().GetAppenders().FirstOrDefault(c => c is RollingFileAppender);
                    if (appender != null)
                    {
                        appender.File = filePathToLogTo;
                        appender.ActivateOptions();
                    }
                }
            }

            log.Debug(defaultStr);
        }

    }
}
