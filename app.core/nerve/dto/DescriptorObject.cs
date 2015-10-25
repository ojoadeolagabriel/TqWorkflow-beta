using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using app.core.utility;

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
            obj.Name = XmlHelper.GetValue<string>(xmlData, "name");
            obj.ModelVersion = XmlHelper.GetValue<string>(xmlData, "version");
            obj.Author = XmlHelper.GetValue(xmlData, "author", "default.author");
            obj.GroupId = XmlHelper.GetValue(xmlData, "groupid", "com.nerve.group");
            obj.Priority = XmlHelper.GetValue(xmlData, "priority", "low");

            return obj;
        }

        public string Priority { get; set; }

        public string GroupId { get; set; }
        public string Author { get; set; }
        public string ModelVersion { get; set; }
        public string Name { get; set; }
    }
}
