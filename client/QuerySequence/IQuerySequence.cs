namespace client.QuerySequence
{
    internal interface IQuerySequence
    {
        int GetNextQueryNumber();
        bool CanGetNextQueryNumber();
    }
}