using SharpPcap;

namespace IPK_Project2.Interfaces;

public interface IPacketInfo {
	public void PacketToObject(PacketCapture packetCapture);

	public void PrintPacket(int currentPacket, int max);
}