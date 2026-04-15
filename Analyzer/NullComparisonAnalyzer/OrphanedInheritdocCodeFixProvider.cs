using System.Collections.Generic;
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

namespace NullComparisonAnalyzer
{
  [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OrphanedInheritdocCodeFixProvider)), Shared]
  public class OrphanedInheritdocCodeFixProvider : CodeFixProvider
  {
    public override ImmutableArray<string> FixableDiagnosticIds
        => ImmutableArray.Create(OrphanedInheritdocAnalyzer.DiagnosticId);

    public override FixAllProvider GetFixAllProvider()
        => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
      var root = await context.Document
          .GetSyntaxRootAsync(context.CancellationToken)
          .ConfigureAwait(false);

      if (root is null)
        return;

      var diagnostic = context.Diagnostics[0];
      var node = root.FindNode(diagnostic.Location.SourceSpan)
                     .FirstAncestorOrSelf<MemberDeclarationSyntax>();

      if (node is null)
        return;

      context.RegisterCodeFix(
          CodeAction.Create(
              title: "Remove <inheritdoc/>",
              createChangedDocument: ct => RemoveInheritDocAsync(context.Document, root, node, ct),
              equivalenceKey: "RemoveInheritDoc"
          ),
          diagnostic
      );
    }

    private static Task<Document> RemoveInheritDocAsync(
        Document document,
        SyntaxNode root,
        MemberDeclarationSyntax memberDecl,
        CancellationToken ct)
    {
      // Find the trivia list and the specific trivia containing <inheritdoc/>
      var leadingTrivia = memberDecl.GetLeadingTrivia();

      var inheritdocTrivia = leadingTrivia
          .Where(t => t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                   || t.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
          .FirstOrDefault(t =>
              t.GetStructure() is DocumentationCommentTriviaSyntax doc &&
              doc.ChildNodes()
                 .OfType<XmlEmptyElementSyntax>()
                 .Any(el => el.Name.LocalName.Text == "inheritdoc"));

      if (inheritdocTrivia == default)
        return Task.FromResult(document);

      // Also remove the preceding whitespace/newline trivia to avoid leaving a blank line
      var triviaIndex = leadingTrivia.IndexOf(inheritdocTrivia);
      var triviaToRemove = new List<SyntaxTrivia> { inheritdocTrivia };

      if (triviaIndex > 0)
      {
        var preceding = leadingTrivia[triviaIndex - 1];
        if (preceding.IsKind(SyntaxKind.WhitespaceTrivia) ||
            preceding.IsKind(SyntaxKind.EndOfLineTrivia))
        {
          triviaToRemove.Add(preceding);
        }
      }

      var newTrivia = leadingTrivia;
      // Iterate in reverse order so indices remain stable as we remove items
      foreach (var trivia in triviaToRemove.OrderByDescending(t => leadingTrivia.IndexOf(t)))
      {
        newTrivia = newTrivia.Remove(trivia);
      }

      var newMemberDecl = memberDecl.WithLeadingTrivia(newTrivia);
      var newRoot = root.ReplaceNode(memberDecl, newMemberDecl);

      return Task.FromResult(document.WithSyntaxRoot(newRoot));
    }
  }
}
