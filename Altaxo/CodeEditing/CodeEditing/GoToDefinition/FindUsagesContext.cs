﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
// Originated from: Roslyn, EditorFeatures, Core/FindUsages/FindUsagesContext.cs

#if !NoGotoDefinition
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.CodeAnalysis.FindUsages
{
  internal abstract class FindUsagesContext : IFindUsagesContext
  {
    public virtual CancellationToken CancellationToken { get; }

    protected FindUsagesContext()
    {
    }

    public virtual Task ReportMessageAsync(string message) => Task.CompletedTask;

    public virtual Task SetSearchTitleAsync(string title) => Task.CompletedTask;

    public virtual Task OnCompletedAsync() => Task.CompletedTask;

    public virtual Task OnDefinitionFoundAsync(DefinitionItem definition) => Task.CompletedTask;

    public virtual Task OnReferenceFoundAsync(SourceReferenceItem reference) => Task.CompletedTask;

    public virtual Task ReportProgressAsync(int current, int maximum) => Task.CompletedTask;
  }
}
#endif
