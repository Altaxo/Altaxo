// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, Features, Core/Portable/DocumentHighlighting/IDocumentHighlightsService.cs
#if !NoReferenceHighlighting
extern alias MCW;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using MCW::Microsoft.CodeAnalysis;
using MCW::Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.ReferenceHighlighting
{
  public enum HighlightSpanKind
  {
    None,
    Definition,
    Reference,
    WrittenReference,
  }

  public struct HighlightSpan
  {
    public TextSpan TextSpan { get; }
    public HighlightSpanKind Kind { get; }

    public HighlightSpan(TextSpan textSpan, HighlightSpanKind kind) : this()
    {
      TextSpan = textSpan;
      Kind = kind;
    }
  }

  public struct DocumentHighlights
  {
    public Document Document { get; }
    public ImmutableArray<HighlightSpan> HighlightSpans { get; }

    public DocumentHighlights(Document document, ImmutableArray<HighlightSpan> highlightSpans)
    {
      Document = document;
      HighlightSpans = highlightSpans;
    }
  }

  public interface IDocumentHighlightsService : ILanguageService
  {
    Task<ImmutableArray<DocumentHighlights>> GetDocumentHighlightsAsync(
        Document document, int position, IImmutableSet<Document> documentsToSearch, CancellationToken cancellationToken);
  }
}
#endif
