using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NexusSupport.SourceGenerator.Generators;
using Xunit;

namespace NexusSupport.SourceGenerator.Tests.Generators;

public class ServiceRegistrationGeneratorTests
{
    [Fact]
    public void Generator_AlwaysEmitsServiceAttributeSource()
    {
        var run = RunGenerator("namespace TestNamespace;");

        Assert.Contains(run.DriverResult.Results.Single().GeneratedSources, source => source.HintName == "ServiceAttribute.g.cs");
    }

    [Fact]
    public void Generator_ProducesNoRegistrationSource_WhenNoServiceAttributesArePresent()
    {
        var run = RunGenerator("""
            namespace TestNamespace;

            public class PlainClass { }
            """);

        Assert.Empty(run.Diagnostics);
        Assert.DoesNotContain(run.DriverResult.Results.Single().GeneratedSources,
            source => source.HintName.StartsWith("ServiceRegistrationExtensions_"));
    }

    [Theory]
    [InlineData("Scoped", "AddScoped")]
    [InlineData("Transient", "AddTransient")]
    [InlineData("Singleton", "AddSingleton")]
    public void Generator_EmitsRegistrationWithDeclaredInterface(string lifetime, string expectedMethod)
    {
        var source = $$"""
            using Nexus.Generated;

            namespace TestNamespace
            {
                public interface IFoo { }

                [Service(ServiceLifetime.{{lifetime}}, typeof(IFoo))]
                public class Foo : IFoo { }
            }
            """;

        var run = RunGenerator(source);
        var generatedText = GetRegistrationSourceText(run);

        Assert.Empty(run.Diagnostics);
        Assert.Contains($"services.{expectedMethod}<TestNamespace.IFoo, TestNamespace.Foo>();", generatedText);
        AssertCompilesWithoutErrors(run.OutputCompilation);
    }

    [Fact]
    public void Generator_EmitsSelfRegistration_WhenNoServiceTypeIsDeclared()
    {
        var source = """
            using Nexus.Generated;

            namespace TestNamespace
            {
                [Service(ServiceLifetime.Transient)]
                public class Foo { }
            }
            """;

        var run = RunGenerator(source);
        var generatedText = GetRegistrationSourceText(run);

        Assert.Empty(run.Diagnostics);
        Assert.Contains("services.AddTransient<TestNamespace.Foo>();", generatedText);
        Assert.DoesNotContain("AddTransient<TestNamespace.Foo,", generatedText);
        AssertCompilesWithoutErrors(run.OutputCompilation);
    }

    [Fact]
    public void Generator_OrdersMultipleRegistrationsOrdinally()
    {
        var source = """
            using Nexus.Generated;

            namespace TestNamespace
            {
                public interface IBeta { }
                public interface IAlpha { }

                [Service(ServiceLifetime.Scoped, typeof(IBeta))]
                public class BetaService : IBeta { }

                [Service(ServiceLifetime.Scoped, typeof(IAlpha))]
                public class AlphaService : IAlpha { }
            }
            """;

        var run = RunGenerator(source);
        var generatedText = GetRegistrationSourceText(run);

        Assert.Empty(run.Diagnostics);
        var alphaIndex = generatedText.IndexOf("AlphaService", StringComparison.Ordinal);
        var betaIndex = generatedText.IndexOf("BetaService", StringComparison.Ordinal);
        Assert.True(alphaIndex >= 0 && betaIndex >= 0 && alphaIndex < betaIndex,
            "Expected the ordinally-first registration (AlphaService) to appear before BetaService.");
        AssertCompilesWithoutErrors(run.OutputCompilation);
    }

    [Fact]
    public void Generator_ReportsDiagnostic_WhenClassDoesNotImplementDeclaredServiceType()
    {
        var source = """
            using Nexus.Generated;

            namespace TestNamespace
            {
                public interface IFoo { }
                public interface IBar { }

                [Service(ServiceLifetime.Scoped, typeof(IBar))]
                public class Foo : IFoo { }
            }
            """;

        var run = RunGenerator(source);

        var diagnostic = Assert.Single(run.Diagnostics);
        Assert.Equal("NSG001", diagnostic.Id);
        Assert.Equal(DiagnosticSeverity.Error, diagnostic.Severity);
        Assert.DoesNotContain(run.DriverResult.Results.Single().GeneratedSources,
            source => source.HintName.StartsWith("ServiceRegistrationExtensions_"));
    }

    [Fact]
    public void Generator_SanitizesAssemblyNameForGeneratedClassName()
    {
        var source = """
            using Nexus.Generated;

            namespace TestNamespace
            {
                public interface IFoo { }

                [Service(ServiceLifetime.Scoped, typeof(IFoo))]
                public class Foo : IFoo { }
            }
            """;

        var run = RunGenerator(source, assemblyName: "My.Test-Assembly");

        Assert.Empty(run.Diagnostics);
        var registrationSource = run.DriverResult.Results.Single().GeneratedSources
            .Single(s => s.HintName.StartsWith("ServiceRegistrationExtensions_"));

        Assert.Equal("ServiceRegistrationExtensions_My_Test_Assembly.g.cs", registrationSource.HintName);
        Assert.Contains("class ServiceRegistrationExtensions_My_Test_Assembly", registrationSource.SourceText.ToString());
    }

    private static string GetRegistrationSourceText(GenerationOutcome run) =>
        run.DriverResult.Results.Single().GeneratedSources
            .Single(source => source.HintName.StartsWith("ServiceRegistrationExtensions_"))
            .SourceText.ToString();

    private static void AssertCompilesWithoutErrors(Compilation compilation)
    {
        var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        Assert.True(errors.Count == 0, $"Expected no compile errors, got: {string.Join("\n", errors)}");
    }

    private static GenerationOutcome RunGenerator(string source, string assemblyName = "TestAssembly")
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create(
            assemblyName,
            [syntaxTree],
            GetMetadataReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var driver = CSharpGeneratorDriver.Create(new ServiceRegistrationGenerator());
        driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(
            compilation, out var outputCompilation, out var diagnostics);

        return new GenerationOutcome(driver.GetRunResult(), diagnostics, outputCompilation);
    }

    private static IEnumerable<MetadataReference> GetMetadataReferences()
    {
        var trustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator);

        return trustedAssembliesPaths.Select(path => (MetadataReference)MetadataReference.CreateFromFile(path));
    }

    private sealed record GenerationOutcome(
        GeneratorDriverRunResult DriverResult,
        ImmutableArray<Diagnostic> Diagnostics,
        Compilation OutputCompilation);
}
