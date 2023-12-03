using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Scriban;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace IncrementalGenerator;

[Generator]
public class BaseGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //var provider = context.SyntaxProvider.CreateSyntaxProvider(
        //    predicate: IsSyntaxTargetForGeneration,
        //    transform: GetTargetForGeneration
        //).Where(node => node is not null);

        //var compilation = context.CompilationProvider.Combine(provider.Collect());

        //context.RegisterSourceOutput(compilation, Execute);
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode, CancellationToken token)
    {
        return syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
            classDeclarationSyntax.AttributeLists.Count > 0 &&
            classDeclarationSyntax.AttributeLists
                .Any(al => al.Attributes
                    .Any(a => a.Name.ToString() == "GenerateService"));
    }

    private static ClassDeclarationSyntax GetTargetForGeneration(GeneratorSyntaxContext context, CancellationToken token)
    {
        return (ClassDeclarationSyntax)context.Node;
    }

    private void Execute(SourceProductionContext context, (Compilation Left, ImmutableArray<ClassDeclarationSyntax> Right) tuple)
    {
        var (compilation, list) = tuple;

        var names = string.Join(",\n", list.Select(node => $"\t\"{compilation
            .GetSemanticModel(node.SyntaxTree)
            .GetDeclaredSymbol(node)}\""));

        // Parse a liquid template
        var template = Template.ParseLiquid("Hello {{name}}!");
        var result = template.Render(new { Name = "World" }); // => "Hello World!" 

        var code = $$"""
            namespace ClassListGenerator;

            public static class ClassNames
            {
                public static List<string> Names = new() 
                {
                    {{names}}
                };
            }
            """;

        context.AddSource("ClassList.g.cs", code);
    }
}
