using System.IO;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.file
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

            Camel.TryLog(exchange, "provider");
            return exchange;
        }
    }
}
