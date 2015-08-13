using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace app.core.workflow.utility
{
    public class Configuration : ConfigBase<Configuration>
    {
        public int SocketTimeOut { get; set; }
        public int AmqTestPort { get; set; }
        public int ServerPort { get; set; }
        public int HttpTestPort { get; set; }

        public int FilePollingTime { get; set; }
        public string HttpPollIp { get; set; }
        public string ESBJWTSharedKey { get; set; }

        public Configuration()
        {
            Load();
        }

        public CacheType CacheType { get; set; }
    }


    public class CacheType
    {
        public int TypeId { get; set; }
    }
}
