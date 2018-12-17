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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using Markdig.Syntax;

namespace Altaxo.Text.Renderers.OpenXML
{
  /// <summary>
  /// OpenXML renderer for a <see cref="HeadingBlock" />.
  /// </summary>
  /// <seealso cref="Altaxo.Text.Renderers.OpenXML.OpenXMLObjectRenderer{Markdig.Syntax.HeadingBlock}" />
  /// <seealso cref="OpenXMLObjectRenderer{T}" />
  public class HeadingRenderer : OpenXMLObjectRenderer<HeadingBlock>
  {
    protected override void Write(OpenXMLRenderer renderer, HeadingBlock obj)
    {


      ParaStyleName id;
      switch (obj.Level)
      {
        case 1:
          id = ParaStyleName.Heading1;
          break;
        case 2:
          id = ParaStyleName.Heading2;
          break;
        case 3:
          id = ParaStyleName.Heading3;
          break;
        case 4:
          id = ParaStyleName.Heading4;
          break;
        case 5:
          id = ParaStyleName.Heading5;
          break;
        case 6:
          id = ParaStyleName.Heading6;
          break;
        case 7:
          id = ParaStyleName.Heading7;
          break;
        case 8:
          id = ParaStyleName.Heading8;
          break;
        case 9:
          id = ParaStyleName.Heading9;
          break;
        default:
          throw new NotImplementedException();
      }

      renderer.PushParagraphFormat(id);
      var paragraph = renderer.PushNewParagraph();
      renderer.WriteLeafInline(obj);
      renderer.PopTo(paragraph);
      renderer.PopParagraphFormat();
    }
  }
}
