using System.Collections.Generic;
using System.Linq;
using mrbn.GlobalBalancer.Data;

namespace mrbn.GlobalBalancer
{
    class GlobalBalancer
    {
        private readonly List<RBN> _rbns;

        private readonly object _rbnsSyncObject;

        public GlobalBalancer()
        {
            _rbnsSyncObject = new object();
            _rbns = new List<RBN>();
        }

        public bool AddRbn(RBN rbn)
        {
            bool result = false;

            lock (_rbnsSyncObject)
            {
                RBN existRbn = _rbns.FirstOrDefault(curRbn => curRbn.RegionId == rbn.RegionId);
                if (existRbn == null)
                {
                    _rbns.Add(rbn);
                    result = true;
                }
            }

            return result;
        }

        public void Remove(RBN rbn)
        {
            lock (_rbnsSyncObject)
            {
                if (_rbns.Contains(rbn))
                {
                    _rbns.Remove(rbn);
                }
            }
        }
    }
}
