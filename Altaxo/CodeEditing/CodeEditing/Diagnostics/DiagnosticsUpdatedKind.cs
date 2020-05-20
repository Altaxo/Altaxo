// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Diagnostics/DiagnosticsUpdatedKind.cs

#if !NoDiagnostics

namespace Altaxo.CodeEditing.Diagnostics
{
  /// <summary>
  /// Designate the type of event used in <see cref="DiagnosticsUpdateArgs"/>
  /// </summary>
  public enum DiagnosticsUpdatedKind
  {
    DiagnosticsRemoved,
    DiagnosticsCreated
  }
}
#endif
