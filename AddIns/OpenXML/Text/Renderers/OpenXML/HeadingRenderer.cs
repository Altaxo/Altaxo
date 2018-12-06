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
  public class HeadingRenderer : OpenXMLObjectRenderer<HeadingBlock>
  {
    protected override void Write(OpenXMLRenderer renderer, HeadingBlock obj)
    {
      var paragraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph();
      renderer.Paragraph = renderer.Body.AppendChild(paragraph);
      renderer.Run = null;

      string id, name;
      switch (obj.Level)
      {
        case 1:
          id = StyleNames.Heading1Id;
          name = StyleNames.Heading1Name;
          break;
        case 2:
          id = StyleNames.Heading2Id;
          name = StyleNames.Heading2Name;
          break;
        case 3:
          id = StyleNames.Heading3Id;
          name = StyleNames.Heading3Name;
          break;
        case 4:
          id = StyleNames.Heading4Id;
          name = StyleNames.Heading4Name;
          break;
        case 5:
          id = StyleNames.Heading5Id;
          name = StyleNames.Heading5Name;
          break;
        case 6:
          id = StyleNames.Heading6Id;
          name = StyleNames.Heading6Name;
          break;
        case 7:
          id = StyleNames.Heading7Id;
          name = StyleNames.Heading7Name;
          break;
        case 8:
          id = StyleNames.Heading8Id;
          name = StyleNames.Heading9Name;
          break;
        case 9:
          id = StyleNames.Heading9Id;
          name = StyleNames.Heading9Name;
          break;
        default:
          throw new NotImplementedException();
      }

      renderer.ApplyStyleToParagraph(id, name, paragraph);
      renderer.WriteLeafInline(obj);
      // renderer.EnsureLine

    }
  }
}
