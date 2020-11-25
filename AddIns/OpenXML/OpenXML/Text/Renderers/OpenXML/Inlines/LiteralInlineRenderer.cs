#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

using System;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using Markdig.Syntax.Inlines;

namespace Altaxo.Text.Renderers.OpenXML.Inlines
{
  /// <summary>
  /// OpenXML renderer for a <see cref="LiteralInline"/>.
  /// </summary>
  /// <seealso cref="OpenXMLObjectRenderer{T}" />
  public class LiteralInlineRenderer : OpenXMLObjectRenderer<LiteralInline>
  {
    /// <inheritdoc/>
    protected override void Write(OpenXMLRenderer renderer, LiteralInline obj)
    {
      if (obj.Content.IsEmpty)
        return;

      if (renderer.FigureCaptionList is { } figureCaptionList &&
          renderer.CurrentFigureCaptionListIndex is { } currentFigureCaptionListIndex &&
          Includes(obj.Span, figureCaptionList[currentFigureCaptionListIndex].Number.Position, figureCaptionList[currentFigureCaptionListIndex].Number.Count)
        )
      {
        // we are inside a figure caption, and the use of automatic figure numbering was chosen
        WriteFigureCaptionLiteralInline(renderer, obj);
      }
      else if (
        renderer.FigureLinkList is { } figureLinkList &&
        renderer.CurrentFigureLinkListIndex is { } currentFigureLinkListIndex &&
        Includes(obj.Span, figureLinkList[currentFigureLinkListIndex].Number.Position, figureLinkList[currentFigureLinkListIndex].Number.Count)
        )
      {
        // we are inside a link to a figure, and the use of automatic figure numbering was chosen
        WriteFigureLinkLiteralInline(renderer, obj);
      }
      else
      {
        // Write a normal inline
        var run = renderer.PushNewRun();
        run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text() { Space = SpaceProcessingModeValues.Preserve, Text = obj.Content.ToString() });
        renderer.PopTo(run);
      }
    }


    private static void WriteFigureCaptionLiteralInline(OpenXMLRenderer renderer, LiteralInline obj)
    {
      if (!(renderer.CurrentFigureCaptionListIndex is { } currentFigureCaptionListIndex))
        throw new InvalidProgramException("Current figure caption list index must be set in the renderer!");

      // This is probably a figure caption, and maybe the category identifier and the number needs to be replaced by special elements

      // Split the text in text before the number, and after the number

      var text = obj.Content.ToString();

      var numberPosition = renderer.FigureCaptionList![currentFigureCaptionListIndex].Number.Position;
      var numberLength = renderer.FigureCaptionList[currentFigureCaptionListIndex].Number.Count;
      var categoryName = renderer.FigureCaptionList[currentFigureCaptionListIndex].Category.Name;

      var textBeforeNumber = text.Substring(0, numberPosition - obj.Span.Start);
      var textAfterNumber = text.Substring(numberPosition + numberLength - obj.Span.Start);

      if (!string.IsNullOrEmpty(textBeforeNumber))
      {
        var run = renderer.PushNewRun();
        run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text() { Space = SpaceProcessingModeValues.Preserve, Text = textBeforeNumber });
        renderer.PopTo(run);
      }

      var para = renderer.Peek();
      var bookmarkId = GetBookmarkId(renderer, renderer.CurrentFigureCaptionListIndex.Value);
      {
        var bookmarkStart = new BookmarkStart
        {
          Id = bookmarkId.ToString(System.Globalization.CultureInfo.InvariantCulture),
          Name = "_REF" + bookmarkId.ToString(System.Globalization.CultureInfo.InvariantCulture)
        };


        para.AppendChild(bookmarkStart);
      }
      {
        var field = renderer.Push(new SimpleField { Instruction = $" SEQ {categoryName} \\* ARABIC " });

        // include the number
        var captionNumber = renderer.FigureCaptionIndices![renderer.CurrentFigureCaptionListIndex.Value];
        var run = renderer.PushNewRun();
        run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text() { Space = SpaceProcessingModeValues.Preserve, Text = captionNumber.ToString() });
        renderer.PopTo(field);
      }

      {
        var bookmarkEnd = new BookmarkEnd() { Id = bookmarkId.ToString(System.Globalization.CultureInfo.InvariantCulture) };
        para.AppendChild(bookmarkEnd);
      }
      if (!string.IsNullOrEmpty(textAfterNumber))
      {
        var run = renderer.PushNewRun();
        run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text() { Space = SpaceProcessingModeValues.Preserve, Text = textAfterNumber });
        renderer.PopTo(run);
      }
    }

    private static void WriteFigureLinkLiteralInline(OpenXMLRenderer renderer, LiteralInline obj)
    {
      if (!(renderer.CurrentFigureCaptionListIndex is { } currentFigureCaptionListIndex))
        throw new InvalidProgramException("Current figure caption list index must be set in the renderer!");

      var text = obj.Content.ToString();
      var numberPosition = renderer.FigureLinkList![currentFigureCaptionListIndex].Number.Position;
      var numberLength = renderer.FigureLinkList[currentFigureCaptionListIndex].Number.Count;
      var textBeforeNumber = text.Substring(0, numberPosition - obj.Span.Start);
      var textAfterNumber = text.Substring(numberPosition + numberLength - obj.Span.Start);

      var figureCaptionIndex = renderer.FigureLinkList[currentFigureCaptionListIndex].CaptionListIndex;
      var bookmarkId = GetBookmarkId(renderer, figureCaptionIndex);

      if (!string.IsNullOrEmpty(textBeforeNumber))
      {
        var run = renderer.PushNewRun();
        run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text() { Space = SpaceProcessingModeValues.Preserve, Text = textBeforeNumber });
        renderer.PopTo(run);
      }

      {
        var bookmarkRef = "_REF" + bookmarkId.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var field = renderer.Push(new SimpleField { Instruction = $" REF {bookmarkRef} \\h " });

        var captionNumber = renderer.FigureCaptionIndices![figureCaptionIndex];
        // include the number
        var run = renderer.PushNewRun();
        run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text() { Space = SpaceProcessingModeValues.Preserve, Text = captionNumber.ToString() });
        // renderer.ApplyStyleToRun(StyleDictionary.IdToName[FormatStyle.Link], run); // Note: This would not work - word is not formatting the text as hyperlink if the text is, like here, inside a field
        renderer.PopTo(field);
      }

      if (!string.IsNullOrEmpty(textAfterNumber))
      {
        var run = renderer.PushNewRun();
        run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text() { Space = SpaceProcessingModeValues.Preserve, Text = textAfterNumber });
        renderer.PopTo(run);
      }
    }


    private static long GetBookmarkId(OpenXMLRenderer renderer, int figureCaptionIndex)
    {
      return (renderer.FigureLinkRandom + 97L * figureCaptionIndex) % 1000000;
    }

    private bool Includes(Markdig.Syntax.SourceSpan span, int position, int length)
    {
      return position >= span.Start && position <= span.End && length <= span.Length;
    }
  }
}
