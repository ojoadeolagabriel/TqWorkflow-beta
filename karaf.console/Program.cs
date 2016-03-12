using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using app.core.security.onetime;
using app.core.utility;
using camelcontext.server.facade.util;

namespace karaf.console
{
    class Program
    {
        [BlitzTableMap(Table = "activitylogdetail")]
        public class LogDetail : Blitz<LogDetail, int>
        {
            public string ExtraInfoMessage { get; set; }
        }

        [BlitzTableMap(Table = "activitylogger")]
        public class ActivityLogger : Blitz<ActivityLogger, long>
        {
            public string Body { get; set; }
            public string ExchangeId { get; set; }

            [BlitzForignKey(ForeignType = typeof(LogDetail), ColumnId = "DetailId")]
            public LogDetail Detail { get; set; }

            public void Read()
            {
                var ress = ExecuteUniqueStoreProcedure("[dbo].[usp_FetchLog]", new List<SqlParameter> { new SqlParameter("@Id", 1) });
            }
        }

        public class DataStore
        {
            public static Blitz<ActivityLogger, long> ActionLogger
            {
                get { return ActivityLogger.StartSession("DefaultConnection"); }
            }
        }

        static void Main(string[] args)
        {
            //var processCount = DataStore.ActionLogger.ExecuteScalar<long>("[dbo].[usp_process_count]");
            //var res = DataStore.ActionLogger.ExecuteUniqueStoreProcedure("[dbo].[usp_FetchLog]", new List<SqlParameter> { new SqlParameter("@Id", 1) });

            Thread.Sleep(1000);

            var startDateTime = DateTime.Now;
            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine("sent " + i);
                var res = new Poster().Post(i);
            }
            var elapaed = DateTime.Now - startDateTime;
            Console.WriteLine(elapaed + " passed");
            Console.Read();
        }

        public class Poster
        {
            readonly Dictionary<int, HttpClient> _clients = new Dictionary<int, HttpClient>(); 

            public async Task<bool> Post(int i)
            {
                var client = new HttpClient();
                _clients.Add(i, client);

                var content = new StringContent(@"{'CbnCode':null,'EncryptedPin':null,'DestAccount':null,'SourceAccount':null,'Amount':45.78}", Encoding.UTF8);
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri("http://127.0.0.1:4554/autopay.transactor/api2"),
                    Method = HttpMethod.Post,
                    Content = content
                };
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                request.Headers.Add("counter", i.ToString());
                var task = await client.SendAsync(request);
                return true;
            }

        }
    }
}
