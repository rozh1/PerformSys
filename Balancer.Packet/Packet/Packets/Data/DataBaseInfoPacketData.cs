﻿using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Balancer.Common.Packet.Packets.Data
{
    [ProtoContract]
    public class DataBaseInfoPacketData : PacketBase
    {
        [ProtoMember(1)]
        public Dictionary<string, UInt64> TableSizes { get; set; }
    }
}