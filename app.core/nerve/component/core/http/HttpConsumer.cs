using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using app.core.nerve.dto;

namespace app.core.nerve.component.core.http
{
    public class HttpConsumer : PollingConsumer
    {
        private readonly HttpProcessor _httpProcessor;

        class PassData
        {
            public HttpListener HttpListener { get; set; }
            public Exchange Exchange { get; set; }
        }

        public HttpConsumer(HttpProcessor processor)
        {
            _httpProcessor = processor;
        }

        public override Exchange Poll()
        {
            Task.Factory.StartNew(PollHandler);
            return null;
        }

        public HttpListener HttpListener;

        private void PollHandler()
        {
            try
            {
                var exchange = new Exchange(_httpProcessor.Route);
                var initialDelay = _httpProcessor.UriInformation.GetUriProperty("initialDelay", 1000);
                var portId = _httpProcessor.UriInformation.GetUriProperty("port", 9000, exchange);
                var path = _httpProcessor.UriInformation.GetUriProperty("path", "");

                if (initialDelay > 0)
                    Thread.Sleep(initialDelay);

                HttpListener = new HttpListener();

                if (path.StartsWith("\\"))
                    path = path.Remove(0, 1);

                var uriPref = string.Format("http://{0}:{1}/{2}/", _httpProcessor.UriInformation.ComponentPath, portId, path);
                if (!uriPref.EndsWith("/"))
                    uriPref = uriPref + "/";

                Console.WriteLine("Activiating HTTP Endpoint: {0}", uriPref);
                HttpListener.Prefixes.Add(uriPref);

                HttpListener.Start();
                HttpListener.BeginGetContext(ProcessIncommingClientAsync, new PassData
                {
                    Exchange = exchange,
                    HttpListener = HttpListener
                });
            }
            catch (Exception exception)
            {
                Console.WriteLine("{0}-{1}",exception.Message,exception.StackTrace);
            }
        }

        private void ProcessIncommingClientAsync(IAsyncResult res)
        {
            var passData = (PassData)res.AsyncState;
            var listener = passData.HttpListener;
            var client = listener.EndGetContext(res);
            HttpListener.BeginGetContext(ProcessIncommingClientAsync, new PassData
            {
                Exchange = new Exchange(_httpProcessor.Route),
                HttpListener = listener
            });

            if (_httpProcessor.Route.BundleInfo.BundleStatus != BundleDescriptorObject.Status.Active)
            {
                Console.WriteLine("Bundle [{0}]: NotActive", _httpProcessor.Route.BundleInfo.Name);
            }
            else
            {               
                var exchange = passData.Exchange;
                var body = new StreamReader(client.Request.InputStream).ReadToEnd();

                BuildRequestMessage(client, exchange, body);
                exchange.InMessage.Body = body;
                Camel.TryLog(exchange, "consumer", _httpProcessor.UriInformation.ComponentName);
                _httpProcessor.Process(exchange);
                var b = Encoding.UTF8.GetBytes(exchange.InMessage.Body.ToString());
                
                foreach (var headers in exchange.InMessage.HeaderCollection)
                {
                    try
                    {
                        client.Response.Headers.Add(headers.Key, WebUtility.HtmlEncode(headers.Value.ToString()));
                    }
                    catch (Exception exception)
                    {
                        exchange.Exception.Push(exception);
                    }
                }

                client.Response.ContentLength64 = b.Length;
                var output = client.Response.OutputStream;
                output.Write(b, 0, b.Length);
            }

            client.Response.StatusCode = 200;
            client.Response.KeepAlive = false;
            client.Response.Close();
        }

        private static void BuildRequestMessage(HttpListenerContext client, Exchange exchange, string body)
        {
            var msg = Environment.NewLine + Environment.NewLine + @"{0} {1} {2}" + Environment.NewLine +
                         "Host: {3}" + Environment.NewLine +
                         "Content-Type: {4}" + Environment.NewLine +
                         "Content-Length: {5}" + Environment.NewLine + Environment.NewLine +
                         "{6}" + Environment.NewLine + Environment.NewLine;

            var data = string.Format(msg, client.Request.HttpMethod, client.Request.Url, "HTTP/1.1",
                client.Request.UserHostName, client.Request.ContentType, client.Request.ContentLength64, body);

            exchange.InMessage.SetHeader("HttpRequestLog", data);
        }
    }
}
