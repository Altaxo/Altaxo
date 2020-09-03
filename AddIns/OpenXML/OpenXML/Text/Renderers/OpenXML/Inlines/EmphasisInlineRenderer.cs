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
using DocumentFormat.OpenXml.Wordprocessing;
using Markdig.Syntax.Inlines;

namespace Altaxo.Text.Renderers.OpenXML.Inlines
{
  /// <summary>
  /// OpenXML renderer for an <see cref="EmphasisInline"/>.
  /// </summary>
  /// <seealso cref="OpenXMLObjectRenderer{T}" />
  public class EmphasisInlineRenderer : OpenXMLObjectRenderer<EmphasisInline>
  {

    protected override void Write(OpenXMLRenderer renderer, EmphasisInline obj)
    {
      int nPushed = 0;

      switch (obj.DelimiterChar)
      {
        case '*':
        case '_':
          if (obj.DelimiterCount==2)
          {
            renderer.PushInlineFormat(OpenXMLRenderer.InlineFormat.Bold);
            ++nPushed;
          }
          else
          {
            renderer.PushInlineFormat(OpenXMLRenderer.InlineFormat.Italic);
            ++nPushed;
          }
          break;

        case '~':
          if (obj.DelimiterCount==2)
          {
            renderer.PushInlineFormat(OpenXMLRenderer.InlineFormat.Strikethrough);
            ++nPushed;
          }
          else
          {
            renderer.PushInlineFormat(OpenXMLRenderer.InlineFormat.Subscript);
            ++nPushed;
          }

          break;

        case '^':
          renderer.PushInlineFormat(OpenXMLRenderer.InlineFormat.Superscript);
          ++nPushed;
          break;

        case '+':
          // Inserted style
          {
            renderer.PushInlineFormat(OpenXMLRenderer.InlineFormat.Underline);
            ++nPushed;
          }
          break;

        case '=':
          // Marked style
          break;
      }

      renderer.WriteChildren(obj);

      for (int i = 0; i < nPushed; ++i)
        renderer.PopInlineFormat();
    }
  }
}
