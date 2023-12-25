using System.IO;
using System;
using System.Reflection;
using System.Collections.Generic;
using Scriban.Runtime;

namespace IncrementalGenerator.Templates.Abstractions;

internal abstract class BaseTemplate
{
    private static readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();

    protected string BaseDir { get; } = ExecutingAssembly.GetName().Name;
    protected virtual string TemplatesDir { get; } = "Templates";
    protected abstract string TemplateFileName { get; }
    private string TemplatePath => $"{BaseDir}.{TemplatesDir}.{TemplateFileName}";
    public abstract string Namespace { get; }

    public List<string> Usings = [];

    [ScriptMemberIgnore]
    public string GetTemplate()
    {
        using var stream = ExecutingAssembly.GetManifestResourceStream(TemplatePath);

        if (stream == null)
        {
            throw new FileNotFoundException($"Template not found. Template path: {TemplatePath}.");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}