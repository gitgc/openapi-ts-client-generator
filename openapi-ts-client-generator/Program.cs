
using System.CommandLine;

internal static class Program
{
    private static readonly GeneratorCli G_CLI = new();

    public static int Main(string[] args)
    {
        RootCommand rootCommand = G_CLI.Start();
        ParseResult parseResult = rootCommand.Parse(args);

        return parseResult.Invoke();
    }
}