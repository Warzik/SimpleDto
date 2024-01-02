using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SimpleDto.Generator.UnitTests.TestUtils;

public static class TestHelper
{
    public static SettingsTask VerifyGenerator<TGenerator>(string source = "", params IIncrementalGenerator[] dependentGenerators)
        where TGenerator : IIncrementalGenerator, new()
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        // Create references for assemblies we require
        // We could add multiple references if required
        IEnumerable<PortableExecutableReference> references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };

        Compilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references); // 👈 pass the references to the compilation

        var generator = new TGenerator();
        
        if(dependentGenerators.Length > 0)
        {
            CSharpGeneratorDriver
                .Create(dependentGenerators)
                .RunGeneratorsAndUpdateCompilation(compilation, out compilation, out var _);
        }

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);

        return Verifier.Verify(driver)
            .UseDirectory(Path.Combine(@"..\Snapshots", "Generators", generator.GetType().Name));
    }
}