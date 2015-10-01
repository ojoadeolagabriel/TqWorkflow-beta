using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve.dto;

namespace app.core.nerve.component.core.csv
{
    public class CsvEndPoint : DefaultEndpoint
    {
        private CsvProcessor _processor;
        private CsvConsumer _consumer;

        public CsvEndPoint(string uri, Route route) :
            base(uri, route)
        {

        }

        public override void Start()
        {
            _consumer = (CsvConsumer)CreateConsumer();

            if (_consumer.GetType().IsSubclassOf(typeof(PollingConsumer)))
            {
                _consumer.Poll();
            }
            else
            {
                _consumer.Execute();
            }
        }

        public DefaultConsumer CreateConsumer()
        {
            return new CsvConsumer(new CsvProcessor(UriInformation, base.Route));
        }

        public DefaultProducer CreateProducer()
        {
            return new CsvProducer(UriInformation, Route);
        }
    }
}
