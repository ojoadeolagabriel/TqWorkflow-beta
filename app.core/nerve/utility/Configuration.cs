namespace app.core.nerve.utility
{
    public class Configuration : ConfigBase<Configuration>
    {
        public class HotConnection
        {
            public string Data { get; set; }
        }

        public string CronConfig { get; set; }
        public HotConnection HeatConnection { get; set; }
        public string ApplicationConfigRootFolderPath { get; set; }
        public string DefaultLogPathRoot { get; set; }
        public string LocalConnectionString { get; set; }
        public int SocketTimeOut { get; set; }
        public int AmqTestPort { get; set; }
        public int ServerPort { get; set; }
        public int HttpTestPort { get; set; }

        public int FilePollingTime { get; set; }
        public string HttpPollIp { get; set; }
        public string ESBJWTSharedKey { get; set; }
        public int MongoDbPort { get; set; }
        public string ServerPath { get; set; }

        public Configuration()
        {
            Load();
            HeatConnection = new HotConnection { Data = "mDison" };
        }

        public CacheType CacheType { get; set; }
    }


    public class CacheType
    {
        public int TypeId { get; set; }
    }
}
