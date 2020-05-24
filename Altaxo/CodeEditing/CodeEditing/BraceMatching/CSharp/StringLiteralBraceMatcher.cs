// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, EditorFeatures, CSharp/BraceMatching/StringLiteralBraceMatcher.cs

#if !NoBraceMatching
extern alias MCW;
using System.Threading;
using System.Threading.Tasks;
using MCW::Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.BraceMatching.CSharp
{
  [ExportBraceMatcher(LanguageNames.CSharp)]
  internal class StringLiteralBraceMatcher : IBraceMatcher
  {
    public async Task<BraceMatchingResult?> FindBracesAsync(Document document, int position, CancellationToken cancellationToken)
    {
      var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
      var token = root.FindToken(position);

      if (!token.ContainsDiagnostics)
      {
        if (token.IsKind(SyntaxKind.StringLiteralToken))
        {
          if (token.IsVerbatimStringLiteral())
          {
            return new BraceMatchingResult(
                new TextSpan(token.SpanStart, 2),
                new TextSpan(token.Span.End - 1, 1));
          }
          else
          {
            return new BraceMatchingResult(
                new TextSpan(token.SpanStart, 1),
                new TextSpan(token.Span.End - 1, 1));
          }
        }
        else if (IsKind(token, SyntaxKind.InterpolatedStringStartToken, SyntaxKind.InterpolatedVerbatimStringStartToken))
        {
          var interpolatedString = token.Parent as InterpolatedStringExpressionSyntax;
          if (interpolatedString != null)
          {
            return new BraceMatchingResult(token.Span, interpolatedString.StringEndToken.Span);
          }
        }
        else if (token.IsKind(SyntaxKind.InterpolatedStringEndToken))
        {
          var interpolatedString = token.Parent as InterpolatedStringExpressionSyntax;
          if (interpolatedString != null)
          {
            return new BraceMatchingResult(interpolatedString.StringStartToken.Span, token.Span);
          }
        }
      }

      return null;
    }

    public static bool IsKind(SyntaxToken token, SyntaxKind kind1, SyntaxKind kind2)
    {
      return token.Kind() == kind1
          || token.Kind() == kind2;
    }
  }
}
#endif
