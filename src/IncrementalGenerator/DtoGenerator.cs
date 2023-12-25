using IncrementalGenerator.Common;
using IncrementalGenerator.Extensions;
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
    private static string DtoFromAttribute = new DtoFromAttributeTemplate().AttributeFullName;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    DtoFromAttribute,
                    (node, _) => node is ClassDeclarationSyntax,
                    (context, _) => context.TargetNode as ClassDeclarationSyntax)
                .Where(static m => m is not null);

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, static (spc, source) => Execute(source.Item1, source.Item2!, spc));
    }

    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        var distinctClasses = classes.Distinct();

        var dtoFromAttribute = compilation.GetBestTypeByMetadataName(DtoFromAttribute);
        if (dtoFromAttribute == null)
        {
            // nothing to do if this type isn't available
            return;
        }

        // we enumerate by syntax tree, to minimize the need to instantiate semantic models (since they're expensive)
        foreach (IGrouping<SyntaxTree, ClassDeclarationSyntax> group in classes.GroupBy(x => x.SyntaxTree))
        {
            var syntaxTree = group.Key;
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            foreach (ClassDeclarationSyntax classDec in group)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                var classSymbol = semanticModel.GetDeclaredSymbol(classDec, context.CancellationToken)!;

                INamedTypeSymbol? entityType = null;

                foreach (AttributeListSyntax classAttributeList in classDec.AttributeLists)
                {
                    foreach (AttributeSyntax classAttribute in classAttributeList.Attributes)
                    {
                        var attrCtorSymbol = semanticModel.GetSymbolInfo(classAttribute, context.CancellationToken).Symbol as IMethodSymbol;
                        if (attrCtorSymbol == null || !dtoFromAttribute.Equals(attrCtorSymbol.ContainingType, SymbolEqualityComparer.Default))
                        {
                            // badly formed attribute definition, or not the right attribute
                            continue;
                        }

                        var boundAttributes = classSymbol.GetAttributes();

                        if (boundAttributes.Length == 0)
                        {
                            continue;
                        }

                        foreach (AttributeData attributeData in boundAttributes)
                        {
                            if (!SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, dtoFromAttribute))
                            {
                                continue;
                            }

                            // supports: [DtoFrom(typeof(Entity))]
                            if (attributeData.ConstructorArguments.Any())
                            {
                                ImmutableArray<TypedConstant> items = attributeData.ConstructorArguments;

                                switch (items.Length)
                                {
                                    // DtoFrom(Type type)
                                    case 1:
                                        entityType = (INamedTypeSymbol)items[0].Value!;
                                        break;

                                    default:
                                        Debug.Assert(false, "Unexpected number of arguments in attribute constructor.");
                                        break;
                                }
                            }
                        }
                    }
                }

                if (entityType is not null)
                {
                    var template = new DtoClassTemplate(classDec, entityType);
                    var hintName = $"{classDec.Identifier}.g.cs";
                    var source = SourceText.From(ScribanRenderer.Render(template), Encoding.UTF8);

                    context.AddSource(hintName, source);
                }
            }
        }
    }
}
