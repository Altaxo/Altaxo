#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#if !NoLiveDocumentFormatting
//extern alias MCW;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.LiveDocumentFormatting
{
  public class LiveDocumentFormatterCSharp : ILiveDocumentFormatter
  {
    /// <summary>
    /// Formats the document after entering a trigger character. Trigger chars are e.g. closing curly brace (then format whole paragraph)
    /// or semicolon (then format line).
    /// </summary>
    /// <param name="caretPosition">The caret position after (!) the trigger char.</param>
    /// <returns></returns>
    public async Task FormatDocumentAfterEnteringTriggerChar(Workspace workspace, DocumentId documentId, RoslynSourceTextContainerAdapter sourceText, int caretPosition, char triggerChar)
    {
      var document = workspace.CurrentSolution.GetDocument(documentId);
      var syntaxTree = await document.GetSyntaxRootAsync();

      TextSpan? textSpanToFormat = null;

      if (triggerChar == '}')
      {
        var visitor = new WalkerForCurlyBrace(caretPosition);
        visitor.Visit(syntaxTree);
        textSpanToFormat = visitor.TextSpanToFormat;
      }
      else if (triggerChar == ';')
      {
        var visitor = new WalkerForSemicolon(caretPosition);
        visitor.Visit(syntaxTree);
        textSpanToFormat = visitor.TextSpanToFormat;
      }

      if (textSpanToFormat.HasValue)
      {
        var textChanges = Formatter.GetFormattedTextChanges(syntaxTree, textSpanToFormat.Value, workspace);
        sourceText.ApplyTextChangesToAvalonEdit(textChanges);
      }
    }

    private class WalkerForCurlyBrace : CSharpSyntaxWalker
    {
      private int _caretPosition;
      public TextSpan TextSpanToFormat { get; private set; }

      public WalkerForCurlyBrace(int caretPosition)
      {
        _caretPosition = caretPosition;
      }

      public override void Visit(SyntaxNode node)
      {
        if (node.Span.End == _caretPosition)
        {
          TextSpanToFormat = node.Span;
          return;
        }

        base.Visit(node);
      }
    }

    private class WalkerForSemicolon : CSharpSyntaxWalker
    {
      private int _caretPosition;
      public TextSpan TextSpanToFormat { get; private set; }

      public WalkerForSemicolon(int caretPosition)
      {
        _caretPosition = caretPosition;
      }

      public override void Visit(SyntaxNode node)
      {
        if (node.Span.End == _caretPosition)
        {
          TextSpanToFormat = new TextSpan(node.Span.Start - 1, node.Span.Length + 1);
          return;
        }

        base.Visit(node);
      }
    }
  }
}
#endif
