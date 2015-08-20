using System.IO;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.file
{
    public class FileProducer : DefaultProducer
    {
        public FileProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {

        }

        public override Exchange Process(Exchange exchange, UriDescriptor endPointDescriptor)
        {
            var fileData = exchange.OutMessage.Body != null ? exchange.OutMessage.Body.ToString() : "";
            var fileName = endPointDescriptor.ComponentPath;

            if (!string.IsNullOrEmpty(fileData) && !string.IsNullOrEmpty(fileName))
                File.AppendAllText(fileName, fileData);

            Camel.TryLog(exchange, "provider", endPointDescriptor.ComponentName);
            return exchange;
        }
    }
}
