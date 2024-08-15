using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SimpleDto.Generator.Common;
using SimpleDto.Generator.Templates.Abstractions;
using SimpleDto.Generator.Templates.Attributes;
using System.Text;

namespace SimpleDto.Generator;

[Generator]
internal sealed class AttributesGenerator : IIncrementalGenerator
{
    private readonly BaseAttributeTemplate[] AllAttributes = [
        new DtoFromAttributeTemplate(),
        new DtoMemberIgnoreAttributeTemplate()
    ];

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        foreach (var attribute in AllAttributes)
        {
            context.RegisterPostInitializationOutput(ctx =>
                ctx.AddSource($"{attribute.AttributeName}.g.cs", attribute.Render()));
        }
    }
}
