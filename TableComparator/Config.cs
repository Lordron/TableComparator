using System;

namespace TableComparator
{
    [Serializable]
    public class Config
    {
        public string Host;
        public uint Port;
        public string UserName;
        public string Password;
        public string DataBase;
        public uint ConnectionTimeOut;

        public Config()
        {
            Host = "localhost";
            Port = 3306;
            UserName = Password = DataBase = "mangos";
            ConnectionTimeOut = 12000;
        }
    }
}
