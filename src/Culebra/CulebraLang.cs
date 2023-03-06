namespace Culebra;

using Culebra.Interpreter.Treewalk;
using CommandLine;
using CommandLine.Text;

public class CulebraLang {
    static int rValue = 0;
    public static int Main(string[] args) {
        Parser cliParser = new CommandLine.Parser(with => with.HelpWriter = null);
        var result = cliParser.ParseArguments<DefaultOptions, RunOptions>(args);

        
        result.WithParsed<RunOptions>(RunOptions);
        result.WithParsed<DefaultOptions>(DefaultOptions);
        result.WithNotParsed(errs => DisplayHelp(result, errs));


        return rValue;
    }

    static void RunOptions(RunOptions opt) {
        TreewalkInterpreter interpreter = new(opt.path);
        interpreter.run();
    }

    static void DefaultOptions(DefaultOptions opt) {
        if (opt.tokenizeIn) {
            while(true) {
                Console.Write(">> ");
                string input = Console.ReadLine();
                if (input is null) return;
                Parsing.Scanner scanner = new(input);
                foreach(var tok in scanner.tokenize()) {
                    Console.WriteLine(tok);
                }
            }
        }
    }

    static void ParseError(IEnumerable<Error> errors) {
    }

    static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errors) {
        var help = HelpText.AutoBuild(result, h => {
            h.AdditionalNewLineAfterOption = false;
            h.Heading = $"Culebra Language .NET Interpreter | version {typeof(CulebraLang).Assembly.GetName().Version}";
            h.Copyright = "";

            return h;
        });

        Console.WriteLine(help);
    }

}