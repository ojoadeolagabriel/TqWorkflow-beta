using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using app.core.workflow.dto;
using app.core.workflow.facade;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace app.core.workflow.component.core.httpclient
{
    public class HttpClientProducer : DefaultProducer
    {
        public HttpClientProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {

        }

        public override Exchange Process(Exchange exchange, UriDescriptor descriptor)
        {
            var connectionTimeToLive = descriptor.GetUriProperty<int>("connectionTimeToLive");
            var httpMethod = exchange.InMessage.GetHeader(CamelConstant.HttpMethod) as string;
            var httpUri = exchange.InMessage.GetHeader(CamelConstant.HttpUri);
            var httpQuery = exchange.InMessage.GetHeader(CamelConstant.HttpQuery) as string;
            var httpCharacterEncoding = exchange.InMessage.GetHeader(CamelConstant.HttpCharacterEncoding);
            var httpContentType = exchange.InMessage.GetHeader(CamelConstant.HttpContentType);
            var path = !string.IsNullOrEmpty((descriptor.ComponentPath)) ? descriptor.ComponentPath : httpUri as string;

            path = string.Format("http://{0}", path);

            try
            {
                using (var client = new WebClient())
                {
                    switch (httpMethod)
                    {
                        case "POST":
                            if (exchange.InMessage.Body != null)
                            {
                                var payLoad = exchange.InMessage.Body != null
                                    ? exchange.InMessage.Body.ToString()
                                    : string.Empty;
                                client.Headers[HttpRequestHeader.ContentType] = httpContentType as string;
                                client.UploadString(path, payLoad);
                            }
                            break;
                        case "PUT":
                            if (exchange.InMessage.Body != null)
                            {
                                var payLoad = exchange.InMessage.Body != null
                                    ? exchange.InMessage.Body.ToString()
                                    : string.Empty;
                                client.UploadData(path, "PUT", Encoding.ASCII.GetBytes(payLoad));
                            }
                            break;
                        case "GET":
                            var queryKeyValue = UriDescriptor.BuildKeyValueListWithEquality(httpQuery);
                            queryKeyValue.ForEach(c => client.QueryString.Add(c.Key, c.Value));

                            var response = client.DownloadString(path);
                            exchange.InMessage.Body = response;
                            break;
                    }

                    for (int i = 0; i < client.ResponseHeaders.Count; i++)
                    {
                        var key = client.ResponseHeaders.GetKey(i);
                        var strings = client.ResponseHeaders.GetValues(i);
                        if (strings == null) continue;

                        var value = strings[0];
                        exchange.InMessage.SetHeader("http-response-" + key, value);
                    }
                }
            }
            catch (WebException exception)
            {
                if (exception.Status == WebExceptionStatus.ProtocolError)
                {
                    exchange.Exception.Push(exception);
                }
            }
            catch (Exception ex)
            {
                exchange.Exception.Push(ex);
            }

            return base.Process(exchange, descriptor);
        }
    }
}
