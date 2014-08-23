﻿using BalancerLogger.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalancerLogger.Writers
{
    /// <summary>
    /// Класс писателя события Event2.
    /// </summary>
    public class EventWriter2 : BaseWriter, IWriter
    {
        public EventWriter2()
            : base()
        {
        }

        /// <summary>
        /// Метод записи лога.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <param name="data">Данные.</param>
        public void Write(string filePath, string[] data)
        {
            base.Write(filePath,data);
        }
    }
}
