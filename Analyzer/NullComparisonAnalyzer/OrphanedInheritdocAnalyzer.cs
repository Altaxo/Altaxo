using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NullComparisonAnalyzer
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public class OrphanedInheritdocAnalyzer : DiagnosticAnalyzer
  {
    public const string DiagnosticId = "LED002";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        id: DiagnosticId,
        title: "Member with <inheritdoc/> does not inherit or implement anything",
        messageFormat: "'{0}' uses <inheritdoc/> but does not override a base member or implement an interface member",
        category: "Documentation",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();

      context.RegisterSyntaxNodeAction(Analyze,
          SyntaxKind.MethodDeclaration,
          SyntaxKind.PropertyDeclaration,
          SyntaxKind.EventDeclaration,
          SyntaxKind.ConstructorDeclaration,
          SyntaxKind.IndexerDeclaration);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
      var memberDecl = (MemberDeclarationSyntax)context.Node;

      // Check if the member has an <inheritdoc/> XML doc comment
      if (!HasInheritDoc(memberDecl))
        return;

      // Get the symbol for this member
      if (context.SemanticModel.GetDeclaredSymbol(memberDecl) is not ISymbol symbol)
        return;

      // If there is something to inherit from, all is fine
      if (HasInheritanceSource(symbol))
        return;

      // Find the location of the <inheritdoc/> tag for precise squiggling
      var location = GetInheritDocLocation(memberDecl) ?? memberDecl.GetLocation();

      context.ReportDiagnostic(Diagnostic.Create(Rule, location, symbol.Name));
    }

    private static bool HasInheritDoc(MemberDeclarationSyntax memberDecl)
    {
      return memberDecl
          .GetLeadingTrivia()
          .Where(t => t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                   || t.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
          .Select(t => t.GetStructure())
          .OfType<DocumentationCommentTriviaSyntax>()
          .SelectMany(doc => doc.ChildNodes().OfType<XmlEmptyElementSyntax>())
          .Any(el => el.Name.LocalName.Text == "inheritdoc"
                  && !el.Attributes
                        .OfType<XmlCrefAttributeSyntax>()
                        .Any());
    }



    private static bool HasInheritanceSource(ISymbol symbol)
    {
      return symbol switch
      {
        IMethodSymbol method =>
            method.IsOverride ||
            method.ExplicitInterfaceImplementations.Any() ||
            IsImplicitInterfaceImplementation(method),

        IPropertySymbol property =>
            property.IsOverride ||
            property.ExplicitInterfaceImplementations.Any() ||
            IsImplicitInterfaceImplementation(property),

        IEventSymbol evt =>
            evt.IsOverride ||
            evt.ExplicitInterfaceImplementations.Any() ||
            IsImplicitInterfaceImplementation(evt),

        INamedTypeSymbol => false, // constructors can't inherit

        _ => false
      };
    }

    private static bool IsImplicitInterfaceImplementation(ISymbol symbol)
    {
      return symbol.ContainingType
          .AllInterfaces
          .SelectMany(iface => iface.GetMembers())
          .Any(ifaceMember =>
              symbol.ContainingType
                  .FindImplementationForInterfaceMember(ifaceMember)
                  ?.Equals(symbol, SymbolEqualityComparer.Default) == true);
    }

    private static Location? GetInheritDocLocation(MemberDeclarationSyntax memberDecl)
    {
      return memberDecl
          .GetLeadingTrivia()
          .Where(t => t.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                   || t.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
          .Select(t => t.GetStructure())
          .OfType<DocumentationCommentTriviaSyntax>()
          .SelectMany(doc => doc.ChildNodes().OfType<XmlEmptyElementSyntax>())
          .Where(el => el.Name.LocalName.Text == "inheritdoc"
                    && !el.Attributes
                          .OfType<XmlCrefAttributeSyntax>()
                          .Any())
          .Select(el => el.GetLocation())
          .FirstOrDefault();
    }
  }
}
