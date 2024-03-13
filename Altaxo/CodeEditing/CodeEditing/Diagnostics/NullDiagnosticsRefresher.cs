#region Copyright

// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Diagnostics/NullDiagnosticRefresher.cs

#endregion Copyright


using Microsoft.CodeAnalysis.Diagnostics;

namespace Altaxo.CodeEditing.Diagnostics
{
  [System.Composition.Export(typeof(IDiagnosticsRefresher))]
  internal class NullDiagnosticsRefresher : IDiagnosticsRefresher
  {
    public int GlobalStateVersion { get; }

    public void RequestWorkspaceRefresh()
    {
    }
  }
}
