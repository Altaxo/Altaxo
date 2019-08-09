// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Diagnostics/IDiagnosticService.cs

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.Diagnostics
{
  /// <summary>
  /// Interface to the diagnostics service. Workspaces that
  /// want to receive diagnostic messages have i) to implement <see cref="IDiagnosticsEventSink"/> and ii)
  /// register with the diagnostics by calling <see cref="DiagnosticProvider.Enable"/>.
  /// </summary>
  public interface IDiagnosticService
  {
    IEnumerable<DiagnosticData> GetDiagnostics(Workspace workspace, ProjectId projectId, DocumentId documentId, object id, bool includeSuppressedDiagnostics, CancellationToken cancellationToken);

    IEnumerable<UpdatedEventArgs> GetDiagnosticsUpdatedEventArgs(Workspace workspace, ProjectId projectId, DocumentId documentId, CancellationToken cancellationToken);
  }
}
