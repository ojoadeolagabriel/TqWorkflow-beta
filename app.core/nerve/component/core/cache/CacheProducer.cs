using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.cache
{
    public class CacheProducer : DefaultProducer
    {
        public CacheProducer(UriDescriptor uriInformation, Route route) : base(uriInformation, route)
        {
        }

        public override Exchange Process(Exchange exchange, UriDescriptor descriptor)
        {
            Camel.TryLog(exchange, "producer", descriptor.ComponentName);
            var action = descriptor.ComponentPath ?? "add";

            try
            {
                switch (action)
                {
                    case "add":
                        var coll = UriDescriptor.BuildKeyValueListWithEquality(descriptor.ComponentQueryPath);
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                
            }
            return exchange;
        }
    }
}
