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

using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using Markdig.Syntax;

namespace Altaxo.Text.Renderers.OpenXML
{
  /// <summary>
  /// OpenXML renderer for a <see cref="ThematicBreakBlock"/>.
  /// </summary>
  /// <seealso cref="OpenXMLObjectRenderer{T}" />
  public class ThematicBreakRenderer : OpenXMLObjectRenderer<ThematicBreakBlock>
  {
    protected override void Write(OpenXMLRenderer renderer, ThematicBreakBlock obj)
    {
      var paragraph = renderer.PushNewParagraph();
      var paraProperties = paragraph.ChildElements.FirstOrDefault() as ParagraphProperties;
      if (null == paraProperties)
        paraProperties = paragraph.AppendChild(new ParagraphProperties());

      var paraBorders = new ParagraphBorders();
      var bottom = new BottomBorder() { Val = BorderValues.Single, Color = "auto", Size = 12, Space = 1 };
      paraBorders.Append(bottom);
      paraProperties.Append(paraBorders);
      renderer.PopTo(paragraph);
    }
  }
}
