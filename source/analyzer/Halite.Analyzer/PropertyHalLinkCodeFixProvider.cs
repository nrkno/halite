using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Halite
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PropertyHalLinkCodeFixProvider)), Shared]
    public class PropertyHalLinkCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Change return type to HalLink.";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(HalLinksSubclassAnalyzer.DiagnosticIdProperty);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var syntaxNode = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf();
            var declaration = syntaxNode.OfType<PropertyDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedDocument: c => FixPropertyReturnType(context.Document, declaration, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private static async Task<Document> FixPropertyReturnType(Document document, PropertyDeclarationSyntax propertyDeclarationSyntax, CancellationToken cancellationToken)
        {
            var typeSyntax = SyntaxFactory.IdentifierName(SyntaxFactory.Identifier("HalLink"));
            var replacementSyntax = propertyDeclarationSyntax.WithType(typeSyntax);
            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.ReplaceNode(propertyDeclarationSyntax, replacementSyntax);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}