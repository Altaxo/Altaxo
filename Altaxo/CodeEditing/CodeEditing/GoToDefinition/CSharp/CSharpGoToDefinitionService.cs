// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, EditorFeatures, CSharp/GoToDefinition/CSharpGoToDefinitionService.cs

#if !NoGotoDefinition
using System.Composition;
using Microsoft.CodeAnalysis.Editor.GoToDefinition;
using Microsoft.CodeAnalysis.Host.Mef;

namespace Microsoft.CodeAnalysis.Editor.CSharp.GoToDefinition
{
  [ExportLanguageService(typeof(IGoToDefinitionService), LanguageNames.CSharp), Shared]
  internal class CSharpGoToDefinitionService : AbstractGoToDefinitionService
  {
#if NoModificationForAltaxoCodeEditing // can not use this constructor because we don't have streamingPresenters
    [ImportingConstructor]
		public CSharpGoToDefinitionService(
				[ImportMany]IEnumerable<Lazy<IStreamingFindUsagesPresenter>> streamingPresenters)
				: base(streamingPresenters)
		{
		}
#endif
  }
}
#endif
