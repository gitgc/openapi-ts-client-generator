namespace OpenApiTsClientGenerator.Tests;
/// <summary>
/// Tests for the TypeScriptConverter class, which is responsible for converting OpenAPI schemas to TypeScript code using NSwag's TypeScript client generator.
/// </summary>
internal sealed class TypeScriptConverterTests
{
    private TypeScriptConverter? _converter;

    [SetUp]
    public void Setup()
    {
        _converter = new TypeScriptConverter();
    }

    [Test]
    public void TestValidApiSpecFile()
    {
        string result = _converter.ConvertFromFile("./TestData/petstore-swagger.json");
        Assert.That(result, Does.Contain("petstore.swagger.io"));
    }

    [Test]
    public void TestInvalidApiSpecFile()
    {
        _ = Assert.Throws<FileNotFoundException>(() => _converter.ConvertFromFile("./TestData/nonexistent-file.json"));
    }
}