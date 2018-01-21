using System;
using System.IO;
using PerformSys.Common.Logger.Enums;
using PerformSys.Common.Logger.Helpers;
using PerformSys.Common.Logger.Interfaces;

namespace PerformSys.Common.Logger
{
    /// <summary>
    ///     Класс логировщика.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        ///     Писатель.
        /// </summary>
        private static readonly IWriter _writer;

        /// <summary>
        ///     Конструктор.
        /// </summary>
        static Logger()
        {
            _writer = new Writer();

            LogLevel = LogLevel.DEBUG;
            LogMode = LogMode.MULTIPLE;
            LogDir = Environment.CurrentDirectory;
            WriteToConsole = true;
        }

        /// <summary>
        ///     Уровень логирования.
        /// </summary>
        public static LogLevel LogLevel { get; set; }

        /// <summary>
        ///     Тип логирования.
        /// </summary>
        public static LogMode LogMode { get; set; }

        /// <summary>
        ///     Директория логирования.
        /// </summary>
        public static string LogDir { get; set; }

        /// <summary>
        ///     Выводить в консоль
        /// </summary>
        public static bool WriteToConsole { get; set; }

        public static void Configure(ILogConfig config)
        {
            LogLevel = config.LogLevel;
            LogMode = config.LogMode;
            LogDir = string.IsNullOrEmpty(config.LogDir) ? Environment.CurrentDirectory : config.LogDir;
            WriteToConsole = config.WriteToConsole;
        }

        /// <summary>
        ///     Метод записи лога.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <param name="concreteDataType">Писатель.</param>
        /// <param name="logLevel">Уровень логирования.</param>
        public static void Write(string fileName, ILogData concreteDataType, LogLevel logLevel)
        {
            bool needWriteToConsole = WriteToConsole;
            string filePath = CommonHelpers.GetAbsoluteFilePath(LogDir, fileName);
            CommonHelpers.CreateIfNotExistDirectory(filePath);

            if (concreteDataType is ICsvLogData)
            {
                if (!File.Exists(filePath))
                {
                    _writer.Write(filePath, ((ICsvLogData) concreteDataType).DataColumnNames);
                }
                needWriteToConsole = false;
            }

            if (needWriteToConsole) Console.WriteLine(concreteDataType.DataParams[0]);

            if (logLevel >= LogLevel && LogMode == LogMode.MULTIPLE)
            {
                _writer.Write(filePath, concreteDataType.DataParams);
            }

            if (logLevel == LogLevel && LogMode == LogMode.SINGLE)
            {
                _writer.Write(filePath, concreteDataType.DataParams);
            }
        }
    }
}