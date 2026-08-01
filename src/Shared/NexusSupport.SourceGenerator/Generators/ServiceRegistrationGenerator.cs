using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NexusSupport.SourceGenerator.Generators
{
    //[Generator(LanguageNames.CSharp)]
    public class ServiceRegistrationGenerator : IIncrementalGenerator
    {
        private const string TransientServiceAttributeName = "TransientServiceAttribute";
        private const string ScopedServiceAttributeName = "ScopedServiceAttribute";
        private const string SingletonServiceAttributeName = "SingletonServiceAttribute";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // var declarations = context.SyntaxProvider
            //     .CreateSyntaxProvider(
            //         predicate: (node, _) => node is ClassDeclarationSyntax cls,
            //         transform: (ctx, _) => {}
            //     )
            // .Where(m => m != null);
        }
    }
}