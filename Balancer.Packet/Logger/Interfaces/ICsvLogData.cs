namespace Balancer.Common.Logger.Interfaces
{
    /// <summary>
    ///     Интерфейс формата данных логировщика .
    /// </summary>
    public interface ICsvLogData : ILogData
    {
        string[] DataColumnNames { get; }
    }
}