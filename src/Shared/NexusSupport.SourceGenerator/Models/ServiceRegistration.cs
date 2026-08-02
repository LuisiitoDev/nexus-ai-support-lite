namespace NexusSupport.SourceGenerator.Models;

internal readonly record struct ServiceRegistration(ServiceLifetime LifeTime, string? InterfaceType, string ImplementationType);
