using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using app.core.data;
using app.core.nerve;
using app.core.nerve.utility;
using app.core.utility;
using camelcontext.server.facade.util;

namespace camelcontext.server
{
    public class StartServer
    {
        static void Main(string[] args)
        {
            //new TaskManager().Run();
            //CsvProcessor.ProcessDb();
            //CsvProcessor.ProcessCsv();
            Console.Write("..starting camel context");
            Camel.LoadCamelContext(CamelFilePath);
            Console.WriteLine(" ...ready!");

            Console.ReadLine();
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
