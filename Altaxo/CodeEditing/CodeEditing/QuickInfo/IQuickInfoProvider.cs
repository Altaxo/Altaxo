// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Originated from: Roslyn, EditorFeatures, Core/Extensibility/QuickInfo/IQuickInfoProvider.cs

#if !NoQuickInfo
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.QuickInfo
{
  public interface IQuickInfoProvider
  {
    Task<QuickInfoItem> GetItemAsync(Document document, int position, CancellationToken cancellationToken);
  }
}
#endif
