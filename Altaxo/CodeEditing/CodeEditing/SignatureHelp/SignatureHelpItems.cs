// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, Features, Core/Portable/SignatureHelp/SignatureHelpItems.cs

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.SignatureHelp
{
  public class SignatureHelpItems
  {
    public IList<SignatureHelpItem> Items { get; }

    public TextSpan ApplicableSpan { get; }

    public int ArgumentIndex { get; }

    public int ArgumentCount { get; }

    public string ArgumentName { get; }

    public int? SelectedItemIndex { get; internal set; }

    internal SignatureHelpItems(Microsoft.CodeAnalysis.SignatureHelp.SignatureHelpItems inner)
    {
      Items = inner.Items.Select(x => new SignatureHelpItem(x)).ToArray();
      ApplicableSpan = inner.ApplicableSpan;
      ArgumentIndex = inner.ArgumentIndex;
      ArgumentCount = inner.ArgumentCount;
      ArgumentName = inner.ArgumentName;
      SelectedItemIndex = inner.SelectedItemIndex;
    }
  }
}
