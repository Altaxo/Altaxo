// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Diagnostics/DiagnosticsAnalyzerService.cs
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.Diagnostics;

[Export(typeof(IDiagnosticAnalyzerService)), Shared]
[method: ImportingConstructor]
internal sealed class DiagnosticAnalyzerService(Microsoft.CodeAnalysis.Diagnostics.IDiagnosticAnalyzerService inner) : IDiagnosticAnalyzerService
{
  public async Task<ImmutableArray<DiagnosticData>> GetDiagnosticsForSpanAsync(TextDocument document, TextSpan? range, CancellationToken cancellationToken)
  {
    var diagnostics = await inner.GetDiagnosticsForSpanAsync(document, range, DiagnosticKind.All, cancellationToken).ConfigureAwait(false);

    return ConvertDiagnostics(diagnostics);
  }

  private static ImmutableArray<DiagnosticData> ConvertDiagnostics(ImmutableArray<Microsoft.CodeAnalysis.Diagnostics.DiagnosticData> diagnostics) =>
      diagnostics.SelectAsArray(d => new DiagnosticData(d));
}
