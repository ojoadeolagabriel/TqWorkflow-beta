using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.mongodb
{
    public class MongoDbProcessor: DefaultProcessor
    {
        public MongoDbProcessor(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {
            
        }
    }
}
