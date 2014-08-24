namespace Balancer.Common.Logger.Interfaces
{
    /// <summary>
    ///     Интерфейс писателя логов.
    /// </summary>
    public interface IWriter
    {
        void Write(string filePath, string[] data);
    }
}