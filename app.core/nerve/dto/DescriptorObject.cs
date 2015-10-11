using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace app.core.nerve.dto
{
    public class DescriptorObject
    {
        private DescriptorObject()
        {
            
        }

        public static DescriptorObject Init(string data)
        {
            var obj = new DescriptorObject();

            var xmlData = XElement.Parse(data);
            obj.Name = xmlData.Element("name").Value;
            obj.Version = xmlData.Element("version").Value;
            return obj;
        }

        public string Version { get; set; }
        public string Name { get; set; }
    }
}
