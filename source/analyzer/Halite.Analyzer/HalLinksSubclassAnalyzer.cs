using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Halite
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HalLinksSubclassAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticIdConstructor = "HalLinksConstructorDeclaration";
        public const string DiagnosticIdConstructorParameter = "HalLinksConstructorParameterDeclaration";
        public const string DiagnosticIdProperty = "HalLinksPropertyDeclaration";

        private const string Category = "Design";

        private static readonly DiagnosticDescriptor ConstructorParametersForAllPropertiesRule =
            new DiagnosticDescriptor(
                DiagnosticIdConstructor,
                "HalLinks subclass must assign all properties in constructor.",
                "HalLinks subclass must assign all properties in constructor.",
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: "HalLinks subclass must assign all properties in constructor.");

        private static readonly DiagnosticDescriptor HalLinksPropertyTypeRule =
            new DiagnosticDescriptor(
                DiagnosticIdProperty,
                "Properties of HalLinks subclasses must be links.",
                "{0} is of type {1} which is not a HAL link type. {2} should only have properties whose types derive from HalLink/HalTemplateLink or IEnumerables of those types.",
                Category,
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: "Properties of HalLinks subclasses should have types that derive from HalLink/HalTemplateLink or IEnumerables of those types.");


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(HalLinksPropertyTypeRule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeConstructorDeclarationSyntaxNode, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeParameterSyntaxNode, SyntaxKind.Parameter);
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

            if (propertySymbol.ContainingType.IsHalLinks())
            {
                if (!propertySymbol.Type.InheritsFromHalLinkObject() && !propertySymbol.Type.ImplementsIEnumerableOfHalLinkObject())
                {
                    var diagnostic = Diagnostic.Create(HalLinksPropertyTypeRule, declarationNode.Type.GetLocation(), propertySymbol.Name, propertySymbol.Type.Name, propertySymbol.ContainingType.Name);
                    context.ReportDiagnostic(diagnostic);
                }

                if (!AnnotatedWithJsonProperty(propertySymbol))
                {
                    
                }
            }
        }

        private static bool AnnotatedWithJsonProperty(IPropertySymbol propertySymbol)
        {
            return true;
        }

        /// <summary>
        /// All parameters should match name/type of some property.
        /// </summary>
        /// <param name="context"></param>
        private static void AnalyzeParameterSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var node = (ParameterSyntax) context.Node;
        }

        /// <summary>
        /// There must be a constructor parameter for each property.
        /// </summary>
        /// <param name="context"></param>
        private static void AnalyzeConstructorDeclarationSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var declarationNode = (ConstructorDeclarationSyntax)context.Node;
            var constructorSymbol = context.SemanticModel.GetDeclaredSymbol(declarationNode);
            if (constructorSymbol.ContainingType.IsHalLinks())
            {
                constructorSymbol.Parameters.All(p => OkParam(p, constructorSymbol.ContainingType));
            }
        }

        private static IEnumerable<INamedTypeSymbol> GetInheritanceChain(INamedTypeSymbol typeSymbol)
        {
            var currentSymbol = typeSymbol;
            while (currentSymbol != null)
            {
                yield return currentSymbol;
                currentSymbol = currentSymbol.BaseType;
            }
        }

        private static IEnumerable<IPropertySymbol> GetImmediatePropertySymbols(INamedTypeSymbol typeSymbol)
        {
            return typeSymbol.GetMembers().Where(it => it.Kind == SymbolKind.Property).Cast<IPropertySymbol>();
        }

        private static IEnumerable<IPropertySymbol> GetAllPropertySymbols(INamedTypeSymbol typeSymbol)
        {
            return GetInheritanceChain(typeSymbol).SelectMany(GetImmediatePropertySymbols);
        }

        private static bool OkParam(IParameterSymbol parameterSymbol, INamedTypeSymbol typeSymbol)
        {
            return GetAllPropertySymbols(typeSymbol).Any(propertySymbol => MatchesParam(propertySymbol, parameterSymbol));
        }

        private static bool MatchesParam(IPropertySymbol propertySymbol, IParameterSymbol parameterSymbol)
        {
            return parameterSymbol.Type.Equals(propertySymbol.Type) && string.Equals(parameterSymbol.Name,
                       propertySymbol.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
