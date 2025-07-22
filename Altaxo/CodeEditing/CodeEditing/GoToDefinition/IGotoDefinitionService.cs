// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, EditorFeatures, Core/GoToDefinition/IGoToDefinitionService.cs

#if !NoGotoDefinition
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Navigation;

namespace Microsoft.CodeAnalysis.Editor
{
  internal interface IGoToDefinitionService : ILanguageService
  {
    /// <summary>
    /// Finds the definitions for the symbol at the specific position in the document.
    /// </summary>
    public Task<IEnumerable<INavigableItem>> FindDefinitionsAsync(Document document, SemanticModel semanticModel, int position, CancellationToken cancellationToken);

    /// <summary>
    /// Finds the definitions for the symbol at the specific position in the document and then 
    /// navigates to them.
    /// </summary>
    /// <returns>True if navigating to the definition of the symbol at the provided position succeeds.  False, otherwise.</returns>
    public bool TryGoToDefinition(Document document, SemanticModel semanticModel, int position, CancellationToken cancellationToken);
  }
}
#endif
