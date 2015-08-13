using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using app.core.workflow.component;
using app.core.workflow.dto;
using app.core.workflow.expression;
using app.core.workflow.facade;

namespace app.core.workflow.handlers.pattern.eip
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
                var leafNodeParts = UriDescriptor.Parse(path);
                SimpleExpression.ResolveExpression(leafNodeParts, exchange);
                EndPointBuilder.HandleTo(leafNodeParts, exchange, route);
            }
            catch
            {
                
            }
        }
    }
}
