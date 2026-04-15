using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

      if (root is null)
        return;

      var diagnostic = context.Diagnostics[0];

      // Retrieve the expected name that was stored in the diagnostic properties
      if (!diagnostic.Properties.TryGetValue(InterfaceParameterNameAnalyzer.ExpectedNameKey, out var expectedName)
          || expectedName is null)
        return;

      // Find the parameter syntax node that was flagged
      var parameterSyntax = root
          .FindNode(diagnostic.Location.SourceSpan)
          .FirstAncestorOrSelf<ParameterSyntax>();

      if (parameterSyntax is null)
        return;

      context.RegisterCodeFix(
          CodeAction.Create(
              title: $"Rename to '{expectedName}'",
              createChangedSolution: ct =>
                  RenameParameterAsync(context.Document, parameterSyntax, expectedName, ct),
              equivalenceKey: "RenameToMatchInterface"
          ),
          diagnostic
      );
    }

    private static async Task<Solution> RenameParameterAsync(
        Document document,
        ParameterSyntax parameterSyntax,
        string newName,
        CancellationToken ct)
    {
      var semanticModel = await document
          .GetSemanticModelAsync(ct)
          .ConfigureAwait(false);

      if (semanticModel is null)
        return document.Project.Solution;

      // Get the parameter symbol — this also covers all usages inside the method body
      if (semanticModel.GetDeclaredSymbol(parameterSyntax, ct) is not IParameterSymbol parameterSymbol)
        return document.Project.Solution;

      return await Renamer
          .RenameSymbolAsync(
              document.Project.Solution,
              parameterSymbol,
              new SymbolRenameOptions(),
              newName,
              ct)
          .ConfigureAwait(false);
    }
  }
}
