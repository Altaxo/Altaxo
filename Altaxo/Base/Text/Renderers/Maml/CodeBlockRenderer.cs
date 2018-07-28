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

using Markdig.Syntax;

namespace Altaxo.Text.Renderers.Maml
{
  /// <summary>
  /// Maml renderer for a <see cref="CodeBlock"/>.
  /// </summary>
  /// <seealso cref="MamlObjectRenderer{T}" />
  public class CodeBlockRenderer : MamlObjectRenderer<CodeBlock>
  {
    protected override void Write(MamlRenderer renderer, CodeBlock obj)
    {
      renderer.EnsureLine();

      renderer.Push(MamlElements.code);
      renderer.WriteLeafRawLines(obj, true, true);
      renderer.PopTo(MamlElements.code);
    }
  }
}
