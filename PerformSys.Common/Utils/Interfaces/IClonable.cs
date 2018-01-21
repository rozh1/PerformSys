namespace PerformSys.Common.Utils.Interfaces
{
    public interface ICloneable<out T>
    {
        T Clone();
    }
}
