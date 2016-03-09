using System;
using System.Net;
using System.Text;
using app.core.nerve.dto;
using app.core.nerve.facade;

namespace app.core.nerve.component.core.httpclient
{
    public class HttpClientProducer : DefaultProducer
    {
        private class WebCamelClient : WebClient
        {
            public int Timeout { private get; set; }
            public string ContentType { get; set; }

            protected override WebRequest GetWebRequest(Uri uri)
            {
                var lWebRequest = base.GetWebRequest(uri);
                if (lWebRequest != null)
                {
                    lWebRequest.Timeout = Timeout;
                    ((HttpWebRequest)lWebRequest).ReadWriteTimeout = Timeout;
                }
                return lWebRequest;
            }
        }

        public HttpClientProducer(UriDescriptor uriInformation, Route route)
            : base(uriInformation, route)
        {

        }

        public override Exchange Process(Exchange exchange, UriDescriptor descriptor)
        {
            Camel.TryLog(exchange, "producer", descriptor.ComponentName);

            var connectionTimeOut = descriptor.GetUriProperty<int>("connectionTimeOut");
            var noHeaderPolicy = descriptor.GetUriProperty<bool>("noHeaderPolicy");
            var httpMethod = exchange.InMessage.GetHeader(CamelConstant.HttpMethod) as string;
            var httpUri = exchange.InMessage.GetHeader(CamelConstant.HttpUri);
            var httpQuery = exchange.InMessage.GetHeader(CamelConstant.HttpQuery) as string;

            var httpContentType = exchange.InMessage.GetHeader(CamelConstant.HttpContentType, "application/x-www-form-urlencoded");
            var path = !string.IsNullOrEmpty((descriptor.ComponentPath)) ? descriptor.ComponentPath : httpUri as string;

            path = string.Format("{0}", path);

            try
            {
                using (var client = new WebCamelClient())
                {
                    client.Timeout = connectionTimeOut > 1000 ? connectionTimeOut : 1000;

                    switch (httpMethod)
                    {
                        case "POST":
                            if (exchange.InMessage.Body != null)
                            {
                                var payLoad = exchange.InMessage.Body != null
                                    ? exchange.InMessage.Body.ToString()
                                    : string.Empty;
                                client.Headers[HttpRequestHeader.ContentType] = httpContentType as string;
                                client.ContentType = httpContentType;

                                if(noHeaderPolicy)
                                    client.Headers.Clear();
                                client.UploadString(path, payLoad);
                            }
                            break;
                        case "PUT":
                            if (exchange.InMessage.Body != null)
                            {
                                var payLoad = exchange.InMessage.Body != null
                                    ? exchange.InMessage.Body.ToString()
                                    : string.Empty;


                                if (noHeaderPolicy)
                                    client.Headers.Clear();
                                client.UploadData(path, "PUT", Encoding.ASCII.GetBytes(payLoad));
                            }
                            break;
                        case "GET":
                            var queryKeyValue = UriDescriptor.BuildKeyValueListWithEquality(httpQuery);
                            if (queryKeyValue != null)
                                queryKeyValue.ForEach(c => client.QueryString.Add(c.Key, c.Value));


                            if (noHeaderPolicy)
                                client.Headers.Clear();

                            var response = client.DownloadString(path);
                            exchange.InMessage.Body = response;
                            break;
                        case "TRACE":
                            var queryKeyValueTrace = UriDescriptor.BuildKeyValueListWithEquality(httpQuery);
                            if (queryKeyValueTrace != null)
                                queryKeyValueTrace.ForEach(c => client.QueryString.Add(c.Key, c.Value));


                            if (noHeaderPolicy)
                                client.Headers.Clear();

                            var responsetrace = client.DownloadString(path);
                            exchange.InMessage.Body = responsetrace;
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

                    Camel.TryLog(exchange, "provider", descriptor.ComponentName);
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
