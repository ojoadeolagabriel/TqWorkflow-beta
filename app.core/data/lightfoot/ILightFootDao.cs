using System.Collections.Generic;

namespace app.core.data.LightFoot
{
    public interface ILightFootDao<TDto> where TDto : new()
    {
        string Datastore { get; set; }

        /// <summary>
        /// Execute Unique Sp
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="spParams"></param>
        /// <param name="ignoreProps"></param>
        /// <returns></returns>
        TDto ExecuteUniqueSp(string spName, Dictionary<string, string> spParams = null,
            List<string> ignoreProps = null);

        /// <summary>
        /// Execute Sp
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="spParams"></param>
        /// <param name="ignoreProps"></param>
        /// <returns></returns>
        List<TDto> ExecuteSp(string spName, Dictionary<string, string> spParams = null,
            List<string> ignoreProps = null);

        /// <summary>
        /// Execute Non Query
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="spParams"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string spName, Dictionary<string, string> spParams = null);
    }
}