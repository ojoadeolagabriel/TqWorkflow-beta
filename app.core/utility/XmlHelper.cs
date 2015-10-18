using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace app.core.utility
{
    public class XmlHelper
    {
        public static T GetValue<T>(XElement element, string tag, T defaultValue = default (T), string parentTag = null)
        {
            try
            {
                if (parentTag == null)
                {
                    return (T)Convert.ChangeType(element.Element(tag).Value, typeof(T));
                }
                return (T)Convert.ChangeType(element.Element(parentTag).Element(tag).Value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
