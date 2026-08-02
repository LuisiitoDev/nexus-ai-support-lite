using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NexusSupport.SourceGenerator.Diagnostics;
using NexusSupport.SourceGenerator.Models;

namespace NexusSupport.SourceGenerator.Extraction;

internal static class ServiceRegistrationExtractor
{
    private const string AttributeName = "ServiceAttribute";

    public static ExtractionResult Extract(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        if (context.SemanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken)
            is not INamedTypeSymbol classSymbol)
            return default;

        var attribute = classSymbol.GetAttributes()
            .FirstOrDefault(static a => a.AttributeClass?.Name == AttributeName);

        if (attribute is null || attribute.ConstructorArguments.Length == 0)
            return default;

        if (attribute.ConstructorArguments[0].Value is not int lifetimeValue)
            return default;

        var lifetime = (ServiceLifetime)lifetimeValue;

        string? interfaceType = null;
        if (attribute.ConstructorArguments.Length > 1 &&
            attribute.ConstructorArguments[1].Value is INamedTypeSymbol serviceTypeSymbol)
        {
            if (!Implements(classSymbol, serviceTypeSymbol))
            {
                var diagnostic = Diagnostic.Create(
                    ServiceRegistrationDiagnostics.ServiceTypeNotImplemented,
                    classDeclaration.Identifier.GetLocation(),
                    classSymbol.ToDisplayString(),
                    serviceTypeSymbol.ToDisplayString());

                return new ExtractionResult(null, ImmutableArray.Create(diagnostic));
            }

            interfaceType = serviceTypeSymbol.ToDisplayString();
        }

        var registration = new ServiceRegistration(lifetime, interfaceType, classSymbol.ToDisplayString());
        return new ExtractionResult(registration, ImmutableArray<Diagnostic>.Empty);
    }

    private static bool Implements(INamedTypeSymbol classSymbol, INamedTypeSymbol serviceType)
    {
        if (SymbolEqualityComparer.Default.Equals(classSymbol, serviceType))
            return true;

        foreach (var @interface in classSymbol.AllInterfaces)
        {
            if (SymbolEqualityComparer.Default.Equals(@interface, serviceType))
                return true;
        }

        for (var baseType = classSymbol.BaseType; baseType is not null; baseType = baseType.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(baseType, serviceType))
                return true;
        }

        return false;
    }
}
