﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, EditorFeatures, Core/GoToDefinition/AbstractGoToDefinitionService.cs

#if !NoGotoDefinition
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.GoToDefinition;
using Microsoft.CodeAnalysis.LanguageService;
using Microsoft.CodeAnalysis.Navigation;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Editor.GoToDefinition
{
  // GoToDefinition
  internal abstract class AbstractGoToDefinitionService : IGoToDefinitionService
  {
#if NoModificationForAltaxoCodeEditing
        private readonly IEnumerable<Lazy<IStreamingFindUsagesPresenter>> _streamingPresenters;

    protected AbstractGoToDefinitionService(
            IEnumerable<Lazy<IStreamingFindUsagesPresenter>> streamingPresenters
        )
    {
      _streamingPresenters = streamingPresenters;
    }
#endif

    public async Task<IEnumerable<INavigableItem>> FindDefinitionsAsync(Document document, SemanticModel semanticModel, int position, CancellationToken cancellationToken)
    {
      var symbolService = document.GetLanguageService<IGoToDefinitionSymbolService>();
      var (symbol, project, span) = await symbolService.GetSymbolProjectAndBoundSpanAsync(document, semanticModel, position, cancellationToken).ConfigureAwait(false);

      // Try to compute source definitions from symbol.
      var items = symbol != null
          ? NavigableItemFactory.GetItemsFromPreferredSourceLocations(document.Project.Solution, symbol, displayTaggedParts: null, cancellationToken: cancellationToken)
          : ImmutableArray<INavigableItem>.Empty;

      // realize the list here so that the consumer await'ing the result doesn't lazily cause
      // them to be created on an inappropriate thread.
      return items.ToList();
    }

    public bool TryGoToDefinition(Document document, SemanticModel semanticModel, int position, CancellationToken cancellationToken)
    {
      // Try to compute the referenced symbol and attempt to go to definition for the symbol.
      var symbolService = document.GetLanguageService<IGoToDefinitionSymbolService>();
      var (symbol, _, _) = symbolService.GetSymbolProjectAndBoundSpanAsync(document, semanticModel, position, cancellationToken).WaitAndGetResult(cancellationToken);
      if (symbol is null)
      {
        return false;
      }

      var isThirdPartyNavigationAllowed = IsThirdPartyNavigationAllowed(symbol, position, document, cancellationToken);

      return GoToDefinitionHelpers.TryGoToDefinition(symbol,
          document.Project,
#if NoModificationForAltaxoCodeEditing
          _streamingPresenters,
#else
          null,
#endif
          thirdPartyNavigationAllowed: isThirdPartyNavigationAllowed,
          throwOnHiddenDefinition: true,
          cancellationToken: cancellationToken);
    }

    private static bool IsThirdPartyNavigationAllowed(ISymbol symbolToNavigateTo, int caretPosition, Document document, CancellationToken cancellationToken)
    {
      var syntaxRoot = document.GetSyntaxRootSynchronously(cancellationToken);
      var syntaxFactsService = document.GetLanguageService<ISyntaxFactsService>();
      var containingTypeDeclaration = syntaxFactsService.GetContainingTypeDeclaration(syntaxRoot, caretPosition);

      if (containingTypeDeclaration != null)
      {
        var semanticModel = document.GetSemanticModelAsync(cancellationToken).WaitAndGetResult(cancellationToken);

        // Allow third parties to navigate to all symbols except types/constructors
        // if we are navigating from the corresponding type.

        if (semanticModel.GetDeclaredSymbol(containingTypeDeclaration, cancellationToken) is ITypeSymbol containingTypeSymbol &&
    (symbolToNavigateTo is ITypeSymbol || symbolToNavigateTo.IsConstructor()))
        {
          var candidateTypeSymbol = symbolToNavigateTo is ITypeSymbol
              ? symbolToNavigateTo
              : symbolToNavigateTo.ContainingType;

          if (Equals(containingTypeSymbol, candidateTypeSymbol))
          {
            // We are navigating from the same type, so don't allow third parties to perform the navigation.
            // This ensures that if we navigate to a class from within that class, we'll stay in the same file
            // rather than navigate to, say, XAML.
            return false;
          }
        }
      }

      return true;
    }
  }
}
#endif
