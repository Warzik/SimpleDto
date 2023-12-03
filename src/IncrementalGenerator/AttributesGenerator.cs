using IncrementalGenerator.Templates;
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
            var scriptObject = new ScriptObject();
            scriptObject.Import(attribute);

            var templateContext = new TemplateContext();
            templateContext.PushGlobal(scriptObject);

            var template = Template.Parse(attribute.GetTemplate());

            var code = template.Render(templateContext);

            context.RegisterPostInitializationOutput(ctx =>
                ctx.AddSource($"{attribute.AttributeName}.g.cs", SourceText.From(code, Encoding.UTF8)));
        }
    }
}
