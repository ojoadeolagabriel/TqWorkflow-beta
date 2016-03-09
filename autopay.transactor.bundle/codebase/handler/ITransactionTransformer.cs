using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using autopay.transactor.bundle.codebase.dto;

namespace autopay.transactor.bundle.codebase.handler
{
    public interface ITransactionTransformer
    {
        void PerformAction(TransactionObj obj);
    }
}
