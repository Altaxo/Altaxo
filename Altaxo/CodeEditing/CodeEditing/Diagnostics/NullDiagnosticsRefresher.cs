#region Copyright

// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Diagnostics/NullDiagnosticRefresher.cs

#endregion Copyright

#if !NoDiagnostics

using System;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Altaxo.CodeEditing.Diagnostics;

[System.Composition.Export(typeof(IDiagnosticsRefresher))]
internal class NullDiagnosticsRefresher : IDiagnosticsRefresher
{
  public int GlobalStateVersion { get; }

  public event Action? WorkspaceRefreshRequested;

  public void RequestWorkspaceRefresh()
  {
    WorkspaceRefreshRequested?.Invoke();
  }
}
#endif
