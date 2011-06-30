using System;

namespace TableComparator
{
    [Serializable]
    public class Config
    {
        public string host;
        public uint port;
        public string userName;
        public string password;
        public string dataBase;
        public uint connectionTimeOut;

        public Config()
        {
            host = "localhost";
            port = 3306;
            userName = password = dataBase = "mangos";
            connectionTimeOut = 12000;
        }
    }
}
