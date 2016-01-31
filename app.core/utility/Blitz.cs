using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using app.core.utility.reflectivity;

namespace app.core.utility
{
    public class BlitzForignKeyAttribute : Attribute
    {
        public string ColumnId { get; set; }
        public Type ForeignType { get; set; }
    }

    public class BlitzTableMap : Attribute
    {
        public string Table { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class Blitz<TEntity, TId>
        where TEntity : class, new()
    {

        public string SpPrefix = "";

        public TId Id { get; private set; }

        public static Blitz<TEntity, TId> StartSession(string dataSource, string spPrefix = "usp_")
        {
            return new Blitz<TEntity, TId> { DataSource = dataSource, SpPrefix = spPrefix } ;
        }

        public string GetTableName(Type t)
        {
            var attr = ReflectorSimple.GetAttribute<BlitzTableMap>(t);
            return attr == null ? t.Name : attr.Table;
        }

        /// <summary>
        /// Execute Scalar
        /// </summary>
        /// <typeparam name="TScalarResult"></typeparam>
        /// <param name="sp"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public TScalarResult ExecuteScalar<TScalarResult>(string sp, List<SqlParameter> parameters = null)
        {
            try
            {
                var connStr = ConfigurationManager.ConnectionStrings[DataSource].ConnectionString;
                using (var conn = new SqlConnection(connStr))
                {
                    var command = new SqlCommand(sp) { CommandType = CommandType.StoredProcedure, Connection = conn };
                    if (parameters != null)
                        command.Parameters.AddRange(parameters.ToArray());
                    conn.Open();

                    var result = command.ExecuteScalar();
                    return (TScalarResult)Convert.ChangeType(result, typeof(TScalarResult));
                }
            }
            catch (Exception exception)
            {
                return default(TScalarResult);
            }
        }

        public void ExecuteNonQuery(string sp, List<SqlParameter> parameters = null)
        {
            try
            {
                var connStr = ConfigurationManager.ConnectionStrings[DataSource].ConnectionString;
                using (var conn = new SqlConnection(connStr))
                {
                    var command = new SqlCommand(sp) { CommandType = CommandType.StoredProcedure, Connection = conn };
                    if (parameters != null)
                        command.Parameters.AddRange(parameters.ToArray());
                    conn.Open();

                    var result = command.ExecuteScalar();
                }
            }
            catch (Exception exception)
            {
            }
        }

        public TEntity ExecuteUniqueStoreProcedure(string sp, List<SqlParameter> parameters = null, Type resultType = null)
        {
            return ExecuteUniqueSp(sp, parameters, resultType) as TEntity;
        }

        public List<TEntity> ExecuteStoreProcedure(string sp, List<SqlParameter> parameters = null, Type resultType = null)
        {
            return ExecuteSp(sp, parameters, resultType);
        }



        private object ExecuteUniqueSp(string sp, List<SqlParameter> parameters = null, Type resultType = null)
        {
            try
            {
                var connStr = ConfigurationManager.ConnectionStrings[DataSource].ConnectionString;
                using (var conn = new SqlConnection(connStr))
                {
                    var command = new SqlCommand(sp) { CommandType = CommandType.StoredProcedure, Connection = conn };
                    if (parameters != null) command.Parameters.AddRange(parameters.ToArray());
                    conn.Open();

                    var reader = command.ExecuteReader();
                    if (!reader.HasRows)
                        return null;

                    var result = resultType != null ? Activator.CreateInstance(resultType) : Activator.CreateInstance(typeof(TEntity));
                    while (reader.Read())
                    {
                        var propertys = result.GetType().GetProperties();
                        foreach (var propertyInfo in propertys)
                        {
                            try
                            {
                                var attr = ReflectorSimple.GetAttribute<BlitzForignKeyAttribute>(propertyInfo);
                                if (attr == null)
                                {
                                    var res = reader[propertyInfo.Name];
                                    propertyInfo.SetValue(result, Convert.ChangeType(res, propertyInfo.PropertyType), null);
                                }
                                else
                                {
                                    var res = reader[attr.ColumnId];
                                    var subRes = ExecuteUniqueSp(string.Format("{0}{1}_GetById", SpPrefix, GetTableName(attr.ForeignType)), new List<SqlParameter> { new SqlParameter("Id", res) }, attr.ForeignType);
                                    propertyInfo.SetValue(result, subRes, null);
                                }
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

        private List<TEntity> ExecuteSp(string sp, List<SqlParameter> parameters = null, Type resultType = null)
        {
            try
            {
                var connStr = ConfigurationManager.ConnectionStrings[DataSource].ConnectionString;
                using (var conn = new SqlConnection(connStr))
                {
                    var command = new SqlCommand(sp) { CommandType = CommandType.StoredProcedure, Connection = conn };
                    if (parameters != null) command.Parameters.AddRange(parameters.ToArray());
                    conn.Open();

                    var reader = command.ExecuteReader();
                    if (!reader.HasRows)
                        return null;

                    var collResult = new List<TEntity>();

                    while (reader.Read())
                    {
                        var result = resultType != null ? Activator.CreateInstance(resultType) : Activator.CreateInstance(typeof(List<TEntity>));
                        var propertys = result.GetType().GetProperties();

                        foreach (var propertyInfo in propertys)
                        {
                            try
                            {
                                var attr = ReflectorSimple.GetAttribute<BlitzForignKeyAttribute>(propertyInfo);
                                if (attr == null)
                                {
                                    var res = reader[propertyInfo.Name];
                                    propertyInfo.SetValue(result, Convert.ChangeType(res, propertyInfo.PropertyType), null);
                                }
                                else
                                {
                                    var res = reader[attr.ColumnId];
                                    var subRes = ExecuteUniqueSp(string.Format("{0}{1}_GetById", SpPrefix, GetTableName(attr.ForeignType)), new List<SqlParameter> { new SqlParameter("Id", res) }, attr.ForeignType);
                                    propertyInfo.SetValue(result, subRes, null);
                                }
                            }
                            catch
                            {

                            }
                        }
                        collResult.Add(result as TEntity);
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
