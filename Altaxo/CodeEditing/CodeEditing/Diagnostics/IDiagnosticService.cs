// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Diagnostics/IDiagnosticService.cs

#if !NoDiagnostics

extern alias MCW;
using System;
using System.Collections.Generic;
using System.Threading;
using MCW::Microsoft.CodeAnalysis;
using MCW::Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Common;

namespace Altaxo.CodeEditing.Diagnostics
{
  /// <summary>
  /// Interface to the diagnostics service. Workspaces that
  /// want to receive diagnostic messages have i) to implement <see cref="IDiagnosticsEventSink"/> and ii)
  /// register with the diagnostics by calling <see cref="DiagnosticProvider.Enable"/>.
  /// </summary>
  public interface IDiagnosticService
  {
    internal IEnumerable<DiagnosticData> GetDiagnostics(Workspace workspace, ProjectId projectId, DocumentId documentId, object id, bool includeSuppressedDiagnostics, CancellationToken cancellationToken);
  }
}
#endif
