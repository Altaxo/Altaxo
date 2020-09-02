#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    AltaxoMarkdownEditing
//    Copyright (C) 2018 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//    See the LICENSE.md file in the root of the AltaxoMarkdownEditing library for more information.
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.AvalonEdit.Folding;

namespace Altaxo.Gui.Markdown
{
  /// <summary>
  /// Responsible for the folding markers in the markdown source editor.
  /// </summary>
  public class SyntaxTreeFoldingStrategy
  {
    /// <summary>
    /// Central routine of the folding strategy for markdown.
    /// It uses the parsed syntax tree in <paramref name="markdownDocument"/> to calculate all folding positions.
    /// </summary>
    /// <param name="markdownDocument">The parsed syntax tree of the markdown document.</param>
    /// <returns>Enumeration of foldings.</returns>
    public virtual IEnumerable<NewFolding> GetNewFoldings(Markdig.Syntax.MarkdownDocument markdownDocument)
    {
      const int maxLevel = 4;
      var newFoldMarkers = new List<NewFolding>();

      if (markdownDocument is not null)
      {
        // since we fold only the h1, h2, h3, h3 tags, they all are top level blocks
        // thus there is no need to go down in the tree

        var previousBlocks = new Markdig.Syntax.HeadingBlock[maxLevel + 1]; // +1 because heading.Level is one-based

        for (int i = 0; i < markdownDocument.Count; ++i)
        {
          if (markdownDocument[i] is Markdig.Syntax.HeadingBlock heading && heading.Level <= maxLevel)
          {
            // A heading block will close all previous heading with the same level or any sublevel above
            for (int j = heading.Level; j <= maxLevel; ++j)
            {
              if (previousBlocks[j] is not null)
              {
                newFoldMarkers.Add(new NewFolding(previousBlocks[j].Span.End + 1, markdownDocument[i - 1].Span.End + 1));
                previousBlocks[j] = null; // the previous block is processed then
              }
            }
            previousBlocks[heading.Level] = heading;
          }
        }

        // after all blocks have been processed, we have to close the unfinished blocks
        var end = markdownDocument.Span.End;
        for (int i = 0; i < previousBlocks.Length; ++i)
        {
          if (previousBlocks[i] is not null)
          {
            newFoldMarkers.Add(new NewFolding(previousBlocks[i].Span.End + 1, markdownDocument.Span.End + 1));
          }
        }
      }

      return newFoldMarkers.OrderBy(f => f.StartOffset);
    }
  }
}
