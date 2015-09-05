using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using app.core.nerve;
using app.core.nerve.utility;

namespace camelcontext.server
{
    public class StartServer
    {
        static void Main(string[] args)
        {
            Console.Write("..starting camel context");
            Camel.LoadCamelContext(CamelFilePath);
            Console.WriteLine(" ...ready!");
            
            Console.ReadLine();
        }

        private static List<string> CamelFilePath
        {
            get
            {
                var config = new Configuration();
                var directFile = string.Format("{0}\\camel.xml", config.ApplicationConfigRootFolderPath);

                var paths = new List<string> {directFile};
                return paths;
            }
        }
    }
}
