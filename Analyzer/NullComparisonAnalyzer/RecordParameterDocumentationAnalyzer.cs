using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NullComparisonAnalyzer
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public class RecordParameterDocumentationAnalyzer : DiagnosticAnalyzer
  {
    public const string DiagnosticId = "LED003";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        id: DiagnosticId,
        title: "Positional record parameter is missing documentation",
        messageFormat: "Parameter '{0}' of record '{1}' is missing a <param> documentation tag",
        category: "Documentation",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "All positional parameters of a record should have a <param> documentation tag."
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();

      context.RegisterSyntaxNodeAction(
          Analyze,
          SyntaxKind.RecordDeclaration,
          SyntaxKind.RecordStructDeclaration
      );
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
      var recordDecl = (RecordDeclarationSyntax)context.Node;

      // Only interested in records with a positional parameter list
      if (recordDecl.ParameterList is null || recordDecl.ParameterList.Parameters.Count == 0)
        return;

      // Must have an XML doc comment at all — no doc comment means a
      // different rule (missing documentation entirely) should fire instead
      var docComment = recordDecl
          .GetLeadingTrivia()
          .Select(t => t.GetStructure())
          .OfType<DocumentationCommentTriviaSyntax>()
          .FirstOrDefault();

      if (docComment is null)
        return;

      // Collect all <param name="..."> tags that are already present
      var documentedParamNames = new HashSet<string>(
    docComment
        .ChildNodes()
        .OfType<XmlElementSyntax>()
        .Where(el => el.StartTag.Name.LocalName.Text == "param")
        .Select(el => el.StartTag.Attributes
            .OfType<XmlNameAttributeSyntax>()
            .FirstOrDefault()?.Identifier.Identifier.Text)
        .Where(name => name is not null)
        .Select(name => name!),   // null-forgiving cast after the Where filter
    StringComparer.Ordinal
);

      // Report a diagnostic for each parameter that has no <param> tag
      foreach (var parameter in recordDecl.ParameterList.Parameters)
      {
        var paramName = parameter.Identifier.Text;

        if (documentedParamNames.Contains(paramName))
          continue;

        context.ReportDiagnostic(Diagnostic.Create(
            Rule,
            parameter.Identifier.GetLocation(),
            paramName,
            recordDecl.Identifier.Text
        ));
      }
    }
  }
}
