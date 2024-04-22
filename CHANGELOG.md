# Changelog

## Usage
```
./ipk-sniffer [-i interface | --interface interface] {-p|--port-source|--port-destination port [--tcp|-t] [--udp|-u]} [--arp] [--ndp] [--icmp4] [--icmp6] [--igmp] [--mld] {-n num}

-i, --interface       Interface to sniff
-p, --port            Filter by port number
--port-destination    Filter by destination port number
--port-source         Filter by source port number
-t, --tcp             Display only TCP packets
-u, --udp             Display only UDP packets
--icmp4               Display only ICMPv4 packets
--icmp6               Display only ICMPv6 packets
--arp                 Display only ARP packets
--ndp                 Display only NDP packets
--igmp                Display only IGMP packets
--mld                 Display only MLD packets
-n                    Number of packets to capture (Default: 1)
--help                Display this help screen.
--version             Display version information.
```

## Known Limitations
I am not aware of any limitations.