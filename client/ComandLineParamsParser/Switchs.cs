namespace client.ComandLineParamsParser
{
    /// <summary>
    ///     Параметры запуска
    /// </summary>
    internal enum ComandSwitch
    {
        None,
        ClientCount,
        QueryPerClient,
        BalancerHost,
        BalancerPort,
        LogName,
        CsvLogName,
        LogDir,
        WriteLogToConsole
    }

    /// <summary>
    ///     Ключи параметров
    /// </summary>
    internal static class Switchs
    {
        public static ComandSwitch Parse(string input)
        {
            switch (input)
            {
                case "-c":
                case "--clients":
                    return ComandSwitch.ClientCount;
                case "-q":
                case "--queries":
                    return ComandSwitch.QueryPerClient;
                case "-h":
                case "--host":
                    return ComandSwitch.BalancerHost;
                case "-p":
                case "--port":
                    return ComandSwitch.BalancerPort;
                case "--log":
                    return ComandSwitch.LogName;
                case "--csv":
                    return ComandSwitch.CsvLogName;
                case "--logdir":
                    return ComandSwitch.LogDir;
                case "--log-to-console":
                    return ComandSwitch.WriteLogToConsole;
                default:
                    return ComandSwitch.None;
            }
            return ComandSwitch.None;
        }
    }
}