using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NexusSupport.SourceGenerator.Attributes;
using NexusSupport.SourceGenerator.Extraction;
using NexusSupport.SourceGenerator.Models;
using NexusSupport.SourceGenerator.Rendering;

namespace NexusSupport.SourceGenerator.Generators;

[Generator(LanguageNames.CSharp)]
public class ServiceRegistrationGenerator : IIncrementalGenerator
{
    private static void Execute(SourceProductionContext context, string assemblyName, ImmutableArray<ServiceRegistration> registrations)
    {
        if (registrations.IsDefaultOrEmpty)
            return;

        var lines = registrations.Select(ServiceRegistrationSourceBuilder.RenderRegistrationLine);
        var sourceCode = ServiceRegistrationSourceBuilder.RenderExtensionClass(lines, assemblyName);
        context.AddSource($"ServiceRegistrationExtensions_{assemblyName}.g.cs", sourceCode);
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx =>
            ctx.AddSource(ServiceAttributeSource.HintName, ServiceAttributeSource.SourceText));

        var extractionResults = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) =>
                    node is ClassDeclarationSyntax cls && cls.AttributeLists.Count > 0,
                transform: static (ctx, ct) => ServiceRegistrationExtractor.Extract(ctx, ct))
            .Where(static result => result.Registration is not null || !result.Diagnostics.IsDefaultOrEmpty);

        context.RegisterSourceOutput(extractionResults, static (productionContext, result) =>
        {
            foreach (var diagnostic in result.Diagnostics)
                productionContext.ReportDiagnostic(diagnostic);
        });

        var registrationsProvider = extractionResults
            .Select(static (result, _) => result.Registration)
            .Where(static registration => registration is not null)
            .Select(static (registration, _) => registration!.Value);

        var assemblyNameProvider = context.CompilationProvider
            .Select(static (compilation, _) =>
                (compilation.AssemblyName ?? "Generated").Replace('.', '_').Replace('-', '_'));

        var combined = registrationsProvider.Collect().Combine(assemblyNameProvider);

        context.RegisterSourceOutput(combined, static (SourceProductionContext productionContext, (ImmutableArray<ServiceRegistration> registrations, string assemblyName) pair) =>
            Execute(productionContext, pair.assemblyName, pair.registrations));
    }
}
