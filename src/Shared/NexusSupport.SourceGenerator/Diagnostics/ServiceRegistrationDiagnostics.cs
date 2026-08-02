using Microsoft.CodeAnalysis;

namespace NexusSupport.SourceGenerator.Diagnostics;

internal static class ServiceRegistrationDiagnostics
{
    public static readonly DiagnosticDescriptor ServiceTypeNotImplemented = new(
        id: "NSG001",
        title: "Declared service type not implemented",
        messageFormat: "Class '{0}' does not implement declared service type '{1}'",
        category: "NexusSupport.SourceGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
