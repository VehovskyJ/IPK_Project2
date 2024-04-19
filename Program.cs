﻿using System.Net.NetworkInformation;
using CommandLine;
using CommandLine.Text;
using Microsoft.VisualBasic;

namespace IPK_Project2;

static class Program {
	static void Main(string[] args) {
		// Parse arguments
		ParserResult<Cli> cli = Parser.Default.ParseArguments<Cli>(args);
		cli.WithNotParsed(o => {
			Environment.Exit(0);
		});

		// Print available interfaces if the value for -i or --interface was not specified or no argument were provided
		if (cli.Value.Interface == null || args.Length == 0) {
			Interface.ListNetworkInterfaces();
			Environment.Exit(0);
		}

		// Handle invalid interface
		if (!Interface.InterfaceExists(cli.Value.Interface)) {
			Error.Print($"Interface {cli.Value.Interface} does not exist");
			Interface.ListNetworkInterfaces();
			Environment.Exit(1);
		}
	}
}