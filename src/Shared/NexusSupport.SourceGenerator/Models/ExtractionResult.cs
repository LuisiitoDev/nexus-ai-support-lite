using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace NexusSupport.SourceGenerator.Models;

internal readonly record struct ExtractionResult(ServiceRegistration? Registration, ImmutableArray<Diagnostic> Diagnostics);
