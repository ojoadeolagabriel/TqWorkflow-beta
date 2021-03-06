﻿using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace app.core.nerve.utility
{
    public class XmlHelper
    {
        public static string Path(string xmlDoc, string path)
        {
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlDoc ?? ""));
            var reader = XmlReader.Create(ms);

            var docNav = new XPathDocument(reader);
            var nav = docNav.CreateNavigator();

            var result = nav.Select(path).Current.Value;
            return "";
        }
    }
}
