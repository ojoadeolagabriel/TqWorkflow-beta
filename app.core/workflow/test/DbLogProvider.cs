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
    public class DbLogProvider : ISystemLogProvider
    {
        public string ConnectionString { get; set; }


        private void InitLog(Exchange exchange, string processorType, string componentName)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("logMessage", conn);

                cmd.Parameters.AddWithValue("@exhangeid", exchange.ExchangeId);
                cmd.Parameters.AddWithValue("@body", exchange.InMessage.Body.ToString());
                cmd.Parameters.AddWithValue("@processorType", processorType);
                cmd.Parameters.AddWithValue("@component", componentName);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

            }
        }

        public void Log(Exchange exchange, string processorType, string componentName, Exception exception = null)
        {
            try
            {
                System.Threading.Tasks.Task.Factory.StartNew(() => InitLog(exchange, processorType, componentName));
            }
            catch (Exception)
            {

            }
        }
    }
}
