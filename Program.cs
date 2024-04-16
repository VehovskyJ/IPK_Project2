using CommandLine;

namespace IPK_Project2;

static class Program {
    static void Main(string[] args) {
        // Parse arguments
        Cli? cli = null;
        Parser.Default.ParseArguments<Cli>(args)
            .WithParsed<Cli>(o => { cli = o; })
            .WithNotParsed<Cli>((errs) => { Console.Error.WriteLine("Error occured while parsing arguments."); });

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