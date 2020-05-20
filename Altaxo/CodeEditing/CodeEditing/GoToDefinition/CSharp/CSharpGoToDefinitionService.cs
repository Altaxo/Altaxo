// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, EditorFeatures, CSharp/GoToDefinition/CSharpGoToDefinitionService.cs

#if !NoGotoDefinition
extern alias MCW;
using System;
using System.Collections.Generic;
using System.Composition;
using MCW::Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editor.GoToDefinition;

namespace Microsoft.CodeAnalysis.Editor.CSharp.GoToDefinition
{
  [ExportLanguageService(typeof(IGoToDefinitionService), LanguageNames.CSharp), Shared]
  internal class CSharpGoToDefinitionService : AbstractGoToDefinitionService
  {
    /*
		[ImportingConstructor]
		public CSharpGoToDefinitionService(
				[ImportMany]IEnumerable<Lazy<IStreamingFindUsagesPresenter>> streamingPresenters)
				: base(streamingPresenters)
		{
		}
		*/
  }
}
#endif
