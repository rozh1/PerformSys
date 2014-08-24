using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace BalancerLogger.Helpers.WriterHelpers
{
    public class TxtWriterHelper
    {
        /// <summary>
        /// Мьютекс для синхронизации записи лога в файл.
        /// </summary>
        private static readonly Mutex Mut = new Mutex();

        /// <summary>
        /// Метод записи лога в .txt файл
        /// </summary>
        /// <param name="filePath">Путь к .txt файлу.param>
        /// <param name="data">Данные лога.</param>
        public static void TxtWrite(string filePath, string[] data)
        {
            Mut.WaitOne();

            foreach (string item in data)
            {
                File.AppendAllText(filePath,
                @"[" + DateTime.Now.ToLongDateString() + @" " + DateTime.Now.ToLongTimeString() + @"] " + item + @"");
            }
            
            Mut.ReleaseMutex();
        }
    }
}
