using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NullComparisonAnalyzer
{
  [DiagnosticAnalyzer(LanguageNames.CSharp)]
  public class InterfaceParameterNameAnalyzer : DiagnosticAnalyzer
  {
    public const string DiagnosticId = "LEL001";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        id: DiagnosticId,
        title: "Parameter name differs from interface declaration",
        messageFormat: "Parameter '{0}' in '{1}' should be named '{2}' to match the interface '{3}'",
        category: "Naming",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Parameter names in interface implementations should match the interface declaration."
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();

      // Analyze method declarations
      context.RegisterSyntaxNodeAction(
          AnalyzeMethodDeclaration,
          SyntaxKind.MethodDeclaration
      );
    }

    private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
      var methodDecl = (MethodDeclarationSyntax)context.Node;
      var semanticModel = context.SemanticModel;

      // Get the symbol for this method
      if (semanticModel.GetDeclaredSymbol(methodDecl) is not IMethodSymbol methodSymbol)
        return;

      // Only interested in methods that implement an interface
      var implementedInterfaceMethods = methodSymbol
          .ContainingType
          .AllInterfaces
          .SelectMany(iface => iface.GetMembers().OfType<IMethodSymbol>())
          .Where(ifaceMethod => methodSymbol
              .ContainingType
              .FindImplementationForInterfaceMember(ifaceMethod)
              ?.Equals(methodSymbol, SymbolEqualityComparer.Default) == true
          );

      foreach (var ifaceMethod in implementedInterfaceMethods)
      {
        var ifaceParams = ifaceMethod.Parameters;
        var implParams = methodSymbol.Parameters;

        // Zip the parameter lists and compare names
        for (int i = 0; i < implParams.Length && i < ifaceParams.Length; i++)
        {
          if (implParams[i].Name == ifaceParams[i].Name)
            continue;

          // Find the exact syntax location of the mismatched parameter
          var paramSyntax = methodDecl.ParameterList.Parameters[i];

          var properties = ImmutableDictionary<string, string?>.Empty
            .Add("expectedName", ifaceParams[i].Name);

          var diagnostic = Diagnostic.Create(
              Rule,
              paramSyntax.Identifier.GetLocation(),
              properties,
              implParams[i].Name,          // actual name
              methodSymbol.Name,            // method name
              ifaceParams[i].Name,          // expected name
              ifaceMethod.ContainingType.Name  // interface name
          );

          context.ReportDiagnostic(diagnostic);
        }
      }
    }
  }
}

