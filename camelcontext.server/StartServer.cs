using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using app.core.data;
using app.core.nerve;
using app.core.nerve.dto;
using app.core.nerve.utility;
using app.core.utility;
using camelcontext.server.facade.util;

namespace camelcontext.server
{
    public class StartServer
    {
        static void Main(string[] args)
        {
            Camel.LoadBundle(new List<string>{ @"C:\Users\Adeola Ojo\Documents\Visual Studio 2013\TqWorkflow-beta\tcp.consumer.bundle\bin\Debug\tcp.consumer.bundle.dll" });
            Camel.LoadBundle(new List<string> { @"C:\Users\Adeola Ojo\Documents\Visual Studio 2013\TqWorkflow-beta\mina.nibss.consumer.bunde\bin\Debug\mina.nibss.consumer.bundle.dll" });
        
            Camel.StartEngine();
            Console.ReadLine();
        }

        /// <summary>
        /// Get Resource Text File
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public string GetResourceTextFile(string resource, string filename)
        {
            string result;

            var assembly = Assembly.LoadFile(filename);
            using (var stream = assembly.GetManifestResourceStream(resource))
            {
                if (stream == null) 
                    return null;
                using (var sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        private static List<string> CamelFilePath
        {
            get
            {
                var config = new Configuration();
                var directFile = string.Format("{0}\\camel.xml", config.ApplicationConfigRootFolderPath);

                var paths = new List<string> { directFile };
                return paths;
            }
        }
    }
}
