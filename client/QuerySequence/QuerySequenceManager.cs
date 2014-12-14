using System;
using client.Config;
using client.Config.Data;

namespace client.QuerySequence
{
    internal class QuerySequenceManager
    {
        private readonly ClientConfig _config;

        public QuerySequenceManager(ClientConfig config)
        {
            _config = config;
        }

        public IQuerySequence GetQuerySequence(int startQueryNumber, int clientNumber, int queriesCount)
        {
            switch (_config.QuerySequence.Mode)
            {
                case QuerySequenceMode.Sequential:
                    return new SequentialQuerySequence(queriesCount, startQueryNumber);
                case QuerySequenceMode.Random:
                    return new RandomSequentialQuerySequence(queriesCount);
                case QuerySequenceMode.FromList:
                    return new ListQuerySequence(_config.QuerySequence.List, clientNumber, _config.Scenario.ClientCount);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}