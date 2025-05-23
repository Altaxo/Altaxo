﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, EditorFeatures, Core/GoToDefinition/GoToDefinitionHelpers.cs
#if !NoGotoDefinition
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editor.FindUsages;
using Microsoft.CodeAnalysis.Editor.Host;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.FindUsages;
using Microsoft.CodeAnalysis.Navigation;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Editor.GoToDefinition
{
  public static class GoToDefinitionHelpers
  {
    internal static ImmutableArray<DefinitionItem> GetDefinitions(
    ISymbol symbol,
    Project project,
        bool thirdPartyNavigationAllowed,
        CancellationToken cancellationToken)
    {
      var alias = symbol as IAliasSymbol;
      if (alias != null)
      {
        if (alias.Target is INamespaceSymbol ns && ns.IsGlobalNamespace)
        {
          return ImmutableArray.Create<DefinitionItem>();
        }
      }

      // VB global import aliases have a synthesized SyntaxTree.
      // We can't go to the definition of the alias, so use the target type.

      var solution = project.Solution;
      if (alias != null)
      {
        var sourceLocations = NavigableItemFactory.GetPreferredSourceLocations(
            solution, symbol, cancellationToken);

        if (sourceLocations.All(l => project.Solution.GetDocument(l.SourceTree) == null))
        {
          symbol = alias.Target;
        }
      }

      var definition = SymbolFinder.FindSourceDefinitionAsync(symbol, solution, cancellationToken).WaitAndGetResult(cancellationToken);
      cancellationToken.ThrowIfCancellationRequested();

      symbol = definition ?? symbol;

      // If it is a partial method declaration with no body, choose to go to the implementation
      // that has a method body.
      if (symbol is IMethodSymbol method)
      {
        symbol = method.PartialImplementationPart ?? symbol;
      }

      var definitions = ArrayBuilder<DefinitionItem>.GetInstance();

      // Going to a symbol may end up actually showing the symbol in the Find-Usages window.
      // This happens when there is more than one location for the symbol (i.e. for partial
      // symbols) and we don't know the best place to take you to.
      //
      // The FindUsages window supports showing the classified text for an item.  It does this
      // in two ways.  Either the item can pass along its classified text (and the window will
      // defer to that), or the item will have no classified text, and the window will compute
      // it in the BG.
      //
      // Passing along the classified information is valuable for OOP scenarios where we want
      // all that expensive computation done on the OOP side and not in the VS side.
      //
      // However, Go To Definition is all in-process, and is also synchronous.  So we do not
      // want to fetch the classifications here.  It slows down the command and leads to a
      // measurable delay in our perf tests.
      //
      // So, if we only have a single location to go to, this does no unnecessary work.  And,
      // if we do have multiple locations to show, it will just be done in the BG, unblocking
      // this command thread so it can return the user faster.
      var definitionItem = symbol.ToNonClassifiedDefinitionItem(project, includeHiddenLocations: true);

      if (thirdPartyNavigationAllowed)
      {
        var factory = solution.Workspace.Services.GetService<IDefinitionsAndReferencesFactory>();
        var thirdPartyItem = factory?.GetThirdPartyDefinitionItemAsync(solution, definitionItem, cancellationToken).Result;
        definitions.AddIfNotNull(thirdPartyItem);
      }

      definitions.Add(definitionItem);
      return definitions.ToImmutableAndFree();
    }

    internal static bool TryGoToDefinition(
           ISymbol symbol,
           Project project,
           IEnumerable<Lazy<IStreamingFindUsagesPresenter>> streamingPresenters,
           CancellationToken cancellationToken,
           bool thirdPartyNavigationAllowed = true,
           bool throwOnHiddenDefinition = false)
    {
      var definitions = GetDefinitions(symbol, project, thirdPartyNavigationAllowed, cancellationToken);

      var presenter = streamingPresenters.FirstOrDefault()?.Value;
      var title = string.Format("_0_declarations",
          Microsoft.CodeAnalysis.FindUsages.FindUsagesHelpers.GetDisplayName(symbol));

      return presenter.TryNavigateToOrPresentItemsAsync(
          project.Solution.Workspace, title, definitions).WaitAndGetResult(cancellationToken);
    }

    internal static bool TryGoToDefinition(
     ImmutableArray<DefinitionItem> definitions,
     Project project,
     string title,
     IEnumerable<Lazy<IStreamingFindUsagesPresenter>> streamingPresenters,
     CancellationToken cancellationToken)
    {
      if (definitions.IsDefaultOrEmpty)
      {
        return false;
      }

      var presenter = streamingPresenters.FirstOrDefault()?.Value;

      return presenter.TryNavigateToOrPresentItemsAsync(
          project.Solution.Workspace, title, definitions).WaitAndGetResult(cancellationToken);
    }
  }
}
#endif
