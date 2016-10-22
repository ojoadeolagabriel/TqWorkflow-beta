using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autopay.mobile.api.bundle.code.dto
{
    public class ResponseCode
    {
        public const string Success = "90000";
        public const string Unknown = "10001";
        public const string AuthenticationUnknownError = "70000";
        public const string InCorrectAuthenticationParamaters = "70001";
    }
}
