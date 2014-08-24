using Balancer.Common.Logger.Enums;
using Balancer.Common.Logger.Helpers;
using Balancer.Common.Logger.Helpers.WriterHelpers;
using Balancer.Common.Logger.Interfaces;

namespace Balancer.Common.Logger
{
    /// <summary>
    ///     Базовый класс писателя.
    /// </summary>
    public class Writer : IWriter
    {
        /// <summary>
        ///     Тип файла.
        /// </summary>
        private FileType _fileType;

        /// <summary>
        ///     Метод записи лога.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="data">Данные.</param>
        public virtual void Write(string filePath, string[] data)
        {
            _fileType = CommonHelpers.DefineTypeFromPath(filePath);

            switch (_fileType)
            {
                case FileType.TXT:
                {
                    WriteTxt(filePath, data);
                    break;
                }
                case FileType.CSV:
                {
                    WriteCsv(filePath, data);
                    break;
                }
            }
        }

        /// <summary>
        ///     Метод записи лога в txt-файл.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="data">Данные.</param>
        private void WriteTxt(string filePath, string[] data)
        {
            TxtWriterHelper.TxtWrite(filePath, data);
        }

        /// <summary>
        ///     Метод записи лога в csv-файл.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="data">Данные.</param>
        private void WriteCsv(string filePath, string[] data)
        {
            CsvWriterHelper.CsvWrite(filePath, data);
        }
    }
}