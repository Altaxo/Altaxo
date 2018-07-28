// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Diagnostics/DiagnosticsService.cs

using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.Diagnostics
{
  [Export(typeof(IDiagnosticService)), Shared]
  internal sealed class DiagnosticsService : IDiagnosticService
  {
    private readonly Microsoft.CodeAnalysis.Diagnostics.IDiagnosticService _inner;

    [ImportingConstructor]
    public DiagnosticsService(Microsoft.CodeAnalysis.Diagnostics.IDiagnosticService inner)
    {
      _inner = inner;
      inner.DiagnosticsUpdated += OnDiagnosticsUpdated;
    }

    // ReSharper disable once UnusedParameter.Local
    private void OnDiagnosticsUpdated(object sender, Microsoft.CodeAnalysis.Diagnostics.DiagnosticsUpdatedArgs e)
    {
      if (e.Solution.Workspace is IDiagnosticsEventSink diagnosticsEventSink)
        diagnosticsEventSink.OnDiagnosticsUpdated(this, new DiagnosticsUpdatedArgs(e));
    }

    public IEnumerable<DiagnosticData> GetDiagnostics(Workspace workspace, ProjectId projectId, DocumentId documentId, object id,
        bool includeSuppressedDiagnostics, CancellationToken cancellationToken)
    {
      return _inner.GetDiagnostics(workspace, projectId, documentId, id, includeSuppressedDiagnostics,
          cancellationToken).Select(x => new DiagnosticData(x));
    }

    public IEnumerable<UpdatedEventArgs> GetDiagnosticsUpdatedEventArgs(Workspace workspace, ProjectId projectId, DocumentId documentId,
        CancellationToken cancellationToken)
    {
      return _inner.GetDiagnosticsUpdatedEventArgs(workspace, projectId, documentId, cancellationToken)
          .Select(x => new UpdatedEventArgs(x));
    }
  }
}
