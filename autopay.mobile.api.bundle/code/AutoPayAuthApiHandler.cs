using app.core.nerve.facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using app.core.nerve.dto;
using Newtonsoft.Json;
using autopay.mobile.api.bundle.code.dto;

namespace autopay.mobile.api.bundle.code
{
    public class AutoPayAuthApiHandler : ProcessorBase
    {
        public class UserAuth
        {
            public class UserInfo
            {
                public string Name { get; set; }
                public string Password { get; set; }
                public string CorporateCode { get; set; }
                public string Otp { get; set; }
            }

            public string SessionToken { get; set; }
            public string ResponseCode { get; set; }
            public string ResponseMsg { get; set; }
        }

        public override void Process(Exchange exchange)
        {
            ValidateUser(exchange);
        }

        private void ValidateUser(Exchange exchange)
        {
            var token = exchange.InMessage.GetHeader<String>("securityToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                try
                {
                    var secToken = JWT.JsonWebToken.Decode(token, Encoding.ASCII.GetBytes("password"));
                    var userInfo = JsonConvert.DeserializeObject<UserAuth.UserInfo>(secToken);

                    if (userInfo.Name == "adeola")
                    {
                        var data = new UserAuth { SessionToken = "djsdjhj3i9sdhnw3", ResponseCode = ResponseCode.Success };
                        exchange.InMessage.Body = JsonConvert.SerializeObject(data);
                    }else
                    {
                        var data = new UserAuth { SessionToken = "", ResponseCode = ResponseCode.InCorrectAuthenticationParamaters, ResponseMsg = "Incorrect login details" };
                        exchange.InMessage.Body = JsonConvert.SerializeObject(data);
                    }
                }
                catch (Exception e)
                {
                    var data = new UserAuth { SessionToken = "", ResponseCode = ResponseCode.AuthenticationUnknownError, ResponseMsg = e.Message };
                    exchange.InMessage.Body = JsonConvert.SerializeObject(data);
                }
            }
            else
            {
                var data = new UserAuth { SessionToken = "", ResponseCode = ResponseCode.Unknown };
                exchange.InMessage.Body = JsonConvert.SerializeObject(data);
            }
        }
    }
}
