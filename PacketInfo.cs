using System.Net.NetworkInformation;
using PacketDotNet;
using SharpPcap;

namespace IPK_Project2;

public class PacketInfo {
	private string? Timestamp { get; set; }
	private string? SourceMac { get; set; }
	private string? DestinationMac { get; set; }
	private int FrameLength { get; set; }
	private string? SourceIp { get; set; }
	private string? DestinationIp { get; set; }
	private int SourcePort { get; set; }
	private int DestinationPort { get; set; }
	private ProtocolType Protocol { get; set; }
	private EthernetType EthernetType { get; set; }
	private Packet? Packet { get; set; }

	// Convert packet into object
	public void PacketToObject(PacketCapture packetCapture) {
		Packet = Packet.ParsePacket(packetCapture.GetPacket().LinkLayerType,
			packetCapture.GetPacket().Data);

		ParseEthernetPacket();

		if (EthernetType is EthernetType.IPv4 or EthernetType.IPv6) {
			ParseIpPacket();
		}

		Timestamp = packetCapture.Header.Timeval.Date.ToLocalTime().ToString("s");
		FrameLength = packetCapture.Data.Length;
	}

	// Extract information from ethernet frame
	private void ParseEthernetPacket() {
		var ethPacket = (EthernetPacket)Packet!;
		EthernetType = ethPacket.Type;
		SourceMac = FormatMacAddress(ethPacket.SourceHardwareAddress);
		DestinationMac = FormatMacAddress(ethPacket.DestinationHardwareAddress);
	}

	// Extract information from IP packet
	private void ParseIpPacket() {
		var ipPacket = Packet!.Extract<IPPacket>();

		Protocol = ipPacket.Protocol;
		switch (Protocol) {
			case ProtocolType.Tcp: {
				var tpcPacket = Packet.Extract<TcpPacket>();

				SourcePort = tpcPacket.SourcePort;
				DestinationPort = tpcPacket.DestinationPort;
				break;
			}
			case ProtocolType.Udp: {
				var udpPacket = Packet.Extract<UdpPacket>();

				SourcePort = udpPacket.SourcePort;
				DestinationPort = udpPacket.DestinationPort;
				break;
			}
		}

		SourceIp = ipPacket.SourceAddress.ToString();
		DestinationIp = ipPacket.DestinationAddress.ToString();
	}

	// Print the packet to console
	public void PrintPacket(int currentPacket, int max) {
		// Print separator
		if (currentPacket > 1) {
			Console.WriteLine();
			Console.WriteLine(new string('-', 74));
			Console.WriteLine();
		}

		Console.WriteLine($"Packet {currentPacket}/{max}");

		PrintGeneralPacketInformation();

		PrintPacketHexDump();
	}

	private void PrintGeneralPacketInformation() {
		Console.WriteLine($"timestamp: {Timestamp}");
		Console.WriteLine($"src MAC: {SourceMac}");
		Console.WriteLine($"dst MAC: {DestinationMac}");
		Console.WriteLine($"frame length: {FrameLength} bytes");
		if (EthernetType is EthernetType.IPv4 or EthernetType.IPv6) {
			Console.WriteLine($"src IP: {SourceIp}");
			Console.WriteLine($"dst IP: {DestinationIp}");

			if (Protocol is ProtocolType.Tcp or ProtocolType.Udp) {
				Console.WriteLine($"src port: {SourcePort}");
				Console.WriteLine($"dst port: {DestinationPort}");
			}

			Console.WriteLine($"protocol: {Protocol.ToString()}");
		}

		Console.WriteLine($"ethernet type: {EthernetType.ToString()}");
		Console.WriteLine();
	}

	private void PrintPacketHexDump() {
		if (Packet == null) {
			return;
		}

		var bytes = Packet.Bytes;
		for (int i = 0; i < bytes.Length; i += 16) {
			// Left side
			Console.Write($"0x{i:x4}: ");
			char[] chars = FormatHexLine(bytes, i);
			// Right side
			Console.Write(" ");
			Console.WriteLine(chars);
		}
	}

	private char[] FormatHexLine(byte[] bytes, int offset) {
		char[] chars = new char[16];
		// 2x 16 per line
		for (int j = 0; j < 16; j++) {
			chars[j] = FormatByte(bytes, offset + j);
			// Every 8 byted add space
			if (j == 7) {
				Console.Write(" ");
			}
		}

		return chars;
	}

	private char FormatByte(byte[] bytes, int index) {
		if (index < bytes.Length) {
			Console.Write($"{bytes[index]:x2} ");
			// Non-printable characters are replaced with '.'
			char c = (char)bytes[index];
			return char.IsControl(c) ? '.' : c;
		}
		
		// Fill the last row if it's less than 16 bytes
		Console.Write("   ");
		return ' ';
	}

	// Converts mac address into "00:1b:3f:56:8a:00" format
	private static string FormatMacAddress(PhysicalAddress address) {
		return string.Join(":", address.GetAddressBytes().Select(x => x.ToString("x2")).ToArray());
	}
}