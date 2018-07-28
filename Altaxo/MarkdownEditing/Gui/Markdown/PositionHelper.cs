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
using System.Windows.Documents;

namespace Altaxo.Gui.Markdown
{
  public class PositionHelper
  {
    #region From Viewer to SourceText

    /// <summary>
    /// Converts a position in the flow document into a position in the source text. A flag is delivered too, which
    /// indicates whether the returned position is considered to be accurate or only approximate.
    /// </summary>
    /// <param name="textPosition">The text position in the viewer.</param>
    /// <returns>The source text position and a flag indicating if the returned source text position is accurate.</returns>
    public static (int sourceTextOffset, bool isReturnedPositionAccurate) ViewersTextPositionToSourceEditorsTextPosition(TextPointer textPosition)
    {
      bool isReturnedPositionAccurate = true;
      TextElement parent;
      if (textPosition.Parent is TextElement pe)
      {
        parent = pe;
      }
      else
      {
        parent = textPosition.Paragraph;
        isReturnedPositionAccurate = false; // we are maybe in an image or a checkbox, thus the returned position may not be accurate
      }

      // search parent or the ancestors of parent for a Markdig tag
      Markdig.Syntax.MarkdownObject markdigTag = null;
      while (null != parent)
      {
        if (parent.Tag is Markdig.Syntax.MarkdownObject mdo)
        {
          markdigTag = mdo;
          break;
        }
        parent = parent.Parent as TextElement;
        isReturnedPositionAccurate = false; // we have to use the parent of this text element, thus the returned positon may not be accurate
      }

      if (null != markdigTag)
      {
        int charOffset = parent.ContentStart.GetOffsetToPosition(textPosition);
        int sourceTextOffset = ContentSpan(markdigTag).Start + charOffset;

        return (sourceTextOffset, isReturnedPositionAccurate);
      }
      else
      {
        return (0, false);
      }
    }

    #endregion From Viewer to SourceText

    #region From SourceText to Viewer

    /// <summary>
    ///	Given a position in the source text, this searches for the viewer's corresponding text position.
    /// </summary>
    /// <param name="sourceTextPosition">The position (given as zero based offset) in the source text.</param>
    /// <param name="blocks">The first level blocks of the <see cref="FlowDocument"/>, as given by <see cref="FlowDocument.Blocks"/>.</param>
    /// <returns>If the position in the viewer could be determined accurately, the returned value is the <see cref="TextPointer"/> and true;
    /// if the position in the viewer could be determined only approximately, the returned value is the approximate <see cref="TextPointer"/> and false;
    /// if the position in the viewer could not be determined, the returned value is (null, false).</returns>
    public static (TextPointer textPointer, bool isReturnedPositionAccurate) SourceEditorTextPositionToViewersTextPosition(int textOffset, System.Collections.IList blocks)
    {
      var textElement = BinarySearchBlocksForTextOffset(blocks, textOffset);
      if (null != textElement && textElement.Tag is Markdig.Syntax.MarkdownObject markdigTag)
      {
        var viewTextPosition = textElement.ContentStart;

        if (textElement is Run run && run.Text.Length > 0)
        {
          var sourceSpan = ContentSpan(markdigTag);
          int offsetIntoRun = textOffset - sourceSpan.Start;
          if (sourceSpan.Length == run.Text.Length && offsetIntoRun >= 0 && offsetIntoRun <= run.Text.Length)
          {
            //var c1 = _guiRawText.Text[textOffset]; // the char at this offset is the char after the cursor
            //var c2 = run.Text[offsetIntoRun];      // the char at this offset is the char after the cursor
            viewTextPosition = viewTextPosition.GetPositionAtOffset(offsetIntoRun);
            return (viewTextPosition, true);
          }
          else
          {
            return (viewTextPosition, false);
          }
        }
        else
        {
          return (viewTextPosition, false);
        }
      }
      else
      {
        return (null, false);
      }
    }

    /// <summary>
    /// Returns the span of the contents of a <see cref="Markdig.Syntax.MarkdownObject"/>.
    /// </summary>
    /// <param name="markdownObj">The markdown object.</param>
    /// <returns></returns>
    public static Markdig.Syntax.SourceSpan ContentSpan(Markdig.Syntax.MarkdownObject markdownObj)
    {
      if (markdownObj is Markdig.Syntax.Inlines.CodeInline ci)
      {
        return new Markdig.Syntax.SourceSpan(ci.Span.Start + 1, ci.Span.End - 1);
      }
      else
      {
        return markdownObj.Span;
      }
    }

    #endregion From SourceText to Viewer

    #region Helpers for Viewer

    /// <summary>
    /// Searches a <see cref="FlowDocument" or any contiguous blocks of such a document for the <see cref="TextElement"/> which corresponds to a given source text position. />
    /// </summary>
    /// <param name="blocks">The blocks of the <see cref="FlowDocument"/>, as given by <see cref="FlowDocument.Blocks"/>.</param>
    /// <param name="textPosition">The zero based source text offset.</param>
    /// <returns>The <see cref="TextElement"/> which corresponds to the given source text position; or null (if no such <see cref="TextElement"/> could be found.</returns>
    public static TextElement BinarySearchBlocksForTextOffset(System.Collections.IList blocks, int textPosition)
    {
      var count = blocks.Count;
      if (0 == count)
        return null;

      // Skip forward lowerIdx unil we find a markdown tag
      int lowerIdx;
      for (lowerIdx = 0; lowerIdx < count; ++lowerIdx)
      {
        if (((TextElement)blocks[lowerIdx]).Tag is Markdig.Syntax.MarkdownObject lowerMdo)
        {
          if (lowerMdo.Span.Start > textPosition)
            return (TextElement)blocks[lowerIdx]; // then we have already passed the position without finding the element
          else
            break;
        }
      }

      if (lowerIdx == count)
        return null; // ups - no element with a tag found in the entire list of elements

      // Skip backward upperIdx until we find a markdown tag
      int upperIdx;
      for (upperIdx = count - 1; upperIdx >= lowerIdx; --upperIdx)
      {
        if (((TextElement)blocks[upperIdx]).Tag is Markdig.Syntax.MarkdownObject upperMdo)
        {
          break;
        }
      }

      // lowerMdo.TextPosition should now be less than or equal to the textposition  we are looking for

      for (; ; )
      {
        if (lowerIdx == upperIdx || (lowerIdx + 1) == upperIdx)
          break;

        // calculate a block inbetween lowerIdx and upperIdx

        var middleIdx = (lowerIdx + upperIdx) / 2;
        // skip items that do not contain a tag

        for (int offs = 0; !(middleIdx + offs > upperIdx && middleIdx - offs < lowerIdx); ++offs)
        {
          if ((middleIdx + offs < upperIdx) && ((TextElement)blocks[middleIdx + offs]).Tag is Markdig.Syntax.MarkdownObject)
          {
            middleIdx = middleIdx + offs;
            break;
          }
          else if ((middleIdx - offs > lowerIdx) && ((TextElement)blocks[middleIdx - offs]).Tag is Markdig.Syntax.MarkdownObject)
          {
            middleIdx = middleIdx - offs;
            break;
          }
        }

        if (!(((TextElement)blocks[middleIdx]).Tag is Markdig.Syntax.MarkdownObject middleMdo))
          break;
        else if (middleMdo.Span.Start > textPosition)
          upperIdx = middleIdx;
        else
          lowerIdx = middleIdx;
      }

      // now we have bracketed our search: lowerIdx should have a lineNumber less than our searched lineNumber,
      // and upperIdx can have a line number less than, or greater than our searched line number
      // our only chance is to search the children of the lowerIdx


      int lowerIdxSpanEnd = (((TextElement)blocks[lowerIdx]).Tag as Markdig.Syntax.MarkdownObject).Span.End;
      int upperIdxSpanStart = (((TextElement)blocks[upperIdx]).Tag as Markdig.Syntax.MarkdownObject).Span.Start;

      // if our search position is neither at the end of the lower nor at the start of the upper, but inbetween
      if (textPosition > lowerIdxSpanEnd && textPosition < upperIdxSpanStart)
      {
        // we look forward then and decide to use the position of the next element forward
        return (TextElement)blocks[upperIdx];
      }

      int diveIntoIdx = upperIdxSpanStart <= textPosition ? upperIdx : lowerIdx;

      var childs = GetChildList((TextElement)blocks[diveIntoIdx]);
      if (null == childs)
      {
        return (TextElement)blocks[diveIntoIdx];
      }
      else // there are child, so search in them
      {
        var result = BinarySearchBlocksForTextOffset(childs, textPosition);
        if (null != result)
          return result; // we have found a child, so return it
        else
          return (TextElement)blocks[diveIntoIdx]; // no child found, then diveIntoIdx may be the best choice.
      }
    }

    /// <summary>
    /// Performs a recursive binaries search in a list of <see cref="TextElement"/>s, most of them tagged with a <see cref="Markdig.Syntax.MarkdownObject"/> in order to find the
    /// element with corresponds to a given line number in the source markdown.
    /// </summary>
    /// <param name="blocks">The list of <see cref="TextElement"/>s. Most of them should be tagged with the corresponding a <see cref="Markdig.Syntax.MarkdownObject"/> from which they are created.</param>
    /// <param name="lineNumber">The line number in the source markdown text to be searched for.</param>
    /// <param name="lineNumber">The line number in the source markdown text to be searched for.</param>
    /// <returns>The <see cref="TextElement"/> which corresponds to a line number equal to or greater than the searched line number.</returns>
    public static TextElement BinarySearchBlocksForLineNumber(System.Collections.IList blocks, int lineNumber, int columnNumber)
    {
      var count = blocks.Count;
      if (0 == count)
        return null;

      // Skip forward lowerIdx unil we find a markdown tag
      int lowerIdx;
      for (lowerIdx = 0; lowerIdx < count; ++lowerIdx)
      {
        if (((TextElement)blocks[lowerIdx]).Tag is Markdig.Syntax.MarkdownObject lowerMdo)
        {
          if (CompareLineColumn(lowerMdo.Line, lowerMdo.Column, lineNumber, columnNumber) > 0)
            return (TextElement)blocks[lowerIdx]; // we have already passed the position without finding them
          else
            break;
        }
      }

      if (lowerIdx == count)
        return null; // ups - no element with a tag found in the entire list of elements

      // Skip backward upperIdx until we find a markdown tag
      int upperIdx;
      for (upperIdx = count - 1; upperIdx >= lowerIdx; --upperIdx)
      {
        if (((TextElement)blocks[upperIdx]).Tag is Markdig.Syntax.MarkdownObject upperMdo)
        {
          break;
        }
      }

      // lowerMdo.Line should now be less than the lineNumber we are searching for

      for (; ; )
      {
        if (lowerIdx == upperIdx || (lowerIdx + 1) == upperIdx)
          break;

        // calculate a block inbetween lowerIdx and upperIdx

        var middleIdx = (lowerIdx + upperIdx) / 2;
        // skip items that do not contain a tag

        for (int offs = 0; !(middleIdx + offs > upperIdx && middleIdx - offs < lowerIdx); ++offs)
        {
          if ((middleIdx + offs < upperIdx) && ((TextElement)blocks[middleIdx + offs]).Tag is Markdig.Syntax.MarkdownObject)
          {
            middleIdx = middleIdx + offs;
            break;
          }
          else if ((middleIdx - offs > lowerIdx) && ((TextElement)blocks[middleIdx - offs]).Tag is Markdig.Syntax.MarkdownObject)
          {
            middleIdx = middleIdx - offs;
            break;
          }
        }

        if (!(((TextElement)blocks[middleIdx]).Tag is Markdig.Syntax.MarkdownObject middleMdo))
          break;

        if (CompareLineColumn(middleMdo.Line, middleMdo.Column, lineNumber, columnNumber) > 0)
          upperIdx = middleIdx;
        else
          lowerIdx = middleIdx;
      }

      // now we have bracketed our search: lowerIdx should have a lineNumber less than our searched lineNumber,
      // and upperIdx can have a line number less than, or greater than our searched line number
      // our only chance is to search the children of the lowerIdx

      int diveIntoIdx = lowerIdx;
      if (((TextElement)blocks[upperIdx]).Tag is Markdig.Syntax.MarkdownObject upperMdo2 && CompareLineColumn(upperMdo2.Line, upperMdo2.Column, lineNumber, columnNumber) <= 0)
        diveIntoIdx = upperIdx;

      var childs = GetChildList((TextElement)blocks[diveIntoIdx]);
      if (null == childs)
      {
        return (TextElement)blocks[diveIntoIdx]; // no childs, then diveIntoIdx element is the best choice
      }
      else // there are child, so search in them
      {
        var result = BinarySearchBlocksForLineNumber(childs, lineNumber, columnNumber);
        if (null != result)
          return result; // we have found a child, so return it
        else
          return (TextElement)blocks[upperIdx]; // no child found, then upperIdx may be the best choice.
      }
    }

    private static int CompareLineColumn(int lineA, int columA, int lineB, int columnB)
    {
      if (lineA < lineB)
        return -1;
      else if (lineA > lineB)
        return +1;
      else if (columA < columnB)
        return -1;
      else if (columA > columnB)
        return +1;
      else
        return 0;
    }

    /// <summary>
    /// Gets the childs of a given <see cref="TextElement"/>.
    /// </summary>
    /// <param name="parent">The text element for which to get the childs.</param>
    /// <returns>The list of child elements, or null if the text element does not support childs.</returns>
    public static System.Collections.IList GetChildList(TextElement parent)
    {
      if (parent is Paragraph para)
      {
        return para.Inlines;
      }
      else if (parent is List list)
      {
        return list.ListItems;
      }
      else if (parent is ListItem listItem)
      {
        return listItem.Blocks;
      }
      else if (parent is Span span)
      {
        return span.Inlines;
      }
      else if (parent is Section section)
      {
        return section.Blocks;
      }
      return null;
    }

    /// <summary>
    /// Enumerates all text elements recursively, given a bunch of top level text elements
    /// </summary>
    /// <param name="toplevelTextElements">The toplevel text elements.</param>
    /// <returns>Enumeration of all text elements.</returns>
    public static IEnumerable<TextElement> EnumerateAllTextElementsRecursively(IEnumerable<TextElement> toplevelTextElements)
    {
      if (null != toplevelTextElements)
      {
        foreach (var child in toplevelTextElements)
        {
          foreach (TextElement childAndSub in EnumerateAllTextElementsRecursively(child))
            yield return childAndSub;
        }
      }
    }

    /// <summary>
    /// Enumerates all text elements recursively, starting with one text element.
    /// </summary>
    /// <param name="startElement">The start element.</param>
    /// <returns>All text element (the given text element and all its childs).</returns>
    public static IEnumerable<TextElement> EnumerateAllTextElementsRecursively(TextElement startElement)
    {
      yield return startElement;
      var childList = GetChildList(startElement);
      if (null != childList)
      {
        foreach (TextElement child in GetChildList(startElement))
        {
          foreach (TextElement childAndSub in EnumerateAllTextElementsRecursively(child))
            yield return childAndSub;
        }
      }
    }

    #endregion Helpers for Viewer

    #region Helpers for Markdig

    /// <summary>
    /// Enumerates all objects in a markdown parse tree recursively, starting with the given element.
    /// </summary>
    /// <param name="startElement">The start element.</param>
    /// <returns>All text element (the given text element and all its childs).</returns>
    public static IEnumerable<Markdig.Syntax.MarkdownObject> EnumerateAllMarkdownObjectsRecursively(Markdig.Syntax.MarkdownObject startElement)
    {
      yield return startElement;
      var childList = GetChildList(startElement);
      if (null != childList)
      {
        foreach (var child in GetChildList(startElement))
        {
          foreach (var childAndSub in EnumerateAllMarkdownObjectsRecursively(child))
            yield return childAndSub;
        }
      }
    }

    /// <summary>
    /// Gets the childs of a markdown object. Null is returned if no childs were to be found.
    /// </summary>
    /// <param name="parent">The markdown object from which to get the childs.</param>
    /// <returns>The childs of the given markdown object, or null.</returns>
    public static IEnumerable<Markdig.Syntax.MarkdownObject> GetChilds(Markdig.Syntax.MarkdownObject parent)
    {
      if (parent is Markdig.Syntax.LeafBlock leafBlock)
        return leafBlock.Inline;
      else if (parent is Markdig.Syntax.Inlines.ContainerInline containerInline)
        return containerInline;
      else if (parent is Markdig.Syntax.ContainerBlock containerBlock)
        return containerBlock;
      else
        return null;
    }

    /// <summary>
    /// Gets the childs of a markdown object. Null is returned if no childs were to be found.
    /// </summary>
    /// <param name="parent">The markdown object from which to get the childs.</param>
    /// <returns>The childs of the given markdown object, or null.</returns>
    public static IReadOnlyList<Markdig.Syntax.MarkdownObject> GetChildList(Markdig.Syntax.MarkdownObject parent)
    {
      if (parent is Markdig.Syntax.LeafBlock leafBlock)
        return leafBlock.Inline?.ToArray<Markdig.Syntax.MarkdownObject>();
      else if (parent is Markdig.Syntax.Inlines.ContainerInline containerInline)
        return containerInline.ToArray<Markdig.Syntax.MarkdownObject>();
      else if (parent is Markdig.Syntax.ContainerBlock containerBlock)
        return containerBlock;
      else
        return null;
    }

    #endregion Helpers for Markdig
  }
}
