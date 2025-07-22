// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Diagnostics/DiagnosticsChangedArgs.cs
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.Diagnostics;

public record DiagnosticsChangedArgs(DocumentId DocumentId, HashSet<DiagnosticData> AddedDiagnostics, HashSet<DiagnosticData> RemovedDiagnostics);
