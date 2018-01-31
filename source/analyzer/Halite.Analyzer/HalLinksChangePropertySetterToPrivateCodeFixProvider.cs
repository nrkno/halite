using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Halite
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HalLinksChangePropertySetterToPrivateCodeFixProvider)), Shared]
    public class HalLinksChangePropertySetterToPrivateCodeFixProvider : CodeFixProvider
    {
        private const string Title = "Mark setter as private";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(HalLinksSubclassPropertySetterAnalyzer.DiagnosticIdProperty);

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
                    createChangedDocument: c => MarkPropertySetterPrivate(context.Document, declaration, c),
                    equivalenceKey: Title),
                diagnostic);
        }

        private static async Task<Document> MarkPropertySetterPrivate(Document document, PropertyDeclarationSyntax propertyDeclarationSyntax, CancellationToken cancellationToken)
        {
            var oldSetter = propertyDeclarationSyntax.AccessorList.Accessors.Single(a => a.Keyword.Kind() == SyntaxKind.SetKeyword);

            var newSetter = oldSetter
                .WithModifiers(SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)));

            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.ReplaceNode(oldSetter, newSetter);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
