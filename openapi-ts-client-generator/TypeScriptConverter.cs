using NSwag;
using NSwag.CodeGeneration.TypeScript;

internal sealed class TypeScriptConverter
{
    private readonly TypeScriptClientGeneratorSettings _settings = new()
    {
        ClassName = "{controller}Client",
    };

    public string ConvertFromUrl(string openApiSchemaUrl)
    {
        OpenApiDocument document = OpenApiDocument.FromUrlAsync(openApiSchemaUrl).GetAwaiter().GetResult();

        return ConvertFromDocument(document);
    }

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