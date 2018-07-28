// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, Features, SignatureHelp\SignatureHelpItem.cs

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.SignatureHelp
{
  public class SignatureHelpItem
  {
    public bool IsVariadic { get; }

    public ImmutableArray<TaggedText> PrefixDisplayParts { get; }

    public ImmutableArray<TaggedText> SuffixDisplayParts { get; }

    public ImmutableArray<TaggedText> SeparatorDisplayParts { get; }

    public ImmutableArray<SignatureHelpParameter> Parameters { get; }

    public ImmutableArray<TaggedText> DescriptionParts { get; }

    public Func<CancellationToken, IEnumerable<TaggedText>> DocumentationFactory { get; }

    internal SignatureHelpItem(Microsoft.CodeAnalysis.SignatureHelp.SignatureHelpItem inner)
    {
      IsVariadic = inner.IsVariadic;
      PrefixDisplayParts = inner.PrefixDisplayParts;
      SuffixDisplayParts = inner.SuffixDisplayParts;
      SeparatorDisplayParts = inner.SeparatorDisplayParts;
      Parameters = ImmutableArray.CreateRange(inner.Parameters.Select(source => new SignatureHelpParameter(source)));
      DescriptionParts = inner.DescriptionParts;
      IsVariadic = inner.IsVariadic;
      DocumentationFactory = inner.DocumentationFactory;
    }
  }
}
