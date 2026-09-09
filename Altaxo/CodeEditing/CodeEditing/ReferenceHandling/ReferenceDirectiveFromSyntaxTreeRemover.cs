using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

/// <summary>
/// Provides functionality to remove reference and load directives (e.g., <c>#r</c> and <c>#load</c>)  from a C# syntax
/// tree.
/// </summary>
/// <remarks>This class is a specialized implementation of <see cref="CSharpSyntaxRewriter"/> that traverses  the
/// syntax tree and removes any <c>#r</c> (reference) or <c>#load</c> directives found in the  leading or trailing
/// trivia of tokens. It can be used to preprocess C# code by stripping out  directives that are not needed for further
/// compilation or analysis.</remarks>
internal class ReferenceDirectiveFromSyntaxTreeRemover : CSharpSyntaxRewriter
{
  /// </inheritdoc>
  public override SyntaxToken VisitToken(SyntaxToken token)
  {
    // Directives are structured trivia attached to tokens rather than ordinary syntax nodes.
    // Check both trivia lists because a directive can be associated with either side of a token.
    var hasLeadingDirective = token.LeadingTrivia.Any(IsReferenceDirective);
    var hasTrailingDirective = token.TrailingTrivia.Any(IsReferenceDirective);

    // Reusing the original token is important: creating a replacement token unnecessarily can
    // discard parser diagnostics stored internally on the token's immutable syntax representation.
    if (!hasLeadingDirective && !hasTrailingDirective)
      return token;

    // Replace only the selected directive trivia. All other trivia, including comments and
    // formatting, is returned unchanged. SelectMany is needed because one directive may be
    // replaced by several trivia items when its text contains line breaks.
    return token.WithLeadingTrivia(token.LeadingTrivia.SelectMany(ReplaceReferenceDirectiveWithWhitespace))
                .WithTrailingTrivia(token.TrailingTrivia.SelectMany(ReplaceReferenceDirectiveWithWhitespace));
  }

  /// <summary>
  /// Determines whether the specified trivia represents a script reference or load directive.
  /// </summary>
  /// <param name="trivia">The trivia to examine.</param>
  /// <returns><see langword="true"/> for <c>#r</c> and <c>#load</c> directives; otherwise, <see langword="false"/>.</returns>
  private static bool IsReferenceDirective(SyntaxTrivia trivia)
  {
    return trivia.IsKind(SyntaxKind.LoadDirectiveTrivia) || trivia.IsKind(SyntaxKind.ReferenceDirectiveTrivia);
  }

  /// <summary>
  /// Replaces a reference or load directive with whitespace occupying exactly the same source width.
  /// </summary>
  /// <remarks>
  /// Preserving the width and the original line endings ensures that syntax and diagnostic positions
  /// following the removed directive remain identical to their positions in the original source text.
  /// Non-directive trivia is returned without modification.
  /// </remarks>
  /// <param name="trivia">The trivia to preserve or replace.</param>
  /// <returns>The original trivia, or whitespace and end-of-line trivia having the same full width.</returns>
  private static IEnumerable<SyntaxTrivia> ReplaceReferenceDirectiveWithWhitespace(SyntaxTrivia trivia)
  {
    // Preserve the original trivia object whenever it is not one of the directives being removed.
    if (!IsReferenceDirective(trivia))
    {
      yield return trivia;
      yield break;
    }

    // ToFullString includes every character represented by the structured directive trivia.
    // The replacement must account for all of these UTF-16 characters to preserve TextSpan values.
    var text = trivia.ToFullString();
    var whitespaceStart = 0;

    // Replace ordinary characters with spaces, but emit line endings separately. Replacing a line
    // ending with spaces would preserve absolute offsets while changing line and column positions.
    for (var i = 0; i < text.Length; ++i)
    {
      if (text[i] is not ('\r' or '\n'))
        continue;

      // Emit one space for every directive character preceding this line ending.
      if (i > whitespaceStart)
        yield return SyntaxFactory.Whitespace(new string(' ', i - whitespaceStart));

      // Treat CRLF as one end-of-line trivia while retaining its two-character source width.
      // Lone CR and LF line endings are retained in their original form as well.
      if (text[i] == '\r' && i + 1 < text.Length && text[i + 1] == '\n')
      {
        yield return SyntaxFactory.EndOfLine("\r\n");
        ++i;
      }
      else
      {
        yield return SyntaxFactory.EndOfLine(text[i].ToString());
      }

      whitespaceStart = i + 1;
    }

    // Emit the final run of spaces after the last line ending, or the entire replacement when the
    // directive contains no line ending. No trivia is emitted for an empty trailing run.
    if (whitespaceStart < text.Length)
      yield return SyntaxFactory.Whitespace(new string(' ', text.Length - whitespaceStart));
  }

  /// <summary>
  /// Removes all reference directives (e.g., <c>#r</c> and <c>#load</c>) from the specified syntax tree.
  /// </summary>
  /// <remarks>This method processes the provided syntax tree and generates a new syntax tree with the same
  /// structure, excluding any reference directives. It is useful for scenarios where such directives are not supported
  /// or need to be excluded for further processing.</remarks>
  /// <param name="tree">The <see cref="SyntaxTree"/> from which reference directives will be removed.</param>
  /// <returns>A new <see cref="SyntaxTree"/> with all reference directives removed.</returns>
  public SyntaxTree RemoveReferenceDirectivesFromSyntaxTree(SyntaxTree tree)
  {
    var rootNode = tree.GetRoot();
    var newRoot = Visit(rootNode);

    // Keep the original parse options, file path, and encoding instead of creating an unrelated
    // syntax tree. The equal-width replacements keep existing source positions meaningful.
    var newTree = tree.WithRootAndOptions((CompilationUnitSyntax)newRoot, tree.Options);
    return newTree;
  }


}
