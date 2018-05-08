using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Halite 
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HalLinksSubclassPropertySetterAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticIdProperty = "HalLinksPropertyNoPublicSetter";

        private const string Category = "Design";

        private static readonly DiagnosticDescriptor HalLinksPropertyNoSetterRule =
           new DiagnosticDescriptor(
               DiagnosticIdProperty,
               "Properties of HalLinks subclass must not have public setter.",
               "Property {0} in {1}, a subclass of HalLinks, must not have externally accessible setter.",
               Category,
               DiagnosticSeverity.Warning,
               isEnabledByDefault: true,
               description: "Properties of HalLinks subclass must not have public setter.");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(HalLinksPropertyNoSetterRule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzePropertyDeclarationNoSetter, SyntaxKind.PropertyDeclaration);
        }

        /// <summary>
        /// Property does not expose externally accessible setter
        /// </summary>
        /// <param name="context"></param>
        private static void AnalyzePropertyDeclarationNoSetter(SyntaxNodeAnalysisContext context)
        {
            var declarationNode = (PropertyDeclarationSyntax)context.Node;
            var parent = (ClassDeclarationSyntax)declarationNode.Parent;
            var propertySymbol = context.SemanticModel.GetDeclaredSymbol(declarationNode);
            if (propertySymbol.ContainingType.IsHalLinks())
            {

                var accessorList = declarationNode.AccessorList;
                if (accessorList.Accessors.Any(a => a.Keyword.Kind() == SyntaxKind.SetKeyword))
                {
                    var setter = accessorList.Accessors.Single(a => a.Keyword.Kind() == SyntaxKind.SetKeyword);
                    if (setter.Modifiers.Any())
                    {
                        if (!setter.Modifiers.All(m => m.Kind() == SyntaxKind.PrivateKeyword))
                        {
                            var diagnostic = Diagnostic.Create(HalLinksPropertyNoSetterRule, setter.GetLocation(), new object[] { declarationNode.Identifier.Value, parent.Identifier.Value });
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                    else
                    {
                        var diagnostic = Diagnostic.Create(HalLinksPropertyNoSetterRule, setter.GetLocation(), new object[] { declarationNode.Identifier.Value, parent.Identifier.Value });
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

    }
}
