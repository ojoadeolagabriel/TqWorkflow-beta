using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services.Description;
using System.Xml.Linq;
using app.core.workflow.dto;

namespace app.core.workflow.component.core.http
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
            var exchange = new Exchange(_httpProcessor.Route);
            var initialDelay = _httpProcessor.UriInformation.GetUriProperty("initialDelay", 1000);
            var portId = _httpProcessor.UriInformation.GetUriProperty("port", 9000, exchange);
            var path = _httpProcessor.UriInformation.GetUriProperty("path","");

            if(initialDelay > 0)
                Thread.Sleep(initialDelay);

            HttpListener = new HttpListener();

            if (path.StartsWith("\\"))
                path = path.Remove(0, 1);

            var uriPref = string.Format("http://{0}:{1}/{2}/", _httpProcessor.UriInformation.ComponentPath, portId, path);
            if (!uriPref.EndsWith("/"))
                uriPref = uriPref + "/";

            HttpListener.Prefixes.Add(uriPref);

            HttpListener.Start();
            HttpListener.BeginGetContext(ProcessIncommingClientAsync, new PassData
            {
                Exchange = exchange,
                HttpListener = HttpListener
            });
        }

        private void ProcessIncommingClientAsync(IAsyncResult res)
        {
            var passData = (PassData)res.AsyncState;
            var listener = passData.HttpListener;

            HttpListener.BeginGetContext(ProcessIncommingClientAsync, new PassData
            {
                Exchange = new Exchange(_httpProcessor.Route),
                HttpListener = listener
            });

            var exchange = passData.Exchange;
            var client = listener.EndGetContext(res);
            var body = new StreamReader(client.Request.InputStream).ReadToEnd();

            BuildRequestMessage(client, exchange, body);

            exchange.InMessage.Body = body;
            Camel.TryLog(exchange, "consumer", _httpProcessor.UriInformation.ComponentName);

            _httpProcessor.Process(exchange);

            var b = Encoding.UTF8.GetBytes(exchange.InMessage.Body.ToString());
            client.Response.StatusCode = 200;
            client.Response.KeepAlive = false;

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
            client.Response.Close();
        }

        private void BuildRequestMessage(HttpListenerContext client, Exchange exchange, string body)
        {
            var msg = Environment.NewLine + Environment.NewLine + @"{0} {1} {2}" + Environment.NewLine + 
                         "Host: {3}" + Environment.NewLine + 
                         "Content-Type: {4}" + Environment.NewLine + 
                         "Content-Length: {5}" +  Environment.NewLine + Environment.NewLine +
                         "{6}" + Environment.NewLine + Environment.NewLine;

            var data = string.Format(msg, client.Request.HttpMethod, client.Request.Url, "HTTP/1.1",
                client.Request.UserHostName, client.Request.ContentType, client.Request.ContentLength64, body);

            exchange.InMessage.SetHeader("HttpRequest", data);
        }

        private Exchange ProcessResponse(HttpListenerContext getContext, Exchange exchange)
        {
            var body = new StreamReader(getContext.Request.InputStream).ReadToEnd();
            exchange.InMessage.Body = body;
            _httpProcessor.Process(exchange);

            var b = Encoding.UTF8.GetBytes(exchange.OutMessage.Body.ToString());
            getContext.Response.StatusCode = 200;
            getContext.Response.KeepAlive = false;

            foreach (var headers in exchange.InMessage.HeaderCollection)
            {
                getContext.Response.Headers.Add(headers.Key, headers.Value.ToString());    
            }

            getContext.Response.ContentLength64 = b.Length;

            var output = getContext.Response.OutputStream;
            output.Write(b, 0, b.Length);
            getContext.Response.Close();

            return exchange;
        }
    }
}
