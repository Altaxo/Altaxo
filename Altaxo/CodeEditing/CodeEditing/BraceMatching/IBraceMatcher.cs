// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, EditorFeatures, Extensibility\BraceMatching\IBraceMatcher.cs

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.BraceMatching
{
  public interface IBraceMatcher
  {
    Task<BraceMatchingResult?> FindBracesAsync(Document document, int position, CancellationToken cancellationToken = default(CancellationToken));
  }
}
