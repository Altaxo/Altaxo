// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Diagnostics/DiagnosticsUpdatedArgs.cs
#if !NoDiagnostics
extern alias MCW;
using System.Collections.Immutable;
using System.Linq;
using MCW::Microsoft.CodeAnalysis;
using MCW::Microsoft.CodeAnalysis.Diagnostics;

namespace Altaxo.CodeEditing.Diagnostics
{
  public class DiagnosticsUpdatedArgs : UpdatedEventArgs
  {
    public DiagnosticsUpdatedKind Kind { get; }
    public Solution Solution { get; }
    internal ImmutableArray<DiagnosticData> Diagnostics { get; }

    internal DiagnosticsUpdatedArgs(Microsoft.CodeAnalysis.Diagnostics.DiagnosticsUpdatedArgs inner) : base(inner)
    {
      Solution = inner.Solution;
      Diagnostics = inner.Diagnostics.Select(x => new DiagnosticData(x)).ToImmutableArray();
      Kind = (DiagnosticsUpdatedKind)inner.Kind;
    }
  }
}
#endif
