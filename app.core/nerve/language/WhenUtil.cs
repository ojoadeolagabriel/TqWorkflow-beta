using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve.dto;
using app.core.nerve.pattern.eip;

namespace app.core.nerve.language
{
    public class WhenUtil
    {
        private Exchange _exchange;
        public WhenUtil(Exchange exchange)
        {
            _exchange = exchange;
        }

        public ToUtil When(bool isSuccessful)
        {
            if (isSuccessful)
            {
                MessageRouterPattern.Execute(null, _exchange);
                return new ToUtil(_exchange);
            }

            return null;
        }
    }
}
