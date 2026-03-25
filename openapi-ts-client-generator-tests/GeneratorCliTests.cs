using System.CommandLine;

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace OpenApiTsClientGenerator.Tests;

/// <summary>
/// Tests for the GeneratorCli class, which is responsible for defining the command-line interface (CLI) for the OpenAPI TypeScript client generator, including command definitions, options, and validation logic.
/// </summary>
internal sealed class GeneratorCliTests
{
    private GeneratorCli _cli = null!;

    [SetUp]
    public void Setup()
    {
        _cli = new GeneratorCli();
    }

    [Test]
    public void ProcessCommand_WithNonExistentFile_HasValidationError()
    {
        ParseResult parseResult = _cli.Parse(["process", "--file", "/nonexistent/path/file.json"]);

        Assert.That(parseResult.Errors, Is.Not.Empty);
        Assert.That(parseResult.Errors[0].Message, Does.Contain("does not exist"));
    }

    [Test]
    public void ProcessCommand_WithInvalidUrl_HasValidationError()
    {
        ParseResult parseResult = _cli.Parse(["process", "--url", "not-a-valid-url"]);

        Assert.That(parseResult.Errors, Is.Not.Empty);
        Assert.That(parseResult.Errors[0].Message, Does.Contain("not valid"));
    }

    [Test]
    public void ProcessCommand_WithBothFileAndUrl_PrintsError()
    {
        string inputFile = Path.GetTempFileName();
        string outputFile = Path.GetTempFileName();
        try
        {
            string output = CaptureConsoleOutput(() =>
                _cli.Parse(["process", "--file", inputFile, "--url", "https://example.com/api.json", "--output", outputFile]).Invoke());

            Assert.That(output, Does.Contain("Please specify only one"));
        }
        finally
        {
            File.Delete(inputFile);
            File.Delete(outputFile);
        }
    }

    [Test]
    public void ProcessCommand_WithNeitherFileNorUrl_PrintsError()
    {
        string outputFile = Path.GetTempFileName();
        try
        {
            string output = CaptureConsoleOutput(() =>
                _cli.Parse(["process", "--output", outputFile]).Invoke());

            Assert.That(output, Does.Contain("Please specify either"));
        }
        finally
        {
            File.Delete(outputFile);
        }
    }

    [Test]
    public void ProcessCommand_WithValidFile_WritesGeneratedCodeToOutput()
    {
        string outputFile = Path.GetTempFileName();
        try
        {
            _ = CaptureConsoleOutput(() =>
                _cli.Parse(["process", "--file", "./TestData/petstore-swagger.json", "--output", outputFile]).Invoke());

            string content = File.ReadAllText(outputFile);
            Assert.That(content, Does.Contain("petstore.swagger.io"));
        }
        finally
        {
            File.Delete(outputFile);
        }
    }

    [Test]
    public async Task ProcessCommand_WithValidUrl_WritesGeneratedCodeToOutput()
    {
        string testDataPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
        string outputFile = Path.GetTempFileName();

        IContainer container = new ContainerBuilder()
            .WithImage("nginx:alpine")
            .WithPortBinding(80, true)
            .WithResourceMapping(testDataPath, "/usr/share/nginx/html")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(80)))
            .Build();

        try
        {
            await container.StartAsync().ConfigureAwait(false);

            ushort port = container.GetMappedPublicPort(80);
            string url = $"http://localhost:{port}/petstore-swagger.json";

            _ = CaptureConsoleOutput(() =>
                _cli.Parse(["process", "--url", url, "--output", outputFile]).Invoke());

            string content = await File.ReadAllTextAsync(outputFile).ConfigureAwait(false);
            Assert.That(content, Does.Contain("petstore.swagger.io"));
        }
        finally
        {
            File.Delete(outputFile);
            await container.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static string CaptureConsoleOutput(Action action)
    {
        TextWriter originalOut = Console.Out;
        using StringWriter sw = new();
        Console.SetOut(sw);
        try
        {
            action();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
        return sw.ToString();
    }
}