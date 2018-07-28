// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, CSharpEditorFeatures, BraceMatching/AbstractCSharpBraceMatcher.cs

using Microsoft.CodeAnalysis.CSharp;

namespace Altaxo.CodeEditing.BraceMatching.CSharp
{
  internal abstract class AbstractCSharpBraceMatcher : AbstractBraceMatcher
  {
    protected AbstractCSharpBraceMatcher(SyntaxKind openBrace, SyntaxKind closeBrace)
        : base(new BraceCharacterAndKind(SyntaxFacts.GetText(openBrace)[0], (int)openBrace),
               new BraceCharacterAndKind(SyntaxFacts.GetText(closeBrace)[0], (int)closeBrace))
    {
    }
  }
}
