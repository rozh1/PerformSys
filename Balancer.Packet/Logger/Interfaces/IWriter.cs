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
