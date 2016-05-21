using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace app.core.utility
{
    public class SqlDataReaderUtil
    {
        public static T Read<T>(string key, SqlDataReader reader, T defaultVal = default (T), bool throwOnError = false)
        {
            try
            {
                return (T)Convert.ChangeType(reader[key], typeof(T));
            }
            catch (Exception exception)
            {
                if (throwOnError)
                    throw new Exception(string.Format("Error reading field : [{0}]", key), exception);
            }

            return defaultVal;
        }
    }
}
