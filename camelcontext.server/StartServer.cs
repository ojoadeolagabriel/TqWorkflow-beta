using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using app.core.data;
using app.core.data.LightFoot;
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
        public static void Test()
        {
            var dao = new LightFootDao<UserDto>("app.core");
            var result = dao.ExecuteUniqueSp("uspGetUserById", new Dictionary<string, string> {{"userid", "1"}});
        }

        static void Main(string[] args)
        {
            Test();
            const string rootPath = @"C:\Users\AdeolaOjo\Documents\workflow";
            Camel.LoadBundle(new List<string> { string.Format(@"{0}\ClassLibrary1\bin\Debug\paymentnotification.generic.bundle.dll", rootPath) });
        
            Camel.StartEngine();
            SystemBundleManagement.StartService("http://127.0.0.1:7098/api/v2/app.management.api/");

            Console.ReadLine();
        }
    }
}
