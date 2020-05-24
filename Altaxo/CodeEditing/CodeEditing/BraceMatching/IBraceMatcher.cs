// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, EditorFeatures, Core/Extensibility/BraceMatching/IBraceMatcher.cs

#if !NoBraceMatching
extern alias MCW;
using System.Threading;
using System.Threading.Tasks;
using MCW::Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.BraceMatching
{
  internal interface IBraceMatcher
  {
    Task<BraceMatchingResult?> FindBracesAsync(Document document, int position, CancellationToken cancellationToken = default);
  }
}
#endif
