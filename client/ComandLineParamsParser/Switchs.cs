namespace client.ComandLineParamsParser
{
    /// <summary>
    ///     Параметры запуска
    /// </summary>
    internal enum ComandSwitch
    {
        None,
        BalancerHost,
        BalancerPort,
        LogName,
        CsvLogName,
        LogDir,
        WriteLogToConsole,
        ScenarioFile,
        DefaultScenario
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
                case "-s":
                case "--scenario":
                    return ComandSwitch.ScenarioFile;
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
                case "--default-scenario":
                    return ComandSwitch.DefaultScenario;
                default:
                    return ComandSwitch.None;
            }
            return ComandSwitch.None;
        }
    }
}