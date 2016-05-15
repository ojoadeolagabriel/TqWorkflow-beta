using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using app.core.data;
using app.core.nerve;
using app.core.nerve.dto;
using app.core.nerve.management;
using app.core.nerve.utility;
using app.core.utility;                                                                                             
using camelcontext.server.facade.util;

namespace camelcontext.server
{
    public class StartServer
    {
        static void Main(string[] args)
        {
            const string rootPath = @"C:\Users\AdeolaOjo\Documents\workflow";
            Camel.LoadBundle(new List<string>{ String.Format(@"{0}\tcp.consumer.bundle\bin\Debug\tcp.consumer.bundle.dll", rootPath) });
            Camel.LoadBundle(new List<string> { string.Format(@"{0}\mina.nibss.consumer.bunde\bin\Debug\mina.nibss.consumer.bundle.dll", rootPath) });
            Camel.LoadBundle(new List<string> { string.Format(@"{0}\ClassLibrary1\bin\Debug\paymentnotification.generic.bundle.dll", rootPath) });
            Camel.LoadBundle(new List<string> { String.Format(@"{0}\autopay.ftp.consumer.bundle\bin\Debug\autopay.ftp.consumer.bundle.dll", rootPath) });
            Camel.LoadBundle(new List<string> { String.Format(@"{0}\softalliance.processor.bundle\bin\Debug\softalliance.processor.bundle.dll", rootPath) });
            Camel.LoadBundle(new List<string> { String.Format(@"{0}\isofield.transformer.bundle\bin\Debug\isofield.transformer.bundle.dll", rootPath) });
            Camel.LoadBundle(new List<string> { String.Format(@"{0}\autopay.transactor.bundle\bin\Debug\autopay.transactor.bundle.dll", rootPath) });
        
            Camel.StartEngine();
            SystemBundleManagement.StartService("http://127.0.0.1:7098/api/v2/app.management.api/");

            Console.ReadLine();
        }
    }
}
