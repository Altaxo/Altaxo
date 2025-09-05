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
    var newLeadingTrivia = token.LeadingTrivia
        .Where(trivia => !trivia.IsKind(SyntaxKind.LoadDirectiveTrivia) && !trivia.IsKind(SyntaxKind.ReferenceDirectiveTrivia));

    var newTrailingTrivia = token.TrailingTrivia
        .Where(trivia => !trivia.IsKind(SyntaxKind.LoadDirectiveTrivia) && !trivia.IsKind(SyntaxKind.ReferenceDirectiveTrivia));


    return token.WithLeadingTrivia(newLeadingTrivia)
                .WithTrailingTrivia(newTrailingTrivia);
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
    // new SyntaxTree without #r und #load
    var newTree = CSharpSyntaxTree.Create((CompilationUnitSyntax)newRoot);
    return newTree;
  }


}
