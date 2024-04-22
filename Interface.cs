using System.Net.NetworkInformation;

namespace IPK_Project2;

public static class Interface {
	// Print list of available network interfaces
	public static void ListNetworkInterfaces() {
		NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
		if (interfaces.Length == 0) {
			Error.Print("No interfaces found");
		}
        
		// Create nicely formatted list of interfaces
		//
		// Name                 Type                           Status              
		// ----------------------------------------------------------------------
		// lo                   Loopback                       Up                  
		// eno1                 Ethernet                       Down                
		// wlan0                Ethernet                       Up                  
		Console.WriteLine("Available interfaces");
		Console.WriteLine();
		Console.WriteLine("{0,-20} {1,-20} {2,-20}", "Name", "Type", "Status");
		Console.WriteLine(new string('-', 60));
        
		foreach (var intf in interfaces) {
			Console.WriteLine("{0,-20} {1,-20} {2,-20}", intf.Name, intf.NetworkInterfaceType, intf.OperationalStatus);
		}
	}
	
	// Check if interface exists
	public static bool InterfaceExists(string intf) {
		NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

		return interfaces.Any(networkInterface => networkInterface.Name == intf);
	}
}