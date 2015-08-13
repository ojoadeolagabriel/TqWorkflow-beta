using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using app.core.workflow;
using app.core.workflow.handlers.routepipeline;
using TqWorkflow.Shared.Facade;

namespace TestKit
{
    public static class ReflectionHelper
    {
        public const string Data = "The_OtherGirl!";


        public static string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            switch (propertyLambda.Body.NodeType)
            {
                case ExpressionType.Constant:
                    return ((ConstantExpression)propertyLambda.Body).Value.ToString();
                    break;
                case ExpressionType.MemberAccess:
                    break;
            }
            var me = propertyLambda.Body as MemberExpression;

            if (me == null)
            {
                throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
            }

            return me.Member.Name;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var enctoken = TokenManager.CreateJwtToken("aojo", "admin");
            var token = enctoken;
            const string secretKey = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";

            try
            {
                var jsonPayload = JWT.JsonWebToken.Decode(token, secretKey);
                Console.WriteLine(jsonPayload);
            }
            catch (JWT.SignatureVerificationException)
            {
                Console.WriteLine("Invalid token!");
            }

            const string camelFile = @"C:\Users\Adeola Ojo\Documents\Visual Studio 2013\Projects\TqWorkflow\TqWorkflow\app.core\workflow\resource\core-route.xml";
            //const string altFile = @"C:\Users\Adeola Ojo\Documents\Visual Studio 2013\Projects\TqWorkflow\TqWorkflow\app.core\workflow\resource\alt-route.xml";
            Camel.InitDependencyLibs(new List<string> { "app.core.workflow.component.core" });

            RoutePipelineEngine.Initialize(camelFile);
            //RoutePipelineEngine.Initialize(altFile);

            Camel.StartAllRoutes();

            var startTime = DateTime.Now;

            Thread.Sleep(2000);
            for (var i = 0; i < 1; i++)
            {
                Task.Factory.StartNew(Exec);
                Console.WriteLine("Response [{0}], received @ {1}", i, DateTime.Now);
            }

            var interval = DateTime.Now - startTime;
            Console.WriteLine("Total time: {0}", interval);
            Console.ReadLine();
        }
        static readonly List<string> Resp = new List<string>();

        private static void Exec()
        {
            try
            {
                var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(IPAddress.Parse("127.0.0.1"), 9800);
                client.ReceiveTimeout = 10000;

                const string msg = @"<request><claim-type>90000</claim-type><claim>Success</claim></request>\r\n";
                var buffer = Encoding.ASCII.GetBytes(msg);
                client.Send(buffer);

                int totalBytes;
                var readbuffer = new byte[1];
                var message = "";

                while ((totalBytes = client.Receive(readbuffer, 0, readbuffer.Length, SocketFlags.None)) > 0)
                {
                    var data = Encoding.ASCII.GetString(readbuffer, 0, totalBytes);
                    message = message + data;

                    if (message.EndsWith(Environment.NewLine) || message.EndsWith("\\r\\n"))
                        break;

                    if (!client.Connected)
                    {

                    }
                }

                Resp.Add(message);
            }
            catch (AggregateException exception)
            {
                Console.WriteLine("[AggregateException]: {0}", exception.Message);
            }
            catch (Exception exc)
            {
                Console.WriteLine("[Exception]: {0}", exc.Message);
            }
        }
    }

    public class TokenManager
    {
        public static double EpocTime
        {
            get
            {
                var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var now = Math.Round((DateTime.UtcNow - unixEpoch).TotalSeconds);
                return now;
            }
        }

        public static string CreateJwtToken(string username, string role)
        {
            var payload = new Dictionary<string, object>()
            {
                { "username", username },
                { "role", role },
                { "iat" , EpocTime },
                //{ "exp" , EpocTime },
                { "nbf" , EpocTime - 10000 },
            };
            var secretKey = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
            string token = JWT.JsonWebToken.Encode(payload, secretKey, JWT.JwtHashAlgorithm.HS256);
            Console.WriteLine(token);

            return token;
        }
    }
}
