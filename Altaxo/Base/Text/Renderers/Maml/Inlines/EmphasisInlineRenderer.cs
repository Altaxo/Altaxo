﻿#region Copyright

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

#nullable enable
using Markdig.Syntax.Inlines;

namespace Altaxo.Text.Renderers.Maml.Inlines
{
  /// <summary>
  /// Maml renderer for an <see cref="EmphasisInline"/>.
  /// </summary>
  /// <seealso cref="MamlObjectRenderer{T}" />
  public class EmphasisInlineRenderer : MamlObjectRenderer<EmphasisInline>
  {

    protected override void Write(MamlRenderer renderer, EmphasisInline obj)
    {
      MamlElement? mamlElement = null;

      switch (obj.DelimiterChar)
      {
        case '*':
        case '_':
          if (obj.DelimiterCount == 2)
            mamlElement = MamlElements.legacyBold;
          else
            mamlElement = MamlElements.legacyItalic;
          break;

        case '~':
          if (obj.DelimiterCount == 2)
            mamlElement = MamlElements.legacyStrikethrough;
          else
            mamlElement = MamlElements.subscript;

          break;

        case '^':
          mamlElement = MamlElements.superscript;
          break;

        case '+':
          // Inserted style
          mamlElement = MamlElements.legacyUnderline;
          break;

        case '=':
          // Marked style
          break;
      }

      if (mamlElement is not null)
        renderer.Push(mamlElement);
      renderer.WriteChildren(obj);
      if (mamlElement is not null)
        renderer.PopTo(mamlElement);
    }
  }
}
