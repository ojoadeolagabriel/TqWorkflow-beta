using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Apache.NMS.ActiveMQ.Threads;
using app.core.workflow.dto;

namespace app.core.workflow.test
{
    public class DbLogProvider : IDbLogProvider
    {
        public string ConnectionString { get; set; }

        public void Log(Exchange exchange, string processorType)
        {
            try
            {
                System.Threading.Tasks.Task.Factory.StartNew(() => InitLog(exchange, processorType));
            }
            catch (Exception)
            {

            }
        }

        private void InitLog(Exchange exchange, string processorType)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("logMessage", conn);

                cmd.Parameters.AddWithValue("@exhangeid", exchange.ExchangeId);
                cmd.Parameters.AddWithValue("@body", exchange.InMessage.Body.ToString());
                cmd.Parameters.AddWithValue("@processorType", processorType);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

            }
        }
    }
}
