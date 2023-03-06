namespace Culebra;

using CommandLine;

[Verb("def", true, HelpText = "Default options")]
class DefaultOptions {
    [Option('t', "tokens", HelpText = "Scans and tokenizes lines from STDIN")]
    public bool tokenizeIn {get; set;}
}

[Verb("run", false, HelpText = "Run a single file")]
class RunOptions {
    [Value(0, HelpText = "Input file for running.", Required = true)]
    public string path {get; set;}
}