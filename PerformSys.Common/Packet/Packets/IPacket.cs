namespace PerformSys.Common.Packet.Packets
{
    internal interface IPacket
    {
        uint GlobalId { get; set; }
        uint RegionId { get; set; }
        uint ClientId { get; set; }
        Packet GetPacket();
    }
}