using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using app.core.data.lightfoot;
using app.core.data.lightfoot.contracts;
using app.core.utility;
using Google.Apis.Util;

namespace app.core.data.LightFoot
{
    public class LightFootDao<TDto> : ILightFootDao<TDto> 
        where TDto : Dto, new()
    {
        public string Datastore { get; set; }
        public LightFootDao(string dataStore)
        {
            Datastore = dataStore;
        }

        /// <summary>
        /// Execute Unique Sp
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="spParams"></param>
        /// <param name="ignoreProps"></param>
        /// <returns></returns>
        public TDto ExecuteUniqueSp(string spName, Dictionary<string, string> spParams = null,
            List<string> ignoreProps = null)
        {
            var instance = Activator.CreateInstance<TDto>();

           var connStr =  ConfigurationManager.ConnectionStrings[Datastore].ConnectionString;
            var resultColl = new List<TDto>();
            using (var conn = new SqlConnection(connStr))
            {
                var cmd = new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure };
                conn.Open();

                if (spParams != null && spParams.Count > 0)
                {
                    foreach (var spParam in spParams)
                        cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", spParam.Key), spParam.Value));
                }

                var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (!reader.HasRows)
                    return default(TDto);            

                while (reader.Read())
                {
                    instance = Activator.CreateInstance<TDto>();
                    var propertyColl = instance.GetType().GetProperties();
                    foreach (var prop in propertyColl)
                    {
                        var attr = prop.GetCustomAttribute<LightFootMetadataAttribute>();
                        try
                        {
                            if (prop.PropertyType.GetInterfaces().Contains(typeof(IDto)))
                            {
                                var daoType = Type.GetType("app.core.data.LightFoot.LightFootDao`1");
                                var typeArgs = typeof(Dto);
                                var genericTypeRig = daoType.MakeGenericType(typeArgs);

                                var daoInstance = Activator.CreateInstance(genericTypeRig, Datastore);
                                var sourceTblColumnData = SqlDataReaderUtil.Read<string>(attr.MapsTo, reader);

                                //daoInstance.ExecuteUniqueSp(attr.MapsToSp, new Dictionary<string, string> { { String.Format("@{0}", attr.MapsToSpParam), sourceTblColumnData } });
                            }
                            else
                            {
                                var propName = prop.Name;
                                var columnData = SqlDataReaderUtil.Read<string>(attr == null ? propName : attr.ColumnNameOverride, reader);
                                prop.SetValue(instance, Convert.ChangeType(columnData, prop.PropertyType), null);
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                        }
                    }
                    return instance;
                }
                return instance;
            }
        }

        /// <summary>
        /// Execute Sp
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="spParams"></param>
        /// <param name="ignoreProps"></param>
        /// <returns></returns>
        public List<TDto> ExecuteSp(string spName, Dictionary<string, string> spParams = null,
            List<string> ignoreProps = null)
        {
            return null;
        }

        /// <summary>
        /// Execute Non Query
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="spParams"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string spName, Dictionary<string, string> spParams = null)
        {
            return 0;
        }
    }
}
