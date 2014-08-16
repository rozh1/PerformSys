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

        public void ConnectRbns()
        {
            var lowLoadRbn = new RBN { RbnClient = null, RegionId = 0, Weight = 1 };
            var highLoadRbn = new RBN { RbnClient = null, RegionId = 0, Weight = 0 }; 

            lock (_rbnsSyncObject)
            {
                foreach (var rbn in _rbns)
                {
                    if (rbn.Weight > highLoadRbn.Weight) highLoadRbn = rbn;
                    if (rbn.Weight < lowLoadRbn.Weight) lowLoadRbn = rbn;
                }

                if (highLoadRbn != lowLoadRbn)
                {
                    if (highLoadRbn.RegionId > 0)
                    {
                        highLoadRbn.RelayRbn = lowLoadRbn;
                    }
                }
            }
        }

        public RBN GetRbnByRegionId(int id)
        {
            return _rbns.FirstOrDefault(curRbn => curRbn.RegionId == id);
        }

        public RBN[] GetAllRBNs()
        {
            RBN[] rbns;

            lock (_rbnsSyncObject)
            {
                rbns = _rbns.ToArray();
            }

            return rbns;
        }
    }
}
