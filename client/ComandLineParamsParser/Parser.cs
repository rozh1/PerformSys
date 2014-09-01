using System;
using System.IO;
using Balancer.Common.Logger.Enums;
using client.Config.Data;
using client.Properties;

namespace client.ComandLineParamsParser
{
    /// <summary>
    ///     Обработчик входных параметров
    /// </summary>
    public class Parser
    {
        private readonly string[] _args;
        private readonly Config.Config _config;

        public Parser(string[] args)
        {
            _config = new Config.Config
            {
                Log = new Log
                {
                    LogFile = "clientLog.txt",
                    LogLevel = LogLevel.DEBUG,
                    LogMode = LogMode.MULTIPLE,
                    WriteToConsole = false,
                    StatsFile = "clientStats.csv"
                }
            };
            _args = args;
        }

        public string ErrorText { get; private set; }

        /// <summary>
        ///     Парсер параметров
        /// </summary>
        private void Parse()
        {
            for (int i = 0; i < _args.Length; i++)
            {
                ComandSwitch comandSwitch = Switchs.Parse(_args[i]);
                switch (comandSwitch)
                {
                    case ComandSwitch.DefaultScenario:
                        _config.ScenarioFile = "defaultScenario.xml";
                        File.WriteAllLines(_config.ScenarioFile, new[] {Resources.defaultScenario});
                        break;
                    case ComandSwitch.ScenarioFile:
                        _config.ScenarioFile = _args[i + 1];
                        break;
                    case ComandSwitch.BalancerHost:
                        _config.BalancerHost = _args[i + 1];
                        break;
                    case ComandSwitch.BalancerPort:
                        _config.BalancerPort = TryParseInt(_args[i + 1]);
                        break;
                    case ComandSwitch.LogName:
                        _config.Log.LogFile = _args[i + 1];
                        break;
                    case ComandSwitch.LogDir:
                        _config.Log.LogDir = _args[i + 1];
                        break;
                    case ComandSwitch.CsvLogName:
                        _config.Log.StatsFile = _args[i + 1];
                        break;
                    case ComandSwitch.WriteLogToConsole:
                        _config.Log.WriteToConsole = true;
                        break;
                }
            }
        }

        private int TryParseInt(string str)
        {
            int output;
            if (int.TryParse(str, out output))
                return output;
            Environment.Exit(-1);
            return 0;
        }

        /// <summary>
        ///     проверка конфигурации
        /// </summary>
        private void CheckConfig()
        {
            string error = "";
            if (_config.ScenarioFile == null) error += "Не указан сценарий!\n";
            if (_config.BalancerHost == string.Empty) error += "Не указан адрес балансировщика!\n";
            if (_config.BalancerPort == null) error += "Не указан порт балансировщика!\n";

            ErrorText = error;
        }

        /// <summary>
        ///     Получение конфигурации из строки параметров
        /// </summary>
        /// <returns></returns>
        public Config.Config GetConfig()
        {
            Parse();
            CheckConfig();
            return _config;
        }
    }
}