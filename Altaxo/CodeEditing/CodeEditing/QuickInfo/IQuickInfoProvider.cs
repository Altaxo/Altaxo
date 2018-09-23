// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, EditorFeatures, Core/Extensibility/QuickInfo/IQuickInfoProvider.cs

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.QuickInfo
{
  public interface IQuickInfoProvider
  {
    Task<QuickInfoItem> GetItemAsync(Document document, int position, CancellationToken cancellationToken);
  }
}
