#nullable enable
#if false
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
  public class StaticMemberAnalyzer : DiagnosticAnalyzer
  {
    public const string DiagnosticId = "StaticMemberAnalyzer_Title";

    // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
    // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.StaticMemberAnalyzer_Title), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.StaticMemberAnalyzer_MessageFormat), Resources.ResourceManager, typeof(Resources));
    private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.StaticMemberAnalyzer_Description), Resources.ResourceManager, typeof(Resources));
    private const string Category = "Naming";

    private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

    public override void Initialize(AnalysisContext context)
    {
      context.EnableConcurrentExecution();
      context.RegisterSemanticModelAction(EhAnalyzeSemanticModel);
    }

    private void EhAnalyzeSemanticModel(SemanticModelAnalysisContext semanticModelContext)
    {
      var model = semanticModelContext.SemanticModel;


      foreach (var node in model.SyntaxTree.GetRoot().DescendantNodes().OfType<FieldDeclarationSyntax>())
      {
        if (!(node.IsKind(SyntaxKind.FieldDeclaration)))
          continue;

        if (!node.Modifiers.Any(t => t.ValueText == "static"))
          continue;

        if (node.Modifiers.Any(t => t.ValueText == "readonly"))
          continue;

        // For all such symbols, produce a diagnostic.
        var diagnostic = Diagnostic.Create(Rule, node.GetLocation(), string.Empty);
        semanticModelContext.ReportDiagnostic(diagnostic);
      }


      foreach (var node in model.SyntaxTree.GetRoot().DescendantNodes().OfType<PropertyDeclarationSyntax>())
      {
        if (!(node.IsKind(SyntaxKind.PropertyDeclaration)))
          continue;

        if (!node.Modifiers.Any(t => t.ValueText == "static"))
          continue;

        // For all such symbols, produce a diagnostic.
        var diagnostic = Diagnostic.Create(Rule, node.GetLocation(), string.Empty);
        semanticModelContext.ReportDiagnostic(diagnostic);
      }

    }
  }
}
#endif
