using IncrementalGenerator.Common;
using IncrementalGenerator.Extensions;
using IncrementalGenerator.Parsers;
using IncrementalGenerator.Resolvers;
using IncrementalGenerator.Templates.Attributes;
using IncrementalGenerator.Templates.Classes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace IncrementalGenerator;

[Generator]
public class DtoGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    new DtoFromAttributeTemplate().AttributeFullName,
                    (node, _) => node is ClassDeclarationSyntax or RecordDeclarationSyntax,
                    (context, _) => context.TargetNode as TypeDeclarationSyntax)
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
        var propertiesResolver = new SimplePropertiesResolver(context.ReportDiagnostic, context.CancellationToken);

        foreach (var dtoClass in parser.GetDtoTypes(distinctTypes))
        {
            var template = new DtoTemplate(dtoClass, propertiesResolver);
            var source = SourceText.From(ScribanRenderer.Render(template), Encoding.UTF8);

            context.AddSource($"{dtoClass.DtoSyntax.Identifier}.g.cs", source);
        }
    }
}
