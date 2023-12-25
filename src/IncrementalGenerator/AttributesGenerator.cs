using IncrementalGenerator.Common;
using IncrementalGenerator.Templates;
using IncrementalGenerator.Templates.Abstractions;
using IncrementalGenerator.Templates.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;
using Scriban.Runtime;
using System.Text;

namespace IncrementalGenerator;

[Generator]
internal sealed class AttributesGenerator : IIncrementalGenerator
{
    private readonly BaseAttributeTemplate[] AllAttributes = [
        new DtoFromAttributeTemplate()
    ];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        foreach (var attribute in AllAttributes)
        {
            var code = ScribanRenderer.Render(attribute);

            context.RegisterPostInitializationOutput(ctx =>
                ctx.AddSource($"{attribute.AttributeName}.g.cs", SourceText.From(code, Encoding.UTF8)));
        }
    }
}
