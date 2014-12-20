using System;
using client.Config;
using client.Config.Data;

namespace client.QuerySequence
{
    internal class QuerySequenceManager
    {
        private readonly ClientConfig _config;
        private readonly ListQuerySequence _listQuerySequence;

        public QuerySequenceManager(ClientConfig config)
        {
            _config = config;
            _listQuerySequence = new ListQuerySequence(_config.QuerySequence.List);
        }

        public IQuerySequence GetQuerySequence(int startQueryNumber, int queriesCount)
        {
            switch (_config.QuerySequence.Mode)
            {
                case QuerySequenceMode.Sequential:
                    return new SequentialQuerySequence(queriesCount, startQueryNumber);
                case QuerySequenceMode.Random:
                    return new RandomSequentialQuerySequence(queriesCount);
                case QuerySequenceMode.FromList:
                    return _listQuerySequence;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}