using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
            var processCount = DataStore.ActionLogger.ExecuteScalar<long>("[dbo].[usp_process_count]");
            var res = DataStore.ActionLogger.ExecuteUniqueStoreProcedure("[dbo].[usp_FetchLog]", new List<SqlParameter> { new SqlParameter("@Id", 1) });
        }
    }
}
