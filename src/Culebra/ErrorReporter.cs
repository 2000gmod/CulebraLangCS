namespace Culebra;

public static class ErrorReporter {
    private const string prefix = "# \t";
    public static void reportError(string error, int errorCode = 1) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(prefix + error);
        Environment.Exit(errorCode);
    }

    public static void reportWarning(string warn) {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(prefix + warn);
        Console.ResetColor();
    }
}