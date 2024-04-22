using CommandLine;

namespace IPK_Project2;

static class Program {
	static void Main(string[] args) {
		// Handle CTRL+C
		Console.CancelKeyPress += HandleCancelEvent;

		// Parse arguments
		ParserResult<Cli> cli = Parser.Default.ParseArguments<Cli>(args);
		cli.WithNotParsed(_ => { Environment.Exit(0); });
		// Validate arguments
		cli.WithParsed(o => {
			if (o is { Port: not null, PortDestination: not null } or { Port: not null, PortSource: not null } or
			    { PortDestination: not null, PortSource: not null }) {
				Error.Exit(
					"Invalid combination of arguments. -p --port-destination and --port-source cannot be used in combination");
			}

			if ((o.Port != null || o.PortDestination != null || o.PortSource != null) &&
			    o is { Tcp: false, Udp: false }) {
				Error.Exit(
					"Invalid combination of arguments. -p --port-destination and --port-source have to be used in combination with --tcp or --udp");
			}
		});

		// Validate interface
		CheckInterface(cli.Value);

		try {
			Sniffer.Sniff(cli.Value);
		} catch (Exception e) {
			Error.Exit(e.Message);
		}
	}

	// Handle CTRL+C
	private static void HandleCancelEvent(object? sender, ConsoleCancelEventArgs eventArgs) {
		eventArgs.Cancel = true;
		Sniffer.Close();
		eventArgs.Cancel = false;
	}
	
	// Print available interfaces if the value for -i or --interface was not specified or the interface does nto exist
	private static void CheckInterface(Cli args) {
		try {
			if (args.Interface == null) {
				Interface.ListNetworkInterfaces();
				Environment.Exit(0);
			}

			if (!Interface.InterfaceExists(args.Interface)) {
				Error.Print($"Interface {args.Interface} does not exist");
				Interface.ListNetworkInterfaces();
				Environment.Exit(1);
			}
			
		} catch (Exception e) {
			Error.Exit(e.Message);
		}
	}
}