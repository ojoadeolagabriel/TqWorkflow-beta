using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace app.core.utility.reflectivity
{
    public class ReflectorSimple
    {
        public static T GetAttribute<T>(PropertyInfo info) where T : class
        {
            var attr = info.GetCustomAttributes(typeof (T), false).FirstOrDefault();
            return attr as T;
        }

        public static T GetAttribute<T>(Type info) where T : class
        {
            var attr = info.GetCustomAttributes(typeof(T), false).FirstOrDefault();
            return attr as T;
        }
    }
}
