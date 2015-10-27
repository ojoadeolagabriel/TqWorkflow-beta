﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using app.core.nerve.dto;
using app.core.nerve.utility;
using Newtonsoft.Json;

namespace app.core.nerve.management
{
    public class SystemBundleManagement
    {
        public static HttpListener Server { get; set; }

        public class StartData
        {

            public HttpListener ServerListener { get; set; }
        }

        /// <summary>
        /// Start Service
        /// </summary>
        /// <param name="uriPrefix"></param>
        public static void StartService(string uriPrefix)
        {
            Server = new HttpListener();
            Server.Prefixes.Add(uriPrefix);
            Server.Prefixes.Add(uriPrefix + "status/");
            Server.Prefixes.Add(uriPrefix + "bundle.pause/");
            Server.Prefixes.Add(uriPrefix + "bundle.start/");
            Server.Start();

            Console.WriteLine("Starting [nerve.management.api.v2] @ {0}", uriPrefix);
            Server.BeginGetContext(InitServer, new StartData
            {
                ServerListener = Server
            });
        }

        private static void InitServer(IAsyncResult ar)
        {
            var passData = (StartData)ar.AsyncState;
            var listener = passData.ServerListener;
            Server.BeginGetContext(InitServer, new StartData
            {
                ServerListener = Server
            });

            var client = listener.EndGetContext(ar);
            var method = client.Request.Url.Segments[4].Replace("/", "");

            var body = new StreamReader(client.Request.InputStream).ReadToEnd();
            var response = ProcessRequest(body, client, method);
            var b = Encoding.UTF8.GetBytes(response);

            client.Response.StatusCode = 200;
            client.Response.KeepAlive = false;

            client.Response.ContentLength64 = b.Length;
            var output = client.Response.OutputStream;
            output.Write(b, 0, b.Length);
            client.Response.Close();
        }

        private static string ProcessRequest(string body, HttpListenerContext client, string method)
        {
            try
            {
                switch (method)
                {
                    case "status":
                        return HandleStatus(client, body);
                        break;
                    case "bundle.pause":
                        return HandlePause(client, body);
                        break;
                    case "bundle.start":
                        return HandleStart(client, body);
                        break;
                }
            }
            catch (Exception exception)
            {
                return JsonConvert.SerializeObject(new
                {
                    ResponseMessage = exception.Message,
                    ResponseCode = "E21",
                    InnerResponseDescription = exception.StackTrace
                });
            }

            return JsonConvert.SerializeObject(new
            {
                ResponseMessage = "[UnKnownRequest]",
                ResponseCode = "E01",
                InnerResponseDescription = "Contact Admin."
            });
        }

        private static string HandleStart(HttpListenerContext client, string body)
        {
            var id = client.Request.Url.Segments[5].Replace("/", "");
            var bundle = Camel.RouteCollection.FirstOrDefault(c => c.Value.BundleInfo != null && c.Value.BundleInfo.GuidData == id);

            if (!bundle.IsNull())
            {
                bundle.Value.BundleInfo.BundleStatus = BundleDescriptorObject.Status.Active;
                var result = JsonConvert.SerializeObject(new
                {
                    ResponseMessage = "[BundleUpdateComplete]",
                    ResponseCode = "90000",
                    InnerResponseDescription = "Active"
                });
                return PrepareResponse(result, client);
            }

            var res = JsonConvert.SerializeObject(new
            {
                ResponseMessage = "[BundleNotFound]",
                ResponseCode = "E02",
                InnerResponseDescription = "Contact Admin."
            });

            return PrepareResponse(res, client);
        }

        private static string HandlePause(HttpListenerContext client, string body)
        {
            var id = client.Request.Url.Segments[5].Replace("/", "");
            var bundle = Camel.RouteCollection.FirstOrDefault(c => c.Value.BundleInfo != null && c.Value.BundleInfo.GuidData == id);

            if (!bundle.IsNull())
            {
                bundle.Value.BundleInfo.BundleStatus = BundleDescriptorObject.Status.Stopped;
                var result = JsonConvert.SerializeObject(new
                {
                    ResponseMessage = "[BundleUpdateComplete]",
                    ResponseCode = "90000",
                    InnerResponseDescription = "Stopped"
                });
                return PrepareResponse(result, client);
            }

            var res = JsonConvert.SerializeObject(new
            {
                ResponseMessage = "[BundleNotFound]",
                ResponseCode = "E02",
                InnerResponseDescription = "Contact Admin."
            });

            return PrepareResponse(res, client);
        }

        private static string HandleStatus(HttpListenerContext client, string body)
        {
            
            var details = Camel.RouteCollection.Select(c => new
            {
                Author = c.Value.BundleInfo.Author,
                GroupId = c.Value.BundleInfo.GroupId,
                GuidData = c.Value.BundleInfo.GuidData,
                Model = c.Value.BundleInfo.ModelVersion,
                Name = c.Value.BundleInfo.Name,
                Priority = c.Value.BundleInfo.Priority,
                BundleState = c.Value.BundleInfo.BundleStatus.ToString()
            }).Distinct();

            var res = JsonConvert.SerializeObject(new
            {
                ResponseCode = "90000",
                ResponseMessage = "",
                Routes = details
            });

            return PrepareResponse(res, client);
        }

        private static string PrepareResponse(string res, HttpListenerContext client)
        {
            var callback = client.Request.QueryString["callback"];
            return !string.IsNullOrEmpty(callback) ? string.Format("{0}({1})", callback, res) : res;
        }
    }
}
