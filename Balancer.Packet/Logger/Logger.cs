using System;
using System.IO;
using System.Threading;
using Balancer.Common.Logger.Data;

namespace Balancer.Common.Logger
{
    /// <summary>
    ///     Класс логгера
    /// </summary>
    public class Logger
    {
        private static string _fileName = "log.txt";
        private static string _csvFileName = "log.csv";
        private static int _logLevel = 7;
        private static readonly Mutex Mut = new Mutex();

        /// <summary>
        ///     Записыват сообщение в файл с отметкой времени
        /// </summary>
        /// <param name="message">сообщение</param>
        /// <param name="level">уровень</param>
        public static void Write(string message, int level)
        {
            Mut.WaitOne();
            if (level <= _logLevel)
            {
                //if (!Directory.Exists(Path.GetDirectoryName(_fileName)))
                //    Directory.CreateDirectory(Path.GetDirectoryName(_fileName));
                File.AppendAllText(_fileName,
                    @"[" + DateTime.Now.ToLongDateString() + @" " + DateTime.Now.ToLongTimeString() + @"] " + message +
                    @"
");
            }
            Console.WriteLine(message);
            Mut.ReleaseMutex();
        }

        /// <summary>
        ///     Записыват строку в CSV файл
        /// </summary>
        /// <param name="messages">сообщения</param>
        public static void WriteCsv(params string[] messages)
        {
            Mut.WaitOne();
            string csvLine = string.Join(";", messages);
            File.AppendAllText(_csvFileName, csvLine + Environment.NewLine);
            Mut.ReleaseMutex();
        }

        /// <summary>
        ///     Записыват строку в CSV файл
        /// </summary>
        public static void WriteCsv(ILogStats logStats)
        {
            WriteCsv(logStats.GetCsvParams());
        }

        /// <summary>
        ///     Записыват сообщение в файл с отметкой времени и уровнем 0
        /// </summary>
        /// <param name="message">сообщение</param>
        public static void Write(string message)
        {
            Write(message, 0);
        }

        /// <summary>
        ///     Устанавливает файл для записи
        /// </summary>
        /// <param name="path">путь к файлу</param>
        public static void SetLogFile(string path)
        {
            _fileName = path;
        }

        /// <summary>
        ///     Устанавливает CSV файл для записи
        /// </summary>
        /// <param name="path">путь к файлу</param>
        public static void SetCsvLogFile(string path)
        {
            _csvFileName = path;
        }

        /// <summary>
        ///     Устанавливает уровень лога
        /// </summary>
        /// <param name="level">
        ///     <para>Уровень лога</para>
        ///     <para>7 - debug</para>
        ///     <para>0 - критические ошибки</para>
        /// </param>
        public static void SetLogLevel(int level)
        {
            if (level > 7) level = 7;
            if (level < 0) level = 0;
            _logLevel = level;
        }
    }
}