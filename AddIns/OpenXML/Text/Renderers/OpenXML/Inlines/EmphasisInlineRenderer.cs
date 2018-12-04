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
  /// Maml renderer for an <see cref="EmphasisInline"/>.
  /// </summary>
  /// <seealso cref="OpenXMLObjectRenderer{T}" />
  public class EmphasisInlineRenderer : OpenXMLObjectRenderer<EmphasisInline>
  {

    protected override void Write(OpenXMLRenderer renderer, EmphasisInline obj)
    {
      renderer.Run = renderer.Paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run());

      var runProperties = renderer.Run.AppendChild(new RunProperties());

      switch (obj.DelimiterChar)
      {
        case '*':
        case '_':
          if (obj.IsDouble)
          {
            var bold = new Bold { Val = OnOffValue.FromBoolean(true) };
            runProperties.AppendChild(bold);
          }
          else
          {
            var italic = new Italic { Val = OnOffValue.FromBoolean(true) };
            runProperties.AppendChild(italic);
          }
          break;

        case '~':
          if (obj.IsDouble)
          {
            var strike = new Strike { Val = OnOffValue.FromBoolean(true) };
            runProperties.AppendChild(strike);
          }
          else
          {
            runProperties.VerticalTextAlignment = new VerticalTextAlignment() { Val = VerticalPositionValues.Subscript };
          }

          break;

        case '^':
          runProperties.VerticalTextAlignment = new VerticalTextAlignment() { Val = VerticalPositionValues.Superscript };
          break;

        case '+':
          // Inserted style
          {
            var underline = new Underline();
            runProperties.AppendChild(underline);
          }
          break;

        case '=':
          // Marked style
          break;
      }

      //renderer.Push(run);
      renderer.WriteChildren(obj);

      renderer.Run = null;
    }
  }
}
