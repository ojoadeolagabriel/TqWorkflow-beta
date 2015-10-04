using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve.dto;

namespace app.core.nerve.strategy
{
    public interface ISplitterStrategy
    {
        List<string> Split(Exchange exchange);
    }
}
