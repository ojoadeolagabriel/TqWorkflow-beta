using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve.dto;
using app.core.nerve.facade;
using autopay.transactor.bundle.codebase.dto;
using autopay.transactor.bundle.codebase.handler;
using Newtonsoft.Json;

namespace autopay.transactor.bundle.codebase
{
    public class TransactionDecider : ProcessorBase
    {
        public const string AutoGateStandard = "AutoGate_Standard";
        public const string AutoGateBatchGate = "AutoGate_BatchGate";
        public const string AutoGateRealtimeSettlement = "AutoGate_RealtimeSettlement";
        public const string AutoGateTssFlow = "AutoGate_TssFlow";

        public override void Process(Exchange exchange)
        {
            var inBody = exchange.InMessage.Body;
            var tranObj = JsonConvert.DeserializeObject<TransactionObj>(inBody as string);

            var processType = exchange.InMessage.GetHeader<string>("process_type");
            ITransactionTransformer transformer;

            switch (processType)
            {
                case AutoGateStandard:
                    transformer = new StandardAutoGateTransformer();
                    transformer.PerformAction(tranObj);
                    break;
                case AutoGateBatchGate:
                    transformer = new BatchGateTransformer();
                    transformer.PerformAction(tranObj);
                    break;
                case AutoGateRealtimeSettlement:
                    transformer = new RtsAutoGateTransformer();
                    transformer.PerformAction(tranObj);
                    break;
                case AutoGateTssFlow:
                    break;
            }

            exchange.InMessage.Body = JsonConvert.SerializeObject(tranObj);
            base.Process(exchange);
        }
    }
}
