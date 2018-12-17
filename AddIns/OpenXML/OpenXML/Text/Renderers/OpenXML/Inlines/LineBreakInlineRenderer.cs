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

using DocumentFormat.OpenXml;
using Markdig.Syntax.Inlines;
using W = DocumentFormat.OpenXml.Wordprocessing;

namespace Altaxo.Text.Renderers.OpenXML.Inlines
{
  /// <summary>
  /// OpenXML renderer for a <see cref="LineBreakInline"/>.
  /// </summary>
  /// <seealso cref="OpenXMLObjectRenderer{T}" />
  public class LineBreakInlineRenderer : OpenXMLObjectRenderer<LineBreakInline>
  {
    protected override void Write(OpenXMLRenderer renderer, LineBreakInline obj)
    {
      if (obj.IsHard)
      {
        var element = renderer.Peek();
        if (element is W.Paragraph)
        {
          var run = renderer.Push(new W.Run());
          run.AppendChild(new W.Break());
          renderer.PopTo(run);
        }
        else
        {
          var paragraph = renderer.Push(new W.Paragraph());
          renderer.PopTo(paragraph);
        }
      }
      else // neither hard nor backslash -> but we have to add a space at least
      {
        var element = renderer.Peek();
        if (element is W.Paragraph)
        {
          var run = renderer.PushNewRun();
          run.AppendChild(new W.Text() { Space = SpaceProcessingModeValues.Preserve, Text = " " });
          renderer.PopTo(run);
        }
        else // this should not happen, because we already are in a paragraph
        {
          var paragraph = renderer.Push(new W.Paragraph());
          renderer.PopTo(paragraph);
        }
      }
    }
  }
}
