using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using app.core.nerve.dto;
using app.core.nerve.expression;

namespace app.core.nerve.component.core.file
{
    public class FileConsumer : PollingConsumer
    {
        private readonly FileProcessor _fileProcessor;

        public FileConsumer(FileProcessor fileProcessor)
        {
            _fileProcessor = fileProcessor;
        }

        public override Exchange Poll()
        {
            //PollHandler();
            Task.Factory.StartNew(PollHandler);
            return null;
        }

        private void PollHandler()
        {

            var pollInterval = _fileProcessor.UriInformation.GetUriProperty("poll", 1000);
            var fileFolderPath = _fileProcessor.UriInformation.ComponentPath;
            var maxThreadCount = _fileProcessor.UriInformation.GetUriProperty("threadCount", 3);
            var initialDelay = _fileProcessor.UriInformation.GetUriProperty("initialDelay", 1000);
            Thread.Sleep(initialDelay);

            var totalCount = 0;

            while (true)
            {
                if (totalCount >= maxThreadCount)
                    continue;

                var exchange = new Exchange(_fileProcessor.Route);
                var fileData = "";

                if (Directory.Exists(fileFolderPath))
                {
                    var fileInfo = Directory.GetFiles(fileFolderPath).FirstOrDefault();
                    if (fileInfo != null)
                    {
                        fileFolderPath = fileInfo;
                        fileData = File.ReadAllText(fileInfo);
                    }
                }
                else if (File.Exists(fileFolderPath))
                {
                    fileData = File.ReadAllText(fileFolderPath);
                }

                ProcessResponse(fileData, exchange, fileFolderPath);
                --totalCount;

                Thread.Sleep(pollInterval);
            }
        }

        private void ProcessResponse(string fileData, Exchange exchange, string fileFolderPath)
        {
            exchange.InMessage.SetHeader("fileFolderPath", fileFolderPath);
            exchange.InMessage.Body = fileData;

            Camel.TryLog(exchange);
            _fileProcessor.Process(exchange);
            exchange.OutMessage.Body = exchange.InMessage.Body;
        }
    }
}
