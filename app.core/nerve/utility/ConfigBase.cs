using System;
using System.Configuration;
using System.Globalization;
using System.Reflection;

namespace app.core.nerve.utility
{
    /// <summary>
    /// Config Base
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConfigBase<T>
    {
        /// <summary>
        /// Load
        /// </summary>
        /// <returns></returns>
        public void Load()
        {
            var typeData = this.GetType();
            var properties = typeData.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var propertyInfo in properties)
            {
                try
                {
                    var propValue = ConfigurationManager.AppSettings[propertyInfo.Name];
                    var convertedType = Convert.ChangeType(propValue, propertyInfo.PropertyType);
                    propertyInfo.SetValue(this, convertedType, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, CultureInfo.CurrentCulture);
                }
                catch (Exception)
                {
                }            
            }
        }
    }
}
