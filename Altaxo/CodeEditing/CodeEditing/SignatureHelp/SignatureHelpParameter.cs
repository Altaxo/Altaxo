// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, Features, Core/Portable/SignatureHelp/SignatureHelpParameter.cs

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.SignatureHelp
{
  public class SignatureHelpParameter
  {
    public string Name { get; }

    public Func<CancellationToken, IEnumerable<TaggedText>> DocumentationFactory { get; }

    public IList<TaggedText> PrefixDisplayParts { get; }

    public IList<TaggedText> SuffixDisplayParts { get; }

    public IList<TaggedText> DisplayParts { get; }

    public bool IsOptional { get; }

    public IList<TaggedText> SelectedDisplayParts { get; }

    internal SignatureHelpParameter(Microsoft.CodeAnalysis.SignatureHelp.SignatureHelpParameter inner)
    {
      Name = inner.Name;
      DocumentationFactory = inner.DocumentationFactory;
      PrefixDisplayParts = inner.PrefixDisplayParts;
      SuffixDisplayParts = inner.SuffixDisplayParts;
      DisplayParts = inner.DisplayParts;
      IsOptional = inner.IsOptional;
      SelectedDisplayParts = inner.SelectedDisplayParts;
    }
  }
}
