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
using WpfMath.Atoms;

namespace Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering
{
  /// <summary>
  /// Base interface for a renderer for a WpfMath <see cref="Atom"/>.
  /// </summary>
  internal interface IWpfMathRenderer
  {
    /// <summary>
    /// Occurs when before writing an object.
    /// </summary>
    event Action<IWpfMathRenderer, Atom> ObjectWriteBefore;

    /// <summary>
    /// Occurs when after writing an object.
    /// </summary>
    event Action<IWpfMathRenderer, Atom> ObjectWriteAfter;

    /// <summary>
    /// Gets the object renderers.
    /// </summary>
    ObjectRendererCollection ObjectRenderers { get; }

    /// <summary>
    /// Renders the specified <see cref="Atom"/>.
    /// </summary>
    /// <param name="atom">The WpfMath atom.</param>
    /// <returns>The result of the rendering.</returns>
    object Render(Atom atom);


    /// <summary>
    /// Announces the completion of an element by firing the <see cref="ObjectWriteAfter"/> event.
    /// </summary>
    /// <param name="completedElement">The element that was just completed.</param>
    void OnCompletionOfElement(Atom completedElement);
  }
}
