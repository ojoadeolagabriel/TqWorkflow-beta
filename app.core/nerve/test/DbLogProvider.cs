using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using app.core.nerve.dto;

namespace app.core.nerve.test
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
                cmd.Parameters.AddWithValue("@body", exchange.InMessage.Body.ToString() == "System.Object" ? "[Empty]" : exchange.InMessage.Body.ToString());
                cmd.Parameters.AddWithValue("@processorType", processorType);
                cmd.Parameters.AddWithValue("@component", componentName);
                cmd.Parameters.AddWithValue("@routeId", exchange.Route.RouteId);
                var builder = new StringBuilder();
                if (exchange.CurrentException != null)
                {
                    builder.AppendFormat("error message: {0}, stack trace: {1}", exchange.CurrentException.Message,
                        exchange.CurrentException.StackTrace);
                }

                cmd.Parameters.AddWithValue("@errorMessage", builder.ToString());
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
