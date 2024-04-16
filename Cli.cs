using CommandLine;

namespace IPK_Project2;

public class Cli {
    [Option('i', "interface", Required = false, HelpText = "Interface to sniff")]
    public string? Interface { get; set; }
    
    [Option('p', "port", Required = false, HelpText = "Filter by port number")]
    public int? Port { get; set; }
    
    [Option("port-destination", Required = false, HelpText = "Filter by destination port number")]
    public int? PortDestination { get; set; }

    [Option("port-source", Required = false, HelpText = "Filter by source port number")]
    public int? PortSource { get; set; }
    
    [Option('t', "tcp", Required = false, HelpText = "Display only TCP packets")]
    public bool Tcp { get; set; }

    [Option('u', "udp", Required = false, HelpText = "Display only UDP packets")]
    public bool Udp { get; set; }
    
    [Option("icmp4", Required = false, HelpText = "Display only ICMPv4 packets")]
    public bool Icmp4 { get; set; }

    [Option("icmp6", Required = false, HelpText = "Display only ICMPv6 packets")]
    public bool Icmp6 { get; set; }

    [Option("arp", Required = false, HelpText = "Display only ARP packets")]
    public bool Arp { get; set; }

    [Option("ndp", Required = false, HelpText = "Display only NDP packets")]
    public bool Ndp { get; set; }

    [Option("igmp", Required = false, HelpText = "Display only IGMP packets")]
    public bool Igmp { get; set; }

    [Option("mld", Required = false, HelpText = "Display only MLD packets")]
    public bool Mld { get; set; }

    [Option('n', Required = false, HelpText = "Number of packets to capture", Default = 1)]
    public int Num { get; set; }
}
