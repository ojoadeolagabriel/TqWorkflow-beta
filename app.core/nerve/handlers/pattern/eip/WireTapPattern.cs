using System.Threading.Tasks;
using app.core.nerve.dto;
using app.core.nerve.expression;
using app.core.nerve.facade;

namespace app.core.nerve.handlers.pattern.eip
{
    public class WireTapPattern
    {
        public static void Execute(Exchange exchange, string path, Route route)
        {
            Task.Factory.StartNew(() => Tap(exchange, path, route));
        }

        private static void Tap(Exchange exchange, string path, Route route)
        {
            try
            {
                var leafNodeParts = UriDescriptor.Parse(path, exchange);
                EndPointBuilder.HandleTo(leafNodeParts, exchange, route);
            }
            catch
            {

            }
        }
    }
}
