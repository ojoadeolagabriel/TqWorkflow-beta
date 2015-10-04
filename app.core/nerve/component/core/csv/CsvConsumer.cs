using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using app.core.nerve.dto;
using app.core.utility;

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
            var pollInterval = _csvProcessor.UriInformation.GetUriProperty("poll", 500);
            var fileFolderPath = _csvProcessor.UriInformation.ComponentPath;
            var maxThreadCount = _csvProcessor.UriInformation.GetUriProperty("threadCount", 3);
            var initialDelay = _csvProcessor.UriInformation.GetUriProperty("initialDelay", 1000);
            var pathToCsv = _csvProcessor.UriInformation.GetUriProperty<string>("pathToCsv");
            var csvFileExt = _csvProcessor.UriInformation.GetUriProperty<string>("csvFileExt");
            var createDirectory = _csvProcessor.UriInformation.GetUriProperty<bool>("createDirectory");
            var deleteIfErrorFound = _csvProcessor.UriInformation.GetUriProperty<bool>("deleteIfErrorFound");
            Thread.Sleep(initialDelay);

            while (true)
            {
                if (string.IsNullOrEmpty(pathToCsv))
                {
                    Thread.Sleep(1000);
                    continue;
                }

                Thread.Sleep(pollInterval);

                if (!Directory.Exists(pathToCsv))
                    Directory.CreateDirectory(pathToCsv);

                var firstFile = Directory.GetFiles(pathToCsv, "*.csv").FirstOrDefault();
                if (firstFile == null)
                {
                    continue;
                }


                var filedata = File.ReadAllText(firstFile);
                var dao = CsvDao.ParseFromString(filedata);

                if (dao == null)
                {
                    if (deleteIfErrorFound)
                        File.Delete(firstFile);
                    Thread.Sleep(1000);
                    continue;
                }

                var exchange = new Exchange(_csvProcessor.Route) { InMessage = { Body = dao.OriginalCsvData } };
                _csvProcessor.Process(exchange);
            }
        }

    }
}
