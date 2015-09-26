using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace camelcontext.server.facade.util
{
    public class PaymentContext
    {
        public Batch BatchInfo { get; set; }
        public Secure SecureInfo { get; set; }

        public List<Payment> PaymentCollection = new List<Payment>();

        public class Payment
        {
            public string Reference { get; set; }
            public string BeneficiaryCode { get; set; }
            public int Amount { get; set; }
            public string Narration { get; set; }
            public string Currency { get; set; }
            public string ToAccount { get; set; }
            public string ToAccountType { get; set; }
            public string Email { get; set; }
            public bool IsPrepaid { get; set; }
            public bool Phone { get; set; }
            public bool Name { get; set; }
        }

        public class Batch
        {
            public string Reference { get; set; }
            public string Description { get; set; }
            public string TerminalId { get; set; }
        }

        public class Secure
        {
            public string SecureData { get; set; }
            public string MacData { get; set; }
            public string FromAccount { get; set; }
            public string FromAccountType { get; set; }
        }
    }
}
