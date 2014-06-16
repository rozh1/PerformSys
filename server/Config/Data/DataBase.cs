namespace server.Config.Data
{
    public class DataBase
    {
        public DataBaseType DataBaseType { get; set; }
        public string Host { get; set; }
        public uint Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DataBaseName { get; set; }
    }
}