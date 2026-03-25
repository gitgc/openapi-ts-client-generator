using System.CommandLine;

namespace OpenApiTsClientGenerator;

/// <summary>
/// A class responsible for defining the command-line interface (CLI) for the OpenAPI TypeScript client generator, including command definitions, options, and validation logic.
/// </summary>
internal sealed class GeneratorCli
{
    private readonly Option<FileInfo> _fileArgument = new("--file", "-f")
    {
        Description = "The OpenAPI schema file to process",
        Arity = ArgumentArity.ZeroOrOne
    };

    private readonly Option<string> _urlArgument = new("--url", "-u")
    {
        Description = "The URL of the OpenAPI schema to process",
        Arity = ArgumentArity.ZeroOrOne
    };

    private readonly Option<FileInfo> _outputArgument = new("--output", "-o")
    {
        Description = "The output file to write the generated code to (default: output.ts)",
        Arity = ArgumentArity.ZeroOrOne,
        DefaultValueFactory = result => new FileInfo("output.ts")
    };

    private readonly RootCommand _rootCommand = new("OpenAPI Code Generator");
    private readonly Command _processCommand = new("process", "Process a file");

    /// <summary>
    /// Initializes a new instance of the GeneratorCli class, configuring the command-line options and validators.
    /// </summary>
    public GeneratorCli()
    {
        ConfigureValidators();
        ConfigureCommands();
    }

    /// <summary>
    /// Parses the command-line arguments and returns the resulting ParseResult, which can be invoked to execute the corresponding command action.
    /// </summary>
    /// <param name="args">The command-line arguments to parse.</param>
    /// <returns>The ParseResult representing the parsed command-line arguments.</returns>
    internal ParseResult Parse(string[] args)
    {
        return _rootCommand.Parse(args);
    }

    private void ConfigureValidators()
    {
        _fileArgument.Validators.Add(result =>
        {
            FileInfo? file = result.GetValueOrDefault<FileInfo>();
            if (file is not null && !file.Exists)
            {
                result.AddError($"The specified file does not exist: {file.FullName}");
            }
        });

        _urlArgument.Validators.Add(result =>
        {
            string? url = result.GetValueOrDefault<string>();
            if (url is not null && !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                result.AddError($"The specified URL is not valid: {url}");
            }
        });

        _outputArgument.Validators.Add(result =>
        {
            FileInfo? outputFile = result.GetValueOrDefault<FileInfo>();
            if (outputFile is not null)
            {
                try
                {
                    // Check if we can write to the specified output file
                    using FileStream fs = outputFile.OpenWrite();
                }
                catch (IOException ex)
                {
                    result.AddError($"Cannot write to the specified output file: {outputFile.FullName}. Error: {ex.Message}");
                }
            }
        });
    }

    private void ConfigureCommands()
    {
        _processCommand.Options.Add(_fileArgument);
        _processCommand.Options.Add(_urlArgument);
        _processCommand.Options.Add(_outputArgument);

        _processCommand.SetAction(parseResult =>
        {
            FileInfo? file = parseResult.GetValue(_fileArgument);
            string? url = parseResult.GetValue(_urlArgument);
            FileInfo outputFile = parseResult.GetValue(_outputArgument)!;

            if (file is not null && url is not null)
            {
                Console.WriteLine("Please specify only one of file or URL, not both.");
                return;
            }
            else if (file is null && url is null)
            {
                Console.WriteLine("Please specify either a file or a URL to process.");
                return;
            }

            TypeScriptConverter converter = new();

            if (file is not null)
            {
                Console.WriteLine($"Processing file: {file.FullName}");
                string code = converter.ConvertFromFile(file.FullName);
                File.WriteAllText(outputFile.FullName, code);
                Console.WriteLine($"Generated code has been written to {outputFile.FullName}");
            }
            else if (url is not null)
            {
                Console.WriteLine($"Processing URL: {url}");
                string code = converter.ConvertFromUrl(url);
                File.WriteAllText(outputFile.FullName, code);
                Console.WriteLine($"Generated code has been written to {outputFile.FullName}");
            }
        });

        _rootCommand.Subcommands.Add(_processCommand);
    }
}