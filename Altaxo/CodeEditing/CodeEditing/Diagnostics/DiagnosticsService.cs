// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Diagnostics/DiagnosticsService.cs

#if !NoDiagnostics

extern alias MCW;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using MCW::Microsoft.CodeAnalysis;
using MCW::Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Common;

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

    private void OnDiagnosticsUpdated(object sender, Microsoft.CodeAnalysis.Diagnostics.DiagnosticsUpdatedArgs e)
    {
      if (e.Solution.Workspace is IDiagnosticsEventSink diagnosticsEventSink)
        diagnosticsEventSink.OnDiagnosticsUpdated(this, e);
    }

    IEnumerable<DiagnosticData> IDiagnosticService.GetDiagnostics(Workspace workspace, ProjectId projectId, DocumentId documentId, object id,
        bool includeSuppressedDiagnostics, CancellationToken cancellationToken)
    {
      return _inner.GetDiagnostics(workspace, projectId, documentId, id, includeSuppressedDiagnostics,
          cancellationToken);
    }
  }
}
#endif
