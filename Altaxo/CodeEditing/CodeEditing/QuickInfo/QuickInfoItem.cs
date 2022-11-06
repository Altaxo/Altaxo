// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Originated from: Roslyn, EditorFeatures, Core/Extensibility/QuickInfo/QuickInfoItem.cs

#if !NoQuickInfo
using System;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.QuickInfo
{
  public sealed class QuickInfoItem
  {
    private readonly Func<object> _contentFactory;

    public TextSpan TextSpan { get; }

    public object Create() => _contentFactory();

    internal QuickInfoItem(TextSpan textSpan, Func<object> contentFactory)
    {
      TextSpan = textSpan;
      _contentFactory = contentFactory;
    }
  }
}
#endif
