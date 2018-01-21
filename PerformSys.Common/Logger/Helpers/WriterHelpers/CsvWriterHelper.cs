using System;
using System.IO;
using System.Text;
using System.Threading;

namespace PerformSys.Common.Logger.Helpers.WriterHelpers
{
    /// <summary>
    /// </summary>
    public static class CsvWriterHelper
    {
        /// <summary>
        ///     Мьютекс для синхронизации записи лога в файл.
        /// </summary>
        private static readonly Mutex Mut = new Mutex();

        /// <summary>
        ///     Метод записи лога в .csv файл
        /// </summary>
        /// <param name="filePath">
        ///     Путь к .csv файлу.param>
        ///     <param name="data">Данные лога.</param>
        public static void CsvWrite(string filePath, string[] data)
        {
            Mut.WaitOne();

            string csvLine = string.Join(";", data);
            File.AppendAllText(filePath, csvLine + Environment.NewLine, Encoding.UTF8);

            Mut.ReleaseMutex();
        }
    }
}