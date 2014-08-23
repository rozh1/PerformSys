using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalancerLogger.Interfaces
{
    /// <summary>
    /// Интерфейс писателя логов.
    /// </summary>
    public interface IWriter
    {
        void Write(string filePath, string[] data);
    }
}
