using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace autopay.mobile.api.bundle.code.dto
{
    public class UserLogin
    {
        public string SecureData { get; set; }
       
        public class UserLoginStatus
        {
            public string ResponseCode;
            public string ResponseMessage;
        }
    }
}
