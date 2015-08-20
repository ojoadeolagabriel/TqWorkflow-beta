namespace app.core.nerve.utility
{
    public class Configuration : ConfigBase<Configuration>
    {
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
