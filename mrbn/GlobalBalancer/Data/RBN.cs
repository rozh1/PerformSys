using System;
using System.Net.Sockets;
using Balancer.Common.Utils.Interfaces;

namespace mrbn.GlobalBalancer.Data
{
    internal class RBN : ICloneable<RBN>
    {
        private double _weight;

        public TcpClient RbnClient { get; set; }
        public uint RegionId { get; set; }
        public uint GlobalId { get; set; }

        public double Weight
        {
            get { return _weight; }
            set
            {
                if (Math.Abs(_weight - value) > 0.001)
                {
                    _weight = value;
                    if (WeightChanged != null)
                    {
                        WeightChanged();
                    }
                }
            }
        }

        public RBN RelayRbn { get; set; }

        public event Action WeightChanged;

        public RBN Clone()
        {
            var ret = new RBN
            {
                Weight = Weight,
                GlobalId = GlobalId,
                RegionId = RegionId,
                RbnClient = RbnClient
            };
            return ret;
        }
    }
}