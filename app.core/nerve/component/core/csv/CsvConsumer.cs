using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using app.core.nerve.dto;
using app.core.utility;
using Quartz;
using Quartz.Impl;

namespace app.core.nerve.component.core.csv
{
    public class CsvConsumer : PollingConsumer
    {
        private readonly CsvProcessor _csvProcessor;

        public CsvConsumer(CsvProcessor processor)
        {
            _csvProcessor = processor;
        }

        public override Exchange Poll()
        {
            Task.Factory.StartNew(PollHandler);
            return null;
        }

        private void PollHandler()
        {
            var cron = _csvProcessor.UriInformation.GetUriProperty<string>("cron");

            if (!string.IsNullOrWhiteSpace(cron))
            {
                var jobDesc = _csvProcessor.UriInformation.ComponentPath + "_job";
                var jobGroup = _csvProcessor.UriInformation.ComponentPath + "_group";

                ISchedulerFactory schedFact = new StdSchedulerFactory();
                var sched = schedFact.GetScheduler();
                sched.Start();

                var job = JobBuilder.Create<CsvJob>()
                    .WithIdentity(jobDesc, jobGroup)
                    .UsingJobData(new JobDataMap { { "processor", _csvProcessor } })
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity(_csvProcessor.UriInformation.ComponentPath, jobGroup)
                    .WithCronSchedule(cron)
                    .ForJob(jobDesc, jobGroup)
                    .Build();

                sched.ScheduleJob(job, trigger);
            }
            else
            {
                while (true)
                {
                    var pollInterval = _csvProcessor.UriInformation.GetUriProperty("poll", 500);
                    Thread.Sleep(pollInterval);

                    if (!CanRun(_csvProcessor))
                    {
                        Console.WriteLine("Bundle [{0}]: not-active", _csvProcessor.Route.BundleInfo.Name);
                    }
                    else
                    {    
                        CsvJob.RunJob(_csvProcessor);
                    }
                }
            }
        }


        public class CsvJob : IJob
        {
            public void Execute(IJobExecutionContext context)
            {
                var csvProcessor = context.Get("processor") as CsvProcessor;
                RunJob(csvProcessor);
            }

            public static void RunJob(DefaultProcessor csvProcessor)
            {
                var fileFolderPath = csvProcessor.UriInformation.ComponentPath;
                var maxThreadCount = csvProcessor.UriInformation.GetUriProperty("threadCount", 3);
                var initialDelay = csvProcessor.UriInformation.GetUriProperty("initialDelay", 1000);
                var pathToCsv = csvProcessor.UriInformation.GetUriProperty<string>("pathToCsv");
                var csvFileExt = csvProcessor.UriInformation.GetUriProperty<string>("csvFileExt");
                var createDirectory = csvProcessor.UriInformation.GetUriProperty<bool>("createDirectory");
                var deleteIfErrorFound = csvProcessor.UriInformation.GetUriProperty<bool>("deleteIfErrorFound");

                if (!Directory.Exists(pathToCsv))
                    Directory.CreateDirectory(pathToCsv);

                var firstFile = Directory.GetFiles(pathToCsv, "*.csv").FirstOrDefault();
                if (firstFile == null)
                {
                    return;
                }

                var filedata = File.ReadAllText(firstFile);
                var dao = CsvDao.ParseFromString(filedata);

                if (dao == null)
                {
                    if (deleteIfErrorFound)
                        File.Delete(firstFile);

                    Thread.Sleep(1000);
                    return;
                }

                var exchange = new Exchange(csvProcessor.Route) { InMessage = { Body = dao.OriginalCsvData } };
                csvProcessor.Process(exchange);
            }
        }
    }
}
