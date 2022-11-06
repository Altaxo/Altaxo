// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Originated from: Roslyn, EditorFeatures, Core/Extensibility/BraceMatching/IBraceMatcher.cs

#if !NoBraceMatching
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.BraceMatching
{
  internal interface IBraceMatcher
  {
    Task<BraceMatchingResult?> FindBracesAsync(Document document, int position, CancellationToken cancellationToken = default);
  }
}
#endif
