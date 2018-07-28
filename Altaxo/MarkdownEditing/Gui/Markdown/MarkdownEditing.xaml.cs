#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    AltaxoMarkdownEditing
//    Copyright (C) 2018 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//    See the LICENSE.md file in the root of the AltaxoMarkdownEditing library for more information.
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Markdig;
using Markdig.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altaxo.Gui.Markdown
{
  /// <summary>
  /// Interaction logic for MarkdownSimpleEditing.xaml
  /// </summary>
  public partial class MarkdownEditing : UserControl
  {
    private bool useExtensions = true;

    public MarkdownEditing()
    {
      InitializeComponent();
      Loaded += EhLoaded;
      _guiRawText.TextArea.TextView.ScrollOffsetChanged += EhTextEditor_ScrollOffsetChanged;
    }

    private void EhLoaded(object sender, RoutedEventArgs e)
    {
      Viewer.Markdown = _guiRawText.Text;
    }

    private void OpenHyperlink(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
    {
      Process.Start(e.Parameter.ToString());
    }

    private void ToggleExtensionsButton_OnClick(object sender, RoutedEventArgs e)
    {
      /*
			useExtensions = !useExtensions;
			Viewer.Pipeline = useExtensions ? new MarkdownPipelineBuilder().UseSupportedExtensions().Build() : new MarkdownPipelineBuilder().Build();
			*/

      ScrollPreviewWindowToLine(20);
    }

    private void EhTextChanged(object sender, EventArgs e)
    {
      if (null != Viewer && null != _guiRawText)
        Viewer.Markdown = _guiRawText.Text;
    }

    private void EhTextEditor_ScrollOffsetChanged(object sender, EventArgs e)
    {
      var scrollPos = _guiRawText.TextArea.TextView.ScrollOffset;
      var dl = _guiRawText.TextArea.TextView.GetDocumentLineByVisualTop(scrollPos.Y);
      ScrollPreviewWindowToLine(dl.LineNumber);
    }

    // https://stackoverflow.com/questions/561029/scroll-a-wpf-flowdocumentscrollviewer-from-code

    private void ScrollPreviewWindowToLine(int documentLine)
    {
      var flowDocument = Viewer.Document;

      var blocks = flowDocument.Blocks;

      var textElement = BinarySearchBlocksForLineNumber(flowDocument.Blocks, documentLine - 1);
      if (null != textElement)
        textElement.BringIntoView();
    }

    private TextElement SearchBlocksForLineNumber(IEnumerable<TextElement> blocks, int lineNumber)
    {
      TextElement previousElement = null;
      foreach (var block in blocks)
      {
        if (!(block.Tag is Markdig.Syntax.MarkdownObject mdo))
          continue;

        if (mdo.Line == lineNumber)
        {
          return block;
        }
        else if (mdo.Line > lineNumber && null != previousElement)
        {
          var childBlocks = GetChildBlocks(previousElement);
          TextElement foundResult;
          if (null != childBlocks && null != (foundResult = SearchBlocksForLineNumber(childBlocks, lineNumber)))
          {
            return foundResult;
          }
        }
        previousElement = block; // use as previousElement only those that have a MarkdownObject tag
      }

      return null;
    }

    /// <summary>
    /// Performs a recursive binaries search in a list of <see cref="TextElement"/>s, most of them tagged with a <see cref="Markdig.Syntax.MarkdownObject"/> in order to find the
    /// element with corresponds to a given line number in the source markdown.
    /// </summary>
    /// <param name="blocks">The list of <see cref="TextElement"/>s. Most of them should be tagged with the corresponding a <see cref="Markdig.Syntax.MarkdownObject"/> from which they are created.</param>
    /// <param name="lineNumber">The line number in the source markdown text to be searched for.</param>
    /// <returns>The <see cref="TextElement"/> which corresponds to a line number equal to or greater than the searched line number.</returns>
    private TextElement BinarySearchBlocksForLineNumber(System.Collections.IList blocks, int lineNumber)
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
          if (lowerMdo.Line >= lineNumber)
            return (TextElement)blocks[lowerIdx];
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
          if (upperMdo.Line == lineNumber)
            return (TextElement)blocks[upperIdx];
          else
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

        if (middleMdo.Line == lineNumber)
          return (TextElement)blocks[middleIdx];
        else if (middleMdo.Line > lineNumber)
          upperIdx = middleIdx;
        else
          lowerIdx = middleIdx;
      }

      // now we have bracketed our search: lowerIdx should have a lineNumber less than our searched lineNumber,
      // and upperIdx should have a lineNumber greater than our searched line number
      // our only chance is to search the children of the lowerIdx

      var childs = GetChildList((TextElement)blocks[lowerIdx]);
      if (null == childs)
      {
        return (TextElement)blocks[upperIdx]; // no childs, then our upperIdx element is the best choice
      }
      else // there are child, so search in them
      {
        var result = BinarySearchBlocksForLineNumber(childs, lineNumber);
        if (null != result)
          return result; // we have found a child, so return it
        else
          return (TextElement)blocks[upperIdx]; // no child found, then upperIdx may be the best choice.
      }
    }

    private System.Collections.IList GetChildList(TextElement parent)
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

    private IEnumerable<TextElement> GetChildBlocks(TextElement parent)
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

    // https://social.msdn.microsoft.com/Forums/vstudio/en-US/2602ebdb-d4ce-44c0-961c-6a796471043a/hit-test-in-textblock?forum=wpf
  }
}
