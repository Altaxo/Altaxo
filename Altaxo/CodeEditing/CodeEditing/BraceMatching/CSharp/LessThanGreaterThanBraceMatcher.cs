// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Originated from: Roslyn, EditorFeatures, CSharp/BraceMatching/LessThanGreaterThanBraceMatcher.cs

#if !NoBraceMatching
using System;
using System.ComponentModel.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Host.Mef;

namespace Altaxo.CodeEditing.BraceMatching.CSharp
{
  [ExportBraceMatcher(LanguageNames.CSharp)]
  internal class LessThanGreaterThanBraceMatcher : AbstractCSharpBraceMatcher
  {
    [ImportingConstructor]
    [Obsolete(MefConstruction.ImportingConstructorMessage, error: true)]
    public LessThanGreaterThanBraceMatcher()
        : base(SyntaxKind.LessThanToken, SyntaxKind.GreaterThanToken)
    {
    }
  }
}
#endif
