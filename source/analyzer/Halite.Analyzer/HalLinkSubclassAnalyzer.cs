using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Halite
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HalLinkSubclassAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticIdProperty = "HalLinkPropertyDeclaration";

        private const string Category = "Design";

        private static readonly DiagnosticDescriptor HalLinkPropertyRule =
            new DiagnosticDescriptor(
                DiagnosticIdProperty,
                "HalLink subclasses should not define any additional properties.",
                "{0} should only use properties of the superclass and not define any additional ones.",
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(HalLinkPropertyRule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzePropertyDeclarationSyntaxNode, SyntaxKind.PropertyDeclaration);
        }

        /// <summary>
        /// Types of properties in a HalLinks subclass must be (subtypes of) HalLink.
        /// </summary>
        /// <param name="context"></param>
        private static void AnalyzePropertyDeclarationSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var declarationNode = (PropertyDeclarationSyntax)context.Node;
            var propertySymbol = context.SemanticModel.GetDeclaredSymbol(declarationNode);
            var containingType = propertySymbol.ContainingType;
            if (containingType.InheritsFromHalLink() || containingType.InheritsFromHalTemplatedLink())
            {
                var diagnostic = Diagnostic.Create(HalLinkPropertyRule, declarationNode.GetLocation(),
                    propertySymbol.ContainingType.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
