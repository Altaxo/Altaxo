// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
// Originated from: Roslyn, EditorFeatures, Core/GoToDefinition/IGoToDefinitionSymbolService.cs

#if !NoGotoDefinition
extern alias MCW;
using System.Threading;
using System.Threading.Tasks;
using MCW::Microsoft.CodeAnalysis;
using MCW::Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.Editor.GoToDefinition
{
  internal interface IGoToDefinitionSymbolService : ILanguageService
  {
    Task<(ISymbol, TextSpan)> GetSymbolAndBoundSpanAsync(Document document, int position, bool includeType, CancellationToken cancellationToken);
  }
}
#endif
