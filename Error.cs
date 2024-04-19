namespace IPK_Project2;

public static class Error {
	// Print error message to stderr
	public static void Print(string message) {
		Console.Error.WriteLine($"Error: {message}");
	}

	// Print error message to stderr and terminate the program
	public static void Exit(string message) {
		Print(message);
		Environment.Exit(1);
	}
}