using System.Runtime.CompilerServices;

namespace SimpleDto.Generator.UnitTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
    }
}
