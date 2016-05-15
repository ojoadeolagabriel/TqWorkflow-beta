using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve.dto;
using app.core.nerve.handlers.tag;

namespace app.core.nerve.language
{
    public class ToUtil
    {
        private Exchange _exchange;
        public ToUtil(Exchange exchange)
        {
            _exchange = exchange;
        }

        public ToUtil To(string uri)
        {
            ToTag.Execute(uri, _exchange, _exchange.Route);
            return new ToUtil(_exchange);
        }
    }
}
