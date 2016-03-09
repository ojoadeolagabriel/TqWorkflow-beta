using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace autopay.transactor.bundle.codebase.dto
{
    public class TransactionObj
    {
        public string CbnCode { get; set; }
        public string EncryptedPin { get; set; }
        public string DestAccount { get; set; }
        public string SourceAccount { get; set; }
        public double Amount { get; set; }
        public string Narration { get; set; }
        public string PinBlock { get; set; }
        public string ReceivingInstitutionId { get; set; }
    }
}
