// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, EditorFeatures, Core/GoToDefinition/GoToDefinitionFeatureHelpers.cs
#if !NoGotoDefinition
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editor.Host;
using Microsoft.CodeAnalysis.GoToDefinition;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Editor.GoToDefinition
{
  public static class GoToDefinitionHelpers
  {
    internal static bool TryGoToDefinition(
           ISymbol symbol,
           Project project,
           IEnumerable<Lazy<IStreamingFindUsagesPresenter>> streamingPresenters,
           CancellationToken cancellationToken,
           bool thirdPartyNavigationAllowed = true,
           bool throwOnHiddenDefinition = false)
    {
      var definitions = GoToDefinitionFeatureHelpers.GetDefinitionsAsync(symbol, project.Solution, thirdPartyNavigationAllowed, cancellationToken).Result;

      var presenter = streamingPresenters.FirstOrDefault()?.Value;
      var title = string.Format("_0_declarations",
          Microsoft.CodeAnalysis.FindUsages.FindUsagesHelpers.GetDisplayName(symbol));

      return presenter.TryNavigateToOrPresentItemsAsync(
          project.Solution.Workspace, title, definitions).WaitAndGetResult(cancellationToken);
    }


  }
}
#endif
