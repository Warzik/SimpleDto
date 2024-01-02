using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SimpleDto.Generator.Common;
using SimpleDto.Generator.Parsers;
using SimpleDto.Generator.Resolvers;
using SimpleDto.Generator.Templates.Attributes;
using SimpleDto.Generator.Templates.Dtos;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace SimpleDto.Generator;

[Generator]
public class DtoGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    new DtoFromAttributeTemplate().AttributeFullName,
                    static (node, _) => node is ClassDeclarationSyntax or RecordDeclarationSyntax,
                    static (context, _) => context.TargetNode as TypeDeclarationSyntax)
                .Where(static m => m is not null);

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, static (spc, source) => Execute(source.Item1, source.Item2!, spc));
    }

    private static void Execute(Compilation compilation, ImmutableArray<TypeDeclarationSyntax> types, SourceProductionContext context)
    {
        if (types.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        var distinctTypes = types.Distinct();

        var parser = new DtoParser(compilation, context.ReportDiagnostic, context.CancellationToken);
        var propertiesResolver = new SimplePropertiesResolver(compilation, context.ReportDiagnostic, context.CancellationToken);

        foreach (var dtoClass in parser.GetDtoTypes(distinctTypes))
        {
            var template = new DtoTemplate(dtoClass, propertiesResolver);
            var source = SourceText.From(ScribanRenderer.Render(template), Encoding.UTF8);

            context.AddSource($"{dtoClass.DtoSyntax.Identifier}.g.cs", source);
        }
    }
}
