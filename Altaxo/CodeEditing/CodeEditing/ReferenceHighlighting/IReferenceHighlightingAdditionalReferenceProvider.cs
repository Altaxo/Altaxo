// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, EditorFeatures, Implementation\ReferenceHighlighting\IReferenceHighlightingAdditionalReferenceProvider.cs

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.ReferenceHighlighting
{
  internal interface IReferenceHighlightingAdditionalReferenceProvider : ILanguageService
  {
    Task<IEnumerable<Location>> GetAdditionalReferencesAsync(Document document, ISymbol symbol, CancellationToken cancellationToken);
  }
}
