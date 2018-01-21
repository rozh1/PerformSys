using System;
using System.Globalization;
using System.IO;
using PerformSys.Common.Logger.Enums;

namespace PerformSys.Common.Logger.Helpers
{
    public static class CommonHelpers
    {
        /// <summary>
        ///     Метод, определяющий FileType по filePath.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <returns>FileType файла.</returns>
        public static FileType DefineTypeFromPath(string filePath)
        {
            int index = filePath.LastIndexOf('.');
            string exe = filePath.Substring(index + 1, filePath.Length - (index + 1));

            FileType type;

            switch (exe.ToUpper())
            {
                case "TXT":
                {
                    type = FileType.TXT;
                    break;
                }
                case "CSV":
                {
                    type = FileType.CSV;
                    break;
                }
                default:
                {
                    type = FileType.TXT;
                    break;
                }
            }
            return type;
        }

        /// <summary>
        ///     Метод создания директории файла, если директории не суәествует
        /// </summary>
        /// <param name="filePath">Абсолютный путь к файлу.</param>
        public static void CreateIfNotExistDirectory(string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
        }

        /// <summary>
        ///     Метод получения абсолютного пути к файлу.
        /// </summary>
        /// <param name="dir">Директория.</param>
        /// <param name="fileName">Имя файла.</param>
        /// <returns>Абсолютный путь к файлу.</returns>
        public static string GetAbsoluteFilePath(string dir, string fileName)
        {
            if (!String.IsNullOrEmpty(dir) && !String.IsNullOrEmpty(fileName))
            {
                string pathSeparator = Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture);
                if (!dir.EndsWith(pathSeparator))
                {
                    dir = dir + pathSeparator;
                }

                return dir + fileName;
            }
            return null;
        }
    }
}