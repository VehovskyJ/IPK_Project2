using CommandLine;

namespace IPK_Project2;

static class Program {
	static void Main(string[] args) {
		// Parse arguments
		ParserResult<Cli> cli = Parser.Default.ParseArguments<Cli>(args);
		cli.WithNotParsed(o => {
			Environment.Exit(0);
		});

        if (cli != null) {
            var properties = typeof(Cli).GetProperties();

            foreach (var property in properties) {
                string name = property.Name;
                object value = property.GetValue(cli)!;
                Console.WriteLine("{0}: {1}", name, value);
            }
        }
    }
}