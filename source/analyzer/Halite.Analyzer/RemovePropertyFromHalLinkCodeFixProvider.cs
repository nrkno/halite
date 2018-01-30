using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Halite
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RemovePropertyFromHalLinkCodeFixProvider)), Shared]
    public class RemovePropertyFromHalLinkCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Remove unwanted property from HalLink";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(HalLinkSubclassAnalyzer.DiagnosticIdProperty);

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
                    createChangedDocument: c => RemoveProperty(context.Document, declaration, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private static async Task<Document> RemoveProperty(Document document, PropertyDeclarationSyntax propertyDeclarationSyntax, CancellationToken cancellationToken)
        {
            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.RemoveNode(propertyDeclarationSyntax, SyntaxRemoveOptions.KeepNoTrivia);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
