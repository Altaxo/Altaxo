// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Diagnostics/IDiagnosticsUpdater.cs
using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Host;

namespace Altaxo.CodeEditing.Diagnostics;

public interface IDiagnosticsUpdater : IWorkspaceService
{
  public ImmutableHashSet<string> DisabledDiagnostics { get; set; }

  public event Action<DiagnosticsChangedArgs>? DiagnosticsChanged;
}
