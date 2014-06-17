namespace Balancer.Common.Packet.Packets
{
    internal interface IPacket
    {
        uint RegionId { get; set; }
        uint ClientId { get; set; }
        Packet GetPacket();
    }
}