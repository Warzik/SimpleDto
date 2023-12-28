using IncrementalGenerator.Descriptors;
using IncrementalGenerator.Parsers;
using IncrementalGenerator.Templates.Abstractions;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace IncrementalGenerator.Templates.Classes;

internal sealed class DtoTemplate : BaseDtoTemplate
{
    public DtoTemplate(DtoClass dtoClass, Action<Diagnostic> reportDiagnostic) : base(reportDiagnostic)
    {
        TemplateFileName = "DtoTemplate.cs.sbncs";

        Name = dtoClass.DtoSyntax.Identifier.ToString();

        Properties = GetProperties(dtoClass.EntitySymbol)
            .Where(x => !dtoClass.IgnoredProperties.Contains(x.Name))
            .ToArray();

        TypeModifiers = dtoClass.DtoSyntax.Modifiers
            .Select(x => x.ToString())
            .ToArray();

        IsRecord = dtoClass.DtoSymbol.IsRecord;

        Namespace = GetNamespaceFrom(dtoClass.DtoSyntax);
    }

    public override string Name { get; }
    public override PropertyMember[] Properties { get; }
    public override string[] TypeModifiers { get; }
    public override string Namespace { get; }
    public override bool IsRecord { get; }

    protected override string TemplateFileName { get; }
}
