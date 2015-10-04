using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve.dto;
using app.core.nerve.strategy;

namespace camelcontext.server.facade.strategy
{
    public class CsvSplitter : ISplitterStrategy
    {
        public List<string> Split(Exchange exchange)
        {
            if (exchange.InMessage.Body == null)
                return null;

            var bodyLines = exchange.InMessage.Body.ToString().Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            return bodyLines.ToList();
        }
    }
}
