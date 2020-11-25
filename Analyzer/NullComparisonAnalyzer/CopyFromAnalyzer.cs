using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NullComparisonAnalyzer
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public class CopyFromAnalyzer : DiagnosticAnalyzer
  {
    public const string DiagnosticId = "CopyFromAnalyzer";

    // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
    // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
    private const string Category = "Naming";

    private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

    public override void Initialize(AnalysisContext context)
    {
      context.EnableConcurrentExecution();
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
      context.RegisterSemanticModelAction(EhAnalyzeSemanticModel);
    }

    private void EhAnalyzeSemanticModel(SemanticModelAnalysisContext semanticModelContext)
    {
      var model = semanticModelContext.SemanticModel;


      foreach (var node in model.SyntaxTree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>())
      {
        if (
            (node.IsKind(SyntaxKind.NotEqualsExpression) || node.IsKind(SyntaxKind.EqualsExpression)) &&

        ((node.Left is LiteralExpressionSyntax les && les.IsKind(SyntaxKind.NullLiteralExpression)) ||
                node.Right is LiteralExpressionSyntax res && res.IsKind(SyntaxKind.NullLiteralExpression))
         )
        {
          // For all such symbols, produce a diagnostic.
          var diagnostic = Diagnostic.Create(Rule, node.GetLocation(), node.IsKind(SyntaxKind.EqualsExpression) ? "==" : "!=");
          semanticModelContext.ReportDiagnostic(diagnostic);
        }
      }

    }




    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
      // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
      var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

      // Find just those named type symbols with names containing lowercase letters.
      if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
      {
        // For all such symbols, produce a diagnostic.
        var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

        context.ReportDiagnostic(diagnostic);
      }
    }
  }
}
