using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using app.core.workflow.dto;

namespace app.core.workflow.component.core.timer
{
    public class TimerConsumer : PollingConsumer
    {
        private TimerProcessor _processor;
        class PassData
        {
            public Exchange Exchange { get; set; }
        }

        public TimerConsumer(TimerProcessor processor)
        {
            _processor = processor;
        }

        public override Exchange Poll()
        {
            Task.Factory.StartNew(PollHandler);
            return null;
        }

        private Timer timer;

        private void PollHandler()
        {
            var poll = _processor.UriInformation.GetUriProperty("poll", 1000);
            var dueTime = _processor.UriInformation.GetUriProperty("dueTime", 1000);
            timer = new Timer(CallBack, new PassData () , dueTime, poll);
        }

        public Exchange LoadExchange()
        {
            return new Exchange(_processor.Route);
        }

        private void CallBack(object state)
        {
            var passData = (PassData) state;
            passData.Exchange = LoadExchange();
            _processor.Process(passData.Exchange);
        }
    }
}
