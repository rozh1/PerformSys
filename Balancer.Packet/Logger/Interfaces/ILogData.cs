namespace Balancer.Common.Logger.Interfaces
{
    /// <summary>
    ///     Интерфейс формата данных для логировщика.
    /// </summary>
    public interface ILogData
    {
        string[] DataParams { get; }
    }
}