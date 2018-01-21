﻿using System;
using System.IO;
using System.Text;
using PerformSys.Common.Logger;
using PerformSys.Common.Logger.Data;
using PerformSys.Common.Logger.Enums;
using rbn.Config;
using rbn.Properties;

namespace rbn
{
    internal static class Program
    {
        private static void Main()
        {
            const string configFilePath = "rbnConfig.xml";
            if (!File.Exists(configFilePath))
            {
                (new ConfigInit()).Config.Save(configFilePath);
            }
            RBNConfig.Load(configFilePath);

            Logger.Configure(RBNConfig.Instance.Log);
            Logger.Write(RBNConfig.Instance.Log.LogFile,
                new StringLogData("Сервер запущен"), 
                LogLevel.INFO);
            
            new Server((int)RBNConfig.Instance.RBN.Port);
        }
    }
}