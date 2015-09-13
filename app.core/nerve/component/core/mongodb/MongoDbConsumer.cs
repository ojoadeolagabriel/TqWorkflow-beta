using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using app.core.nerve.dto;

namespace app.core.nerve.component.core.mongodb
{
    public class MongoDbConsumer : PollingConsumer
    {
        private MongoDbProcessor _processor;
        public MongoDbConsumer(MongoDbProcessor processor)
        {
            _processor = processor;
        }

        public override Exchange Poll()
        {
            Task.Factory.StartNew(PollHandler);
            return null;
        }

        private void PollHandler()
        {
            throw new NotImplementedException();
        }
    }
}
