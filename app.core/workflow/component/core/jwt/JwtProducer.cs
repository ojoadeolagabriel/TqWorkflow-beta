using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.workflow.dto;
using app.core.workflow.facade;

namespace app.core.workflow.component.core.jwt
{
    public class JwtProducer : DefaultProducer
    {
        public JwtProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {
        }

        public override Exchange Process(Exchange exchange, UriDescriptor uriInformation)
        {
            var name = uriInformation.ComponentPath;
            var role = uriInformation.GetUriProperty("role");
            var sharedKey = uriInformation.GetUriProperty("sharedKey");

            var claims = new Dictionary<string, string>
            {
                {"name", name},
                {"role", role},
            };

            var token = JWT.JsonWebToken.Encode(claims, sharedKey, JWT.JwtHashAlgorithm.HS256);
            exchange.InMessage.SetHeader(uriInformation.ComponentPath, token);
            Camel.TryLog(exchange, "producer", uriInformation.ComponentName);

            return exchange;
        }
    }
}
