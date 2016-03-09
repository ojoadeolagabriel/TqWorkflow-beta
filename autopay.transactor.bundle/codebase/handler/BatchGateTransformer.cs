using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using autopay.transactor.bundle.codebase.dto;

namespace autopay.transactor.bundle.codebase.handler
{
    public class BatchGateTransformer : ITransactionTransformer
    {
        public void PerformAction(TransactionObj obj)
        {
            obj.Narration = "12345/ Jan Salary / LANG";
        }
    }

    public class StandardAutoGateTransformer : ITransactionTransformer
    {
        public void PerformAction(TransactionObj obj)
        {
            obj.PinBlock = "1dfgdikfufiodfjdiufhd!23";
        }
    }

    public class RtsAutoGateTransformer : ITransactionTransformer
    {
        public void PerformAction(TransactionObj obj)
        {
            obj.ReceivingInstitutionId = "62806111";
        }
    }
}
