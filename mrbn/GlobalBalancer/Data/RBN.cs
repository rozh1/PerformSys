using System;
using System.Net.Sockets;
using PerformSys.Common.Utils.Interfaces;

namespace mrbn.GlobalBalancer.Data
{
    internal class RBN : ICloneable<RBN>
    {
        private double _weight;
        private RBN _relayRbn;

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
                    if (RelayRbn != null && (Weight - RelayRbn.Weight) > 0.001)
                    {
                        TransmitRequest();
                    }
                }
            }
        }

        public RBN RelayRbn
        {
            get { return _relayRbn; }
            set
            {
                if (_relayRbn != value)
                {
                    _relayRbn = value;
                    if (RelayRbnChanged != null)
                    {
                        RelayRbnChanged();
                    }
                }
            }
        }

        public event Action WeightChanged;
        public event Action RelayRbnChanged;
        public event Action TransmitRequest;

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