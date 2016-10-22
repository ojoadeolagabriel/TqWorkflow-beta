using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using app.core.nerve;
using app.core.nerve.dto;
using app.core.nerve.management;
using app.core.nerve.utility;
using app.core.utility;
using camelcontext.server.data;
using camelcontext.server.facade.util;

namespace camelcontext.server
{
    public class StartServer
    {
        private static object autopayRootPath;

        public static void Test()
        {

        }

        static void Main(string[] args)
        {
            autopayRootPath = @"C:\Users\AdeolaOjo\Documents\workflow\autopay.mobile.api.bundle\bin\Debug";
            Camel.LoadBundle(new List<string> { string.Format($"{autopayRootPath}\\autopay.mobile.api.bundle.dll") });

            Camel.StartEngine();
            SystemBundleManagement.StartService("http://127.0.0.1:7098/api/v2/app.management.api/");

            Console.ReadLine();
        }
    }
}
