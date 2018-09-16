// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, CSharpEditorFeatures, BraceMatching/CSharpDirectiveTriviaBraceMatcher.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.BraceMatching.CSharp
{
  /* There is no easy way to use this with Roslyn 2.0 RC2

    [ExportBraceMatcher(LanguageNames.CSharp)]
    internal class CSharpDirectiveTriviaBraceMatcher : AbstractDirectiveTriviaBraceMatcher<DirectiveTriviaSyntax,
        IfDirectiveTriviaSyntax, ElifDirectiveTriviaSyntax,
        ElseDirectiveTriviaSyntax, EndIfDirectiveTriviaSyntax,
        RegionDirectiveTriviaSyntax, EndRegionDirectiveTriviaSyntax>
    {
        internal override List<DirectiveTriviaSyntax> GetMatchingConditionalDirectives(DirectiveTriviaSyntax directive, CancellationToken cancellationToken)
                => directive.GetMatchingConditionalDirectives(cancellationToken)?.ToList();

        internal override DirectiveTriviaSyntax GetMatchingDirective(DirectiveTriviaSyntax directive, CancellationToken cancellationToken)
                => directive.GetMatchingDirective(cancellationToken);

        internal override TextSpan GetSpanForTagging(DirectiveTriviaSyntax directive)
                => TextSpan.FromBounds(directive.HashToken.SpanStart, directive.DirectiveNameToken.Span.End);
	}
	*/
}
