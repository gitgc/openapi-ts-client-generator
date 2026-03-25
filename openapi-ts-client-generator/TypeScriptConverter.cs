using NSwag;
using NSwag.CodeGeneration.TypeScript;

namespace OpenApiTsClientGenerator;

/// <summary>
/// A class responsible for converting OpenAPI schemas to TypeScript code using NSwag's TypeScript client generator.
/// </summary>
internal sealed class TypeScriptConverter
{
    private readonly TypeScriptClientGeneratorSettings _settings = new()
    {
        ClassName = "{controller}Client",
    };

    /// <summary>
    /// Converts an OpenAPI schema from a URL to TypeScript code.
    /// </summary>
    /// <param name="openApiSchemaUrl">The URL of the OpenAPI schema.</param>
    /// <returns>The generated TypeScript code.</returns>
    public string ConvertFromUrl(string openApiSchemaUrl)
    {
        OpenApiDocument document = OpenApiDocument.FromUrlAsync(openApiSchemaUrl).GetAwaiter().GetResult();

        return ConvertFromDocument(document);
    }

    /// <summary>
    /// Converts an OpenAPI schema file to TypeScript code.
    /// </summary>
    /// <param name="openApiSchemaFile">The path to the OpenAPI schema file.</param>
    /// <returns>The generated TypeScript code.</returns>
    public string ConvertFromFile(string openApiSchemaFile)
    {
        OpenApiDocument document = OpenApiDocument.FromFileAsync(openApiSchemaFile).GetAwaiter().GetResult();

        return ConvertFromDocument(document);
    }

    private string ConvertFromDocument(OpenApiDocument document)
    {
        TypeScriptClientGenerator generator = new(document, _settings);
        string code = generator.GenerateFile();

        return code;
    }
}