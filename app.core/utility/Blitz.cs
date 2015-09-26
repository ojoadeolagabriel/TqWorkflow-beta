using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace app.core.utility
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class Blitz<TEntity, TId>
        where TEntity : class, new()
    {
        public static Blitz<TEntity, TId> Init(string dataSource)
        {
            return new Blitz<TEntity, TId>() { DataSource = dataSource };
        }

        public object ExecuteUniqueSp(string sp, List<SqlParameter> parameters)
        {
            try
            {
                var connStr = ConfigurationManager.ConnectionStrings[DataSource].ConnectionString;
                using (var conn = new SqlConnection(connStr))
                {
                    var command = new SqlCommand(sp) { CommandType = CommandType.StoredProcedure, Connection = conn };
                    command.Parameters.AddRange(parameters.ToArray());
                    conn.Open();

                    var reader = command.ExecuteReader();
                    if (!reader.HasRows)
                        return null;

                    var result = Activator.CreateInstance(typeof(TEntity));
                    while (reader.Read())
                    {
                        var propertys = typeof(TEntity).GetProperties();
                        foreach (var propertyInfo in propertys)
                        {
                            try
                            {
                                var res = reader[propertyInfo.Name];
                                propertyInfo.SetValue(result, Convert.ChangeType(res, propertyInfo.PropertyType), null);
                            }
                            catch
                            {

                            }
                        }
                        return result;
                    }
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<TEntity> ExecuteSp(string sp, List<SqlParameter> parameters)
        {
            try
            {
                var connStr = ConfigurationManager.ConnectionStrings[DataSource].ConnectionString;
                using (var conn = new SqlConnection(connStr))
                {
                    var command = new SqlCommand(sp) { CommandType = CommandType.StoredProcedure, Connection = conn };
                    command.Parameters.AddRange(parameters.ToArray());
                    conn.Open();

                    var reader = command.ExecuteReader();
                    if (!reader.HasRows)
                        return null;

                    var collResult = new List<TEntity>();
                    
                    while (reader.Read())
                    {
                        var result = Activator.CreateInstance(typeof(TEntity)) as TEntity;
                        var propertys = typeof(TEntity).GetProperties();
                        foreach (var propertyInfo in propertys)
                        {
                            try
                            {
                                var res = reader[propertyInfo.Name];
                                propertyInfo.SetValue(result, Convert.ChangeType(res, propertyInfo.PropertyType), null);
                            }
                            catch
                            {

                            }
                        }
                        collResult.Add(result);
                    }
                    return collResult;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object Execute(string sp, List<SqlParameter> parameters)
        {
            return null;
        }

        public string DataSource { get; set; }
    }

    public class Entity<TId>
    {
        public TId Id { get; set; }
    }
}
