// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
// Originated from: Roslyn, EditorFeatures, CSharp/GoToDefinition/CSharpGoToDefinitionSymbolService.cs
#if !NoGotoDefinition
extern alias MCW;
using System;
using System.Collections.Generic;
using System.Composition;
using MCW::Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Editor.GoToDefinition;
using Microsoft.CodeAnalysis.Editor.Host;

namespace Microsoft.CodeAnalysis.Editor.CSharp.GoToDefinition
{
  [ExportLanguageService(typeof(IGoToDefinitionSymbolService), LanguageNames.CSharp), Shared]
  internal class CSharpGoToDefinitionSymbolService : AbstractGoToDefinitionSymbolService
  {
        [ImportingConstructor]
        public CSharpGoToDefinitionSymbolService()
        {
        }

    protected override ISymbol FindRelatedExplicitlyDeclaredSymbol(ISymbol symbol, Compilation compilation)
    {
      return symbol;
    }
  }
}
#endif
