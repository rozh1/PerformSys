using System;
using System.IO;
using System.Threading;

namespace PerformSys.Common.Logger.Helpers.WriterHelpers
{
    public static class TxtWriterHelper
    {
        /// <summary>
        ///     Мьютекс для синхронизации записи лога в файл.
        /// </summary>
        private static readonly Mutex Mut = new Mutex();

        /// <summary>
        ///     Метод записи лога в .txt файл
        /// </summary>
        /// <param name="filePath">
        ///     Путь к .txt файлу.param>
        ///     <param name="data">Данные лога.</param>
        public static void TxtWrite(string filePath, string[] data)
        {
            Mut.WaitOne();

            foreach (string item in data)
            {
                File.AppendAllText(filePath,
                    @"[" + DateTime.Now.ToLongDateString() + @" " + DateTime.Now.ToLongTimeString() + @"] " + item + @"" + Environment.NewLine);
            }

            Mut.ReleaseMutex();
        }
    }
}