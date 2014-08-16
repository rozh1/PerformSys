namespace Balancer.Common.Logger.Data
{
    public interface ILogStats
    {
        string[] GetCsvParams();
        string[] GetCsvColumnNames();
    }
}