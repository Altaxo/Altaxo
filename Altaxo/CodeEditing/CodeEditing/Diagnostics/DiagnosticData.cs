// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn, Diagnostics/DiagnosticData.cs
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.Diagnostics;

public sealed class DiagnosticData : IEquatable<DiagnosticData>
{
  private readonly Microsoft.CodeAnalysis.Diagnostics.DiagnosticData _inner;

  internal DiagnosticData(Microsoft.CodeAnalysis.Diagnostics.DiagnosticData inner)
  {
    _inner = inner;
  }

  public string Id => _inner.Id;
  public string Category => _inner.Category;
  public string? Message => _inner.Message;
  public string? Description => _inner.Description;
  public string? Title => _inner.Title;
  public string? HelpLink => _inner.HelpLink;
  public DiagnosticSeverity Severity => _inner.Severity;
  public DiagnosticSeverity DefaultSeverity => _inner.DefaultSeverity;
  public bool IsEnabledByDefault => _inner.IsEnabledByDefault;
  public int WarningLevel => _inner.WarningLevel;
  public IReadOnlyList<string> CustomTags => _inner.CustomTags;
  public ImmutableDictionary<string, string?> Properties => _inner.Properties;
  public bool IsSuppressed => _inner.IsSuppressed;
  public ProjectId? ProjectId => _inner.ProjectId;
  public DocumentId? DocumentId => _inner.DocumentId;

  public TextSpan? GetTextSpan(SourceText sourceText) => _inner.DataLocation.MappedFileSpan.GetClampedTextSpan(sourceText);

  public bool Equals(DiagnosticData? other) => _inner.Equals(other?._inner);
  public override bool Equals(object? obj) => obj is DiagnosticData other && Equals(other);
  public override int GetHashCode() => _inner.GetHashCode();
}
