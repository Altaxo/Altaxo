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

namespace Altaxo.Text.Renderers.OpenXML
{
  public enum ParaStyleName
  {
    // Built-in styles (for the names used in word see the StyleDictionary)
    Heading1,
    Heading2,
    Heading3,
    Heading4,
    Heading5,
    Heading6,
    Heading7,
    Heading8,
    Heading9,
    QuoteBlock,
    ListParagraph,
    Link,
    // not built-in styles (must be added manually to the Word document)
    CodeBlock,
    CodeInline
  }

  /// <summary>
  /// Static class containing the style Ids and names used in the style templates.
  /// </summary>
  public static class StyleDictionary
  {
    private static Dictionary<ParaStyleName, string> _idToName;
    /// <summary>
    /// Dictionary that translates the StyleId to the style name.
    /// </summary>
    /// <value>
    /// Dictionary that translates the StyleId to the style name
    /// </value>
    public static IReadOnlyDictionary<ParaStyleName, string> IdToName { get { return _idToName; } }

    static StyleDictionary()
    {
      _idToName = new Dictionary<ParaStyleName, string>()
      {
        [ParaStyleName.Heading1] = "heading 1",
        [ParaStyleName.Heading2] = "heading 2",
        [ParaStyleName.Heading3] = "heading 3",
        [ParaStyleName.Heading4] = "heading 4",
        [ParaStyleName.Heading5] = "heading 5",
        [ParaStyleName.Heading6] = "heading 6",
        [ParaStyleName.Heading7] = "heading 7",
        [ParaStyleName.Heading8] = "heading 8",
        [ParaStyleName.Heading9] = "heading 9",
        [ParaStyleName.QuoteBlock] = "Block Text",
        [ParaStyleName.ListParagraph] = "List Paragraph",
        [ParaStyleName.Link] = "Hyperlink",
        [ParaStyleName.CodeBlock] = "CodeBlock",
        [ParaStyleName.CodeInline] = "CodeInline",
      };
    }


  }
}
