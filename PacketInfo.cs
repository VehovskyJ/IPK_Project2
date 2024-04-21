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
		Packet = PacketDotNet.Packet.ParsePacket(packetCapture.GetPacket().LinkLayerType,
			packetCapture.GetPacket().Data);

		var ethPacket = (EthernetPacket)Packet;
		
		// Do not extract IP information from ARP packets
		EthernetType = ethPacket.Type;
		if (EthernetType != EthernetType.Arp) {
			var ipPacket = (IPPacket)Packet.Extract<IPPacket>();
			
			// Do not extract Ports from ICMP packets
			Protocol = ipPacket.Protocol;
			if (Protocol != ProtocolType.Icmp && Protocol != ProtocolType.IcmpV6) {
				var tpcPacket = (TcpPacket)Packet.Extract<TcpPacket>();

				SourcePort = tpcPacket.SourcePort;
				DestinationPort = tpcPacket.DestinationPort;
			}
			
			SourceIp = ipPacket.SourceAddress.ToString();
			DestinationIp = ipPacket.DestinationAddress.ToString();
		}
		
		Timestamp = packetCapture.Header.Timeval.Date.ToLocalTime().ToString("s");
		SourceMac = FormatMacAddress(ethPacket.SourceHardwareAddress);
		DestinationMac = FormatMacAddress(ethPacket.DestinationHardwareAddress);
		FrameLength = packetCapture.Data.Length;
	}

	// Print packet information to the console
	public void PrintPacket(int currentPacket, int max) {
		// Print separator
		if (currentPacket > 1) {
			Console.WriteLine();
			Console.WriteLine(new string('-', 74));
			Console.WriteLine();
		}

		if (max > 1) {
			Console.WriteLine($"Packet {currentPacket}/{max}");
		}
		
		Console.WriteLine($"timestamp: {Timestamp}");
		Console.WriteLine($"src MAC: {SourceMac}");
		Console.WriteLine($"dst MAC: {DestinationMac}");
		Console.WriteLine($"frame length: {FrameLength} bytes");
		if (EthernetType != EthernetType.Arp) {
			Console.WriteLine($"src IP: {SourceIp}");
			Console.WriteLine($"dst IP: {DestinationIp}");
			
			if (Protocol != ProtocolType.Icmp && Protocol != ProtocolType.IcmpV6) {
				Console.WriteLine($"src port: {SourcePort}");
				Console.WriteLine($"dst port: {DestinationPort}");
			}
			
			Console.WriteLine($"protocol: {Protocol.ToString()}");
		} else {
			Console.WriteLine($"protocol: {EthernetType.ToString()}");
		}
		
		Console.WriteLine();

		// 0x0000: 5c a6 e6 1a 85 f0 2c c8  1b 0a a7 fe 08 00 45 00  \¦æ..ð,È..§þ..E.
		// 0x0010: 00 ec 5d c3 40 00 3c 06  9b 2e a2 9f 88 ea c0 a8  .ì]Ã@.<...¢..êÀ¨
		if (Packet == null) {
			return;
		}

		var bytes = Packet.Bytes;
		for (int i = 0; i < bytes.Length; i += 16) {
			// Left size
			Console.Write($"0x{i:x4}: ");

			char[] chars = new char[16];
			// 2x 16 per line
			for (int j = 0; j < 16; j++) {
				if (i + j < bytes.Length) {
					Console.Write($"{bytes[i + j]:x2} ");
					// Non-printable characters are replaced with '.'
					char c = (char)bytes[i + j];
					chars[j] = char.IsControl(c) ? '.' : c;
				} else {
					// Fill the last row if it's less than 16 bytes
					Console.Write("   ");
					chars[j] = ' ';
				}

				// Every 8 byted add space
				if (j == 7) {
					Console.Write(" ");
				}
			}

			Console.Write(" ");
			// Right side
			Console.WriteLine(chars);
		}
	}

	// Converts mac address into "00:1b:3f:56:8a:00" format
	private static string FormatMacAddress(PhysicalAddress address) {
		return String.Join(":", address.GetAddressBytes().Select(x => x.ToString("x2")).ToArray());
	}
}