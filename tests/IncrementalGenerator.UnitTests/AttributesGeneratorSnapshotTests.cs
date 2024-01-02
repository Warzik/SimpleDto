using SimpleDto.Generator.UnitTests.TestUtils;

namespace SimpleDto.Generator.UnitTests;

[UsesVerify]
public class AttributesGeneratorSnapshotTests
{
    [Fact]
    public Task ShouldGenerateAttributesCorrectly()
    {
        return TestHelper.VerifyGenerator<AttributesGenerator>();
    }
}
