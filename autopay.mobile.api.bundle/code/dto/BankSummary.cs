using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autopay.mobile.api.bundle.code.dto
{
    public class BankSummary
    {
        public List<Bank> BankHealthInformation = new List<Bank>();

        public class Bank
        {
            public List<string> LastResponseCodes = new List<string>();

            public string BankDescription { get; set; }
            public string BankId { get; set; }
            public string LastEventDescription { get; set; }
            public bool IsProcessorEnabled { get; set; }
            public bool IsPaused { get; set; }
            public string BankName { get; set; }
        }
    }
}
