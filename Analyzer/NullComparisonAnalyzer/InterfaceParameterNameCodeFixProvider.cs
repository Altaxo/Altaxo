using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Rename;

namespace NullComparisonAnalyzer
{

  [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(InterfaceParameterNameCodeFixProvider)), Shared]
  public class InterfaceParameterNameCodeFixProvider : CodeFixProvider
  {
    public override ImmutableArray<string> FixableDiagnosticIds
        => ImmutableArray.Create(InterfaceParameterNameAnalyzer.DiagnosticId);

    public override FixAllProvider GetFixAllProvider()
        => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
      var root = await context.Document
          .GetSyntaxRootAsync(context.CancellationToken)
          .ConfigureAwait(false);

      var diagnostic = context.Diagnostics[0];
      var diagnosticSpan = diagnostic.Location.SourceSpan;

      // Find the parameter identifier that was flagged
      var parameterIdentifier = root
          ?.FindToken(diagnosticSpan.Start);

      if (parameterIdentifier is null)
        return;

      // Extract expected name from the diagnostic message arguments
      var expectedName = diagnostic.Properties.TryGetValue("expectedName", out var name)
          ? name
          : null;

      if (expectedName is null)
        return;

      context.RegisterCodeFix(
          CodeAction.Create(
              title: $"Rename to '{expectedName}'",
              createChangedSolution: ct =>
                  RenameParameterAsync(context.Document, parameterIdentifier.Value, expectedName, ct),
              equivalenceKey: "RenameToMatchInterface"
          ),
          diagnostic
      );
    }

    private async Task<Solution> RenameParameterAsync(
        Microsoft.CodeAnalysis.Document document,
        SyntaxToken identifier,
        string newName,
        CancellationToken ct)
    {
      var semanticModel = await document.GetSemanticModelAsync(ct).ConfigureAwait(false);
      var symbol = semanticModel?.GetDeclaredSymbol(identifier.Parent!, ct);

      if (symbol is null)
        return document.Project.Solution;

      return await Renamer
          .RenameSymbolAsync(document.Project.Solution, symbol, new SymbolRenameOptions(), newName, ct)
          .ConfigureAwait(false);
    }
  }
}
