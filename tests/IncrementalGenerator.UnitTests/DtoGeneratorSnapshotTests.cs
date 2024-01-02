using SimpleDto.Generator.UnitTests.TestUtils;

namespace SimpleDto.Generator.UnitTests;

[UsesVerify]
public abstract class DtoGeneratorSnapshotTests(string dtoType)
{
    [Fact]
    public Task ShouldGenerateIdenticalDtoForEntity()
    {
        var source =
            $$"""
            namespace Application.Entities
            {
                using System;
                
                public sealed class Entity
                {
                    public Guid Id {get; set;}
                    public string? Description { get; set; }
                }
            }

            namespace Application.Dtos
            {
                using SimpleDto.Generator.Attributes;
                using Application.Entities;

                [DtoFrom(typeof(Entity))]
                public sealed partial {{dtoType}} EntityDto
                {
                }
            }
            """;

        return Verify(source);
    }

    [Fact]
    public Task ShouldIgnoreMemberByName()
    {
        var source =
            $$"""
            namespace Application.Entities
            {
                using System.Collections.Generic;

                public sealed class Entity
                {
                    public int Id {get; set;}
                    public string? Title { get; set; }
                    public string? Description { get; set; }
                    public ICollection<string>? Items { get; set; }
                }
            }

            namespace Application.Dtos
            {
                using SimpleDto.Generator.Attributes;
                using Application.Entities;

                [DtoFrom(typeof(Entity))]
                [DtoMemberIgnore(nameof(Entity.Description))]
                [DtoMemberIgnore(nameof(Entity.Title))]
                public sealed partial {{dtoType}} EntityDto
                {
                }
            }
            """;

        return Verify(source);
    }

    [Fact]
    public Task ShouldIgnoreMemberByPreciseType()
    {
        var source =
            $$"""
            namespace Application.Entities
            {
                public sealed class Entity : BaseEntity
                {
                    public int Id {get; set;}
                    public string? Title { get; set; }
                    public string? Description { get; set; }
                }
            }

            namespace Application.Dtos
            {
                using SimpleDto.Generator.Attributes;
                using Application.Entities;

                [DtoFrom(typeof(Entity))]
                [DtoMemberIgnore(typeof(string))]
                public sealed partial {{dtoType}} EntityDto
                {
                }
            }
            """;

        return Verify(source);
    }

    [Fact]
    public Task ShouldIgnoreMemberByPreciseGenericType()
    {
        var source =
            $$"""
            namespace Application.Entities
            {
                using System.Collections.Generic;

                public sealed class Entity : BaseEntity
                {
                    public int Id {get; set;}
                    public string? Title { get; set; }
                    public List<string>? Items { get; set; }
                }
            }

            namespace Application.Dtos
            {
                using SimpleDto.Generator.Attributes;
                using System.Collections.Generic;
                using Application.Entities;

                [DtoFrom(typeof(Entity))]
                [DtoMemberIgnore(typeof(List<string>))]
                public sealed partial {{dtoType}} EntityDto
                {
                }
            }
            """;

        return Verify(source);
    }

    [Fact]
    public Task ShouldIgnoreMemberByOpenGenericType()
    {
        var source =
            $$"""
            namespace Application.Entities
            {
                using System.Collections.Generic;

                public sealed class Entity : BaseEntity
                {
                    public int Id {get; set;}
                    public string? Title { get; set; }
                    public List<string>? ListOfStrings { get; set; }
                    public List<int>? ListOfInts { get; set; }
                    public List<object>? ListOfObjects { get; set; }
                }
            }

            namespace Application.Dtos
            {
                using SimpleDto.Generator.Attributes;
                using System.Collections.Generic;
                using Application.Entities;

                [DtoFrom(typeof(Entity))]
                [DtoMemberIgnore(typeof(List<>))]
                public sealed partial {{dtoType}} EntityDto
                {
                }
            }
            """;

        return Verify(source);
    }

    [Fact]
    public Task ShouldIgnoreMemberByBaseType()
    {
        var source =
            $$"""
            namespace Application.Entities
            {
                using System.Collections.Generic;

                public abstract class BaseEntity
                {
                    public int Id {get; set;}
                }

                public sealed class Entity : BaseEntity
                {
                    public string? Title { get; set; }
                    public string? Description { get; set; }
                    public ICollection<string>? Items { get; set; }

                    public SecondEntity? SecondEntity { get; set; }
                }

                public sealed class SecondEntity : BaseEntity
                {
                    public string? Title { get; set; }
                    public string? Description { get; set; }
                }
            }

            namespace Application.Dtos
            {
                using SimpleDto.Generator.Attributes;
                using Application.Entities;

                [DtoFrom(typeof(Entity))]
                [DtoMemberIgnore(typeof(BaseEntity))]
                public sealed partial {{dtoType}} EntityDto
                {
                }
            }
            """;

        return Verify(source);
    }

    [Fact]
    public Task ShouldIgnoreMemberByInterface()
    {
        var source =
            $$"""
            namespace Application.Entities
            {
                public interface IHaveId
                {
                    public int Id {get; set;}
                }

                public sealed class Entity : IHaveId
                {
                    public int Id {get; set;}
                    public string? Title { get; set; }
                    public string? Description { get; set; }

                    public SecondEntity? SecondEntity { get; set; }
                }

                public sealed class SecondEntity : IHaveId
                {
                    public int Id {get; set;}
                    public string? Title { get; set; }
                }
            }

            namespace Application.Dtos
            {
                using SimpleDto.Generator.Attributes;
                using Application.Entities;

                [DtoFrom(typeof(Entity))]
                [DtoMemberIgnore(typeof(IHaveId))]
                public sealed partial {{dtoType}} EntityDto
                {
                }
            }
            """;

        return Verify(source);
    }

    [Fact]
    public Task ShouldIgnoreMemberByPreciseGenericInterface()
    {
        var source =
            $$"""
            namespace Application.Entities
            {
                using System.Collections.Generic;

                public sealed class Entity : IHaveId
                {
                    public int Id {get; set;}
                    public string? Title { get; set; }
                    public List<string>? ListOfItems { get; set; }
                    public string[]? ArrayOfItems { get; set; }
                }
            }

            namespace Application.Dtos
            {
                using SimpleDto.Generator.Attributes;
                using System.Collections.Generic;
                using Application.Entities;

                [DtoFrom(typeof(Entity))]
                [DtoMemberIgnore(typeof(IEnumerable<string>))]
                public sealed partial {{dtoType}} EntityDto
                {
                }
            }
            """;

        return Verify(source);
    }

    [Fact]
    public Task ShouldIgnoreMemberByOpenGenericInterface()
    {
        var source =
            $$"""
            namespace Application.Entities
            {
                using System.Collections.Generic;

                public sealed class Entity : IHaveId
                {
                    public int Id {get; set;}
                    public string? Title { get; set; }
                    public List<string>? ListOfStrings { get; set; }
                    public int[]? ArrayOfInts { get; set; }
                }
            }

            namespace Application.Dtos
            {
                using SimpleDto.Generator.Attributes;
                using System.Collections.Generic;
                using Application.Entities;

                [DtoFrom(typeof(Entity))]
                [DtoMemberIgnore(typeof(IEnumerable<>))]
                public sealed partial {{dtoType}} EntityDto
                {
                }
            }
            """;

        return Verify(source);
    }

    [Fact]
    public Task ShouldShowDiagnosticWarningWhenEntityMemberTypeIsNotExportableAndNotIgnored()
    {
        var source =
            $$"""
            namespace Application.Entities
            {
                using System.Collections.Generic;

                internal enum InternalEnum
                {
                    None,
                    Something
                }

                public sealed class Entity : IHaveId
                {
                    public int Id {get; set;}
                    public InternalEnum? InternalEnum { get; set; }
                }
            }

            namespace Application.Dtos
            {
                using SimpleDto.Generator.Attributes;
                using System.Collections.Generic;
                using Application.Entities;

                [DtoFrom(typeof(Entity))]
                public sealed partial {{dtoType}} EntityDto
                {
                }
            }
            """;

        return Verify(source);
    }

    [Fact]
    public Task ShouldNotShowDiagnosticWarningWhenEntityMemberTypeIsNotExportableButMarkedAsIgnored()
    {
        var source =
            $$"""
            namespace Application.Entities
            {
                using System.Collections.Generic;

                internal enum InternalEnum
                {
                    None,
                    Something
                }

                public sealed class Entity : IHaveId
                {
                    public int Id {get; set;}
                    public InternalEnum? InternalEnum { get; set; }
                }
            }

            namespace Application.Dtos
            {
                using SimpleDto.Generator.Attributes;
                using System.Collections.Generic;
                using Application.Entities;

                [DtoFrom(typeof(Entity))]
                [DtoMemberIgnore(typeof(InternalEnum?))]
                public sealed partial {{dtoType}} EntityDto
                {
                }
            }
            """;

        return Verify(source);
    }

    private static Task Verify(string source)
    {
        return TestHelper
            .VerifyGenerator<DtoGenerator>(source, new AttributesGenerator());
    }
}

public class ClassTests() : DtoGeneratorSnapshotTests("class") { }
public class RecordTests() : DtoGeneratorSnapshotTests("record") { }