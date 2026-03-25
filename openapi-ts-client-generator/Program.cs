
namespace OpenApiTsClientGenerator;

internal static class Program
{
    private static readonly GeneratorCli G_CLI = new();

    public static int Main(string[] args)
    {
        return G_CLI.Parse(args).Invoke();
    }
}