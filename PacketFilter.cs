using PacketDotNet;
using SharpPcap;

namespace IPK_Project2;

public static class PacketFilter {
	public static bool MatchesFilters(PacketCapture packetCapture, Cli _args) {
		var packet = PacketDotNet.Packet.ParsePacket(packetCapture.GetPacket().LinkLayerType,
			packetCapture.GetPacket().Data);
		var ethernetPacket = (EthernetPacket)packet;
		var packetEthernetType = ethernetPacket.Type;

		// Return true if no argument is set
		if (!(_args.Tcp || _args.Udp || _args.Icmp4 || _args.Icmp6 || _args.Arp || _args.Ndp || _args.Igmp ||
		      _args.Mld)) {
			return true;
		}

		// Filter ARP packets
		if (_args.Arp && packetEthernetType == EthernetType.Arp) {
			return true;
		}
		
		// Filter IP packets
		if (packetEthernetType is EthernetType.IPv4 or EthernetType.IPv6) {
			var ipPacket = packet.Extract<IPPacket>();
			var packetProtocol = ipPacket.Protocol;

			if ((_args.Tcp && packetProtocol == ProtocolType.Tcp) ||
			    (_args.Udp && packetProtocol == ProtocolType.Udp)) {
				// If port filtering is set, continue filtering
				if (_args.Port == null && _args.PortDestination == null && _args.PortSource == null) {
					return true;
				}
			}

			if ((_args.Icmp4 && packetProtocol == ProtocolType.Icmp) ||
			    (_args.Icmp6 && packetProtocol == ProtocolType.IcmpV6) ||
			    (_args.Igmp && packetProtocol == ProtocolType.Igmp)) {
				return true;
			}

			if (packetProtocol == ProtocolType.IcmpV6) {
				var icmpPacket = (IcmpV6Packet)packet.Extract<IcmpV6Packet>();

				if ((_args.Ndp && IsNdpPacket(icmpPacket)) ||
				    (_args.Mld && IsMldPacket(icmpPacket))) {
					return true;
				}
			}
			
			// Filter by port
			if (packetProtocol is ProtocolType.Tcp or ProtocolType.Udp) {
				var ports = GetPorts(packet, ipPacket);

				if ((_args.PortDestination != null && _args.PortDestination == ports.destination) ||
				    (_args.PortSource != null && _args.PortSource == ports.source) ||
				    (_args.Port != null && (_args.Port == ports.source || _args.Port == ports.destination))) {
					return true;
				}
			}
		}

		return false;
	}

	// Returns source and destination port
	static (int source, int destination) GetPorts(Packet packet, IPPacket ipPacket) {
		switch (ipPacket.Protocol) {
			case ProtocolType.Tcp: {
				var tpcPacket = packet.Extract<TcpPacket>();

				return new (tpcPacket.SourcePort, tpcPacket.DestinationPort);
			}
			case ProtocolType.Udp: {
				var udpPacket = packet.Extract<UdpPacket>();

				return new (udpPacket.SourcePort, udpPacket.DestinationPort);
			}
			default:
				return new (0, 0);
		}
	}
	
	// Returns if icmpPacket is NDP or not
	private static bool IsNdpPacket(IcmpV6Packet icmpPacket) {
		return icmpPacket.Type is IcmpV6Type.RouterSolicitation or IcmpV6Type.RouterAdvertisement
			or IcmpV6Type.NeighborSolicitation or IcmpV6Type.NeighborAdvertisement
			or IcmpV6Type.RedirectMessage;
	}

	// Returns if icmpPacket is MLD or not
	private static bool IsMldPacket(IcmpV6Packet icmpPacket) {
		return icmpPacket.Type is IcmpV6Type.MulticastListenerQuery or IcmpV6Type.MulticastListenerReport
			or IcmpV6Type.MulticastListenerDone;
	}
}