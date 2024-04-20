using System.Net.NetworkInformation;
using CommandLine;
using CommandLine.Text;
using Microsoft.VisualBasic;
using SharpPcap;

namespace IPK_Project2;

static class Program {
	static void Main(string[] args) {
		// Handle CTRL+C
		Console.CancelKeyPress += new ConsoleCancelEventHandler(HandleCancelEvent);
		
		// Parse arguments
		ParserResult<Cli> cli = Parser.Default.ParseArguments<Cli>(args);
		cli.WithNotParsed(o => {
			Environment.Exit(0);
		});

		// Print available interfaces if the value for -i or --interface was not specified or no argument were provided
		if (cli.Value.Interface == null || args.Length == 0) {
			try {
				Interface.ListNetworkInterfaces();
			} catch (Exception e) {
				Error.Exit(e.Message);
			}
			
			Environment.Exit(0);
		}

		// Handle invalid interface
		if (!Interface.InterfaceExists(cli.Value.Interface)) {
			Error.Print($"Interface {cli.Value.Interface} does not exist");
			try {
				Interface.ListNetworkInterfaces();
			} catch (Exception e) {
				Error.Exit(e.Message);
			}
			
			Environment.Exit(1);
		}

		try {
			Sniffer.Sniff(cli.Value);
		} catch (Exception e) {
			Error.Exit(e.Message);
		}
	}

	// Handle CTRL+C
	static void HandleCancelEvent(object? sender, ConsoleCancelEventArgs eventArgs) {
		eventArgs.Cancel = true;
		Sniffer.Close();
		eventArgs.Cancel = false;
	}
}