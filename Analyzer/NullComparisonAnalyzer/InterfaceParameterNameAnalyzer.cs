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
  public class InterfaceParameterNameAnalyzer : DiagnosticAnalyzer
  {
    public const string DiagnosticId = "LED001";
    public const string ExpectedNameKey = "expectedName";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        id: DiagnosticId,
        title: "Parameter name differs from base or interface declaration",
        messageFormat: "Parameter '{0}' in '{1}' should be named '{2}' to match '{3}'",
        category: "Documentation",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Parameter names in interface implementations and method overrides should match the original declaration."
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
      context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
      context.EnableConcurrentExecution();

      context.RegisterSyntaxNodeAction(
          AnalyzeMethodDeclaration,
          SyntaxKind.MethodDeclaration
      );
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
      var methodDecl = (MethodDeclarationSyntax)context.Node;
      var semanticModel = context.SemanticModel;

      if (semanticModel.GetDeclaredSymbol(methodDecl) is not IMethodSymbol methodSymbol)
        return;

      // Build a list of (baseMethod, sourceName) pairs to check against
      var baseMethods = new List<(IMethodSymbol BaseMethod, string SourceName)>();

      // 1. Interface implementations
      var implementedInterfaceMethods = methodSymbol
          .ContainingType
          .AllInterfaces
          .SelectMany(iface => iface.GetMembers().OfType<IMethodSymbol>())
          .Where(ifaceMethod => methodSymbol
              .ContainingType
              .FindImplementationForInterfaceMember(ifaceMethod)
              ?.Equals(methodSymbol, SymbolEqualityComparer.Default) == true);

      foreach (var ifaceMethod in implementedInterfaceMethods)
        baseMethods.Add((ifaceMethod, ifaceMethod.ContainingType.Name));

      // 2. Override chain — walk up to the original (root) declaration
      if (GetUltimateBaseMethod(methodSymbol) is IMethodSymbol baseMethod)
        baseMethods.Add((baseMethod, $"{baseMethod.ContainingType.Name} (base)"));

      // Compare parameters against each base method found
      foreach (var (baseOrIfaceMethod, sourceName) in baseMethods)
      {
        var baseParams = baseOrIfaceMethod.Parameters;
        var implParams = methodSymbol.Parameters;

        for (int i = 0; i < implParams.Length && i < baseParams.Length; i++)
        {
          if (implParams[i].Name == baseParams[i].Name)
            continue;

          var paramSyntax = methodDecl.ParameterList.Parameters[i];

          var properties = ImmutableDictionary<string, string?>.Empty
              .Add(ExpectedNameKey, baseParams[i].Name);

          var diagnostic = Diagnostic.Create(
              Rule,
              paramSyntax.Identifier.GetLocation(),
              properties,
              implParams[i].Name,             // {0} actual name
              methodSymbol.Name,              // {1} method name
              baseParams[i].Name,             // {2} expected name
              sourceName                      // {3} interface or base class name
          );

          context.ReportDiagnostic(diagnostic);
        }
      }
    }

    private static IMethodSymbol? GetUltimateBaseMethod(IMethodSymbol method)
    {
      var current = method;
      while (current.IsOverride && current.OverriddenMethod is not null)
        current = current.OverriddenMethod;

      // Return null if the method is not an override at all
      return current.Equals(method, SymbolEqualityComparer.Default) ? null : current;
    }
  }
}

