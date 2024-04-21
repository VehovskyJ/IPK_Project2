using System.Net.NetworkInformation;
using PacketDotNet;
using SharpPcap;

namespace IPK_Project2;

public static class Sniffer {
	private static int _packetCaptured = 0;
	private static SharpPcap.ILiveDevice? _dev;
	private static Cli _args;

	// Start capturing packets
	public static void Sniff(Cli args) {
		_args = args;
		
		// Get capturing device
		_dev = CaptureDeviceList.Instance.FirstOrDefault(device => device.Name == args.Interface);

		if (_dev == null) {
			throw new Exception($"Device {args.Interface} not found");
		}

		// Register packet handler
		_dev.OnPacketArrival += new PacketArrivalEventHandler(OnPacket);
		_dev.Open(DeviceModes.Promiscuous);
		_dev.StartCapture();
		
		// Wait until the program is terminated by user or by internal logic
		while (true) { }
	}

	// Stop capturing packets and close the device
	public static void Close() {
		if (_dev != null) {
			_dev.StopCapture();
			_dev.Close();
		}
	}

	// Handle captured packet
	static void OnPacket(object sender, PacketCapture packetCapture) {
		// Terminate the program when the maximum number of packets captured is reached
		if (_args.Num > 0 && _packetCaptured >= _args.Num) {
			Close();
			Environment.Exit(0);
		}

		try {
			// If the packet does not match any filters return
			if (!MatchesFilters(packetCapture)) {
				return;
			}
		} catch (Exception e) {
			Error.Exit(e.Message);
		}

		_packetCaptured++;

		try {
			PacketInfo packetInfo = new PacketInfo();
			packetInfo.PacketToObject(packetCapture);
			packetInfo.PrintPacket(_packetCaptured, _args.Num);
		} catch (Exception e) {
			Error.Exit(e.Message);
		}
	}

	static bool MatchesFilters(PacketCapture packetCapture) {
		// Return true if no argument is set
		if (!(_args.Tcp || _args.Udp || _args.Icmp4 || _args.Icmp6 || _args.Arp || _args.Ndp || _args.Igmp ||
		      _args.Mld)) {
			return true;
		}

		if (GetPacketEthernetType(packetCapture) == EthernetType.IPv4 ||
		    GetPacketEthernetType(packetCapture) == EthernetType.IPv6) {
			if (_args.Tcp && GetPacketProtocol(packetCapture) == ProtocolType.Tcp) {
				return true;
			}

			if (_args.Udp && GetPacketProtocol(packetCapture) == ProtocolType.Udp) {
				return true;
			}

			if (_args.Icmp4 && GetPacketProtocol(packetCapture) == ProtocolType.Icmp) {
				return true;
			}

			if (_args.Icmp6 && GetPacketProtocol(packetCapture) == ProtocolType.IcmpV6) {
				return true;
			}

			if (_args.Igmp && GetPacketProtocol(packetCapture) == ProtocolType.Igmp) {
				return true;
			}

			if (_args.Ndp && GetPacketProtocol(packetCapture) == ProtocolType.IcmpV6) {
				var packet = PacketDotNet.Packet.ParsePacket(packetCapture.GetPacket().LinkLayerType,
					packetCapture.GetPacket().Data);
				var icmpPacket = (IcmpV6Packet)packet.Extract<IcmpV6Packet>();

				if (icmpPacket.Type is IcmpV6Type.RouterSolicitation or IcmpV6Type.RouterAdvertisement
				    or IcmpV6Type.NeighborSolicitation or IcmpV6Type.NeighborAdvertisement
				    or IcmpV6Type.RedirectMessage) {
					return true;
				}
			}

			if (_args.Mld && GetPacketProtocol(packetCapture) == ProtocolType.IcmpV6) {
				var packet = PacketDotNet.Packet.ParsePacket(packetCapture.GetPacket().LinkLayerType,
					packetCapture.GetPacket().Data);
				var icmpPacket = (IcmpV6Packet)packet.Extract<IcmpV6Packet>();

				if (icmpPacket.Type is IcmpV6Type.MulticastListenerQuery or IcmpV6Type.MulticastListenerReport
				    or IcmpV6Type.MulticastListenerDone) {
					return true;
				}
			}
		}

		if (_args.Arp && GetPacketEthernetType(packetCapture) == EthernetType.Arp) {
			return true;
		}

		return false;
	}

	// Return ProtocolType of a PacketCapture
	static ProtocolType GetPacketProtocol(PacketCapture packetCapture) {
		var packet = PacketDotNet.Packet.ParsePacket(packetCapture.GetPacket().LinkLayerType,
			packetCapture.GetPacket().Data);
		var ipPacket = (IPPacket)packet.Extract<IPPacket>();
		return ipPacket.Protocol;
	}

	// Return EthernetType of a PacketCapture
	static EthernetType GetPacketEthernetType(PacketCapture packetCapture) {
		var packet = PacketDotNet.Packet.ParsePacket(packetCapture.GetPacket().LinkLayerType,
			packetCapture.GetPacket().Data);
		var ethPacket = (EthernetPacket)packet;
		return ethPacket.Type;
	}
}