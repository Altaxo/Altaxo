// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Diagnostics/DiagnosticDataLocation.cs

#if !NoDiagnostics

extern alias MCW;
using MCW::Microsoft.CodeAnalysis;
using MCW::Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.Diagnostics
{
  public sealed class DiagnosticDataLocation
  {
    private readonly DiagnosticDataLocation _inner;

    public DocumentId DocumentId => _inner.DocumentId;

    public TextSpan? SourceSpan => _inner.SourceSpan;

    public string MappedFilePath => _inner.MappedFilePath;
    public int MappedStartLine => _inner.MappedStartLine;
    public int MappedStartColumn => _inner.MappedStartColumn;
    public int MappedEndLine => _inner.MappedEndLine;
    public int MappedEndColumn => _inner.MappedEndColumn;
    public string OriginalFilePath => _inner.OriginalFilePath;
    public int OriginalStartLine => _inner.OriginalStartLine;
    public int OriginalStartColumn => _inner.OriginalStartColumn;
    public int OriginalEndLine => _inner.OriginalEndLine;
    public int OriginalEndColumn => _inner.OriginalEndColumn;

    internal DiagnosticDataLocation(DiagnosticDataLocation inner)
    {
      _inner = inner;
    }
  }
}
#endif
